using MJC.common.components;
using MJC.common;
using MJC.forms.qbo;
using MJC.qbo;
using MJC.model;
using System.Drawing.Printing;
using System.Windows.Forms;


namespace MJC.forms
{
    public partial class SystemSettings : GlobalLayout
    {
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);
        private HotkeyButton hkOpenQuickBooks = new HotkeyButton("F5", "Auth QuickBooks", Keys.F5);
        private HotkeyButton hkSyncQuickBooks = new HotkeyButton("F8", "Sync QuickBooks", Keys.F8);

        private FInputBox BusinessName = new FInputBox("Business Name");
        private FInputBox AddressLine1 = new FInputBox("Address Line 1");
        private FInputBox AddressLine2 = new FInputBox("Address Line 2");
        private FInputBox City = new FInputBox("City");
        private FInputBox State = new FInputBox("State");
        private FInputBox Zipcode = new FInputBox("Zipcode");
        private FInputBox Phone = new FInputBox("Phone");
        private FInputBox Fax = new FInputBox("Fax");
        private FInputBox FederalTax = new FInputBox("Federal Tax#");
        private FCheckBox TrainingMode = new FCheckBox("Training Mode");
        private FComboBox TargetPrinter = new FComboBox("Target Printer");
        private FGroupLabel PageQuantities = new FGroupLabel("Page Quantities to Print");
        private FInputBox InvoicePrintQty = new FInputBox("Invoice Qty");
        private FInputBox HoldOrdersPrintQty = new FInputBox("Hold Orders Qty");
        private FInputBox QuotePrintQty = new FInputBox("Quote Qty");
        private FComboBox ProcessingTax = new FComboBox("Processing Tax");
        private FInputBox businessDescription = new FInputBox("Description");
        private FInputBox invoiceFooter = new FInputBox("Invoice Footer");
        private FInputBox invoiceTermsOfService = new FInputBox("Terms Of Service");

        private PictureBox RFPicture;

        public SystemSettings() : base("System Settings", "Manage system settings")
        {
            InitializeComponent();
            _initBasicSize();
            AddHotKeyEvents();

            this.KeyDown += (s, e) => Form_KeyDown(s, e);
            this.FormClosing += SystemSettings_FormClosing;
            HotkeyButton[] hkButtons = new HotkeyButton[3] { hkPreviousScreen, hkOpenQuickBooks, hkSyncQuickBooks };
            _initializeHKButtons(hkButtons);
            InitInputBox();

            GetSettings();

        }

