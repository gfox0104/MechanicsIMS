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
using MJC.forms.payment;

namespace MJC.forms
{
    public partial class Reconcilliation : GlobalLayout
    {

        private HotkeyButton hkClosesReport = new HotkeyButton("ESC", "Closes Report", Keys.Escape);
        private HotkeyButton hkValidateSale = new HotkeyButton("F1", "Validate Sale(line item)", Keys.F1);
        private HotkeyButton hkAddPayment = new HotkeyButton("F3", "Add Payment", Keys.F3);

        private GridViewOrigin reconcilliationGrid = new GridViewOrigin();
        private DataGridView ReconcilGridRefer;

        public Reconcilliation() : base("Reconcilliation Report", "Reconcilliation report to compare posted/validated sales with Quickbooks")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[3] { hkClosesReport, hkValidateSale, hkAddPayment };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitReconcilliationGrid();

            this.VisibleChanged += (s, e) =>
            {
                this.LoadReconcilliationList();
            };
        }

        private void AddHotKeyEvents()
        {
            hkValidateSale.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int status = 4;
                    if (ReconcilGridRefer.RowCount > 0)
                    {
                        int rowIndex = ReconcilGridRefer.SelectedRows[0].Index;
                        DataGridViewRow row = ReconcilGridRefer.Rows[rowIndex];
                        int orderId = (int)row.Cells[0].Value;
                        var refreshData = Session.InvoicesModelObj.UpdateOrderStatus(status, orderId);
                        if (refreshData)
                        {
                            LoadReconcilliationList();
                        }
                    }
                }
            };

            hkAddPayment.GetButton().Click += (sender, e) =>
            {
                //int rowIndex = ReconcilGridRefer.SelectedRows[0].Index;

                //DataGridViewRow row = ReconcilGridRefer.Rows[rowIndex];
                //int customerId = (int)row.Cells[1].Value;
                int customerId = 0;
                NewPayment detailModal = new NewPayment(customerId);
                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadReconcilliationList();
                }
            };
        }

        private void InitReconcilliationGrid()
        {
            ReconcilGridRefer = reconcilliationGrid.GetGrid();
            ReconcilGridRefer.Location = new Point(0, 95);
            ReconcilGridRefer.Width = this.Width;
            ReconcilGridRefer.Height = this.Height - 295;
            ReconcilGridRefer.VirtualMode = true;
            this.Controls.Add(ReconcilGridRefer);
            //this.ReconcilGridRefer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.pricetierGridView_CellDoubleClick);
        }

        private void LoadReconcilliationList()
        {
            List<InvoiceData> invoiceData = Session.InvoicesModelObj.LoadInvoiceData(0, 3);
  
            ReconcilGridRefer.DataSource = invoiceData;
            ReconcilGridRefer.Columns[0].HeaderText = "id";
            ReconcilGridRefer.Columns[0].Visible = false;
            ReconcilGridRefer.Columns[1].HeaderText = "customerId";
            ReconcilGridRefer.Columns[1].Visible = false;
            ReconcilGridRefer.Columns[2].HeaderText = "Invoice Number";
            ReconcilGridRefer.Columns[2].Width = 300;
            ReconcilGridRefer.Columns[3].HeaderText = "Date";
            ReconcilGridRefer.Columns[3].Width = 300;
            ReconcilGridRefer.Columns[4].HeaderText = "Description";
            ReconcilGridRefer.Columns[4].Width = 300;
            ReconcilGridRefer.Columns[5].HeaderText = "Total";
            ReconcilGridRefer.Columns[5].Width = 200;
            ReconcilGridRefer.Columns[6].HeaderText = "Balance";
            ReconcilGridRefer.Columns[6].Width = 200;
        }
    }
}
