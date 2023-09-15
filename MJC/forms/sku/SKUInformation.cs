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
using System.Configuration;
using MJC.forms.vendor;
using MJC.forms.sku.skuQuantityDiscount;
using MJC.forms.sales;
using Sentry.Extensibility;

namespace MJC.forms.sku
{
    public partial class SKUInformation : GlobalLayout
    {
        private HotkeyButton hkSKUMemo = new HotkeyButton("F2", "SKU Memo", Keys.F2);
        private HotkeyButton hkQuickCalcPrice = new HotkeyButton("F3", "Quick Calc Price", Keys.F3);
        private HotkeyButton hkMiscManagement = new HotkeyButton("F4", "Misc Management", Keys.F4);
        private HotkeyButton hkResetPrices = new HotkeyButton("F5", "Reset Prices", Keys.F5);
        private HotkeyButton hkSetArchived = new HotkeyButton("F9", "Set Archived", Keys.F9);
        private HotkeyButton hkSubAssembly = new HotkeyButton("F4", "Sub-assebmlies", Keys.F4);
        private HotkeyButton hkCrossReference = new HotkeyButton("F5", "Cross-references", Keys.F5);
        private HotkeyButton hkQuantityDiscount = new HotkeyButton("F6", "Quantity discounts", Keys.F6);

        private FGroupLabel SKUInfo = new FGroupLabel("SKU Info");
        private FInputBox SKUName = new FInputBox("SKU#");
        private FComboBox categoryCombo = new FComboBox("Category");
        private FInputBox description = new FInputBox("Description");
        private FInputBox measurementUnit = new FInputBox("Unit of Measure");
        private FInputBox weight = new FInputBox("Weight/UOM");
        private FInputBox costCode = new FInputBox("Sales/Cost Code");
        private FInputBox assetAcct = new FInputBox("SKU Asset Acct");

        private FCheckBox taxable = new FCheckBox("taxable");
        private FCheckBox maintainQtys = new FCheckBox("Maintain Qtys");
        private FCheckBox allowDiscount = new FCheckBox("Allow discount");
        private FCheckBox commissionable = new FCheckBox("Commissionable");

        private FComboBox orderForm = new FComboBox("Order From");

        //        orderForm
        private FDateTime lastSold = new FDateTime("Last Sold");
        private FInputBox manufacturer = new FInputBox("Manufacturer");
        private FInputBox location = new FInputBox("Location");

        private FGroupLabel quantityTracking = new FGroupLabel("Quantity Tracking");
        private FCheckBox editingQuantity = new FCheckBox("Enable Editing Quantities");
        private FInputBox quantity = new FInputBox("Quantity");
        private FInputBox qtyAllocated = new FInputBox("Qty Allocated");
        private FInputBox qtyAvaiable = new FInputBox("Qty Available");
        private FInputBox criticalQty = new FInputBox("Cirtical Qty");
        private FInputBox recorderQty = new FInputBox("Recorder Qty");

        private FGroupLabel sales = new FGroupLabel("Sales");
        private FInputBox soldThisMonth = new FInputBox("Sold this Month");
        private FInputBox soldYTD = new FInputBox("Sold YTD");

        private FGroupLabel prices = new FGroupLabel("Prices");
        private FCheckBox freezePrices = new FCheckBox("Freeze prices");
        private FCheckBox billAsLabor = new FCheckBox("Bill as Labor");
        private FInputBox coreCost = new FInputBox("Core Cost");
        private FInputBox invValue = new FInputBox("Inv Value");

        private FInputBox[] priceTiers;

        private PriceTiersModel PriceTiersModelObj = new PriceTiersModel();
        private CategoriesModel CategoriesModelObj = new CategoriesModel();
        private SKUPricesModel SKUPricesModelObj = new SKUPricesModel();
        private VendorsModel VendorsModelObj = new VendorsModel();
        private SKUModel SKUModelObj = new SKUModel();

        private int selectedCategoryId = 0;
        private int skuId = 0;
        private string memo = "";
        private bool disabled = false;