        private void SystemSettings_FormClosing(object? sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to save your changes?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (!SaveSettings())
                {
                    e.Cancel = true;
                    return;
                }

                Messages.ShowInformation("The settings have been saved successfully.");
            }
            else if (result == DialogResult.No)
            {
                _navigateToPrev(sender, e);
            }
        }

        private void GetSettings()
        {
            Session.SalesTaxModelObj.LoadSalesTaxCodeData();
            Session.SettingsModelObj.LoadSettings();

            BusinessName.GetTextBox().Text = Session.SettingsModelObj.Settings.businessName;
            businessDescription.GetTextBox().Text = Session.SettingsModelObj.Settings.businessDescription;
            AddressLine1.GetTextBox().Text = Session.SettingsModelObj.Settings.address1;
            AddressLine2.GetTextBox().Text = Session.SettingsModelObj.Settings.address2;
            City.GetTextBox().Text = Session.SettingsModelObj.Settings.city;
            State.GetTextBox().Text = Session.SettingsModelObj.Settings.state;
            Zipcode.GetTextBox().Text = Session.SettingsModelObj.Settings.postalCode;
            Phone.GetTextBox().Text = Session.SettingsModelObj.Settings.phone;
            Fax.GetTextBox().Text = Session.SettingsModelObj.Settings.fax;
            FederalTax.GetTextBox().Text = Session.SettingsModelObj.Settings.ein;
            TrainingMode.GetCheckBox().Checked = Session.SettingsModelObj.Settings.trainingEnabled;

            invoiceTermsOfService.GetTextBox().Text = Session.SettingsModelObj.Settings.businessTermsOfService;
            invoiceFooter.GetTextBox().Text = Session.SettingsModelObj.Settings.businessFooter;

            TargetPrinter.GetComboBox().Text = Session.SettingsModelObj.Settings.targetPrinter;
            TargetPrinter.GetComboBox().DropDownStyle = ComboBoxStyle.DropDownList;

            InvoicePrintQty.GetTextBox().Text = Session.SettingsModelObj.Settings.invoicePrintQty.ToString();
            HoldOrdersPrintQty.GetTextBox().Text = Session.SettingsModelObj.Settings.holdOrderPrintQty.ToString();
            QuotePrintQty.GetTextBox().Text = Session.SettingsModelObj.Settings.quotePrintQty.ToString();

            var taxCodes = Session.SalesTaxModelObj.SalesTaxCodeDataList.Select(x => x.name).ToArray();
            ProcessingTax.GetComboBox().Items.AddRange(taxCodes);

            var taxCodeId = Session.SettingsModelObj.Settings.taxCodeId;
            // Default selection to 0 if possible
            if (taxCodeId == 0)
            {
                if (taxCodes.Length > 0)
                {
                    ProcessingTax.GetComboBox().SelectedIndex = 0;
                }
            }

            for (int i = 0; i < taxCodes.Length; i++)
            {
                if (Session.SalesTaxModelObj.SalesTaxCodeDataList[i].id == taxCodeId)
                {
                    ProcessingTax.GetComboBox().SelectedIndex = i;
                    break;
                }
            }


            ProcessingTax.GetComboBox().DropDownStyle = ComboBoxStyle.DropDownList;
            ProcessingTax.GetComboBox().AutoCompleteMode = AutoCompleteMode.Suggest;

            InvoicePrintQty.GetTextBox().LostFocus += InvoicePrintQty_LostFocus;
            HoldOrdersPrintQty.GetTextBox().LostFocus += HoldOrdersPrintQty_LostFocus;
            QuotePrintQty.GetTextBox().LostFocus += QuotePrintQty_LostFocus;
        }

        private void InvoicePrintQty_LostFocus(object sender, EventArgs e)
        {
            if (!int.TryParse(InvoicePrintQty.GetTextBox().Text, out int invoicePrintQty))
            {
                Messages.ShowError("Please enter a valid InvoicePrintQty.");
                InvoicePrintQty.GetTextBox().Select();
                return;
            }
        }

        private void HoldOrdersPrintQty_LostFocus(object sender, EventArgs e)
        {
            if (!int.TryParse(HoldOrdersPrintQty.GetTextBox().Text, out int holdOrderPrintQty))
            {
                Messages.ShowError("Please enter a valid HoldOrdersPrintQty.");
                HoldOrdersPrintQty.GetTextBox().Select();
                return;
            }
        }

        private void QuotePrintQty_LostFocus(object sender, EventArgs e)
        {
            if (!int.TryParse(InvoicePrintQty.GetTextBox().Text, out int quotePrintQty))
            {
                Messages.ShowError("Please enter a valid QuotePrintQty.");
                QuotePrintQty.GetTextBox().Select();
                return;
            }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult result = MessageBox.Show("Do you want to save your changes?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (!SaveSettings())
                    {
                        e.SuppressKeyPress = true;
                        e.Handled = true;
                        return;
                    }

                    Messages.ShowInformation("The settings have been saved successfully.");
                }
                else if (result == DialogResult.No)
                {
                    _navigateToPrev(sender, e);
                }
            }
        }

        private bool SaveSettings()
        {
            var businessName = BusinessName.GetTextBox().Text;
            var description = businessDescription.GetTextBox().Text;
            var address1 = AddressLine1.GetTextBox().Text;
            var address2 = AddressLine2.GetTextBox().Text;
            var city = City.GetTextBox().Text;
            var state = State.GetTextBox().Text;
            var zipCode = Zipcode.GetTextBox().Text;
            var phone = Phone.GetTextBox().Text;
            var fax = Fax.GetTextBox().Text;
            var ein = FederalTax.GetTextBox().Text;
            var taxCode = ProcessingTax.GetComboBox().Text;
            var training = TrainingMode.GetCheckBox().Checked;
            var targetPrinter = TargetPrinter.GetComboBox().Text;
            int invoicePrintQty;
            int holdOrderPrintQty;
            int quotePrintQty;

            var footerText = invoiceFooter.GetTextBox().Text;
            var termsOfService = invoiceTermsOfService.GetTextBox().Text;
            var authorizationCode = string.Empty;
            var refreshToken = string.Empty;

            var selectedTaxCode = Session.SalesTaxModelObj.SalesTaxCodeDataList.FirstOrDefault(x => x.name == taxCode);
            if (selectedTaxCode.id == 0)
            {
                // This should never happen
                Messages.ShowError("The Processing Tax Code you entered does not exist.\r\n\r\nPlease make sure that the Tax Code exists and try again.");
                return false;
            }

            if (!int.TryParse(InvoicePrintQty.GetTextBox().Text, out invoicePrintQty))
            {
                Messages.ShowError("Please enter a valid InvoicePrintQty.");
                InvoicePrintQty.GetTextBox().Select();
                return false;
            }

            if (!int.TryParse(HoldOrdersPrintQty.GetTextBox().Text, out holdOrderPrintQty))
            {
                Messages.ShowError("Please enter a valid HoldOrdersPrintQty.");
                HoldOrdersPrintQty.GetTextBox().Select();
                return false;
            }

            if (!int.TryParse(QuotePrintQty.GetTextBox().Text, out quotePrintQty))
            {
                Messages.ShowError("Please enter a valid QuotePrintQty.");
                QuotePrintQty.GetTextBox().Select();
                return false;
            }

            // We want to continue to save the settings
            try
            {
                Session.SettingsModelObj.SaveSetting(selectedTaxCode.id, businessName, description, address1, address2, city, state, zipCode, phone, fax, ein, training, targetPrinter, authorizationCode, refreshToken, footerText, termsOfService, invoicePrintQty, holdOrderPrintQty, quotePrintQty);

            }
            catch (Exception exception)
            {
                Sentry.SentrySdk.CaptureException(exception);

                if (exception.Message.Contains("FOREIGN KEY constraint"))
                {
                    Messages.ShowError("The Processing Tax Code you entered does not exist.\r\n\r\nPlease make sure that the Tax Code exists and try again.");
                }
                else
                {
                    Messages.ShowError("There was an error while saving the settings.");
                }
            }

            GetSettings();

            return true;
        }

        private async void AddHotKeyEvents()
        {
            hkOpenQuickBooks.GetButton().Click += (sender, e) =>
            {
                QboAuth qboAuthModal = new QboAuth();
                qboAuthModal.Show();
            };

            hkSyncQuickBooks.GetButton().Click += async (sender, e) =>
            {
                QboApiService qboClient = new QboApiService();
                try
                {
                    qboClient.InitDatabase();

                    await qboClient.LoadCustomers();

                    ShowInformation("QuickBooks successfully downloaded.");
                }
                catch (Exception exception)
                {
                    ShowError("QuickBooks failed to download data.");
                }
            };
        }

        private void InitInputBox()
        {
            Panel _panel = new System.Windows.Forms.Panel();
            _panel.BackColor = System.Drawing.Color.Transparent;
            _panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            _panel.Location = new System.Drawing.Point(0, 95);
            _panel.Size = new Size(this.Width - 15, this.Height - 340);
            _panel.AutoScroll = true;
            this.Controls.Add(_panel);

            invoiceTermsOfService.GetTextBox().Multiline = true;
            invoiceTermsOfService.GetTextBox().Height = 250;
            invoiceTermsOfService.GetTextBox().Width = 500;

            invoiceFooter.GetTextBox().Width = 400;
            TargetPrinter.GetComboBox().Width = 400;

            InvoicePrintQty.GetTextBox().Width = 400;
            HoldOrdersPrintQty.GetTextBox().Width = 400;
            QuotePrintQty.GetTextBox().Width = 400;
            ProcessingTax.GetComboBox().Width = 400;

            List<dynamic> FormComponents = new List<dynamic>();
            FormComponents.Add(BusinessName);
            FormComponents.Add(businessDescription);
            FormComponents.Add(AddressLine1);
            FormComponents.Add(AddressLine2);
            FormComponents.Add(City);
            FormComponents.Add(State);
            FormComponents.Add(Zipcode);
            FormComponents.Add(Phone);
            FormComponents.Add(Fax);
            FormComponents.Add(FederalTax);
            _addFormInputs(FormComponents, 30, 20, 800, 50, 700, _panel.Controls);

            List<dynamic> FormComponents2 = new List<dynamic>();
            FormComponents2.Add(TrainingMode);
            FormComponents2.Add(TargetPrinter);
            _addFormInputs(FormComponents2, 700, 20, 800, 50, int.MaxValue, _panel.Controls);

            List<dynamic> FormComponents3 = new List<dynamic>();
            FormComponents3.Add(PageQuantities);
            FormComponents3.Add(InvoicePrintQty);
            FormComponents3.Add(HoldOrdersPrintQty);
            FormComponents3.Add(QuotePrintQty);
            _addFormInputs(FormComponents3, 700, 140, 800, 50, int.MaxValue, _panel.Controls);

            List<dynamic> FormComponents4 = new List<dynamic>();
            FormComponents4.Add(ProcessingTax);
            FormComponents4.Add(invoiceFooter);
            FormComponents4.Add(invoiceTermsOfService);
            _addFormInputs(FormComponents4, 700, 370, 800, 50, int.MaxValue, _panel.Controls);

            RFPicture = new PictureBox();
            RFPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            RFPicture.Image = global::MJC.Properties.Resources.refresh_white;
            RFPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            RFPicture.TabStop = false;
            RFPicture.Location = new System.Drawing.Point(TargetPrinter.GetComboBox().Location.X + TargetPrinter.GetComboBox().Width, TargetPrinter.GetComboBox().Location.Y + 2);
            RFPicture.Cursor = Cursors.Hand;
            RFPicture.Click += (s, e) => {
                TargetPrinter.GetComboBox().Items.Clear();
                initTargetPrinter();
            };

            _panel.Controls.Add(RFPicture);

            initTargetPrinter();
        }

        private void initTargetPrinter()
        {
            for (int i = 0; i < System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count; i++)
            {
                string printer = System.Drawing.Printing.PrinterSettings.InstalledPrinters[i];
                TargetPrinter.GetComboBox().Items.Add(new FComboBoxItem(i, printer));
            }

        }

        private void SystemSettings_Load(object sender, EventArgs e)
        {

        }
    }
}
