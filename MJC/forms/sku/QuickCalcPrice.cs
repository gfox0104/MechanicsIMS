using MJC.common;
using MJC.common.components;
using MJC.model;
using MJC.model.MJC.model;

namespace MJC.forms.sku
{
    public partial class QuickCalcPrice : BasicModal
    {
        private HotkeyButton hkSetPrice = new HotkeyButton("F2", "Set prices to these markups", Keys.F5);
        private HotkeyButton hkCancelCalc = new HotkeyButton("ESC", "Cancel calculation", Keys.Escape);

        private GridViewOrigin PriceTierGrid = new GridViewOrigin();
        private DataGridView PriceTierGridRefer;

        private FlabelConstant Category = new FlabelConstant("Category:");
        private FlabelConstant Calculating = new FlabelConstant("Calculating:");
        private FlabelConstant BasedOn = new FlabelConstant("Based on:");

        private QuickCalcPriceModel quickCalcPriceModelObj = new QuickCalcPriceModel();
        private SKUPricesModel SKUPricesModelObj = new SKUPricesModel();
        private CategoryPriceTierModel CategoryPriceTierModelObj = new CategoryPriceTierModel();
        private SKUModel SKUModelObj = new SKUModel();
        private int skuId = 0;
        private int categoryId = 0;

        public QuickCalcPrice(int skuId, int categoryId) : base("Quick Calculate Prices")
        {
            InitializeComponent();
            this.Size = new Size(800, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.skuId = skuId;
            this.categoryId = categoryId;

            InitInputBox();
            InitPriceTierGrid();

            InitMBOKButton();
        }

        private void InitPriceTierGrid()
        {
            PriceTierGridRefer = PriceTierGrid.GetGrid();
            PriceTierGridRefer.Location = new Point(30, 150);
            PriceTierGridRefer.Width = this.Width - 75;
            PriceTierGridRefer.Height = this.Height - 320;
            PriceTierGridRefer.VirtualMode = true;
            PriceTierGridRefer.KeyDown += DataGridView_KeyDown;
            PriceTierGridRefer.EditMode = DataGridViewEditMode.EditProgrammatically;
            PriceTierGridRefer.ReadOnly = false;
            PriceTierGridRefer.CellValueChanged += DataGridView_CellValueChanged;
            this.Controls.Add(PriceTierGridRefer);

            LoadPriceTierList();
        }

        private void InitMBOKButton()
        {
            hkSetPrice.SetPosition(new Point(30, 600));
            this.Controls.Add(hkSetPrice.GetButton());
            this.Controls.Add(hkSetPrice.GetLabel());

            hkSetPrice.GetButton().Click += (s, e) =>
            {
                ResetSkuPriceAll();
            };

            hkCancelCalc.SetPosition(new Point(30, 650));
            this.Controls.Add(hkCancelCalc.GetButton());
            this.Controls.Add(hkCancelCalc.GetLabel());
            hkCancelCalc.GetButton().Click += (s, e) => { this.Close(); };

            this.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.F2)
                {
                    ResetSkuPriceAll();
                };
            };
        }

        private void InitInputBox()
        {
            Category.SetPosition(new Point(30, 30));
            this.Controls.Add(Category.GetLabel());
            this.Controls.Add(Category.GetConstant());

            Calculating.SetPosition(new Point(30, 60));
            this.Controls.Add(Calculating.GetLabel());
            this.Controls.Add(Calculating.GetConstant());

            BasedOn.SetPosition(new Point(30, 90));
            this.Controls.Add(BasedOn.GetLabel());
            this.Controls.Add(BasedOn.GetConstant());

            if (this.skuId != 0)
            {
                var calcPriceInfo = quickCalcPriceModelObj.GetQuickCalcPriceInfo(this.skuId);
                Category.SetContext(calcPriceInfo.categoryName.Replace("&", "&&")); // "& stands for underscore -- use && to escape.
                if (calcPriceInfo.calculateAs == 1)
                    Calculating.SetContext("Markup");
                else Calculating.SetContext("Margin");

                double inventoryValue = SKUModelObj.GetInventoryValue(this.skuId);
                BasedOn.SetContext(inventoryValue.ToString());
            }
        }