        public SKUInformation(bool disabled = false) : base("SKU Information", "Manage details of SKU")
        {
            this.Text = "Sku detail";
            InitializeComponent();
            _initBasicSize();
            this.disabled = disabled;
            this.KeyDown += (s, e) => Form_KeyDown(s, e);
            this.disabled = disabled;

            HotkeyButton[] hkButtons;
            if (disabled)
            {
                hkButtons = new HotkeyButton[4] { hkSKUMemo, hkSubAssembly, hkCrossReference, hkQuantityDiscount };
            }
            else
            {
                hkButtons = new HotkeyButton[5] { hkSKUMemo, hkQuickCalcPrice, hkMiscManagement, hkResetPrices, hkSetArchived };
            }

            _initializeHKButtons(hkButtons, false);
            AddHotKeyEvents();
            allowDiscount.GetCheckBox().Width = 200;
            InitForm();
            this.Load += new System.EventHandler(this.Add_Load);
        }

        private void AddHotKeyEvents()
        {
            hkSKUMemo.GetButton().Click += (sender, e) =>
            {
                SKUMemo MemoModal = new SKUMemo(skuId, memo);
                this.Enabled = false;
                MemoModal.Show();
                MemoModal.FormClosed += (ss, sargs) =>
                {
                    this.memo = MemoModal.getMemo();
                    this.Enabled = true;
                };
            };
            hkQuickCalcPrice.GetButton().Click += (sender, e) =>
            {
                QuickCalcPrice quickCalcPriceModal = new QuickCalcPrice(this.skuId, this.selectedCategoryId);
                this.Enabled = false;
                quickCalcPriceModal.Show();
                quickCalcPriceModal.FormClosed += (ss, sargs) =>
                {
                    this.Enabled = true;
                };
            };
            hkSubAssembly.GetButton().Click += (sender, e) =>
            {
                SubAssemblies subAssemblies = new SubAssemblies(skuId, this.disabled);
                _navigateToForm(sender, e, subAssemblies);
                this.Hide();
            };
            hkCrossReference.GetButton().Click += (sender, e) =>
            {
                CrossReference crossRefModal = new CrossReference(skuId, this.disabled);
                _navigateToForm(sender, e, crossRefModal);
                this.Hide();
            };
            hkQuantityDiscount.GetButton().Click += (sender, e) =>
            {
                SKUQuantityDiscount sKUQuantityDiscountModal = new SKUQuantityDiscount(skuId, this.disabled);
                _navigateToForm(sender, e, sKUQuantityDiscountModal);
                this.Hide();
            };
            hkMiscManagement.GetButton().Click += (sender, e) =>
            {
                MiscManagement MiscManagementActionsModal = new MiscManagement();
                this.Enabled = false;
                MiscManagementActionsModal.Show();
                MiscManagementActionsModal.FormClosed += (ss, sargs) =>
                {
                    this.Enabled = true;
                    int saveFlag = MiscManagementActionsModal.GetSaveFlage();
                    switch (saveFlag)
                    {
                        case 1:
                            VendorCosts vendorCostModal = new VendorCosts(skuId);
                            _navigateToForm(sender, e, vendorCostModal);
                            this.Hide();
                            break;
                        case 2:
                            CrossReference crossRefModal = new CrossReference(skuId, this.disabled);
                            _navigateToForm(sender, e, crossRefModal);
                            this.Hide();
                            break;
                        case 3:
                            SubAssemblies subAssemblies = new SubAssemblies(skuId, this.disabled);
                            _navigateToForm(sender, e, subAssemblies);
                            this.Hide();
                            break;
                        case 4:
                            SKUCostQuantity skuCostQuantityModal = new SKUCostQuantity(skuId, this.disabled);
                            _navigateToForm(sender, e, skuCostQuantityModal);
                            this.Hide();
                            break;
                        case 5:
                            SKUSerialLots skuSerialLotModal = new SKUSerialLots(skuId);
                            _navigateToForm(sender, e, skuSerialLotModal);
                            this.Hide();
                            break;
                        case 6:
                            SKUQuantityDiscount sKUQuantityDiscountModal = new SKUQuantityDiscount(skuId, this.disabled);
                            _navigateToForm(sender, e, sKUQuantityDiscountModal);
                            this.Hide();
                            break;
                        case 7:
                            SalesHisotry salesHistoryModal = new SalesHisotry(skuId);
                            _navigateToForm(sender, e, salesHistoryModal);
                            this.Hide();
                            break;

                    }
                };
            };
            hkResetPrices.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to reset Prices?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (selectedCategoryId != 0)
                    {
                        CategoryData category = CategoriesModelObj.LoadCategoryById(selectedCategoryId);
                        int categoryId = selectedCategoryId;
                        int calculateAs = category.calculateAs;

                        SKUModelObj.SetPrice(categoryId, calculateAs);
                    }
                }
            };
            hkSetArchived.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to reset archived?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SKUModelObj.UpdateSKUArchived(true, this.skuId);
                }
                else if (result == DialogResult.No)
                {
                    SKUModelObj.UpdateSKUArchived(false, this.skuId);
                }
            };
        }

        private void InitForm()
        {

            Panel _panel = new System.Windows.Forms.Panel();
            _panel.BackColor = System.Drawing.Color.Transparent;
            _panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            _panel.Location = new System.Drawing.Point(0, 95);
            _panel.Size = new Size(this.Width - 15, this.Height - 340);
            _panel.AutoScroll = true;
            this.Controls.Add(_panel);

            List<dynamic> FormComponents = new List<dynamic>();

            FormComponents.Add(SKUInfo);
            FormComponents.Add(SKUName);
            FormComponents.Add(categoryCombo);
            FormComponents.Add(description);
            FormComponents.Add(measurementUnit);
            FormComponents.Add(weight);
            FormComponents.Add(costCode);
            FormComponents.Add(assetAcct);

            List<dynamic> LineComponents = new List<dynamic>();

            LineComponents.Add(taxable);
            LineComponents.Add(maintainQtys);
            FormComponents.Add(LineComponents);

            List<dynamic> LineComponents2 = new List<dynamic>();

            LineComponents2.Add(allowDiscount);
            LineComponents2.Add(commissionable);
            FormComponents.Add(LineComponents2);

            FormComponents.Add(orderForm);
            FormComponents.Add(lastSold);
            FormComponents.Add(manufacturer);
            FormComponents.Add(location);
            _addFormInputs(FormComponents, 30, 20, 800, 50, 700, _panel.Controls);

            List<dynamic> FormComponents2 = new List<dynamic>();
            FormComponents2.Add(quantityTracking);
            FormComponents2.Add(editingQuantity);
            FormComponents2.Add(quantity);

            quantity.GetTextBox().Enabled = false;
            qtyAllocated.GetTextBox().Enabled = false;
            qtyAvaiable.GetTextBox().Enabled = false;
            criticalQty.GetTextBox().Enabled = false;
            recorderQty.GetTextBox().Enabled = false;

            FormComponents2.Add(qtyAllocated);
            FormComponents2.Add(qtyAvaiable);

            FormComponents2.Add(criticalQty);
            FormComponents2.Add(recorderQty);

            FormComponents2.Add(sales);

            FormComponents2.Add(soldThisMonth);
            FormComponents2.Add(soldYTD);

            FormComponents2.Add(prices);
            FormComponents2.Add(freezePrices);
            FormComponents2.Add(billAsLabor);
            FormComponents2.Add(coreCost);
            FormComponents2.Add(invValue);

            measurementUnit.GetTextBox().MaxLength = 2;
            costCode.GetTextBox().KeyPress += KeyValidateNumber;
            assetAcct.GetTextBox().KeyPress += KeyValidateNumber;
            manufacturer.GetTextBox().KeyPress += KeyValidateNumber;
            quantity.GetTextBox().KeyPress += KeyValidateNumber;
            qtyAllocated.GetTextBox().KeyPress += KeyValidateNumber;
            qtyAvaiable.GetTextBox().KeyPress += KeyValidateNumber;
            criticalQty.GetTextBox().KeyPress += KeyValidateNumber;
            recorderQty.GetTextBox().KeyPress += KeyValidateNumber;
            soldThisMonth.GetTextBox().KeyPress += KeyValidateNumber;
            soldYTD.GetTextBox().KeyPress += KeyValidateNumber;

            orderForm.GetComboBox().DropDownStyle = ComboBoxStyle.DropDownList;



            List<KeyValuePair<int, string>> VendorsData = VendorsModelObj.GetVendorList();
            foreach (KeyValuePair<int, string> pair in VendorsData)
            {
                orderForm.GetComboBox().Items.Add(new FComboBoxItem(pair.Key, pair.Value));
            }

            string filter = "";
            var refreshData = PriceTiersModelObj.LoadPriceTierData(filter);
            if (refreshData)
            {
                List<PriceTierData> pDatas = PriceTiersModelObj.PriceTierDataList;

                priceTiers = new FInputBox[pDatas.Count];
                for (int i = 0; i < pDatas.Count; i++)
                {
                    priceTiers[i] = new FInputBox(pDatas[i].Name.ToString(), 200, pDatas[i].Id);
                    FormComponents2.Add(priceTiers[i]);
                }
            }

            editingQuantity.GetCheckBox().CheckStateChanged += new EventHandler(EditingQuantity_Changed);

            if (this.disabled)
            {
                SKUName.GetTextBox().Enabled = false;
                categoryCombo.GetComboBox().Enabled = false;
                description.GetTextBox().Enabled = false;
                measurementUnit.GetTextBox().Enabled = false;
                weight.GetTextBox().Enabled = false;
                costCode.GetTextBox().Enabled = false;
                assetAcct.GetTextBox().Enabled = false;
                taxable.GetCheckBox().Enabled = false;
                maintainQtys.GetCheckBox().Enabled = false;
                allowDiscount.GetCheckBox().Enabled = false;
                commissionable.GetCheckBox().Enabled = false;
                orderForm.GetComboBox().Enabled = false;
                lastSold.GetDateTimePicker().Enabled = false;
                manufacturer.GetTextBox().Enabled = false;
                location.GetTextBox().Enabled = false;
                quantity.GetTextBox().Enabled = false;
                qtyAllocated.GetTextBox().Enabled = false;
                qtyAvaiable.GetTextBox().Enabled = false;
                criticalQty.GetTextBox().Enabled = false;
                recorderQty.GetTextBox().Enabled = false;
                soldThisMonth.GetTextBox().Enabled = false;
                soldYTD.GetTextBox().Enabled = false;
                freezePrices.GetCheckBox().Enabled = false;
                coreCost.GetTextBox().Enabled = false;
                invValue.GetTextBox().Enabled = false;

                for (int i = 0; i < priceTiers.Length; i++)
                {
                    priceTiers[i].GetTextBox().Enabled = false;
                }
            }

            _addFormInputs(FormComponents2, 700, 20, 800, 50, int.MaxValue, _panel.Controls);
        }

        public void setDetails(List<dynamic> data, int id)
        {
            this.skuId = id;
            this.selectedCategoryId = (int)data[0].category;
            if (data[0].memo != null && !data[0].memo.Equals(DBNull.Value)) this.memo = data[0].memo;

            this.SKUName.GetTextBox().Text = data[0].sku.ToString();
            this.description.GetTextBox().Text = data[0].description.ToString();
            this.measurementUnit.GetTextBox().Text = data[0].measurementUnit.ToString();
            this.weight.GetTextBox().Text = data[0].weight.ToString();
            this.costCode.GetTextBox().Text = data[0].costCode.ToString();
            this.assetAcct.GetTextBox().Text = data[0].assetAccount.ToString();
            this.taxable.GetCheckBox().Checked = (bool)data[0].taxable;
            this.maintainQtys.GetCheckBox().Checked = (bool)data[0].manageStock;
            this.allowDiscount.GetCheckBox().Checked = (bool)data[0].allowDiscounts;


            foreach (FComboBoxItem item in orderForm.GetComboBox().Items)
            {
                if (item.Id == data[0].orderFrom)
                {
                    orderForm.GetComboBox().SelectedItem = item;
                    break;
                }
            }

            if (data[0].lastSold != null && !data[0].lastSold.Equals(DBNull.Value)) this.lastSold.GetDateTimePicker().Value = data[0].lastSold.ToLocalTime();

            this.manufacturer.GetTextBox().Text = data[0].manufacturer.ToString();
            this.location.GetTextBox().Text = data[0].location.ToString();

            if (data[0].editingQuantity == null) data[0].editingQuantity = false;

            this.editingQuantity.GetCheckBox().Checked = (bool)data[0].editingQuantity;
            this.quantity.GetTextBox().Text = data[0].quantity.ToString();
            this.qtyAllocated.GetTextBox().Text = data[0].qtyAllocated.ToString();
            this.qtyAvaiable.GetTextBox().Text = data[0].qtyAvailable.ToString();
            this.criticalQty.GetTextBox().Text = data[0].qtyCritical.ToString();
            this.recorderQty.GetTextBox().Text = data[0].qtyReorder.ToString();

            this.freezePrices.GetCheckBox().Checked = (bool)data[0].freezePrices;
            this.billAsLabor.GetCheckBox().Checked = (bool)data[0].billAsLabor;

            this.soldThisMonth.GetTextBox().Text = data[0].soldMonthToDate.ToString();
            this.soldYTD.GetTextBox().Text = data[0].soldYearToDate.ToString();

            this.coreCost.GetTextBox().Text = data[0].coreCost.ToString();
            this.invValue.GetTextBox().Text = data[0].inventoryValue.ToString();

            List<KeyValuePair<int, double>> skuPriceData = new List<KeyValuePair<int, double>>();
            skuPriceData = SKUPricesModelObj.LoadPriceTierDataBySKUId(id);

            foreach (KeyValuePair<int, double> pair in skuPriceData)
            {
                for (int i = 0; i < priceTiers.Length; i++)
                    if (priceTiers[i].GetId() == pair.Key)
                        priceTiers[i].GetTextBox().Text = pair.Value.ToString("#0.00");
            }
        }

        private void Add_Load(object sender, EventArgs e)
        {
            bool initFlag = true;
            categoryCombo.GetComboBox().Items.Clear();
            List<KeyValuePair<int, string>> CategoryList = new List<KeyValuePair<int, string>>();
            CategoryList = CategoriesModelObj.GetCategoryItems();
            foreach (KeyValuePair<int, string> item in CategoryList)
            {
                int id = item.Key;
                string name = item.Value;
                if (initFlag && selectedCategoryId == 0)
                {
                    selectedCategoryId = id;
                    initFlag = false;
                }
                categoryCombo.GetComboBox().Items.Add(new FComboBoxItem(id, name));
            }

            foreach (FComboBoxItem item in categoryCombo.GetComboBox().Items)
            {
                if (item.Id == selectedCategoryId)
                {
                    categoryCombo.GetComboBox().SelectedItem = item;
                    break;
                }
            }
        }

        private void EditingQuantity_Changed(object sender, EventArgs e)
        {
            if (editingQuantity.GetCheckBox().Checked)
            {
                quantity.GetTextBox().Enabled = true;
                qtyAllocated.GetTextBox().Enabled = true;
                qtyAvaiable.GetTextBox().Enabled = true;
                criticalQty.GetTextBox().Enabled = true;
                recorderQty.GetTextBox().Enabled = true;
            }
            else
            {
                quantity.GetTextBox().Enabled = false;
                qtyAllocated.GetTextBox().Enabled = false;
                qtyAvaiable.GetTextBox().Enabled = false;
                criticalQty.GetTextBox().Enabled = false;
                recorderQty.GetTextBox().Enabled = false;
            }
        }

        private void saveSKU(object sender, EventArgs e)
        {
            string s_sku_name = SKUName.GetTextBox().Text;
            if (string.IsNullOrWhiteSpace(s_sku_name))
            {
                Messages.ShowError("Please enter a SKU Name.");
                this.SKUName.GetTextBox().Select();
                return;
            }
            if (categoryCombo.GetComboBox().SelectedItem == null)
            {
                Messages.ShowError("Please select a category.");
                return;
            }
            FComboBoxItem seletedItem = (FComboBoxItem)categoryCombo.GetComboBox().SelectedItem;

            int i_category = seletedItem.Id;

            string s_description = description.GetTextBox().Text;
            string s_measurement_unit = measurementUnit.GetTextBox().Text;
            int i_weight; bool is_i_weight = int.TryParse(weight.GetTextBox().Text, out i_weight);
            if (!is_i_weight) i_weight = 0;
            int i_cost_code; bool is_i_cost_code = int.TryParse(costCode.GetTextBox().Text, out i_cost_code);
            if (!is_i_cost_code) i_cost_code = 0;
            int i_asset_acct; bool is_i_asset_acct = int.TryParse(assetAcct.GetTextBox().Text, out i_asset_acct);
            if (!is_i_asset_acct) i_asset_acct = 0;

            bool b_taxable = taxable.GetCheckBox().Checked;
            bool b_maintain_qty = maintainQtys.GetCheckBox().Checked;
            bool b_allow_discount = allowDiscount.GetCheckBox().Checked;
            bool b_commissionable = commissionable.GetCheckBox().Checked;

            FComboBoxItem selectedItem = (FComboBoxItem)orderForm.GetComboBox().SelectedItem;
            int i_order_from = selectedItem.Id;

            DateTime d_last_sold = lastSold.GetDateTimePicker().Value;

            string s_manufacturer = manufacturer.GetTextBox().Text;
            string s_location = location.GetTextBox().Text;
            string memo = this.memo;

            int i_quantity; bool is_i_quantity = int.TryParse(quantity.GetTextBox().Text, out i_quantity);
            if (!is_i_quantity) i_quantity = 0;
            int i_qty_allocated; bool is_i_qty_allocated = int.TryParse(qtyAllocated.GetTextBox().Text, out i_qty_allocated);
            if (!is_i_qty_allocated) i_qty_allocated = 0;
            int i_qty_available; bool is_i_qty_available = int.TryParse(qtyAvaiable.GetTextBox().Text, out i_qty_available);
            if (!is_i_qty_available) i_qty_available = 0;
            int i_qty_critical; bool is_i_qty_critical = int.TryParse(criticalQty.GetTextBox().Text, out i_qty_critical);
            if (!is_i_qty_critical) i_qty_critical = 0;
            int i_qty_reorder; bool is_i_qty_reorder = int.TryParse(recorderQty.GetTextBox().Text, out i_qty_reorder);
            if (!is_i_qty_reorder) i_qty_reorder = 0;

            int i_sold_this_month; bool is_i_solid_this_month = int.TryParse(soldThisMonth.GetTextBox().Text, out i_sold_this_month);
            if (!is_i_solid_this_month) i_sold_this_month = 0;
            int i_sold_ytd; bool is_i_sold_ytd = int.TryParse(soldYTD.GetTextBox().Text, out i_sold_ytd);
            if (!is_i_sold_ytd) i_sold_ytd = 0;

            bool b_freeze_prices = freezePrices.GetCheckBox().Checked;
            bool b_bill_as_Labor = billAsLabor.GetCheckBox().Checked;
            bool b_editing_quantity = editingQuantity.GetCheckBox().Checked;

            double i_core_cost; bool is_i_core_cost = double.TryParse(coreCost.GetTextBox().Text, out i_core_cost);
            if (!is_i_core_cost) i_core_cost = 0;
            double i_inv_value; bool is_i_inv_value = double.TryParse(invValue.GetTextBox().Text, out i_inv_value);
            if (!is_i_inv_value) i_inv_value = 0;
            if (s_sku_name == "")
            {
                MessageBox.Show("Please fill String field.");
                return;
            }

            Dictionary<int, double> priceTierDict = new Dictionary<int, double>();

            for (int i = 0; i < priceTiers.Length; i++)
            {
                double priceData; bool parse_succeed = double.TryParse(priceTiers[i].GetTextBox().Text, out priceData);
                if (parse_succeed) priceTierDict.Add(priceTiers[i].GetId(), priceData);
            }
            string syncToken = "1";
            string qboSkuId = "3";
            bool hidden = false;

            bool refreshData = false;
            if (skuId == 0)
            {
                refreshData = Session.SKUModelObj.AddSKU(s_sku_name, i_category, s_description, s_measurement_unit, i_weight, i_cost_code, i_asset_acct, b_taxable, b_maintain_qty, b_allow_discount, b_commissionable, i_order_from, d_last_sold, s_manufacturer, s_location, i_quantity, i_qty_allocated, i_qty_available, i_qty_critical, i_qty_reorder, i_sold_this_month, i_sold_ytd, b_freeze_prices, i_core_cost, i_inv_value, memo, priceTierDict, b_bill_as_Labor, syncToken, qboSkuId, hidden, b_editing_quantity);
            }
            else refreshData = Session.SKUModelObj.UpdateSKU(s_sku_name, i_category, s_description, s_measurement_unit, i_weight, i_cost_code, i_asset_acct, b_taxable, b_maintain_qty, b_allow_discount, b_commissionable, i_order_from, d_last_sold, s_manufacturer, s_location, i_quantity, i_qty_allocated, i_qty_available, i_qty_critical, i_qty_reorder, i_sold_this_month, i_sold_ytd, b_freeze_prices, i_core_cost, i_inv_value, memo, priceTierDict, b_bill_as_Labor, b_editing_quantity, skuId);
            string modeText = skuId == 0 ? "creating" : "updating";
            if (refreshData)
            {
                this.DialogResult = DialogResult.OK;
                this._navigateToPrev(sender, e);
            }
            else MessageBox.Show("An Error occured while " + modeText + " the vendor.");
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (this.disabled == false)
                {
                    DialogResult result = MessageBox.Show("Do you want to save your changes?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        saveSKU(sender, e);
                    }
                    else if (result == DialogResult.No)
                    {
                        _navigateToPrev(sender, e);
                    }
                }
                else
                {
                    _navigateToPrev(sender, e);
                }
            }
        }

        private void KeyValidateNumber(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void SKUInformation_Load(object sender, EventArgs e)
        {

        }
    }
}
