using MJC.common.components;
using MJC.common;
using MJC.model;

namespace MJC.forms.customer
{
    public partial class CustomerSummary : GlobalLayout
    {
        private HotkeyButton hkPrevScreen = new HotkeyButton("ESC", "Previous screen", Keys.Escape);

        private GridViewOrigin InvoiceAgingGrid = new GridViewOrigin();
        private GridViewOrigin PurchaseGrid = new GridViewOrigin();
        private DataGridView InvoiceAgingGridRefer;
        private DataGridView PurchaseHistoryGridRefer;
        private int CustomerId;

        private InvoicesModel InvoiceModelObj = new InvoicesModel();

        public CustomerSummary(int _customerId, string _customerLabel) : base("Summary of Customer# " + _customerLabel, "Summarized history of the selected customer")
        {
            InitializeComponent();
            _initBasicSize();

            this.CustomerId = _customerId;

            HotkeyButton[] hkButtons = new HotkeyButton[1] { hkPrevScreen };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitInvoiceAgingList();

            this.VisibleChanged += (s, e) =>
            {
                this.LoadInvoiceAgingList();
                this.LoadPurchaseHistoryList();
            };

        }

        private void AddHotKeyEvents()
        {
            //hkPrevScreen.GetButton().Click += (sender, e) =>
            //{
            //    CustomerList CustomerLIstModal = new CustomerList();

            //    this.Enabled = false;
            //    CustomerLIstModal.Show();
            //    CustomerLIstModal.FormClosed += (ss, sargs) =>
            //    {
            //        this.Enabled = true;
            //        //this.();
            //    };
            //};
        }

        private void InitInvoiceAgingList()
        {
            List<int> PurchaseHistoryHeaderData = InvoiceModelObj.GetInvoiceYearList(this.CustomerId);

            InvoiceAgingGridRefer = InvoiceAgingGrid.GetGrid();
            InvoiceAgingGridRefer.Location = new Point(0, 95);
            InvoiceAgingGridRefer.Width = this.Width / 3;
            InvoiceAgingGridRefer.Height = this.Height - 295;
            InvoiceAgingGridRefer.AllowUserToAddRows = false;
            InvoiceAgingGridRefer.AllowUserToOrderColumns = false;
            InvoiceAgingGridRefer.MultiSelect = false;
            InvoiceAgingGridRefer.VirtualMode = true;

            this.Controls.Add(InvoiceAgingGridRefer);

            PurchaseHistoryGridRefer = PurchaseGrid.GetGrid();
            PurchaseHistoryGridRefer.Location = new Point(this.Width / 3, 95);
            PurchaseHistoryGridRefer.Width = this.Width / 3 * 2;
            PurchaseHistoryGridRefer.Height = this.Height - 325;
            PurchaseHistoryGridRefer.AllowUserToAddRows = false;
            PurchaseHistoryGridRefer.AllowUserToOrderColumns = false;
            PurchaseHistoryGridRefer.MultiSelect = false;
            PurchaseHistoryGridRefer.ScrollBars = ScrollBars.Horizontal;
            PurchaseHistoryGridRefer.VirtualMode = true;

            PurchaseHistoryGridRefer.Columns.Add("month", "");
            PurchaseHistoryGridRefer.Columns[0].Width = 300;

            int index = 0;
            foreach (int year in PurchaseHistoryHeaderData)
            {
                PurchaseHistoryGridRefer.Columns.Add(year.ToString(), year.ToString());
                PurchaseHistoryGridRefer.Columns[1 + index++].Width = 200;

            }

            foreach (DataGridViewColumn column in PurchaseHistoryGridRefer.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            this.Controls.Add(PurchaseHistoryGridRefer);
        }

        private void LoadInvoiceAgingList()
        {
            var refreshData = InvoiceModelObj.LoadInvoiceAgingData(this.CustomerId);
            List<InvoiceAgingData> agingData = new List<InvoiceAgingData>();

            InvoiceAgingGridRefer.DataSource = InvoiceModelObj.InvoiceAgingDataList;

            InvoiceAgingGridRefer.Columns[0].HeaderText = "Invoice Aging";
            InvoiceAgingGridRefer.Columns[0].Width = this.Width / 3 / 3 * 2;
            InvoiceAgingGridRefer.Columns[1].HeaderText = "";
            InvoiceAgingGridRefer.Columns[1].Width = this.Width / 3 / 3;
        }

        private void LoadPurchaseHistoryList()
        {
            List<double[]> TotalPaidList = InvoiceModelObj.LoadPurchaseHistoryData(this.CustomerId);
            List<int> PurchaseHistoryHeaderData = InvoiceModelObj.GetInvoiceYearList(this.CustomerId);
            string[] ItemList = new string[13] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", "Totals" };

            for (int monthIndex = 0; monthIndex < 13; monthIndex++)
            {
                int rowIndex = PurchaseHistoryGridRefer.Rows.Add();
                DataGridViewRow newRow = PurchaseHistoryGridRefer.Rows[rowIndex];

                newRow.Resizable = DataGridViewTriState.False;
                newRow.Cells["month"].Value = ItemList[monthIndex];
                int index = 0;
                foreach (int year in PurchaseHistoryHeaderData)
                {
                    newRow.Cells[year.ToString()].Value = TotalPaidList[index][monthIndex];
                    index++;
                }
            }
        }
    }
}
