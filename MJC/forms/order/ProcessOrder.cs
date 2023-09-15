using MJC.common.components;
using MJC.common;
using MJC.model;
using System.ComponentModel;
using System.Data;
using MJC.forms.sku;
using MJC.qbo;
using System;

namespace MJC.forms.order
{
    public partial class ProcessOrder : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdit = new HotkeyButton("Enter", "Edits", Keys.F6);
        private HotkeyButton hkSaveOrder = new HotkeyButton("F1", "Save Order", Keys.F1);
        private HotkeyButton hkAddMessage = new HotkeyButton("F2", "Add message", Keys.F2);
        private HotkeyButton hkCustomerProfiler = new HotkeyButton("F4", "Customer Profiler", Keys.F4);
        private HotkeyButton hkSKUInfo = new HotkeyButton("F5", "SKU Info", Keys.F5);
        private HotkeyButton hkSortLines = new HotkeyButton("Alt+S", "Sort lines", Keys.S, "alt");
        private HotkeyButton hkCloseOrder = new HotkeyButton("ESC", "Close order", Keys.Escape);
        private HotkeyButton hkShippingInformation = new HotkeyButton("F6", "Shipping Information", Keys.F6);
        private FCheckBox ShipOrder = new FCheckBox("Ship Order");

        private FComboBox Customer_ComBo = new FComboBox("Customer#:", 150);
        private FlabelConstant CustomerName = new FlabelConstant("Name:", 150);
        private FlabelConstant Terms = new FlabelConstant("Terms:", 150);
        //private FlabelConstant Zone = new FlabelConstant("Zone", 150);
        // private FlabelConstant Position = new FlabelConstant("PO#:", 150);

        private FlabelConstant Requested = new FlabelConstant("Requested:");
        private FlabelConstant Filled = new FlabelConstant("Filled:");
        private FlabelConstant QtyOnHold = new FlabelConstant("Qty on Hand:");
        private FlabelConstant QtyAllocated = new FlabelConstant("Qty Allocated:");
        private FlabelConstant QtyAvailable = new FlabelConstant("Qty Available:");
        private FlabelConstant Subtotal = new FlabelConstant("Subtotal:");
        private FlabelConstant TaxPercent = new FlabelConstant("7.250% Tax:");
        private FlabelConstant TotalSale = new FlabelConstant("Total Sale:");

        private DataGridView POGridRefer;
        private int POGridSelectedIndex = 0;
        private int customerId = 0;
        private int skuId = 0;
        private int flag = 0;
        private int addedRowIndex = -1;
        private bool isAddNewOrderItem = false;
        private int selectedOrderId = 0;
        private bool isNewOrder = true;
        private int orderId = 0;
        private int oldCustomerIndex = 0;
        private bool changeDetected = true;
        private string searchKey;
        private decimal billAsLabor = 0;
        
        private List<OrderItem> OrderItemData = new List<OrderItem>();
        private List<SKUOrderItem> TotalSkuList = new List<SKUOrderItem>();
        private List<SKUOrderItem> SubSkuList = new List<SKUOrderItem>();

        private string Message;

        public ProcessOrder(int customerId = 0, int orderId = 0, bool isAddNewOrderItem = false) : base("Process an Order", "Fill out the customer order")
        {

            InitializeComponent();
            _initBasicSize();

            this.isAddNewOrderItem = isAddNewOrderItem;
            this.customerId = customerId;
            this.selectedOrderId = orderId;
            this.orderId = selectedOrderId;

            if (this.selectedOrderId != 0)
            {
                isNewOrder = false;
            }

            if (this.orderId == 0)
            {
                this.orderId = Session.OrderModelObj.GetNextOrderId();
            }

            dynamic customer = Session.CustomersModelObj.GetCustomerDataById(customerId);
            this.TotalSkuList = Session.SKUModelObj.LoadSkuOrderItems();
            Session.SKUModelObj.LoadSKUData("", false);

            // Default customer to the first priceTierId
            if (customer?.priceTierId == null)
            {
                customer.priceTierId = 1;
            }

            int priceTierId;
            if (customer != null && int.TryParse(customer?.priceTierId?.ToString() ?? "0", out priceTierId))
            {
                this.SubSkuList = this.TotalSkuList.Where(sku => sku.PriceTierId == customer.priceTierId).ToList();
            }
            else
            {
                this.SubSkuList = this.TotalSkuList;
            }

            // HotkeyButton[] hkButtons = new HotkeyButton[9] { hkAdds, hkDeletes, hkEdit, hkSaveOrder, hkAddMessage, hkCustomerProfiler, hkSKUInfo, hkSortLines, hkCloseOrder };
            HotkeyButton[] hkButtons = new HotkeyButton[] { hkAdds, hkDeletes, hkEdit, hkAddMessage, hkCustomerProfiler, hkSKUInfo, hkSortLines, hkCloseOrder, hkShippingInformation };

            _initializeHKButtons(hkButtons, false);
            AddHotKeyEvents();

            InitOrderItemsList();
            InitCustomerInfo(this.customerId);

            InitGridFooter();
        }