        public void LoadPriceTierList()
        {
            var refreshData = quickCalcPriceModelObj.LoadCalcPrice(this.skuId);

            PriceTierGridRefer.DataSource = quickCalcPriceModelObj.calcPriceList;
            PriceTierGridRefer.Columns[0].HeaderText = "SkuPriceId";
            PriceTierGridRefer.Columns[0].Visible = false;
            PriceTierGridRefer.Columns[1].HeaderText = "PriceTierId";
            PriceTierGridRefer.Columns[1].Visible = false;
            PriceTierGridRefer.Columns[2].HeaderText = "Price Tier";
            PriceTierGridRefer.Columns[2].Width = PriceTierGridRefer.Width / 3;
            PriceTierGridRefer.Columns[3].HeaderText = "InventoryValue";
            PriceTierGridRefer.Columns[3].Visible = false;
            PriceTierGridRefer.Columns[4].HeaderText = "Margin";
            PriceTierGridRefer.Columns[4].Width = 200;
            PriceTierGridRefer.Columns[5].HeaderText = "ProfitMargin";
            PriceTierGridRefer.Columns[5].Visible = false;
            PriceTierGridRefer.Columns[6].HeaderText = "Price";
            PriceTierGridRefer.Columns[6].Width = 280;
            PriceTierGridRefer.Columns[7].HeaderText = "CategoryPriceTierId";
            PriceTierGridRefer.Columns[7].Visible = false;
        }

        private void DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;
            if (e.KeyCode == Keys.Enter)
            {

                if (dataGridView.CurrentCell != null && dataGridView.CurrentCell.IsInEditMode)
                {
                    //if (dataGridView.CurrentRow.Cells[dataGridView.CurrentCell.ColumnIndex + 1].Visible)
                    //{
                    e.Handled = true;
                    int nextColumnIndex = dataGridView.CurrentCell.ColumnIndex + 1;
                    dataGridView.CurrentCell = dataGridView.CurrentRow.Cells[nextColumnIndex];
                    dataGridView.BeginEdit(true);
                    //}
                    //else if (dataGridView.CurrentRow.Index < dataGridView.Rows.Count - 1)
                    //{
                    //    e.Handled = true;
                    //    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.CurrentRow.Index + 1].Cells[0];
                    //    dataGridView.BeginEdit(true);
                    //}
                }
                else if (dataGridView.CurrentCell != null && dataGridView.CurrentCell.IsInEditMode == false)
                {
                    if (dataGridView.CurrentCell.ColumnIndex == 4)
                    {
                        e.Handled = true;
                        dataGridView.BeginEdit(true);
                    }
                }
            }
        }

        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.ColumnIndex != 0 && e.ColumnIndex != 1)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                //DataGridViewCell cell = row.Cells[e.ColumnIndex];

                int skuId = this.skuId;
                int categoryId = this.categoryId;
                int priceTierId = int.Parse(row.Cells[0].Value.ToString());
                double inventoryValue = double.Parse(row.Cells[3].Value.ToString());
                double margin = double.Parse(row.Cells[4].Value.ToString());
                double profitMargin = double.Parse(row.Cells[5].Value.ToString());

                double price = SKUPricesModelObj.CalculateSKUPrice(inventoryValue, margin, profitMargin, priceTierId, categoryId);

                quickCalcPriceModelObj.calcPriceList[e.RowIndex].price = price;
            }
        }

        public void ResetSkuPriceAll()
        {
            List<SKUPrice> calcPriceList = quickCalcPriceModelObj.calcPriceList;

            int categoryId = this.categoryId;
            double categoryMargin = 0;
            int priceTierId = 0;
            int? categoryPriceTierId = 0;
            foreach (SKUPrice item in calcPriceList)
            {
                int skuPriceId = item.skuPriceId;
                int skuId = this.skuId;
                priceTierId = item.priceTierId;
                double price = item.price;
                categoryMargin = item.margin;
                bool active = true;
                int createdBy = 1;
                int updatedBy = 1;
                categoryPriceTierId = item.categoryPriceTierId;

                SKUPricesModelObj.UpdateSKUPrice(skuId, priceTierId, price, active, createdBy, updatedBy);

                if (categoryPriceTierId == null)
                {
                    CategoryPriceTierModelObj.AddCategoryPriceTier(categoryId, priceTierId, categoryMargin);
                }
                else
                {
                    CategoryPriceTierModelObj.UpdateCategoryPriceTier(categoryId, priceTierId, categoryMargin);
                }
            }

            MessageBox.Show("Set Prices successfully");
        }
    }
}
