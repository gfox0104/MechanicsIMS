using MJC.common.components;
using MJC.common;
using MJC.model;

namespace MJC.forms.invoice
{
    public partial class InvoiceInformation : GlobalLayout
    {
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);
        private HotkeyButton hkShipToInfo = new HotkeyButton("F2", "Ship to Information", Keys.F2);

        private FInputBox CustomerNum = new FInputBox("Cust #:");
        private FInputBox CustomerName = new FInputBox("Customer Name:");
        private FInputBox AddressLine1 = new FInputBox("Address:");
        private FInputBox AddressLine2 = new FInputBox("Address Line 2");

        private FInputBox City = new FInputBox("City:", 200);
        private FInputBox State = new FInputBox("State:", 80);
        private FInputBox Zip = new FInputBox("Zip:", 80);

        private FInputBox BusPhone = new FInputBox("Phone:");

        private FGroupLabel AccountTitle = new FGroupLabel("Account Title:");
        private FInputBox InvoiceNumber = new FInputBox("Invoice#:");
        private FDateTime InvoiceDate = new FDateTime("Invoice Date:");
        private FInputBox Description = new FInputBox("Description:");
        private FInputBox Terms = new FInputBox("Terms:");
        private FDateTime PaymentDate = new FDateTime("Payment Date:");
        private FDateTime LastPayment = new FDateTime("Last Payment:");
        private FInputBox InterestRate = new FInputBox("Insterest Rate:");
        private FInputBox InterestApplied = new FInputBox("Insterest Applied:");
        private FInputBox DiscountAllowed = new FInputBox("Discount Allowed:");
        private FInputBox CoreCharges = new FInputBox("Core Charges:");

        private FDateTime ShipDate = new FDateTime("Ship Date:");
        private FInputBox PoNumber = new FInputBox("PO#:");
        private FInputBox ShipVia = new FInputBox("Ship via:");
        private FInputBox Fob = new FInputBox("FOB:");
        private FInputBox Salesman = new FInputBox("Salesman:");

        private FlabelConstant InvoiceTotal = new FlabelConstant("Invoice Total:");
        private FlabelConstant DiscountableAmt = new FlabelConstant("Discountable Amt:");
        private FlabelConstant TotalPayments = new FlabelConstant("Total Payments:");
        private FlabelConstant InvoiceBalance = new FlabelConstant("Invoice Balance:");

        private int customerId = 0;
        private int orderId = 0;

        public InvoiceInformation() : base("Invoice Details", "Details of an invoice")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[2] { hkPreviousScreen, hkShipToInfo };
            _initializeHKButtons(hkButtons, false);
            AddHotKeyEvents();

