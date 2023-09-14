using MJC.common.components;
using MJC.common;
using MJC.model;
using MJC.qbo;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics.Metrics;
using MJC.forms.creditcard;
using MJC.forms.sku;
using System;

namespace MJC.forms.customer
{
    public partial class CustomerInformation : GlobalLayout
    {
        private HotkeyButton hkCustomerMemo = new HotkeyButton("F2", "Customer Memo", Keys.F2);
        private HotkeyButton hkPriceLevels = new HotkeyButton("F3", "Price Levels", Keys.F3);
        private HotkeyButton hkShipToInfo = new HotkeyButton("F4", "Ship-to Info", Keys.F4);
        private HotkeyButton hkCreditCards = new HotkeyButton("F6", "Credit Cards", Keys.F6);
        private HotkeyButton hkSetArchived = new HotkeyButton("F9", "Set Archived", Keys.F9);

        private FGroupLabel CustomerInfo = new FGroupLabel("CustomerInfo");
        private FInputBox CustomerNum = new FInputBox("Cust #");
        private FInputBox DisplayName = new FInputBox("Display Name");
        private FInputBox Title = new FInputBox("Title");
        private FInputBox GiveName = new FInputBox("Given Name");
        private FInputBox MiddleName = new FInputBox("Middle Name");
        private FInputBox Suffix = new FInputBox("Suffix", 80);
        private FInputBox FamilyName = new FInputBox("Family Name");
        private FInputBox AddressLine1 = new FInputBox("Address Line 1");
        private FInputBox AddressLine2 = new FInputBox("Address Line 2");

        private FInputBox City = new FInputBox("City", 200);
        private FInputBox State = new FInputBox("State", 80);
        private FInputBox Zip = new FInputBox("Zip", 80);

        private FMaskedTextBox BusPhone = new FMaskedTextBox("Bus.Phone");
        private FMaskedTextBox HomePhone = new FMaskedTextBox("Home Phone");
        private FInputBox EMail = new FInputBox("E-mail");
        private FMaskedTextBox Fax = new FMaskedTextBox("Fax");
        private FDateTime DateOpened = new FDateTime("Date opened");
        private FInputBox Salesman = new FInputBox("Salesman");
        private FCheckBox Resale = new FCheckBox("Resale");
        private FInputBox StmtCust = new FInputBox("Stmt Cust#");
        private FInputBox StmtName = new FInputBox("Stmt Name");
        private FComboBox PriceTier = new FComboBox("Price Tier");
        private FInputBox Terms = new FInputBox("Terms");
        private FInputBox Limit = new FInputBox("Limit");
        private FInputBox Memo = new FInputBox("Memo");
        
        private FCheckBox Taxable = new FCheckBox("Taxable");
        private FCheckBox SendStatements = new FCheckBox("Send Statements");
        private FInputBox CoreTracking = new FInputBox("Core Tracking");
        private FInputBox CoreBalance = new FInputBox("Core Balance");
        private FCheckBox PrintCoreTot = new FCheckBox("Print Core Tot");
        private FInputBox AccountType = new FInputBox("Account Type");
        private FCheckBox PORequired = new FCheckBox("PO# Required");
        private FComboBox CreditCode = new FComboBox("Credit Code");
        private FInputBox InterestRate = new FInputBox("Interest Rate");
        private FInputBox AcctBalance = new FInputBox("Acct Balance");
        private FInputBox YTDPurchases = new FInputBox("YTDPurchases");
        private FInputBox YTDInterest = new FInputBox("YTDInterest");
        private FDateTime DateLastPurch = new FDateTime("DateLastPurch");

        private CustomersModel CustomersModelObj = new CustomersModel();
        private PriceTiersModel PriceTiersModelObj = new PriceTiersModel();
        private CreditCodeModel CreditCodeModelObj = new CreditCodeModel();

        private int customerId = 0;
        private string qboId = "";
        private string syncToken = "";
        private string memo = "";
        private bool disabled = false;

        public CustomerInformation(bool disabled = false) : base("Customer Information", "Manage details of Customer")
        {
            this.Text = "Customer Detail";
            InitializeComponent();
            _initBasicSize();
            this.KeyDown += (s, e) => Form_KeyDown(s, e);

            HotkeyButton[] hkButtons;
            if (disabled)
            {
                hkButtons = new HotkeyButton[2] { hkCustomerMemo, hkCreditCards };
            } else
            {
                hkButtons = new HotkeyButton[5] { hkCustomerMemo, hkPriceLevels, hkShipToInfo, hkCreditCards, hkSetArchived };
            }
            _initializeHKButtons(hkButtons, false);
            AddHotKeyEvents();

            this.disabled = disabled;
            InitForms();
            this.Load += new System.EventHandler(this.CustomerDetail_Load);
        }

