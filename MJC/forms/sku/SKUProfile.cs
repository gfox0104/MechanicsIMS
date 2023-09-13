using MJC.common.components;
using MJC.common;
using MJC.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MJC.forms.sku
{
    public partial class SKUProfile : GlobalLayout
    {

        private HotkeyButton hkPrevScreen = new HotkeyButton("Esc", "Previous screen", Keys.Escape);
        //private HotkeyButton OpenOrder = new HotkeyButton("Enter", "Open order", Keys.Enter);

        private GridViewOrigin profileInfoGrid = new GridViewOrigin();
        private DataGridView ProfileInfoGridRefer;

        private int skuId = 0;

        public SKUProfile(int skuId = 0, string skuNumber = "") : base("Profile Info", "Review SKU history of invoices")
        {
            InitializeComponent();
            _initBasicSize();
            this.skuId = skuId;
            this._changeFormText("Profile Info - " + skuNumber);

            HotkeyButton[] hkButtons = new HotkeyButton[1] { hkPrevScreen };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitCategoryListGrid();
        }

        private void AddHotKeyEvents()
        {
        }

        private void InitCategoryListGrid()
        {
            ProfileInfoGridRefer = profileInfoGrid.GetGrid();
            ProfileInfoGridRefer.Location = new Point(0, 95);
            ProfileInfoGridRefer.Width = this.Width;
            ProfileInfoGridRefer.Height = this.Height - 295;
            ProfileInfoGridRefer.VirtualMode = true;
            this.Controls.Add(ProfileInfoGridRefer);
            //this.ProfileInfoGridRefer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.addcategory_btn_Click);

            //ProfileInfoGridRefer.Columns.Add("Name", "Date");
            //ProfileInfoGridRefer.Columns.Add("Age", "Cust#");
            //ProfileInfoGridRefer.Columns.Add("Country", "Inv#");
            //ProfileInfoGridRefer.Columns.Add("Country", "Qty");
            //ProfileInfoGridRefer.Columns.Add("Country", "Price");

            //ProfileInfoGridRefer.Columns[0].Width = 300;
            //ProfileInfoGridRefer.Columns[1].Width = 300;
            //ProfileInfoGridRefer.Columns[2].Width = 300;
            //ProfileInfoGridRefer.Columns[3].Width = 300;
            //ProfileInfoGridRefer.Columns[4].Width = 300;

            LoadProfileHistory();
        }

        public void LoadProfileHistory()
        {
            var refreshData = Session.SKUModelObj.LoadSkuProfile(this.skuId);

            if (refreshData)
            {
                ProfileInfoGridRefer.DataSource = Session.SKUModelObj.SKUProfileList;
                ProfileInfoGridRefer.Columns[0].Visible = false;
                ProfileInfoGridRefer.Columns[1].HeaderText = "Date";
                ProfileInfoGridRefer.Columns[1].Width = 200;
                ProfileInfoGridRefer.Columns[2].HeaderText = "Customer Name";
                ProfileInfoGridRefer.Columns[2].Width = 400;
                ProfileInfoGridRefer.Columns[3].HeaderText = "Invoice Number";
                ProfileInfoGridRefer.Columns[3].Width = 400;
                ProfileInfoGridRefer.Columns[4].HeaderText = "Qty";
                ProfileInfoGridRefer.Columns[4].Width = 100;
                ProfileInfoGridRefer.Columns[5].HeaderText = "Price";
                ProfileInfoGridRefer.Columns[5].Width = 100;
            }
        }

        //private void updateCategory()
        //{
        //    CategoryDetail detailModal = new CategoryDetail();

        //    int rowIndex = CLGridRefer.CurrentCell.RowIndex;

        //    if (CLGridRefer.SelectedRows.Count > 0)
        //    {
        //        foreach (DataGridViewRow row in CLGridRefer.SelectedRows)
        //        {
        //            int categoryId = (int)row.Cells[0].Value;
        //            string categoryName = row.Cells[1].Value.ToString();
        //            string calculateAs = row.Cells[2].Value.ToString();

        //            detailModal.setDetails(categoryName, calculateAs, categoryId);
        //        }
        //    }

        //    if (detailModal.ShowDialog() == DialogResult.OK)
        //    {
        //        LoadCategoryList();
        //    }
        //}

        //private void addcategory_btn_Click(object sender, DataGridViewCellEventArgs e)
        //{
        //    updateCategory();
        //}
    }
}
