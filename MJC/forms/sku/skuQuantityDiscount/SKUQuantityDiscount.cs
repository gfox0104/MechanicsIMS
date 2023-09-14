using MJC.common;
using MJC.common.components;
using MJC.forms.price;
using MJC.model;
using System.Security.Cryptography;

namespace MJC.forms.sku.skuQuantityDiscount
{
    public partial class SKUQuantityDiscount : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Edits", Keys.Enter);
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);

        private GridViewOrigin SkuQtyDiscountGrid = new GridViewOrigin();
        private DataGridView SkuQtyDiscountGridRefer;
        
        private int skuId = 0;
        private bool readOnly = false;

        public SKUQuantityDiscount(int skuId, bool readOnly = false) : base("Quantity Discounts for SKU#", "Set discounts for buying a SKU in bulk")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons;
            if (readOnly)
            {
                hkButtons = new HotkeyButton[1] { hkPreviousScreen };
            } else
            {
                hkButtons = new HotkeyButton[4] { hkAdds, hkDeletes, hkEdits, hkPreviousScreen };
            }

            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();
            this.skuId = skuId;
            this.readOnly = readOnly;

            InitSkuQtyDiscount();
            if (skuId != 0)
            {
                string skuName = Session.SKUModelObj.GetSkuNameById(skuId);
                this._changeFormText("Quantity Discounts for SKU# " + skuName);
            }

            this.VisibleChanged += (s, e) =>
            {
                this.LoadSKUQtyDiscount();
            };
        }

        private void InitSkuQtyDiscount()
        {
            SkuQtyDiscountGridRefer = SkuQtyDiscountGrid.GetGrid();
            SkuQtyDiscountGridRefer.Location = new Point(0, 95);
            SkuQtyDiscountGridRefer.Width = this.Width;
            SkuQtyDiscountGridRefer.Height = this.Height - 295;
            SkuQtyDiscountGridRefer.VirtualMode = true;
            this.Controls.Add(SkuQtyDiscountGridRefer);
            this.SkuQtyDiscountGridRefer.CellDoubleClick += (sender, e) =>
            {
                updateSKUQtyDiscount();
            };
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                SKUQuantityDiscountDetail detailModal = new SKUQuantityDiscountDetail(this.skuId);
                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadSKUQtyDiscount();
                }
            };

            hkEdits.GetButton().Click += (sender, e) =>
            {
                updateSKUQtyDiscount();
            };

            hkDeletes.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to delete?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int rowIndex = SkuQtyDiscountGridRefer.CurrentCell.RowIndex;
                    DataGridViewRow row = SkuQtyDiscountGridRefer.Rows[rowIndex];
                    int skuCostQtyId = (int)row.Cells[0].Value;

                    var refreshData = Session.SKUQtyDiscountModelObj.DeleteSKUCostQty(skuCostQtyId);
                    if (refreshData)
                    {
                        LoadSKUQtyDiscount();
                    }
                }
            };
        }

        private void LoadSKUQtyDiscount()
        {
            var refreshData = Session.SKUQtyDiscountModelObj.LoadSKUQtyDiscount();

            if (refreshData)
            {
                SkuQtyDiscountGridRefer.DataSource = Session.SKUQtyDiscountModelObj.SkuQtyDiscountList;
                SkuQtyDiscountGridRefer.Columns[0].HeaderText = "id";
                SkuQtyDiscountGridRefer.Columns[0].Visible = false;
                SkuQtyDiscountGridRefer.Columns[1].HeaderText = "skuId";
                SkuQtyDiscountGridRefer.Columns[1].Visible = false;
                SkuQtyDiscountGridRefer.Columns[2].HeaderText = "Qty From";
                SkuQtyDiscountGridRefer.Columns[2].Width = 300;
                SkuQtyDiscountGridRefer.Columns[3].HeaderText = "Qty To";
                SkuQtyDiscountGridRefer.Columns[3].Width = 300;
                SkuQtyDiscountGridRefer.Columns[4].HeaderText = "Discount Type";
                SkuQtyDiscountGridRefer.Columns[4].Width = 300;
                SkuQtyDiscountGridRefer.Columns[5].HeaderText = "Price Tier";
                SkuQtyDiscountGridRefer.Columns[5].Width = 200;
                SkuQtyDiscountGridRefer.Columns[6].HeaderText = "Discount0";
                SkuQtyDiscountGridRefer.Columns[6].Width = 200;
                SkuQtyDiscountGridRefer.Columns[7].HeaderText = "Discount1";
                SkuQtyDiscountGridRefer.Columns[7].Width = 200;
                SkuQtyDiscountGridRefer.Columns[7].Visible = false;
            }
        }

        private void updateSKUQtyDiscount()
        {
            SKUQuantityDiscountDetail detailModal = new SKUQuantityDiscountDetail(this.skuId);

            int rowIndex = SkuQtyDiscountGridRefer.CurrentCell.RowIndex;
            DataGridViewRow row = SkuQtyDiscountGridRefer.Rows[rowIndex];
            int skuCostQtyId = (int)row.Cells[0].Value;

            SKUQtyDiscount tempSkuQtyDiscount = new SKUQtyDiscount();
            foreach (SKUQtyDiscount skuQtyDiscount in Session.SKUQtyDiscountModelObj.SkuQtyDiscountList)
            {
                if (skuQtyDiscount.id == skuCostQtyId)
                {
                    tempSkuQtyDiscount = skuQtyDiscount;
                    break;
                }
            }

            detailModal.setDetails(tempSkuQtyDiscount);

            if (detailModal.ShowDialog() == DialogResult.OK)
            {
                LoadSKUQtyDiscount();
            }
        }

    }
}