            InitForms();
            //this.Load += new System.EventHandler(this.Detail_Load);
        }

        private void AddHotKeyEvents()
        {
            hkPreviousScreen.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to save your changes?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveInvoice(sender, e);
                }
                else if (result == DialogResult.No)
                {
                    _navigateToPrev(sender, e);
                }
            };
            hkShipToInfo.GetButton().Click += (sender, e) =>
            {
                int customerId = this.customerId;
                ShipInformation ShipInfoModal = new ShipInformation(customerId);
                _navigateToForm(sender, e, ShipInfoModal);
                this.Hide();
            };
        }

        private void SaveInvoice(object sender, EventArgs e)
        {
            string customerNum = CustomerNum.GetTextBox().Text;
            string customerName = CustomerName.GetTextBox().Text;
            string addressLine1 = AddressLine1.GetTextBox().Text;
            string city = City.GetTextBox().Text;
            string state = State.GetTextBox().Text;
            string zip = Zip.GetTextBox().Text;
            string busPhone = BusPhone.GetTextBox().Text;

            string invoiceNumber = InvoiceNumber.GetTextBox().Text;
            DateTime invoiceDate = InvoiceDate.GetDateTimePicker().Value;
            string description = Description.GetTextBox().Text;
            string terms = Terms.GetTextBox().Text;
            DateTime paymentDate = PaymentDate.GetDateTimePicker().Value;
            DateTime lastPayment = LastPayment.GetDateTimePicker().Value;

            double interestRate = 0;
            double.TryParse(InterestRate.GetTextBox().Text, out interestRate);
            bool interestApplied = true;
            bool.TryParse(InterestApplied.GetTextBox().Text, out interestApplied);
            double discountAllowed = 0;
            double.TryParse(DiscountAllowed.GetTextBox().Text, out discountAllowed);
            double coreCharge = 0;
            double.TryParse(CoreCharges.GetTextBox().Text, out coreCharge);
            DateTime dateShipped = ShipDate.GetDateTimePicker().Value;

            string poNumber = PoNumber.GetTextBox().Text;
            string shipVia = ShipVia.GetTextBox().Text;
            string fob = Fob.GetTextBox().Text;
            string salesman = Salesman.GetTextBox().Text;

            if (customerNum == "" || customerName == "")
            {
                MessageBox.Show("Please fill String field.");
                return;
            }

            //if (customerNum.Length > 2)
            //{
            //    MessageBox.Show("Cust # Field mustn't limit 3.");
            //    return;
            //}

            if (state.Length > 2)
            {
                MessageBox.Show("State Field mustn't limit 3.");
                return;
            }

            bool refreshData = false;
            int orderId = this.orderId;

            if (orderId == 0)
                refreshData = true;
            else
                refreshData = Session.InvoicesModelObj.UpdateInvoice(invoiceNumber, invoiceDate, description, terms, paymentDate, lastPayment, interestRate, interestApplied, discountAllowed, coreCharge, dateShipped, poNumber, shipVia, fob, salesman, this.orderId);

            string modeText = orderId == 0 ? "creating" : "updating";

            if (refreshData)
            {
                this.DialogResult = DialogResult.OK;
                this._navigateToPrev(sender, e);
            }
            else MessageBox.Show("An Error occured while " + modeText + " the customer.");
        }

        private void InitForms()
        {
            Panel _panel = new System.Windows.Forms.Panel();
            _panel.BackColor = System.Drawing.Color.Transparent;
            _panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            _panel.Location = new System.Drawing.Point(0, 95);
            _panel.Size = new Size(this.Width - 15, this.Height - 340);
            _panel.AutoScroll = true;
            this.Controls.Add(_panel);

            List<dynamic> FormComponents = new List<dynamic>();

            FormComponents.Add(CustomerNum);
            FormComponents.Add(CustomerName);
            FormComponents.Add(AddressLine1);
            List<dynamic> LineComponents = new List<dynamic>();

            City.GetTextBox().Width = 200;
            LineComponents.Add(City);
            State.GetTextBox().Width = 100;
            LineComponents.Add(State);
            Zip.GetTextBox().Width = 80;
            LineComponents.Add(Zip);
            FormComponents.Add(LineComponents);

            FormComponents.Add(BusPhone);

            FormComponents.Add(AccountTitle);
            FormComponents.Add(InvoiceNumber);
            FormComponents.Add(InvoiceDate);
            FormComponents.Add(Description);
            FormComponents.Add(Terms);
            FormComponents.Add(PaymentDate);
            FormComponents.Add(LastPayment);

            List<dynamic> LineComponents1 = new List<dynamic>();
            LineComponents1.Add(InterestRate);
            LineComponents1.Add(DiscountAllowed);
            List<dynamic> LineComponents2 = new List<dynamic>();
            LineComponents2.Add(InterestApplied);
            LineComponents2.Add(CoreCharges);

            FormComponents.Add(LineComponents1);
            FormComponents.Add(LineComponents2);

            _addFormInputs(FormComponents, 30, 20, 800, 48, int.MaxValue, _panel.Controls);

            List<dynamic> FormComponents2 = new List<dynamic>();
            FormComponents2.Add(ShipDate);
            FormComponents2.Add(PoNumber);
            FormComponents2.Add(ShipVia);
            FormComponents2.Add(Fob);
            FormComponents2.Add(Salesman);

            FormComponents2.Add(InvoiceTotal);
            FormComponents2.Add(DiscountableAmt);
            FormComponents2.Add(TotalPayments);
            FormComponents2.Add(InvoiceBalance);

            _addFormInputs(FormComponents2, 1220, 20, 800, 48, int.MaxValue, _panel.Controls);

        }

        public void setDetails(int _id)
        {
            this.customerId = _id;
            List<dynamic> data = new List<dynamic>();
            data = Session.InvoicesModelObj.GetInvoiceDataById(_id);

            var invoiceData = data[0];
            this.orderId = invoiceData.orderId;
            CustomerNum.GetTextBox().Text = invoiceData.customerNumber.ToString();
            CustomerName.GetTextBox().Text = invoiceData.displayName.ToString();
            AddressLine1.GetTextBox().Text = invoiceData.address1.ToString();
            City.GetTextBox().Text = invoiceData.city.ToString();
            State.GetTextBox().Text = invoiceData.state.ToString();
            Zip.GetTextBox().Text = invoiceData.zipcode.ToString();
            BusPhone.GetTextBox().Text = invoiceData.businessPhone.ToString();

            InvoiceNumber.GetTextBox().Text = invoiceData.invoiceNumber.ToString();
            if (!ReferenceEquals(invoiceData.invoiceDate, DBNull.Value) || invoiceData.invoiceDate != null)
                InvoiceDate.GetDateTimePicker().Value = invoiceData.invoiceDate.ToLocalTime();
            Description.GetTextBox().Text = invoiceData.invoiceDesc.ToString();
            Terms.GetTextBox().Text = invoiceData.terms.ToString();
            if (!string.IsNullOrEmpty(invoiceData.paidDate.ToString()))
                PaymentDate.GetDateTimePicker().Value = invoiceData.paidDate.ToLocalTime();
            if (!string.IsNullOrEmpty(invoiceData.lastPayment.ToString()))
                LastPayment.GetDateTimePicker().Value = invoiceData.lastPayment.ToLocalTime();
            InterestRate.GetTextBox().Text = invoiceData.interestRate.ToString();
            InterestApplied.GetTextBox().Text = invoiceData.interestApplied.ToString();
            DiscountAllowed.GetTextBox().Text = invoiceData.discountAllowed.ToString();
            CoreCharges.GetTextBox().Text = invoiceData.coreCharge.ToString();
            if (!string.IsNullOrEmpty(invoiceData.dateShipped.ToString()))
                ShipDate.GetDateTimePicker().Value = invoiceData.dateShipped.ToLocalTime();
            PoNumber.GetTextBox().Text = invoiceData.poNumber.ToString();
            ShipVia.GetTextBox().Text = invoiceData.shipVia.ToString();
            Fob.GetTextBox().Text = invoiceData.fob.ToString();
            Salesman.GetTextBox().Text = invoiceData.salesman.ToString();

            InvoiceTotal.SetContext(invoiceData.invoiceTotal.ToString());
            DiscountableAmt.SetContext(invoiceData.discountableAmount.ToString());
            TotalPayments.SetContext(invoiceData.totalPaid.ToString());
            InvoiceBalance.SetContext(invoiceData.invoiceBalance.ToString());
        }
    }
}