        private void AddHotKeyEvents()
        {
            hkSetArchived.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to set archived?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    CustomersModelObj.UpdateCustomerArchived(true, this.customerId);
                }
                else if (result == DialogResult.No)
                {
                    CustomersModelObj.UpdateCustomerArchived(false, this.customerId);
                }
            };
            hkCustomerMemo.GetButton().Click += (sender, e) =>
            {
                CustomerMemo MemoModal = new CustomerMemo(customerId, memo, this.disabled);
                this.Enabled = false;
                MemoModal.Show();
                MemoModal.FormClosed += (ss, sargs) =>
                {
                    this.memo = MemoModal.getMemo();
                    this.Enabled = true;
                };

            };
            hkCreditCards.GetButton().Click += (sender, e) =>
            {
                CreditCards creditCardModel = new CreditCards(this.customerId, this.disabled);
                _navigateToForm(sender, e, creditCardModel);
                this.Hide();
            };

        }

        private void InitMBOKButton()
        {
            //            ModalButton_HotKeyDown(MBOk);
            //            MBOk_button = MBOk.GetButton();
            //            MBOk_button.Location = new Point(1025, this.Height - 115);
            //            MBOk_button.Click += (sender, e) => ok_button_Click(sender, e);
            //            this.Controls.Add(MBOk_button);
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

            FormComponents.Add(CustomerInfo);
            FormComponents.Add(CustomerNum);
            FormComponents.Add(DisplayName);
            FormComponents.Add(GiveName);
            FormComponents.Add(MiddleName);
            FormComponents.Add(FamilyName);

            List<dynamic> LineComponents = new List<dynamic>();
            Title.GetTextBox().Width = 90;
            Suffix.GetTextBox().Width = 90;
            LineComponents.Add(Title);
            LineComponents.Add(Suffix);
            FormComponents.Add(LineComponents);
            
            FormComponents.Add(AddressLine1);
            FormComponents.Add(AddressLine2);

            List<dynamic> LineComponents1 = new List<dynamic>();
            City.GetTextBox().Width = 200;
            LineComponents1.Add(City);
            State.GetTextBox().Width = 100;
            State.GetTextBox().MaxLength = 2;
            LineComponents1.Add(State);
            Zip.GetTextBox().Width = 80;
            LineComponents1.Add(Zip);
            FormComponents.Add(LineComponents1);

            List<dynamic> LineComponents2 = new List<dynamic>();
            BusPhone.GetTextBox().Mask = "(999) 000-0000";
            BusPhone.GetTextBox().PromptChar = ' ';
            FormComponents.Add(BusPhone);
            Fax.GetTextBox().Mask = "(999) 000-0000";
            Fax.GetTextBox().PromptChar = ' ';
            FormComponents.Add(Fax);
            HomePhone.GetTextBox().Mask = "(999) 000-0000";
            HomePhone.GetTextBox().PromptChar = ' ';
            FormComponents.Add(HomePhone);

            FormComponents.Add(EMail);

            List<dynamic> LineComponents3 = new List<dynamic>();
            LineComponents3.Add(Salesman);
            LineComponents3.Add(Resale);
            FormComponents.Add(LineComponents3);
            StmtCust.GetTextBox().MaxLength = 50;
            StmtName.GetTextBox().MaxLength = 50;
            FormComponents.Add(StmtCust);
            FormComponents.Add(StmtName);

            _addFormInputs(FormComponents, 30, 20, 800, 43, int.MaxValue, _panel.Controls);

            List<dynamic> FormComponents2 = new List<dynamic>();

            FormComponents2.Add(DateOpened);
            FormComponents2.Add(PriceTier);
            FormComponents2.Add(Terms);
            FormComponents2.Add(Limit);
            FormComponents2.Add(Memo);
             
            List<dynamic> LineComponents5 = new List<dynamic>();
            LineComponents5.Add(Taxable);
            LineComponents5.Add(SendStatements);
            FormComponents2.Add(LineComponents5);

            FormComponents2.Add(CoreTracking);

            List<dynamic> LineComponents6 = new List<dynamic>();
            LineComponents6.Add(CoreBalance);
            LineComponents6.Add(PrintCoreTot);
            FormComponents2.Add(LineComponents6);

            List<dynamic> LineComponents7 = new List<dynamic>();
            LineComponents7.Add(AccountType);
            LineComponents7.Add(PORequired);
            FormComponents2.Add(LineComponents7);

            FormComponents2.Add(CreditCode);
            FormComponents2.Add(InterestRate);
            FormComponents2.Add(AcctBalance);
            FormComponents2.Add(YTDPurchases);
            FormComponents2.Add(YTDInterest);
            FormComponents2.Add(DateLastPurch);

            _addFormInputs(FormComponents2, 1000, 20, 800, 43, int.MaxValue, _panel.Controls);

            CoreBalance.GetTextBox().KeyPress += validateDecimal;
            AcctBalance.GetTextBox().KeyPress += validateDecimal;
            InterestRate.GetTextBox().KeyPress += validateDecimal;
            YTDInterest.GetTextBox().KeyPress += validateDecimal;
            YTDPurchases.GetTextBox().KeyPress += validateNumber;

            if (this.disabled)
            {
                EMail.GetTextBox().Enabled = false;
                Salesman.GetTextBox().Enabled = false;
                Resale.GetCheckBox().Enabled = false;
                StmtCust.GetTextBox().Enabled = false;
                StmtName.GetTextBox().Enabled = false;
                DateOpened.GetDateTimePicker().Enabled = false;
                PriceTier.GetComboBox().Enabled = false;
                Terms.GetTextBox().Enabled = false;
                Limit.GetTextBox().Enabled = false;
                Memo.GetTextBox().Enabled = false;
                Taxable.GetCheckBox().Enabled = false;
                SendStatements.GetCheckBox().Enabled = false;
                CoreTracking.GetTextBox().Enabled = false;
                CoreBalance.GetTextBox().Enabled = false;
                PrintCoreTot.GetCheckBox().Enabled = false;
                AccountType.GetTextBox().Enabled = false;
                PORequired.GetCheckBox().Enabled = false;
                CreditCode.GetComboBox().Enabled = false;
                InterestRate.GetTextBox().Enabled = false;
                AcctBalance.GetTextBox().Enabled = false;
                YTDPurchases.GetTextBox().Enabled = false;
                YTDInterest.GetTextBox().Enabled = false;
                DateLastPurch.GetDateTimePicker().Enabled = false;
                CustomerNum.GetTextBox().Enabled = false;
                DisplayName.GetTextBox().Enabled = false;
                GiveName.GetTextBox().Enabled = false;
                MiddleName.GetTextBox().Enabled = false;
                FamilyName.GetTextBox().Enabled = false;
                Title.GetTextBox().Enabled = false;
                Suffix.GetTextBox().Enabled = false;
                AddressLine1.GetTextBox().Enabled = false;
                AddressLine2.GetTextBox().Enabled = false;
                City.GetTextBox().Enabled = false;
                State.GetTextBox().Enabled = false;
                Zip.GetTextBox().Enabled = false;
                BusPhone.GetTextBox().Enabled = false;
                Fax.GetTextBox().Enabled = false;
                HomePhone.GetTextBox().Enabled = false;
            }
        }

        private void validateNumber(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void validateDecimal(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-')
            {
                e.Handled = true;
            }

            // Allow only one decimal point
            if (e.KeyChar == '.' && ((TextBox)sender).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }

            // Allow negative sign only as the first character
            if (e.KeyChar == '-' && ((TextBox)sender).SelectionStart != 0)
            {
                e.Handled = true;
            }
        }

        public void setDetails(int _id)
        {
            //List<dynamic> data = new List<dynamic>();
            var data = (dynamic)CustomersModelObj.GetCustomerDataById(_id);

            if (data.memo != null && !data.memo.Equals(DBNull.Value)) this.memo = data.memo;

            var customerData = data;

            if (customerData.id != null) this.customerId = customerData.id;
            if (customerData.qboId.ToString() != null) this.qboId = customerData.qboId.ToString();
            if (customerData.syncToken.ToString() != null) this.syncToken = customerData.syncToken.ToString();
            if (customerData.customerNumber.ToString() != null) this.CustomerNum.GetTextBox().Text = customerData.customerNumber.ToString();
            if (customerData.displayName.ToString() != null) this.DisplayName.GetTextBox().Text = customerData.displayName.ToString();
            if (customerData.givenName.ToString() != null) this.GiveName.GetTextBox().Text = customerData.givenName.ToString();
            if (customerData.middleName.ToString() != null) this.MiddleName.GetTextBox().Text = customerData.middleName.ToString();
            if (customerData.familyName.ToString() != null) this.FamilyName.GetTextBox().Text = customerData.familyName.ToString();
            if (customerData.title.ToString() != null) this.Title.GetTextBox().Text = customerData.title.ToString();
            if (customerData.suffix.ToString() != null) this.Suffix.GetTextBox().Text = customerData.suffix.ToString();
            if (customerData.address1.ToString() != null) this.AddressLine1.GetTextBox().Text = customerData.address1.ToString();
            if (customerData.address2.ToString() != null) this.AddressLine2.GetTextBox().Text = customerData.address2.ToString();
            if (customerData.ToString() != null) this.City.GetTextBox().Text = customerData.city.ToString();
            if (customerData.state.ToString() != null) this.State.GetTextBox().Text = customerData.state.ToString();
            if (customerData.zipcode.ToString() != null) this.Zip.GetTextBox().Text = customerData.zipcode.ToString();
            if (customerData.businessPhone.ToString() != null) this.BusPhone.GetTextBox().Text = customerData.businessPhone.ToString();
            if (customerData.fax.ToString() != null) this.Fax.GetTextBox().Text = customerData.fax.ToString();
            if (customerData.email.ToString() != null) this.EMail.GetTextBox().Text = customerData.email.ToString();
            if (customerData.dateOpened.ToLocalTime() != null) this.DateOpened.GetDateTimePicker().Value = customerData.dateOpened.ToLocalTime();
            if (customerData.salesman.ToString() != null) this.Salesman.GetTextBox().Text = customerData.salesman.ToString();
            if (!String.IsNullOrEmpty(customerData.resale.ToString())) this.Resale.GetCheckBox().Checked = bool.Parse(customerData.resale.ToString());
            if (customerData.statementCustomerNumber.ToString() != null) this.StmtCust.GetTextBox().Text = customerData.statementCustomerNumber.ToString();
            if (customerData.statementName.ToString() != null) this.StmtName.GetTextBox().Text = customerData.statementName.ToString();
            var m_priceTierId = customerData.priceTierId;
            int priceTierOut = 0;
            if (customerData.priceTierId != null) this.PriceTier.GetComboBox().SelectedValue = int.TryParse(customerData.priceTierId.ToString(), out priceTierOut);
            if (customerData.terms.ToString() != null) this.Terms.GetTextBox().Text = customerData.terms.ToString();
            if (customerData.limit.ToString() != null) this.Limit.GetTextBox().Text = customerData.limit.ToString();
            if (customerData.memo.ToString() != null) this.Memo.GetTextBox().Text = customerData.memo.ToString();
            if (!String.IsNullOrEmpty(customerData.taxable.ToString())) this.Taxable.GetCheckBox().Checked = bool.Parse(customerData.taxable.ToString());
            if (!String.IsNullOrEmpty(customerData.sendStatements.ToString())) this.SendStatements.GetCheckBox().Checked = bool.Parse(customerData.sendStatements.ToString());
            if (customerData.coreTracking.ToString() != null) this.CoreTracking.GetTextBox().Text = customerData.coreTracking.ToString();
            if (customerData.coreBalance.ToString() != null) this.CoreBalance.GetTextBox().Text = customerData.coreBalance.ToString();
            if (!String.IsNullOrEmpty(customerData.printCoreTotal.ToString())) this.PrintCoreTot.GetCheckBox().Checked = bool.Parse(customerData.printCoreTotal.ToString());
            if (customerData.accountType.ToString() != null) this.AccountType.GetTextBox().Text = customerData.accountType.ToString();
            if (!String.IsNullOrEmpty(customerData.poRequired.ToString())) this.PORequired.GetCheckBox().Checked = bool.Parse(customerData.poRequired.ToString());
            if (customerData.creditCodeId != null) this.CreditCode.GetComboBox().SelectedValue = customerData.creditCodeId;
            if (customerData.interestRate.ToString() != null) this.InterestRate.GetTextBox().Text = customerData.interestRate.ToString();
            if (customerData.accountBalance.ToString() != null) this.AcctBalance.GetTextBox().Text = customerData.accountBalance.ToString();

            if (customerData.yearToDateInterest.ToString() != null) this.YTDInterest.GetTextBox().Text = customerData.yearToDateInterest.ToString();
            if (!string.IsNullOrEmpty(customerData.dateLastPurchased.ToString()))
                this.DateLastPurch.GetDateTimePicker().Value = customerData.dateLastPurchased.ToLocalTime();
        
        }

        private async void SaveCustomer(object sender, KeyEventArgs e)
        {
            string customerNumber = CustomerNum.GetTextBox().Text;
            string displayName = DisplayName.GetTextBox().Text;
            string givenName = GiveName.GetTextBox().Text;
            string middleName = MiddleName.GetTextBox().Text;
            string familyName = FamilyName.GetTextBox().Text;
            string title = Title.GetTextBox().Text;
            string suffix = Suffix.GetTextBox().Text;
            string address1 = AddressLine1.GetTextBox().Text;
            string address2 = AddressLine2.GetTextBox().Text;
            string city = City.GetTextBox().Text;
            string state = State.GetTextBox().Text;
            string zipcode = Zip.GetTextBox().Text;
            string business_phone = BusPhone.GetTextBox().Text;
            string homePhone = HomePhone.GetTextBox().Text;
            string fax = Fax.GetTextBox().Text;
            string email = EMail.GetTextBox().Text;
            DateTime date_opened = DateOpened.GetDateTimePicker().Value;
            string salesman = Salesman.GetTextBox().Text;
            bool resale = Resale.GetCheckBox().Checked;
            string stmtCustomerNumber = StmtCust.GetTextBox().Text;
            string stmtName = StmtName.GetTextBox().Text;
            int selectedPriceTierIndex = PriceTier.GetComboBox().SelectedIndex;
            int? priceTierId = null;
            if (selectedPriceTierIndex != -1)
            {
                PriceTierData priceTierItem = (PriceTierData)PriceTier.GetComboBox().SelectedItem;
                priceTierId = priceTierItem.Id;
            }

            string terms = Terms.GetTextBox().Text;
            string limit = Limit.GetTextBox().Text;
            string memo = Memo.GetTextBox().Text;
            bool taxable = Taxable.GetCheckBox().Checked;
            bool send_stm = SendStatements.GetCheckBox().Checked;
            string core_tracking = CoreTracking.GetTextBox().Text;
            decimal? coreBalance = null;
            if(!string.IsNullOrEmpty(CoreBalance.GetTextBox().Text))
                coreBalance = decimal.Parse(CoreBalance.GetTextBox().Text);
            string acct_type = AccountType.GetTextBox().Text;
            bool print_core_tot = PrintCoreTot.GetCheckBox().Checked;
            bool porequired = PORequired.GetCheckBox().Checked;
            int? creditCodeId = null;
            int selectedCreditCodeIndex = CreditCode.GetComboBox().SelectedIndex;
            if(selectedCreditCodeIndex != -1)
            {
                CreditCodeData creditCodeItem = (CreditCodeData)CreditCode.GetComboBox().SelectedItem;
                creditCodeId = creditCodeItem.Id;
            }
            decimal? interestRate = null;
            if(!string.IsNullOrEmpty(InterestRate.GetTextBox().Text))
                interestRate = decimal.Parse(InterestRate.GetTextBox().Text);
            decimal? accountBalance = null;
            if (!string.IsNullOrEmpty(AcctBalance.GetTextBox().Text))
                accountBalance = decimal.Parse(AcctBalance.GetTextBox().Text);
            int? ytdPurchases = null;
            if(!string.IsNullOrEmpty(YTDPurchases.GetTextBox().Text))
                ytdPurchases = int.Parse(YTDPurchases.GetTextBox().Text.ToString());
            decimal? ytdInterest = null;
            if (!string.IsNullOrEmpty(YTDInterest.GetTextBox().Text))
                ytdInterest = decimal.Parse(YTDInterest.GetTextBox().Text);
            DateTime last_date_purch = DateLastPurch.GetDateTimePicker().Value;

            if (!IsFieldsValidated()) return;
 
            QboApiService qboClient = new QboApiService();

            if (this.customerId == 0)
            {
                var success = await qboClient.CreateCustomer(displayName, givenName, middleName, familyName, title, suffix, business_phone, homePhone, fax, address1, address2, city, state, zipcode, email, date_opened, salesman, resale, stmtCustomerNumber, stmtName, priceTierId, terms, limit, memo, taxable, send_stm, core_tracking, coreBalance, acct_type, print_core_tot, porequired, creditCodeId, interestRate, accountBalance, ytdPurchases, ytdInterest, last_date_purch, customerNumber);
               
                ShowInformation("The customers has been added successfully.");
            }
            else
            {
                qboClient.UpdateCustomer(displayName, givenName, middleName, familyName, title, suffix, business_phone, homePhone, fax, address1, address2, city, state, zipcode, email, date_opened, salesman, resale, stmtCustomerNumber, stmtName, priceTierId, terms, limit, memo, taxable, send_stm, core_tracking, coreBalance, acct_type, print_core_tot, porequired, creditCodeId, interestRate, accountBalance, ytdPurchases, ytdInterest, last_date_purch, this.qboId, this.syncToken, this.customerId, customerNumber);

                ShowInformation("The customers information has been updated successfully.");
            }

            _navigateToPrev(sender, e);
            //string modeText = customerId == 0 ? "creating" : "updating";

            //if (refreshData)
            //{
            //    this.DialogResult = DialogResult.OK;
            //    this._navigateToPrev(sender, e);
            //}
            //else MessageBox.Show("An Error occured while " + modeText + " the customer.");
        }

        private bool IsFieldsValidated()
        {
            string customerNumber = CustomerNum.GetTextBox().Text;
            string displayName = DisplayName.GetTextBox().Text;
            string givenName = GiveName.GetTextBox().Text;
            string middleName = MiddleName.GetTextBox().Text;
            string familyName = FamilyName.GetTextBox().Text;
            string title = Title.GetTextBox().Text;
            string suffix = Suffix.GetTextBox().Text;
            string address1 = AddressLine1.GetTextBox().Text;
            string address2 = AddressLine2.GetTextBox().Text;
            string city = City.GetTextBox().Text;
            string state = State.GetTextBox().Text;
            string zipcode = Zip.GetTextBox().Text;
            string business_phone = BusPhone.GetTextBox().Text;
            string homePhone = HomePhone.GetTextBox().Text;
            string fax = Fax.GetTextBox().Text;
            string email = EMail.GetTextBox().Text;

            if (string.IsNullOrEmpty(customerNumber))
            {
                ShowError("Please provide a customer number.");
                return false;
            }

            if (string.IsNullOrEmpty(displayName))
            {
                ShowError("Please provide a display name.");
                return false;
            }

            if (string.IsNullOrEmpty(givenName))
            {
                ShowError("Please provide the customers first name");
                return false;
            }

            if (string.IsNullOrEmpty(familyName))
            {
                ShowError("Please provide the customers lastname");
                return false;
            }

            if (string.IsNullOrEmpty(address1))
            {
                ShowError("Please provide the customers address.");
                return false;
            }

            if (string.IsNullOrEmpty(city))
            {
                ShowError("Please provide the customers city.");
                return false;
            }

            if (string.IsNullOrEmpty(state))
            {
                ShowError("Please provide the customers state.");
                return false;
            }

            if (string.IsNullOrEmpty(zipcode))
            {
                ShowError("Please provide the customers zip code.");
                return false;
            }

            if (string.IsNullOrEmpty(business_phone))
            {
                ShowError("Please provide the customers business phone number.");
                return false;
            }

            return true;
        }

        private void CustomerDetail_Load(object sender, EventArgs e)
        {
            bool initFlag = true;
  
            var refreshData = PriceTiersModelObj.LoadPriceTierData();
            List<PriceTierData> priceTierList = PriceTiersModelObj.PriceTierDataList;
     
            PriceTier.GetComboBox().DataSource = priceTierList;
            PriceTier.GetComboBox().DisplayMember = "Name";
            PriceTier.GetComboBox().ValueMember = "Id";
            //PriceTier.GetComboBox().SelectedValue = "Id";

            refreshData = CreditCodeModelObj.LoadCreditCodeData();
            List<CreditCodeData> creditCodeDataList = CreditCodeModelObj.CreditCodeDataList;

            CreditCode.GetComboBox().DataSource = creditCodeDataList;
            CreditCode.GetComboBox().DisplayMember = "Code";
            CreditCode.GetComboBox().ValueMember = "Id";
            //CreditCode.GetComboBox().SelectedValue = "Id";

        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if(this.disabled == false)
                {
                    DialogResult result = MessageBox.Show("Do you want to save your changes?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        SaveCustomer(sender, e);
                    }
                    else if (result == DialogResult.No)
                    {
                        _navigateToPrev(sender, e);
                    }
                } else
                {
                    _navigateToPrev(sender, e);
                }
            }
        }
    }
}
