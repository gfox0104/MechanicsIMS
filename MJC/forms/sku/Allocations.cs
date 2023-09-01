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
using MJC.forms.order;

namespace MJC.forms.sku
{
    public partial class Allocations : GlobalLayout
    {

        private HotkeyButton hkPrevScreen = new HotkeyButton("Esc", "Previous screen", Keys.Escape);
        private HotkeyButton hkOpenOrder = new HotkeyButton("Enter", "Open order", Keys.Enter);

        private GridViewOrigin allocationsGrid = new GridViewOrigin();
        private DataGridView ALGridRefer;

        private SKUModel SKUModelObj = new SKUModel();
        private int skuId = 0;

        public Allocations(int skuId = 0, string skuName = "") : base("Allocations for SKU#", "Review held orders with SKU quantity allocated")
        {
            InitializeComponent();
            _initBasicSize();
            if (!string.IsNullOrEmpty(skuName))
                this._changeFormText("Allocations for SKU# " + skuName);
            HotkeyButton[] hkButtons = new HotkeyButton[2] { hkPrevScreen, hkOpenOrder };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();
            this.skuId = skuId;

            InitAllocationsGrid();
        }

        private void AddHotKeyEvents()
        {
            hkOpenOrder.GetButton().Click += (sender, e) =>
            {
                int customerId = 0;

                if (ALGridRefer.Rows.Count > 0)
                {
                    int rowIndex = ALGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = ALGridRefer.Rows[rowIndex];
                    customerId = (int)row.Cells[7].Value;
                }

                //ProcessOrder processForm = new ProcessOrder(skuId, customerId);
                //_navigateToForm(sender, e, processForm);
            };
        }

        private void InitAllocationsGrid()
        {
            ALGridRefer = allocationsGrid.GetGrid();
            ALGridRefer.Location = new Point(0, 95);
            ALGridRefer.Width = this.Width;
            ALGridRefer.Height = this.Height - 295;
            ALGridRefer.VirtualMode = true;
            this.Controls.Add(ALGridRefer);
            //this.ALGridRefer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.addcategory_btn_Click);

            LoadSKUAllocation();
        }

        private void LoadSKUAllocation()
        {
            var refreshData = SKUModelObj.LoadSkuAllocations(this.skuId);

            if (refreshData)
            {
                ALGridRefer.DataSource = SKUModelObj.SKUAllocationList;
                ALGridRefer.Columns[0].HeaderText = "Date";
                ALGridRefer.Columns[0].Width = 200;
                ALGridRefer.Columns[1].HeaderText = "Customer Name";
                ALGridRefer.Columns[1].Width = 400;
                ALGridRefer.Columns[2].HeaderText = "Held Status";
                ALGridRefer.Columns[2].Width = 300;
                ALGridRefer.Columns[3].HeaderText = "Proc By";
                ALGridRefer.Columns[3].Width = 200;
                ALGridRefer.Columns[4].HeaderText = "Qty";
                ALGridRefer.Columns[4].Width = 200;
                ALGridRefer.Columns[5].HeaderText = "Price";
                ALGridRefer.Columns[5].Width = 200;
                ALGridRefer.Columns[6].HeaderText = "Subtotal";
                ALGridRefer.Columns[6].Width = 200;
                ALGridRefer.Columns[7].HeaderText = "CustomerId";
                ALGridRefer.Columns[7].Visible = false;
            }
        }

        //private void updateCategory()
        //{
        //}

        //private void addcategory_btn_Click(object sender, DataGridViewCellEventArgs e)
        //{
        //    updateCategory();
        //}
    }
}
