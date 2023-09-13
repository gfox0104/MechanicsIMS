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
    public partial class ShipInformation : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Edits", Keys.Enter);
        private HotkeyButton hkPrevScreen = new HotkeyButton("Esc", "Previous screen", Keys.Escape);

        private GridViewOrigin shipCustListGrid = new GridViewOrigin();
        private DataGridView CLGridRefer;

        public ShipInformation(int customerId = 0) : base("Ship to Cust#", "Enter a customer ship to information")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[4] { hkAdds, hkDeletes, hkEdits, hkPrevScreen };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitCustomerList();

            this.VisibleChanged += (s, e) =>
            {
                this.LoadCustomerList();
            };
        }

        private void AddHotKeyEvents()
        {
            //hkAdds.GetButton().Click += (sender, e) =>
            //{
            //    this.Hide();
            //    CustomerInformation detailModal = new CustomerInformation();
            //    _navigateToForm(sender, e, detailModal);
            //    //                if (detailModal.ShowDialog() == DialogResult.OK)
            //    //                {
            //    //                    LoadCustomerList("");
            //    //                }
            //};
            //hkDeletes.GetButton().Click += (sender, e) =>
            //{
            //    int selectedCustomerId = 0;
            //    if (CLGridRefer.SelectedRows.Count > 0)
            //    {
            //        foreach (DataGridViewRow row in CLGridRefer.SelectedRows)
            //        {
            //            selectedCustomerId = (int)row.Cells[0].Value;
            //        }
            //    }
            //    bool refreshData = CustomersModelObj.DeleteCustomer(selectedCustomerId);
            //    if (refreshData)
            //    {
            //        LoadCustomerList();
            //    }
            //};
            //hkEdits.GetButton().Click += (sender, e) =>
            //{
            //    updateCustomer(sender, e);
            //};
        }

        private void InitCustomerList()
        {
            CLGridRefer = shipCustListGrid.GetGrid();
            CLGridRefer.Location = new Point(0, 95);
            CLGridRefer.Width = this.Width;
            CLGridRefer.Height = this.Height - 295;
            CLGridRefer.VirtualMode = true;
            this.Controls.Add(CLGridRefer);
            this.CLGridRefer.CellDoubleClick += (sender, e) =>
            {
                //updateCustomer(sender, e);
            };

            LoadCustomerList();
        }

        private void LoadCustomerList()
        {
            //string filter = "";
            //var refreshData = CustomersModelObj.LoadCustomerData(filter);
            //if (refreshData)
            //{
            //    CLGridRefer.DataSource = CustomersModelObj.CustomerDataList;
            //    CLGridRefer.Columns[0].Visible = false;
            //    CLGridRefer.Columns[1].HeaderText = "Customer #";
            //    CLGridRefer.Columns[1].Width = 300;
            //    CLGridRefer.Columns[2].HeaderText = "Name";
            //    CLGridRefer.Columns[2].Width = 300;
            //    CLGridRefer.Columns[3].HeaderText = "Address";
            //    CLGridRefer.Columns[3].Width = 500;
            //    CLGridRefer.Columns[4].HeaderText = "City";
            //    CLGridRefer.Columns[4].Width = 200;
            //    CLGridRefer.Columns[5].HeaderText = "State";
            //    CLGridRefer.Columns[5].Width = 200;
            //    CLGridRefer.Columns[6].HeaderText = "Zipcode";
            //    CLGridRefer.Columns[6].Width = 200;
            //}
        }

        private void updateCustomer(object sender, EventArgs e)
        {
            //CustomerInformation detailModal = new CustomerInformation();

            //if (CLGridRefer.Rows.Count > 0)
            //{
            //    int rowIndex = CLGridRefer.CurrentCell.RowIndex;
            //    DataGridViewRow row = CLGridRefer.Rows[rowIndex];

            //    int id = (int)row.Cells[0].Value;
            //    this.Hide();
            //    detailModal.setDetails(id);
            //    _navigateToForm(sender, e, detailModal);

            //    //            if (detailModal.ShowDialog() == DialogResult.OK)
            //    //          {
            //    //            LoadCustomerList();
            //    //      }
            //}
        }
    }
}
