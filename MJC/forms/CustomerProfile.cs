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

namespace MJC.forms
{
    public partial class CustomerProfile : GlobalLayout
    {
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);
        
        private FComboBox Customer = new FComboBox("Customer#", 150);
        private FlabelConstant CustomerName = new FlabelConstant("Name", 150);

        private DataGridView POGridRefer;
        private string searchKey = "";
        private int customerId = 0;

        public CustomerProfile(int customerId = 0) : base("Customer Profiler", "Profile view of customers and their history of purchases")
        {
            InitializeComponent();
            _initBasicSize();
            HotkeyButton[] hkButtons = new HotkeyButton[] { hkPreviousScreen };
            _initializeHKButtons(hkButtons);
            hkPreviousScreen.GetButton().Click += new EventHandler(_navigateToPrev);

            AddHotKeyEvents();
            InitCustomerInfo(customerId);
            InitCustomerProfileList();
            LoadProfilerList();
        }

        private void AddHotKeyEvents()
        {
        }

        private void InitCustomerInfo(int customerId = 0)
        {
            List<dynamic> FormComponents = new List<dynamic>();

            FormComponents.Add(Customer);
            FormComponents.Add(CustomerName);

            _addFormInputs(FormComponents, 30, 110, 650, 42, 180);

            List<KeyValuePair<int, string>> CustomerList = new List<KeyValuePair<int, string>>();
            CustomerList = Session.CustomersModelObj.GetCustomerNumberList();
            foreach (KeyValuePair<int, string> item in CustomerList)
            {
                int id = item.Key;
                string name = item.Value;
                Customer.GetComboBox().Items.Add(new FComboBoxItem(id, name));
            }

            Customer.GetComboBox().SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);

            if(Customer.GetComboBox().Items.Count > 0)
            {
                if (customerId == 0)
                {
                    Customer.GetComboBox().SelectedIndex = 0;
                    ComboBox_SelectedIndexChanged(Customer.GetComboBox(), EventArgs.Empty);
                }
                else
                {
                    int index = Customer.GetComboBox().Items.Cast<FComboBoxItem>().ToList().FindIndex(item => item.Id == customerId);
                    Customer.GetComboBox().SelectedIndex = index;
                    ComboBox_SelectedIndexChanged(Customer.GetComboBox(), EventArgs.Empty);
                }
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FComboBoxItem selectedItem = (FComboBoxItem)Customer.GetComboBox().SelectedItem;
            if (selectedItem != null)
            {
                this.customerId = selectedItem.Id;

                var customerData = Session.CustomersModelObj.GetCustomerData(this.customerId);
                if (customerData != null)
                {
                    if (customerData.customerName != "") CustomerName.SetContext(customerData.customerName);
                    else CustomerName.SetContext("N/A");
                }
                if (POGridRefer != null)
                    LoadProfilerList();
            }
            else { return; }
        }

        private void InitCustomerProfileList()
        {
            GridViewOrigin OrderEntryLookupGrid = new GridViewOrigin();
            POGridRefer = OrderEntryLookupGrid.GetGrid();
            POGridRefer.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(157, 196, 235);
            POGridRefer.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 63, 96);
            POGridRefer.ColumnHeadersDefaultCellStyle.Padding = new Padding(12);
            POGridRefer.Location = new Point(0, 200);
            POGridRefer.Width = this.Width;
            POGridRefer.Height = this.Height - 295;
            POGridRefer.EditMode = DataGridViewEditMode.EditOnEnter;

            this.Controls.Add(POGridRefer);
        }

        public void LoadProfilerList()
        {
            if (this.searchKey == "")
            {
                this._changeFormText("Customer Profiler");
            }
            else
            {
                this._changeFormText("Customer Profiler searched by " + this.searchKey);
            }
            var refreshData = Session.OrderItemModelObj.LoadCustomerProfiler(this.searchKey, this.customerId);
            if (refreshData)
            {
                POGridRefer.DataSource = Session.OrderItemModelObj.ProcessOIList;
                POGridRefer.ReadOnly = true;
            }

            POGridRefer.Columns[0].Visible = false;
            POGridRefer.Columns[1].HeaderText = "Invoice#";
            POGridRefer.Columns[1].Width = 200;
            POGridRefer.Columns[2].HeaderText = "SKU#";
            POGridRefer.Columns[2].Width = 200;
            POGridRefer.Columns[3].HeaderText = "Description";
            POGridRefer.Columns[3].Width = 400;
            POGridRefer.Columns[4].HeaderText = "Date";
            POGridRefer.Columns[4].Width = 200;
            POGridRefer.Columns[5].HeaderText = "Qty";
            POGridRefer.Columns[5].Width = 200;
            POGridRefer.Columns[6].HeaderText = "Price";
            POGridRefer.Columns[6].Width = 200;
            POGridRefer.Columns[7].HeaderText = "SC";
            POGridRefer.Columns[7].Width = 200;
        }
    }
}
