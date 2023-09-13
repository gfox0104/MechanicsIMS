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

namespace MJC.forms.order
{
    public partial class HeldOrder : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Selects", Keys.Enter);

        private GridViewOrigin heldOrderGrid = new GridViewOrigin();
        private DataGridView HeldOrderRefer;

        private int customerId = 0;
        private string customerName = "";

        public HeldOrder(int customerId = 0, string customerName = "") : base("Held Orders - All", "Select a held order to open")
        {
            InitializeComponent();
            _initBasicSize();
            this.customerId = customerId;
            this.customerName = customerName;

            if (customerId != 0)
            {
                this._changeFormText("Held Orders - " + this.customerName);
            }

            HotkeyButton[] hkButtons = new HotkeyButton[3] { hkAdds, hkDeletes, hkEdits };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitHeldOrderList();
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                int rowIndex = HeldOrderRefer.SelectedRows[0].Index;
                DataGridViewRow row = HeldOrderRefer.Rows[rowIndex];
                int customerId = (int)row.Cells[1].Value;
                bool isAddNewOrderItem = true;

                ProcessOrder processOrderModal = new ProcessOrder(0, 0, isAddNewOrderItem);
                _navigateToForm(sender, e, processOrderModal);
                this.Hide();
            };
            hkDeletes.GetButton().Click += (sender, e) =>
            {
                int rowIndex = HeldOrderRefer.SelectedRows[0].Index;
                DataGridViewRow row = HeldOrderRefer.Rows[rowIndex];
                int selectedOrderId = (int)row.Cells[0].Value;

                bool refreshData = Session.OrderItemModelObj.DeleteOrder(selectedOrderId);
                if (refreshData)
                {
                    LoadHeldOrderList();
                }
            };
            hkEdits.GetButton().Click += (sender, e) =>
            {
                DataGridViewRow row = HeldOrderRefer.SelectedRows[0];
                int customerId = (int)row.Cells[1].Value;
                int orderId = (int)row.Cells[0].Value;

                ProcessOrder processOrderModal = new ProcessOrder(0, orderId);
                _navigateToForm(sender, e, processOrderModal);
                this.Hide();
            };
        }

        private void InitHeldOrderList()
        {
            HeldOrderRefer = heldOrderGrid.GetGrid();
            HeldOrderRefer.Location = new Point(0, 95);
            HeldOrderRefer.Width = this.Width;
            HeldOrderRefer.Height = this.Height - 295;
            HeldOrderRefer.AllowUserToAddRows = false;
            HeldOrderRefer.AllowUserToOrderColumns = false;
            HeldOrderRefer.MultiSelect = false;

            HeldOrderRefer.Columns.Add("id", "Id");
            HeldOrderRefer.Columns[0].Visible = false;
            HeldOrderRefer.Columns.Add("customerId", "CustomerId");
            HeldOrderRefer.Columns[1].Visible = false;
            HeldOrderRefer.Columns.Add("name", "Name");
            HeldOrderRefer.Columns[2].Width = 300;
            HeldOrderRefer.Columns.Add("invoiceDate", "Date");
            HeldOrderRefer.Columns[3].Width = 300;
            HeldOrderRefer.Columns.Add("desc", "Description");
            HeldOrderRefer.Columns[4].Width = 500;
            HeldOrderRefer.Columns.Add("amount", "Amount");
            HeldOrderRefer.Columns[5].Width = 200;
            HeldOrderRefer.Columns.Add("heldFor", "Held for");
            HeldOrderRefer.Columns[6].Width = 200;
            HeldOrderRefer.Columns.Add("procBy", "Proc By");
            HeldOrderRefer.Columns[7].Width = 400;

            foreach (DataGridViewColumn column in HeldOrderRefer.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            this.Controls.Add(HeldOrderRefer);

            LoadHeldOrderList();
        }

        private void LoadHeldOrderList()
        {
            List<Order> OrdersList = Session.OrderItemModelObj.LoadOrdersDataByStatus(2, this.customerId);
            HeldOrderRefer.Rows.Clear();

            foreach (Order OrderItem in OrdersList)
            {
                int rowIndex = HeldOrderRefer.Rows.Add();
                DataGridViewRow newRow = HeldOrderRefer.Rows[rowIndex];

                newRow.Resizable = DataGridViewTriState.False;
                newRow.Cells["id"].Value = OrderItem.Id;
                newRow.Cells["customerId"].Value = OrderItem.CustomerId;
                newRow.Cells["name"].Value = OrderItem.Name;
                string formattedDate = "";
                if (OrderItem.InvoiceDate.HasValue)
                {
                    DateTime? dateTime = null;
                    dateTime = OrderItem.InvoiceDate.Value;
                    formattedDate = dateTime.Value.ToString("dd/MM/yyyy");
                }

                newRow.Cells["invoiceDate"].Value = formattedDate;
                newRow.Cells["desc"].Value = OrderItem.Description;
                newRow.Cells["amount"].Value = OrderItem.InvoiceBalance;
                newRow.Cells["heldFor"].Value = OrderItem.HeldFor;
                newRow.Cells["procBy"].Value = OrderItem.ProcBy;
            }
        }
    }
}
