using MJC.common.components;
using MJC.common;
using MJC.model;
using System.ComponentModel;
using System.Data;
using MJC.forms.sku;
using MJC.qbo;
using System.Windows.Forms;

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

        private FComboBox Customer_ComBo = new FComboBox("Customer#:", 150);
        private FlabelConstant CustomerName = new FlabelConstant("Name:", 150);
        private FlabelConstant Terms = new FlabelConstant("Terms:", 150);
        //private FlabelConstant Zone = new FlabelConstant("Zone", 150);
        private FlabelConstant Position = new FlabelConstant("PO#:", 150);

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

        private string searchKey;

        private CustomersModel CustomersModelObj = new CustomersModel();
        private SKUModel SKUModelObj = new SKUModel();
        private OrderItemsModel OrderItemsModalObj = new OrderItemsModel();
        private OrderModel OrderModelObj = new OrderModel();

        private List<OrderItem> OrderItemData = new List<OrderItem>();
        private List<SKUOrderItem> TotalSkuList = new List<SKUOrderItem>();
        private List<SKUOrderItem> SubSkuList = new List<SKUOrderItem>();

        public ProcessOrder(int customerId = 0, int orderId = 0, bool isAddNewOrderItem = false) : base("Process an Order", "Fill out the customer order")
        {
            InitializeComponent();
            _initBasicSize();
            this.isAddNewOrderItem = isAddNewOrderItem;
            this.customerId = customerId;
            this.selectedOrderId = orderId;

            dynamic customer = CustomersModelObj.GetCustomerDataById(customerId);
            this.TotalSkuList = SKUModelObj.LoadSkuOrderItems();
            if(customer != null)
            {
                this.SubSkuList = this.TotalSkuList.Where(sku => sku.PriceTierId == customer.priceTierId).ToList();
            } else
            {
                this.SubSkuList = this.TotalSkuList;
            }
            HotkeyButton[] hkButtons = new HotkeyButton[9] { hkAdds, hkDeletes, hkEdit, hkSaveOrder, hkAddMessage, hkCustomerProfiler, hkSKUInfo, hkSortLines, hkCloseOrder };
            _initializeHKButtons(hkButtons, false);
            AddHotKeyEvents();

            InitOrderItemsList();
            InitCustomerInfo(this.customerId);

            InitGridFooter();


        }

        private void printInvoice(int orderId, int orderStatus)
        {
            OrderPrint orderPrint = new OrderPrint(orderId, orderStatus);
            orderPrint.PrintForm();
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (s, e) => InsertItem(s, e);
            hkSortLines.GetButton().Click += (s, e) =>
            {
                string sort = "ORDER BY sku";
                LoadOrderItemList(sort);
            };
            //hkEdit.GetButton().Click += (s, e) =>
            //{
            //    EditItem(s, e);
            //};
            hkSaveOrder.GetButton().Click += async (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to save?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    InvoiceAction(sender, e);
                }
            };
            hkCloseOrder.GetButton().Click += async (sender, e) =>
            {
                CloseOrderActions CloseOrderActionsModal = new CloseOrderActions();
                this.Enabled = false;
                CloseOrderActionsModal.Show();
                CloseOrderActionsModal.FormClosed += (ss, sargs) =>
                {
                    this.Enabled = true;
                    int saveFlag = CloseOrderActionsModal.GetSaveFlage();
                    if (saveFlag == 7) _navigateToPrev(sender, e);
                    int status = 0;

                    if (POGridRefer.Rows.Count > 0)
                    {
                        int rowIndex = POGridRefer.SelectedRows[0].Index;
                        DataGridViewRow row = POGridRefer.Rows[rowIndex];
                        int orderId = (int)row.Cells[1].Value;
                        if(saveFlag != 0 && saveFlag != 7)
                        {
                            if (orderId != 0)
                            {
                                if (saveFlag == 1)
                                {
                                    status = 3;
                                    OrderModelObj.UpdateOrderStatus(status, orderId);
                                    printInvoice(orderId, status);
                                }
                                else if (saveFlag == 2)
                                {
                                    status = 3;
                                    OrderModelObj.UpdateOrderStatus(status, orderId);
                                    printInvoice(orderId, status);
                                }
                                else if (saveFlag == 3)
                                {
                                    status = 2;
                                    OrderModelObj.UpdateOrderStatus(status, orderId);
                                    printInvoice(orderId, status);
                                }
                                else if (saveFlag == 4)
                                {
                                    status = 2;
                                    OrderModelObj.UpdateOrderStatus(status, orderId);
                                    printInvoice(orderId, status);
                                }
                                else if (saveFlag == 5)
                                {
                                    status = 1;
                                    OrderModelObj.UpdateOrderStatus(status, orderId);
                                }
                                else if (saveFlag == 6)
                                {
                                    status = 1;
                                    OrderModelObj.UpdateOrderStatus(status, orderId);
                                    printInvoice(orderId, status);
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
                                MessageBox.Show("Order info is not saved yet, please save order info first");
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
                    SKUInformation detailModal = new SKUInformation();

                    int rowIndex = POGridRefer.SelectedRows[0].Index;

                    DataGridViewRow row = POGridRefer.Rows[rowIndex];
                    int skuId = (int)row.Cells[2].Value;
                    List<dynamic> skuData = new List<dynamic>();
                    skuData = SKUModelObj.GetSKUData(skuId);
                    detailModal.setDetails(skuData, skuData[0].id);

                    this.Hide();
                    _navigateToForm(sender, e, detailModal);
                }
            };
            hkAddMessage.GetButton().Click += (sender, e) =>
            {
                if(POGridRefer.RowCount > 0)
                {
                    OrderItemMessage detailModal = new OrderItemMessage();
                    int rowIndex = POGridRefer.SelectedRows[0].Index;

                    DataGridViewRow row = POGridRefer.Rows[rowIndex];
                    int orderItemId = (int)row.Cells[0].Value;
                    detailModal.setDetails(orderItemId);

                    if (detailModal.ShowDialog() == DialogResult.OK)
                    {
                        LoadOrderItemList();
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
            //FormComponents.Add(Zone);
            FormComponents.Add(Position);
            
            _addFormInputs(FormComponents, 30, 110, 650, 42, 180);

            var refreshData = CustomersModelObj.LoadCustomerData("", false);

            Customer_ComBo.GetComboBox().DataSource = CustomersModelObj.CustomerDataList;
            Customer_ComBo.GetComboBox().DisplayMember = "Num";
            Customer_ComBo.GetComboBox().ValueMember = "Id";

            Customer_ComBo.GetComboBox().SelectedIndexChanged += new EventHandler(Customer_SelectedIndexChanged);

            if (customerId == 0)
            {
                Customer_ComBo.GetComboBox().SelectedIndex = 0;
                Customer_SelectedIndexChanged(Customer_ComBo.GetComboBox(), EventArgs.Empty);
            }
            else
            {
                Customer_ComBo.GetComboBox().SelectedValue = customerId;
            }

            Position.GetLabel().Focus();
            //POGridRefer.Select();
        }

        private void Customer_SelectedIndexChanged(object sender, EventArgs e)
        {
            CustomerData selectedItem = (CustomerData)Customer_ComBo.GetComboBox().SelectedItem;
            int customerId = selectedItem.Id;
            this.customerId = customerId;

            var customerData = CustomersModelObj.GetCustomerData(customerId);
            if (customerData != null)
            {
                if (customerData.customerName != "") CustomerName.SetContext(customerData.customerName);
                else CustomerName.SetContext("N/A");

                if (customerData.terms != "") Terms.SetContext(customerData.terms);
                else Terms.SetContext("N/A");

                //if(customerData.zipcode != "") Zone.SetContext(customerData.zipcode);
                //else Zone.SetContext("N/A");

                if(!string.IsNullOrEmpty(customerData.poRequired))
                {
                    if(bool.Parse(customerData.poRequired))
                        Position.SetContext("Yes");
                    else Position.SetContext("No");
                }
                else Position.SetContext("N/A");

                this.TotalSkuList = SKUModelObj.LoadSkuOrderItems();

            }

            if (POGridRefer != null)
                LoadOrderItemList();
            POGridRefer.Select();
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
            POGridRefer.EditMode = DataGridViewEditMode.EditProgrammatically;
            POGridRefer.ReadOnly = false;
            POGridRefer.KeyDown += DataGridView_KeyDown;

            POGridRefer.Select();

            POGridRefer.VirtualMode = true;
            POGridRefer.EditMode = DataGridViewEditMode.EditOnKeystroke;
            POGridRefer.EditingControlShowing += POGridRefer_EditingControlShowing;

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

            if (this.isAddNewOrderItem)
            {
                this.OrderItemData.Add(new OrderItem());
                BindingList<OrderItem> dataList = new BindingList<OrderItem>(this.OrderItemData);

                POGridRefer.DataSource = dataList;

                int rowIndex = POGridRefer.Rows.Count - 1;

                POGridRefer.CurrentCell = POGridRefer.Rows[rowIndex].Cells[3];
                POGridRefer.BeginEdit(true);
            }
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

            if (skuId != 0)
                LoadGridFooterInfo();
        }

        public void LoadGridFooterInfo()
        {
            int skuId = this.skuId;
            var qtyInfo = SKUModelObj.LoadSkuQty(skuId);

            QtyOnHold.SetContext(qtyInfo.qty.ToString());
            QtyAllocated.SetContext(qtyInfo.qtyAllocated.ToString());
            QtyAvailable.SetContext(qtyInfo.qtyAvailable.ToString());
        }

        public void LoadOrderItemList(string sort = "")
        {
            this.OrderItemData = OrderItemsModalObj.GetOrderItemsListByCustomerId(this.customerId, this.selectedOrderId, sort);

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

            POGridRefer.Columns[6].HeaderText = "Quantity";
            POGridRefer.Columns[6].DataPropertyName = "quantity";
            POGridRefer.Columns[6].Width = 200;
            POGridRefer.Columns[7].HeaderText = "Description";
            POGridRefer.Columns[7].DataPropertyName = "description";
            POGridRefer.Columns[7].Width = 400;
            POGridRefer.Columns[8].HeaderText = "Tax";
            POGridRefer.Columns[8].DataPropertyName = "tax";
            POGridRefer.Columns[8].Width = 200;
            POGridRefer.Columns[9].HeaderText = "Disc%";
            POGridRefer.Columns[9].DataPropertyName = "priceTier";
            POGridRefer.Columns[9].Width = 200;
            POGridRefer.Columns[10].HeaderText = "Unit Price";
            POGridRefer.Columns[10].DataPropertyName = "unitPrice";
            POGridRefer.Columns[10].Width = 200;
            POGridRefer.Columns[11].HeaderText = "Line Total";
            POGridRefer.Columns[11].DataPropertyName = "lineTotal";
            POGridRefer.Columns[11].Width = 200;
            POGridRefer.Columns[12].HeaderText = "SC";
            POGridRefer.Columns[12].Name = "salesCode";
            POGridRefer.Columns[12].DataPropertyName = "sc";
            POGridRefer.Columns[12].Width = 200;

            // DataGrid ComboBox column
            DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn();
            comboBoxColumn.DataSource = this.SubSkuList;
            comboBoxColumn.HeaderText = "SKU#";
            comboBoxColumn.Width = 300;
            comboBoxColumn.Name = "skuNumber";
            comboBoxColumn.DataPropertyName = "skuId";
            comboBoxColumn.ValueMember = "Id";
            comboBoxColumn.DisplayMember = "Name";
            comboBoxColumn.AutoComplete = true;
            comboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;

            comboBoxColumn.DisplayIndex = 6;
            comboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            POGridRefer.Columns.Add(comboBoxColumn);
            int columnIndex = POGridRefer.Columns.IndexOf(comboBoxColumn);

            POGridRefer.CellValueChanged += PoGridRefer_CellValueChanged;
        }

        private void PoGridRefer_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0 && e.ColumnIndex == 13)
            {
                DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)POGridRefer.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int selectedValue = int.Parse(comboBoxCell.Value?.ToString());
                DataGridViewRow selectedRow = POGridRefer.SelectedRows[0];

                SKUOrderItem sku = this.SubSkuList.Where(item => item.Id == selectedValue).ToList()[0];
                selectedRow.Cells["sku"].Value = sku.Name;
                selectedRow.Cells["qboSkuId"].Value = sku.QboSkuId;
                selectedRow.Cells["description"].Value = sku.Description;
                selectedRow.Cells["priceTier"].Value = sku.PriceTierId;
                selectedRow.Cells["unitPrice"].Value = sku.Price;
                selectedRow.Cells["lineTotal"].Value = sku.Price * sku.Qty;
                selectedRow.Cells["salesCode"].Value = sku.CostCode;
                selectedRow.Cells["quantity"].Value = 1;
            }
        }

        async private void InvoiceAction(object sender, EventArgs e)
        {
            List<OrderItem> orderItems = this.OrderItemData;
            QboApiService qboApiService = new QboApiService();
            CustomerData customer = (CustomerData)this.Customer_ComBo.GetComboBox().SelectedItem;
            string invoiceNumber = "invoice-" + DateTime.Now.ToString("yyyy-MM-dd");
            if (orderItems.Count > 0)
            {
                this.selectedOrderId = orderItems[orderItems.Count - 1].OrderId;

                if (this.selectedOrderId == 0)
                {
                    orderItems = orderItems.Where(item => item.OrderId == 0).ToList();
                    bool res = await qboApiService.CreateInvoice(customer, invoiceNumber, orderItems);
                    if (res)
                        LoadOrderItemList();
                }
                else
                {
                    orderItems = orderItems.Where(item => item.OrderId == selectedOrderId).ToList();
                    dynamic selectedOrder = OrderModelObj.GetOrderById(this.selectedOrderId);
                    var m_test = selectedOrder.qboOrderId;
                    bool res = await qboApiService.UpdateInvoice(customer, orderItems, selectedOrder);
                    if (res)
                        LoadOrderItemList();
                }
            } else
            {
                MessageBox.Show("Order Item is not existed");
            }

            
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

            this.OrderItemData.Add(new OrderItem { SkuId = sku.Id, QboSkuId = sku.QboSkuId, Description = sku.Description, PriceTier = sku.PriceTierId, UnitPrice = sku.Price, LineTotal = sku.Price * sku.Qty, SC = sku.CostCode.ToString(), Quantity = sku.Qty });
            BindingList<OrderItem> dataList = new BindingList<OrderItem>(this.OrderItemData);

            POGridRefer.DataSource = dataList;

            int index = 0;
            foreach (DataGridViewRow row in POGridRefer.Rows)
            {
                DataGridViewCell cell = row.Cells["SkuNumber"];

                if (this.OrderItemData[index].SkuId != 0)
                    cell.Value = this.OrderItemData[index].SkuId;

                index++;
            }
            POGridRefer.Select();
            POGridRefer.CurrentCell = POGridRefer.Rows[dataList.Count - 1].Cells[13];
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
