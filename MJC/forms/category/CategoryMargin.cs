using MJC.common.components;
using MJC.common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MJC.model;

namespace MJC.forms.category
{
    public partial class CategoryMargin : GlobalLayout
    {

        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkSelects = new HotkeyButton("Enter", "Selects", Keys.Enter);
        private HotkeyButton hkSetsPrices = new HotkeyButton("F4", "Sets Prices", Keys.F4);

        private GridViewOrigin categoryListGrid = new GridViewOrigin();
        private DataGridView CLGridRefer;
        private CategoriesModel CategoriesModelObj = new CategoriesModel();
        private PriceTiersModel PriceTiersModelObj = new PriceTiersModel();
        private SKUModel SKUModelObj = new SKUModel();

        public CategoryMargin() : base("Category Margins", "Manage category margins used to calculate prices")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[4] { hkAdds, hkDeletes, hkSelects, hkSetsPrices };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitCategoryListGrid();
            //this.VisibleChanged += CategoryMargin_VisibleChanged(object sender, EventArgs e);
            this.VisibleChanged += this.CategoryMargin_VisibleChanged;
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                CategoryDetail detailModal = new CategoryDetail();
                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadCategoryList();
                }
            };
            hkDeletes.GetButton().Click += (sender, e) =>
            {
                int selectedCategoryId = 0;

                int rowIndex = CLGridRefer.SelectedRows[0].Index;
                DataGridViewRow row = CLGridRefer.Rows[rowIndex];
                selectedCategoryId = (int)row.Cells[0].Value;

                bool refreshData = CategoriesModelObj.DeleteCategory(selectedCategoryId);
                if (refreshData)
                {
                    LoadCategoryList();
                }
            };
            hkSelects.GetButton().Click += (sender, e) =>
            {
                updateCategory();
            };
            hkSetsPrices.GetButton().Click += (sender, e) =>
            {
                if (CLGridRefer.Rows.Count > 0)
                {
                    int rowIndex = CLGridRefer.SelectedRows[0].Index;

                    DataGridViewRow row = CLGridRefer.Rows[rowIndex];
                    int categoryId = (int)row.Cells[0].Value;
                    int calculateAs = (int)row.Cells[2].Value;

                    DialogResult result = MessageBox.Show("This will reset all prices for SKUs in this category. Proceed?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        updatePrices(categoryId, calculateAs);
                    }
                }
            };
        }

        private void InitCategoryListGrid()
        {
            List<KeyValuePair<int, string>> PriceTierHeaderData = PriceTiersModelObj.GetPriceTierItems();

            CLGridRefer = categoryListGrid.GetGrid();
            CLGridRefer.Location = new Point(0, 95);
            CLGridRefer.Width = this.Width;
            CLGridRefer.Height = this.Height - 295;
            CLGridRefer.AllowUserToAddRows = false;

            CLGridRefer.Columns.Clear();
            CLGridRefer.Columns.Add("id", "id");
            CLGridRefer.Columns[0].Visible = false;
            CLGridRefer.Columns.Add("category", "Category");
            CLGridRefer.Columns[1].Width = 300;
            CLGridRefer.Columns.Add("calculateAs", "Cal As");
            CLGridRefer.Columns[2].Width = 300;
            int index = 0;
            foreach (KeyValuePair<int, string> pair in PriceTierHeaderData)
            {
                CLGridRefer.Columns.Add(pair.Value, pair.Value);
                CLGridRefer.Columns[3 + index++].Width = 300;
            }
            this.Controls.Add(CLGridRefer);
     
            this.CLGridRefer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.addcategory_btn_Click);
      

            LoadCategoryList();
        }

        private void LoadCategoryList()
        {
            string filter = "";
            if (CLGridRefer.Rows.Count == 0)
            {
                List<CategoryData> categoryDataList = CategoriesModelObj.LoadCategoryData(filter);

                foreach (CategoryData categoryData in categoryDataList)
                {
                    List<KeyValuePair<string, double>> priceTierData = new List<KeyValuePair<string, double>>();
                    priceTierData = PriceTiersModelObj.GetPriceTierMargin(categoryData.id);
                    int rowIndex = CLGridRefer.Rows.Add();
                    DataGridViewRow newRow = CLGridRefer.Rows[rowIndex];

                    newRow.Cells["id"].Value = categoryData.id;
                    newRow.Cells["category"].Value = categoryData.categoryName;
                    newRow.Cells["calculateAs"].Value = categoryData.calculateAs == 1 ? "Markup" : "Margin";

                    foreach (KeyValuePair<string, double> pair in priceTierData)
                    {
                        newRow.Cells[pair.Key].Value = pair.Value;
                    }
                }
            }
            else
            {
                
                List<CategoryData> categoryDataList = CategoriesModelObj.LoadCategoryData(filter);

                for (int rowIndex = 0; rowIndex < categoryDataList.Count; rowIndex++)
                {
                    CategoryData categoryData = categoryDataList[rowIndex];
              
                    List<KeyValuePair<string, double>> priceTierData = new List<KeyValuePair<string, double>>();
                    priceTierData = PriceTiersModelObj.GetPriceTierMargin(categoryData.id);

                    DataGridViewRow newRow = CLGridRefer.Rows[rowIndex];
                    newRow.Cells["id"].Value = categoryData.id;
                    newRow.Cells["category"].Value = categoryData.categoryName;
                    newRow.Cells["calculateAs"].Value = categoryData.calculateAs == 1 ? "Markup" : "Margin";

                    foreach (KeyValuePair<string, double> pair in priceTierData)
                    {
                        newRow.Cells[pair.Key].Value = pair.Value;
                    }
                    
                }
            }
        }

        private void updateCategory()
        {
            CategoryDetail detailModal = new CategoryDetail();

            int rowIndex = CLGridRefer.CurrentCell.RowIndex;

            if (CLGridRefer.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in CLGridRefer.SelectedRows)
                {
                    int categoryId = (int)row.Cells[0].Value;
                    string categoryName = row.Cells[1].Value.ToString();
                    string calculateAs = row.Cells[2].Value.ToString();

                    detailModal.setDetails(categoryName, calculateAs, categoryId);
                }
            }

            if (detailModal.ShowDialog() == DialogResult.OK)
            {
                LoadCategoryList();
            }
        }

        private void addcategory_btn_Click(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow selectedRow = CLGridRefer.SelectedRows[0];
            if (selectedRow.Cells[0].Value != null)
                updateCategory();
        }

        private void updatePrices(int categoryId, int calculateAs)
        {
            SKUModelObj.SetPrice(categoryId, calculateAs);
        }

        private void CategoryMargin_VisibleChanged(object sender, EventArgs e)
        {
            // InitCategoryListGrid();
        }
    }
}