        private void printInvoice(int orderId, int orderStatus)
        {
            string totalSale = TotalSale.GetConstant().Text;
            string taxValue = TaxPercent.GetConstant().Text;
            string subTotal = Subtotal.GetConstant().Text;
            string coreValue = "0.00";
            string laborValue = this.billAsLabor.ToString("0.00");
            OrderPrint orderPrint = new OrderPrint(orderId, orderStatus, subTotal, taxValue, laborValue, coreValue, totalSale);
            orderPrint.PrintForm();
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (s, e) => InsertItem(s, e);
            hkSortLines.GetButton().Click += (s, e) =>
            {
                OrderItemData = OrderItemData.OrderBy(x => x.Sku).ToList();
                POGridRefer.DataSource = this.OrderItemData;
            };
            //hkEdit.GetButton().Click += (s, e) =>
            //{
            //    EditItem(s, e);
            //};

            hkCloseOrder.GetButton().Click += async (sender, e) =>
            {
                CloseOrderActions CloseOrderActionsModal = new CloseOrderActions();
                this.Enabled = false;
                CloseOrderActionsModal.Show();
                CloseOrderActionsModal.FormClosed += async (ss, sargs) =>
                {
                    this.Enabled = true;
                    int saveFlag = CloseOrderActionsModal.GetSaveFlage();
                    if (saveFlag == 7)
                    {
                        Session.OrderModelObj.DeleteOrder(orderId);

                        _navigateToPrev(sender, e);
                    }

                    int status = 0;

                    if (POGridRefer.Rows.Count > 0)
                    {
                        int rowIndex = POGridRefer.SelectedRows[0].Index;
                        DataGridViewRow row = POGridRefer.Rows[rowIndex];

                        if (saveFlag != 0 && saveFlag != 7)
                        {
                            if (orderId != 0)
                            {
                                if (saveFlag == 1)
                                {
                                    if (await CreateInvoice())
                                    {

                                        status = 3;
                                        Session.OrderModelObj.UpdateOrderStatus(status, orderId);
                                        printInvoice(orderId, status);
                                        _navigateToPrev(sender, e);
                                    }
                                }
                                else if (saveFlag == 2)
                                {
                                    if (await CreateInvoice())
                                    {

                                        status = 3;
                                        Session.OrderModelObj.UpdateOrderStatus(status, orderId);
                                        printInvoice(orderId, status);

                                        _navigateToPrev(sender, e);
                                    }
                                }
                                else if (saveFlag == 3)
                                {
                                    status = 2;
                                    Session.OrderModelObj.UpdateOrderStatus(status, orderId);
                                    printInvoice(orderId, status);

                                    _navigateToPrev(sender, e);
                                }
                                else if (saveFlag == 4)
                                {
                                    if (await CreateInvoice())
                                    {

                                        status = 2;
                                        Session.OrderModelObj.UpdateOrderStatus(status, orderId);
                                        printInvoice(orderId, status);

                                        _navigateToPrev(sender, e);
                                    }
                                }
                                else if (saveFlag == 5)
                                {
                                    status = 1;
                                    Session.OrderModelObj.UpdateOrderStatus(status, orderId);
                                    _navigateToPrev(sender, e);
                                }
                                else if (saveFlag == 6)
                                {
                                    if (await CreateInvoice())
                                    {
                                        status = 1;
                                        Session.OrderModelObj.UpdateOrderStatus(status, orderId);
                                        printInvoice(orderId, status);
                                        _navigateToPrev(sender, e);
                                    }
                                }
                                else if (saveFlag == 7)
                                {
                                    _navigateToPrev(sender, e);
                                }
                                else if (saveFlag == 8)
                                {

                                }
                            }
                            else
                            {
                                //MessageBox.Show("Order info is not saved yet, please save order info first");
                            }
                        }
                    }
                };
            };
            hkDeletes.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this row?", "Confirm Delete", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    int selectedOrderId = 0;
                    if (POGridRefer.SelectedRows.Count > 0)
                    {
                        foreach (DataGridViewRow row in POGridRefer.SelectedRows)
                        {
                            selectedOrderId = (int)row.Cells[0].Value;
                        }
                    }
                }
            };
            hkCustomerProfiler.GetButton().Click += (sender, e) =>
            {
                CustomerData selectedItem = (CustomerData)Customer_ComBo.GetComboBox().SelectedItem;
                int customerId = selectedItem.Id;

                CustomerProfile customerProfileModal = new CustomerProfile(customerId);
                _navigateToForm(sender, e, customerProfileModal);
                this.Hide();
            };
            hkSKUInfo.GetButton().Click += (sender, e) =>
            {
                if (POGridRefer.RowCount > 0)
                {
                    SKUInformation detailModal = new SKUInformation(true);

                    int rowIndex = POGridRefer.SelectedRows[0].Index;

                    DataGridViewRow row = POGridRefer.Rows[rowIndex];
                    int skuId = (int)row.Cells[2].Value;
                    List<dynamic> skuData = new List<dynamic>();
                    skuData = Session.SKUModelObj.GetSKUData(skuId);
                    detailModal.setDetails(skuData, skuData[0].id);

                    this.Hide();
                    _navigateToForm(sender, e, detailModal);
                }
            };
            hkAddMessage.GetButton().Click += (sender, e) =>
            {
                if (POGridRefer.RowCount > 0)
                {
                    OrderItemMessage detailModal = new OrderItemMessage();

                    int rowIndex = POGridRefer.SelectedRows[0].Index;

                    DataGridViewRow row = POGridRefer.Rows[rowIndex];
                    int orderItemId = (int)row.Cells[0].Value;

                    detailModal.setDetails(orderItemId > 0 ? orderItemId : 1);
                    detailModal.Message.GetTextBox().Text = Message;

                    if (detailModal.ShowDialog() == DialogResult.OK)
                    {
                        Message = detailModal.Message.GetTextBox().Text;
                    }
                }
            };
        }

        private void InitCustomerInfo(int customerId = 0)
        {
            List<dynamic> FormComponents = new List<dynamic>();

            FormComponents.Add(Customer_ComBo);
            FormComponents.Add(CustomerName);
            FormComponents.Add(Terms);
            FormComponents.Add(ShipOrder);
            //FormComponents.Add(Zone);
            // FormComponents.Add(Position);

            _addFormInputs(FormComponents, 30, 110, 650, 42, 180);

            var refreshData = Session.CustomersModelObj.LoadCustomerData("", false);

            Customer_ComBo.GetComboBox().DataSource = Session.CustomersModelObj.CustomerDataList;
            Customer_ComBo.GetComboBox().DisplayMember = "Num";
            Customer_ComBo.GetComboBox().ValueMember = "Id";

            Customer_ComBo.GetComboBox().SelectedValueChanged += new EventHandler(ProcessOrder_SelectedValueChanged);


            if (customerId == 0)
            {
                Customer_ComBo.GetComboBox().SelectedIndex = 0;
                Customer_SelectedIndexChanged(Customer_ComBo.GetComboBox(), EventArgs.Empty);
            }
            else
            {
                Customer_ComBo.GetComboBox().SelectedValue = customerId;
            }

            LoadSelectedCustomerData();

            //Position.GetLabel().Focus();
            //POGridRefer.Select();
        }

        private void LoadSelectedCustomerData()
        {
            CustomerData selectedItem = (CustomerData)Customer_ComBo.GetComboBox().SelectedItem;
            int customerId = selectedItem.Id;
            this.customerId = customerId;

            var customerData = Session.CustomersModelObj.GetCustomerData(customerId);
            if (customerData != null)
            {
                if (customerData.customerName != "") CustomerName.SetContext(customerData.customerName);
                else CustomerName.SetContext("N/A");

                if (customerData.terms != "") Terms.SetContext(customerData.terms);
                else Terms.SetContext("N/A");

                //if(customerData.zipcode != "") Zone.SetContext(customerData.zipcode);
                //else Zone.SetContext("N/A");

                //if (!string.IsNullOrEmpty(customerData.poRequired))
                //{
                //    if (bool.Parse(customerData.poRequired))
                //        Position.SetContext("Yes");
                //    else Position.SetContext("No");
                //}
                //else Position.SetContext("N/A");

                this.TotalSkuList = Session.SKUModelObj.LoadSkuOrderItems();

            }

            if (POGridRefer != null)
                LoadOrderItemList();

            POGridRefer.Select();
        }

        private void ProcessOrder_SelectedValueChanged(object? sender, EventArgs e)
        {
            var index = Customer_ComBo.GetComboBox().SelectedIndex;
            if (index == oldCustomerIndex) return;

            LoadSelectedCustomerData();

            //if (!changeDetected || (MessageBox.Show("The current order will be lost. Are you sure you want to change the customer without saving the current changes?", "Change?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            //{

            //}
            //else
            //{
            //    Customer_ComBo.GetComboBox().SelectedIndex = oldCustomerIndex;
            //}

            oldCustomerIndex = Customer_ComBo.GetComboBox().SelectedIndex;
        }

        private void Customer_SelectedIndexChanged(object sender, EventArgs e)
        {


        }


        private void InitOrderItemsList()
        {
            GridViewOrigin OrderEntryLookupGrid = new GridViewOrigin();
            POGridRefer = OrderEntryLookupGrid.GetGrid();
            POGridRefer.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(157, 196, 235);
            POGridRefer.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 63, 96);
            POGridRefer.ColumnHeadersDefaultCellStyle.Padding = new Padding(12);
            POGridRefer.Location = new Point(0, 200);
            POGridRefer.Width = this.Width;
            POGridRefer.Height = 490;
            POGridRefer.AllowUserToAddRows = false;
            POGridRefer.ReadOnly = false;
            POGridRefer.KeyDown += DataGridView_KeyDown;

            POGridRefer.Select();

            POGridRefer.VirtualMode = true;
            POGridRefer.EditMode = DataGridViewEditMode.EditOnKeystroke;
            POGridRefer.EditingControlShowing += POGridRefer_EditingControlShowing;
            POGridRefer.DataError += POGridRefer_DataError;
            this.Controls.Add(POGridRefer);

            LoadOrderItemList();

            foreach (DataGridViewRow row in POGridRefer.Rows)
            {
                row.ReadOnly = false;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.ReadOnly = false;
                }
            }

            //if (this.isAddNewOrderItem)
            //{
            //    this.OrderItemData.Add(new OrderItem());
            //    BindingList<OrderItem> dataList = new BindingList<OrderItem>(this.OrderItemData);

            //    POGridRefer.DataSource = dataList;

            //    int rowIndex = POGridRefer.Rows.Count - 1;

            //    POGridRefer.CurrentCell = POGridRefer.Rows[rowIndex].Cells[3];
            //    POGridRefer.BeginEdit(true);
            //}
        }

        private void POGridRefer_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            Messages.ShowError("There was a problem setting the cell data.");
        }

        private void InitGridFooter()
        {
            List<dynamic> GridFooterComponents = new List<dynamic>();

            GridFooterComponents.Add(Requested);
            GridFooterComponents.Add(Filled);

            _addFormInputs(GridFooterComponents, 30, 680, 650, 42, 780);

            List<dynamic> GridFooterComponents1 = new List<dynamic>();

            GridFooterComponents1.Add(QtyOnHold);
            GridFooterComponents1.Add(QtyAllocated);
            GridFooterComponents1.Add(QtyAvailable);
            GridFooterComponents1.Add(Subtotal);
            GridFooterComponents1.Add(TaxPercent);
            GridFooterComponents1.Add(TotalSale);

            _addFormInputs(GridFooterComponents1, 680, 680, 650, 42, 780);


            LoadGridFooterInfo();
        }

        public void LoadGridFooterInfo()
        {
            this.skuId = SubSkuList[0].Id;

            PopulateInformationField();
        }

        private void PopulateInformationField()
        {
            if (this.skuId == 0) return;
            if (POGridRefer.SelectedRows.Count == 0) return;

            int skuId = this.skuId;
            var qtyInfo = Session.SKUModelObj.LoadSkuQty(skuId);

            SKUOrderItem sku = this.SubSkuList.FirstOrDefault(x => x.Id == skuId);
            
            SalesTaxCodeModel salesTaxCodeModel = new SalesTaxCodeModel();
            salesTaxCodeModel.LoadSalesTaxCodeData("");
             
            var items = Session.PriceTiersModelObj.GetPriceTierItems();
            var priceTierItem = items[sku.PriceTierId];

            // Make sure we have the default SKU #2 for tax code
            var taxCodeId = Session.SettingsModelObj.Settings.taxCodeId.GetValueOrDefault(2);
            var salesTaxCode = salesTaxCodeModel.GetSalesTaxCodeData(taxCodeId);
            var taxRate = salesTaxCode.rate;

            QtyOnHold.SetContext(qtyInfo.qty.ToString());
            QtyAllocated.SetContext(qtyInfo.qtyAllocated.ToString());
            QtyAvailable.SetContext(qtyInfo.qtyAvailable.ToString());

            DataGridViewRow selectedRow = POGridRefer.SelectedRows[0];

            var unitPrice = sku.Price;
            var quantity = selectedRow.Cells["quantity"].Value as int?;

            selectedRow.Cells["lineTotal"].Value = unitPrice * quantity;

            var total = 0.00;
            var tax = 0.00;

            foreach(var item in OrderItemData)
            {
                var _lineTotal = (item?.UnitPrice * item?.Quantity) ?? 0.00;
                var _taxAmount = _lineTotal * (taxRate / 100);
                double _billAsLabor = 0.0;
                if(item.BillAsLabor == true)
                {
                    _billAsLabor = _lineTotal * (taxRate / 100);
                    this.billAsLabor += Convert.ToDecimal(_billAsLabor);
                }
                var _totalAmount = _taxAmount + _lineTotal + _billAsLabor;
                total += _lineTotal;
                if (item?.Tax.GetValueOrDefault() ?? false)
                {
                    tax += _taxAmount;
                }
            }

            //var lineTotal = unitPrice * quantity;
            //var taxAmount = lineTotal * (taxRate / 100);
            var totalAmount = tax + total;

            TaxPercent.GetLabel().Text = $"{taxRate}% Tax:";

            Subtotal.SetContext(total.ToString("#,##0.00"));
            TaxPercent.SetContext(tax.ToString("0.00"));
            TotalSale.SetContext(totalAmount.ToString("$#,##0.00"));

            var requested = quantity;
            var filled = requested;
            if (requested > qtyInfo.qtyAvailable)
            {
                filled = qtyInfo.qtyAvailable;
            }

            Filled.SetContext(filled.ToString());
            Requested.SetContext(requested.ToString());
        }

        public void LoadOrderItemList(string sort = "")
        {
            this.OrderItemData = new List<OrderItem>(); // this.selectedOrderId

            if (!isAddNewOrderItem)
            {
                OrderItemData = Session.OrderItemModelObj.GetOrderItemsListByCustomerId(this.customerId, 0, sort);
            }

            POGridRefer.Columns.Clear();
            POGridRefer.DataSource = this.OrderItemData;

            POGridRefer.VirtualMode = false;
            POGridRefer.ScrollBars = ScrollBars.Vertical;

            POGridRefer.Columns[0].HeaderText = "OrderItemId";
            POGridRefer.Columns[0].DataPropertyName = "id";
            POGridRefer.Columns[0].Visible = false;
            POGridRefer.Columns[1].HeaderText = "OrderId";
            POGridRefer.Columns[1].DataPropertyName = "orderId";
            POGridRefer.Columns[1].Visible = false;
            POGridRefer.Columns[2].HeaderText = "SkuId";
            POGridRefer.Columns[2].DataPropertyName = "skuId";
            POGridRefer.Columns[2].Visible = false;
            POGridRefer.Columns[3].HeaderText = "QboItemId";
            POGridRefer.Columns[4].DataPropertyName = "qboItemId";
            POGridRefer.Columns[3].Visible = false;
            POGridRefer.Columns[4].HeaderText = "QboSkuId";
            POGridRefer.Columns[4].DataPropertyName = "qboSkuId";
            POGridRefer.Columns[4].Visible = false;
            POGridRefer.Columns[5].HeaderText = "SKU#";
            POGridRefer.Columns[5].DataPropertyName = "sku";
            POGridRefer.Columns[5].Visible = false;

            POGridRefer.Columns[6].HeaderText = "Qty";
            POGridRefer.Columns[6].DataPropertyName = "quantity";
            POGridRefer.Columns[6].Width = 250;
            POGridRefer.Columns[7].HeaderText = "Description";
            POGridRefer.Columns[7].DataPropertyName = "description";
            POGridRefer.Columns[7].Width = 400;
            POGridRefer.Columns[8].HeaderText = "Tax";
            POGridRefer.Columns[8].DataPropertyName = "tax";
            POGridRefer.Columns[8].DefaultCellStyle.Format = "0.00##";
            POGridRefer.Columns[8].Visible = false;

            POGridRefer.Columns[9].HeaderText = "Disc.";
            POGridRefer.Columns[9].Name = "PriceTierCode";
            POGridRefer.Columns[9].DataPropertyName = "priceTierCode";
            POGridRefer.Columns[9].Width = 200;

            POGridRefer.Columns[10].HeaderText = "Unit Price";
            POGridRefer.Columns[10].DataPropertyName = "unitPrice";
            POGridRefer.Columns[10].DefaultCellStyle.Format = "0.00##";
            POGridRefer.Columns[10].Width = 200;

            POGridRefer.Columns[11].HeaderText = "Line Total";
            POGridRefer.Columns[11].DataPropertyName = "lineTotal";
            POGridRefer.Columns[11].DefaultCellStyle.Format = "0.00##";
            POGridRefer.Columns[11].Width = 200;
            POGridRefer.Columns[12].HeaderText = "SC";
            POGridRefer.Columns[12].Name = "salesCode";
            POGridRefer.Columns[12].DataPropertyName = "sc";
            POGridRefer.Columns[12].Width = 200;
            POGridRefer.Columns[13].Visible = false;

            POGridRefer.Columns[6].HeaderText = "Price Tier";
            POGridRefer.Columns[6].Name = "PriceTier";
            POGridRefer.Columns[6].DataPropertyName = "priceTier";
            POGridRefer.Columns[6].Width = 200;
            POGridRefer.Columns[6].Visible = false;

            POGridRefer.Columns[14].Visible = false;

            // DataGrid ComboBox column
            DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn();
            comboBoxColumn.DataSource = Session.SKUModelObj.SKUDataList;
            comboBoxColumn.HeaderText = "SKU#";
            comboBoxColumn.Width = 300;
            comboBoxColumn.Name = "skuNumber";
            comboBoxColumn.DataPropertyName = "skuId";
            comboBoxColumn.ValueMember = "Id";
            comboBoxColumn.DisplayMember = "Name";
            comboBoxColumn.AutoComplete = true;

            comboBoxColumn.DisplayIndex = 6;
            comboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            POGridRefer.Columns.Add(comboBoxColumn);
            int columnIndex = POGridRefer.Columns.IndexOf(comboBoxColumn);

            POGridRefer.CellValueChanged += PoGridRefer_CellValueChanged;
            POGridRefer.CellEndEdit += POGridRefer_CellEndEdit;
            POGridRefer.SelectionChanged += POGridRefer_SelectionChanged;
            InsertItem(null, null);
            POGridRefer.CurrentCell = POGridRefer.Rows[POGridRefer.Rows.Count - 1].Cells[12];
            //POGridRefer.Select();
            //POGridRefer.BeginEdit(true);

            // Tax ComboBox Column
            DataGridViewComboBoxColumn taxComboBoxColumn = new DataGridViewComboBoxColumn();
            List<SKUTax> skuTaxes = new List<SKUTax>();
            skuTaxes.Add(new SKUTax { Value = true, DisplayName = "Yes" });
            skuTaxes.Add(new SKUTax { Value = false, DisplayName = "No" });
            taxComboBoxColumn.DataSource = skuTaxes;
            taxComboBoxColumn.HeaderText = "Tax";
            taxComboBoxColumn.Width = 150;
            taxComboBoxColumn.Name = "skuTax";
            taxComboBoxColumn.DataPropertyName = "tax";
            taxComboBoxColumn.ValueMember = "Value";
            taxComboBoxColumn.DisplayMember = "DisplayName";

            taxComboBoxColumn.DisplayIndex = 9;
            taxComboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            POGridRefer.Columns.Add(taxComboBoxColumn);

        }

        private void POGridRefer_SelectionChanged(object? sender, EventArgs e)
        {
            if (POGridRefer.SelectedRows.Count == 0) return;


            var selectedRow = POGridRefer.SelectedRows[0];
            int selectedValue = int.Parse(selectedRow.Cells["skuId"].Value.ToString());

            var skuId = Session.SKUModelObj.SKUDataList.FirstOrDefault(item => item.Id == selectedValue).Id;
            this.skuId = skuId;

            PopulateInformationField();
        }

        private void POGridRefer_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {

        }

        private void PoGridRefer_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (POGridRefer.SelectedRows.Count == 0) return;

            if (e.ColumnIndex == 8)
            {
                DataGridViewRow selectedRow = POGridRefer.SelectedRows[0];
                DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)POGridRefer.Rows[e.RowIndex].Cells[e.ColumnIndex];
                var selectedValue = comboBoxCell.Value?.ToString();

                if (selectedValue == "True")
                {
                    this.OrderItemData[e.RowIndex].Tax = true;
                }
                else
                {
                    this.OrderItemData[e.RowIndex].Tax = false;
                }

                PopulateInformationField();
            }
            else
            // Quantity changed
            if (e.ColumnIndex == 6)
            {
                PopulateInformationField();
            }
            else
            // SKU Changed
            if (e.ColumnIndex == 15)
            {
                DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)POGridRefer.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int selectedValue = int.Parse(comboBoxCell.Value?.ToString());
                DataGridViewRow selectedRow = POGridRefer.SelectedRows[0];

                var skuId = Session.SKUModelObj.SKUDataList.FirstOrDefault(item => item.Id == selectedValue).Id;

                SKUOrderItem sku = this.SubSkuList.Where(item => item.Id == skuId).ToList()[0];
                selectedRow.Cells["sku"].Value = sku.Name;
                selectedRow.Cells["qboSkuId"].Value = sku.QboSkuId;
                selectedRow.Cells["description"].Value = sku.Description;
                //selectedRow.Cells["priceTier"].Value = sku.PriceTierId;
                selectedRow.Cells["unitPrice"].Value = sku.Price;
                selectedRow.Cells["lineTotal"].Value = sku.Price * sku.Qty;
                selectedRow.Cells["salesCode"].Value = sku.CostCode;
                selectedRow.Cells["quantity"].Value = 1;

                this.skuId = skuId;

                PopulateInformationField();
            }

            changeDetected = true;
        }

        private async Task<bool> CreateInvoice()
        {
            List<OrderItem> orderItems = this.OrderItemData;
            QboApiService qboApiService = new QboApiService();
            CustomerData customer = (CustomerData)this.Customer_ComBo.GetComboBox().SelectedItem;

            string invoiceNumber = $"INV-{orderId}";

            // Started: 1 -
            if (orderItems.Count > 0)
            {
                this.selectedOrderId = orderItems[orderItems.Count - 1].OrderId;
                foreach (var order in orderItems)
                {
                    order.message = Message;
                }

                if (this.selectedOrderId == 0)
                {
                    orderItems = orderItems.Where(item => item.OrderId == 0).ToList();
                    try
                    {
                        bool res = await qboApiService.CreateInvoice(customer, invoiceNumber, orderItems);

                        if (res)
                        {
                            this.isAddNewOrderItem = false;

                            // TODO: Where are order items saved?
                            //LoadOrderItemList();

                            selectedOrderId = orderId;

                        }
                        else
                        {
                            ShowError("The invoice could not be created.");
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message.Contains("TOKEN"))
                        {
                            ShowError("The invoice could not be created: QuickBooks tokens.json was not found.");
                        }
                        else if (e.Message.Contains("Unauthorized"))
                        {
                            ShowError("The invoice could not be created: QuickBooks needs to be reauthorized.");
                        }
                        else
                        {
                            ShowError("The invoice could not be created.");
                        }
                        return false;
                    }
                }
                else
                {
                    orderItems = orderItems.Where(item => item.OrderId == selectedOrderId).ToList();
                    dynamic selectedOrder = Session.OrderModelObj.GetOrderById(this.selectedOrderId);
                    var m_test = selectedOrder.qboOrderId;
                    bool res = await qboApiService.UpdateInvoice(customer, orderItems, selectedOrder);
                    if (res)
                    {
                        // LoadOrderItemList();

                        selectedOrderId = orderId;

                        ShowInformation("The invoice has been updated successfully");
                    }
                    else
                    {
                        ShowError("Invoice was not created #2.");
                        return false;
                    }
                }
            }
            else
            {
                ShowError("Order Item does not exist.");
                return false;
            }

            return true;
        }


        private void InsertItem(object sender, EventArgs e)
        {
            //if (POGridRefer.Rows.Count > 0)
            //{
            //    DataGridViewRow lastRow = POGridRefer.Rows[POGridRefer.Rows.Count - 1];
            //    int m_index = POGridRefer.Columns["id"].Index;
            //    int? lastRowValue = int.Parse(lastRow.Cells[POGridRefer.Columns["id"].Index].Value?.ToString());
            //    if (lastRowValue == 0)
            //    {
            //        return;
            //    }
            //}

            SKUOrderItem sku = this.SubSkuList[0];
            var items = Session.PriceTiersModelObj.GetPriceTierItems();
            var priceTierItem = items[sku.PriceTierId];

            this.OrderItemData.Add(new OrderItem
            {
                SkuId = sku.Id,
                Sku = sku.Name,
                QboSkuId = sku.QboSkuId,
                Description = sku.Description,
                PriceTier = sku.PriceTierId,
                PriceTierCode = sku.PriceTier,
                UnitPrice = sku.Price,
                LineTotal = sku.Price * sku.Qty,
                SC = sku.CostCode.ToString(),
                Quantity = sku.Qty > 0 ? sku.Qty : 1,
                Tax = true,
                BillAsLabor = true
            });

            BindingList<OrderItem> dataList = new BindingList<OrderItem>(this.OrderItemData);

            POGridRefer.DataSource = dataList;

            int index = 0;
            foreach (DataGridViewRow row in POGridRefer.Rows)
            {
                DataGridViewCell cell = row.Cells["SkuNumber"];

                if (this.OrderItemData[index].SkuId != 0)
                {
                    cell.Value = this.OrderItemData[index].SkuId;
                }

                index++;
            }
            POGridRefer.Select();
            POGridRefer.CurrentCell = POGridRefer.Rows[dataList.Count - 1].Cells[12];
            //int rowIndex = POGridRefer.Rows.Count - 1;
            //POGridRefer.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;

            //POGridRefer.CurrentCell = POGridRefer.Rows[rowIndex].Cells[3];
            //POGridRefer.BeginEdit(true);
        }

        private void EditItem(object sender, EventArgs e)
        {
            //int rowIndex = POGridRefer.SelectedRows[0];
            //int rowIndex = POGridRefer.CurrentCell.RowIndex;
            //int columnIndex = POGridRefer.CurrentCell.ColumnIndex;
            //DataGridViewRow row = POGridRefer.Rows[rowIndex];

            //DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)row.Cells[columnIndex];

            //POGridRefer.CurrentCell = POGridRefer.Rows[rowIndex].Cells[columnIndex];
            //POGridRefer.BeginEdit(true);
        }

        private void DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;
            if (e.KeyCode == Keys.Enter)
            {
                int rowIndex = POGridRefer.SelectedRows[0].Index;
                int columnIndex = POGridRefer.CurrentCell.ColumnIndex;

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

                    //if (columnIndex == 13)
                    //{
                    //    DataGridViewComboBoxCell dataGridViewComboBoxCell = (DataGridViewComboBoxCell)dataGridView.CurrentCell;
                    //    if (dataGridViewComboBoxCell.DisplayStyle == DataGridViewComboBoxDisplayStyle.ComboBox)
                    //    {
                    //        dataGridViewComboBoxCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                    //    } else
                    //    {
                    //        dataGridViewComboBoxCell.ReadOnly = false;
                    //        //dataGridViewComboBoxCell.IsInEditMode = true;
                    //        dataGridViewComboBoxCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                    //    }
                    //} else
                    //{
                    e.Handled = true;
                    dataGridView.BeginEdit(true);
                    //}
                }
            }
        }

        private void POGridRefer_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var comboBox = e.Control as DataGridViewComboBoxEditingControl;
            if (comboBox != null)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDown;
                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            }
        }
    }
}
