using MJC.common;
using MJC.common.components;
using MJC.model;

namespace MJC.forms.sku
{
    public partial class SKUCostQuantity : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Edits", Keys.Enter);
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);

        private GridViewOrigin SkuCostGrid = new GridViewOrigin();
        private DataGridView SkuCostGridRefer;
        
        private int skuId = 0;
        private bool readOnly = false;

        public SKUCostQuantity(int skuId, bool readOnly = false) : base("Costs for SKU#", "Record of cost per quantity of a SKU, used for inventory valuation")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[4] { hkAdds, hkDeletes, hkEdits, hkPreviousScreen };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();
            this.skuId = skuId;
            this.readOnly = readOnly;

            InitSkuCostQty();
            if (skuId != 0)
            {
                string skuName = Session.SKUModelObj.GetSkuNameById(skuId);
                this._changeFormText("Costs for SKU " + skuName);
            }

            this.VisibleChanged += (s, e) =>
            {
                this.LoadSKUCostQty();
            };
        }

        private void InitSkuCostQty()
        {
            SkuCostGridRefer = SkuCostGrid.GetGrid();
            SkuCostGridRefer.Location = new Point(0, 95);
            SkuCostGridRefer.Width = this.Width;
            SkuCostGridRefer.Height = this.Height - 295;
            SkuCostGridRefer.VirtualMode = true;
            this.Controls.Add(SkuCostGridRefer);
            this.SkuCostGridRefer.CellDoubleClick += (sender, e) =>
            {
                updateSKUCostQuantity();
            };
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                SKUCostQuantityDetail detailModal = new SKUCostQuantityDetail(this.skuId);
                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadSKUCostQty();
                }
            };

            hkEdits.GetButton().Click += (sender, e) =>
            {
                updateSKUCostQuantity();
            };

            hkDeletes.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to delete?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int rowIndex = SkuCostGridRefer.CurrentCell.RowIndex;
                    DataGridViewRow row = SkuCostGridRefer.Rows[rowIndex];
                    int skuCostQtyId = (int)row.Cells[0].Value;

                    var refreshData = Session.SKUCostModelObj.DeleteSKUCostQty(skuCostQtyId);
                    if (refreshData)
                    {
                        LoadSKUCostQty();
                    }
                }
            };
        }

        private void LoadSKUCostQty()
        {
            var refreshData = Session.SKUCostModelObj.LoadSKUCostQty();

            if (refreshData)
            {
                SkuCostGridRefer.DataSource = Session.SKUCostModelObj.SkuCostQtyList;
                SkuCostGridRefer.Columns[0].HeaderText = "id";
                SkuCostGridRefer.Columns[0].Visible = false;
                SkuCostGridRefer.Columns[1].HeaderText = "skuId";
                SkuCostGridRefer.Columns[1].Visible = false;
                SkuCostGridRefer.Columns[2].HeaderText = "Date";
                SkuCostGridRefer.Columns[2].Width = 300;
                SkuCostGridRefer.Columns[3].HeaderText = "Method";
                SkuCostGridRefer.Columns[3].Width = 300;
                SkuCostGridRefer.Columns[4].HeaderText = "Qty";
                SkuCostGridRefer.Columns[4].Width = 300;
                SkuCostGridRefer.Columns[5].HeaderText = "Cost";
                SkuCostGridRefer.Columns[5].Width = 200;
                SkuCostGridRefer.Columns[6].HeaderText = "Core";
                SkuCostGridRefer.Columns[6].Width = 200;
            }
        }

        private void updateSKUCostQuantity()
        {
            SKUCostQuantityDetail detailModal = new SKUCostQuantityDetail(this.skuId, this.readOnly);

            int rowIndex = SkuCostGridRefer.CurrentCell.RowIndex;
            DataGridViewRow row = SkuCostGridRefer.Rows[rowIndex];
            int skuCostQtyId = (int)row.Cells[0].Value;

            SKUCostQty tempSkuCostQty = new SKUCostQty();
            foreach (SKUCostQty skuCostQty in Session.SKUCostModelObj.SkuCostQtyList)
            {
                if (skuCostQty.id == skuCostQtyId)
                {
                    tempSkuCostQty = skuCostQty;
                    break;
                }
            }

            detailModal.setDetails(tempSkuCostQty);

            if (detailModal.ShowDialog() == DialogResult.OK)
            {
                LoadSKUCostQty();
            }
        }
    }
}
