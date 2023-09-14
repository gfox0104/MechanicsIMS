using MJC.common.components;
using MJC.common;
using MJC.forms.category;
using MJC.model;

namespace MJC.forms.sales
{
    public partial class SalesHisotry : GlobalLayout
    {

        private HotkeyButton hkNavigatePreviousYears = new HotkeyButton("ESC", "Previous screen", Keys.None);
        //private HotkeyButton hkNavigatePreviousYears = new HotkeyButton("<= / =>", "Previous screen", Keys.None);

        private GridViewOrigin slaesHistoryGrid = new GridViewOrigin();
        private DataGridView SHGridRefer;

        private int skuId = 0;

        public SalesHisotry(int skuId) : base("History for SKU", "Sales history for the selected SKU#")
        {
            InitializeComponent();
            _initBasicSize();
            this.skuId = skuId;

            HotkeyButton[] hkButtons = new HotkeyButton[1] { hkNavigatePreviousYears };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitSalesHistoryGrid();
            if (skuId != 0)
            {
                string skuName = Session.SKUModelObj.GetSkuNameById(skuId);
                this._changeFormText("History for SKU# " + skuName);
            }

        }

        private void AddHotKeyEvents()
        {
        }

        private void InitSalesHistoryGrid()
        {
            SHGridRefer = slaesHistoryGrid.GetGrid();
            SHGridRefer.Location = new Point(0, 95);
            SHGridRefer.Width = this.Width;
            SHGridRefer.Height = this.Height - 325;
            SHGridRefer.AllowUserToAddRows = false;
            SHGridRefer.AllowUserToOrderColumns = false;
            SHGridRefer.MultiSelect = false;
            SHGridRefer.ScrollBars = ScrollBars.Horizontal;
            SHGridRefer.VirtualMode = true;
            SHGridRefer.Columns.Add("month", "Quantities");
            SHGridRefer.Columns[0].Width = 300;

            List<int> SalesHistoryHeaderData = Session.SKUModelObj.GetQtyYearList(this.skuId);

            int index = 0;
            foreach (int year in SalesHistoryHeaderData)
            {
                SHGridRefer.Columns.Add(year.ToString(), year.ToString());
                SHGridRefer.Columns[1 + index++].Width = 200;
            }

            foreach (DataGridViewColumn column in SHGridRefer.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            this.Controls.Add(SHGridRefer);

            LoadCategoryList();
        }

        private void LoadCategoryList()
        {
            List<int[]> TotalQtyList = Session.SKUModelObj.LoadQtyHistoryData(this.skuId);
            List<int> SalesHistoryHeaderData = Session.SKUModelObj.GetQtyYearList(this.skuId);
            string[] ItemList = new string[13] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", "Totals" };

            for (int monthIndex = 0; monthIndex < 13; monthIndex++)
            {
                int rowIndex = SHGridRefer.Rows.Add();
                DataGridViewRow newRow = SHGridRefer.Rows[rowIndex];

                newRow.Resizable = DataGridViewTriState.False;
                newRow.Cells["month"].Value = ItemList[monthIndex];
                int index = 0;
                foreach (int year in SalesHistoryHeaderData)
                {
                    newRow.Cells[year.ToString()].Value = TotalQtyList[index][monthIndex];
                    index++;
                }
            }
        }

        private void updateCategory()
        {
            CategoryDetail detailModal = new CategoryDetail();

            int rowIndex = SHGridRefer.CurrentCell.RowIndex;

            if (SHGridRefer.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in SHGridRefer.SelectedRows)
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
            updateCategory();
        }
    }
}
