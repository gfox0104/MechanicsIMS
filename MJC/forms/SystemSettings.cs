using MJC.common.components;
using MJC.common;
using MJC.forms.qbo;
using MJC.qbo;
using MJC.model;
using Sentry;
using System.Drawing.Printing;


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
        private FInputBox ProcessingTax = new FInputBox("Processing Tax");
        private FInputBox businessDescription = new FInputBox("Description");

        private SystemSettingsModel SettingsModelObj = new SystemSettingsModel();
        private SalesTaxCodeModel SalesTaxModelObj = new SalesTaxCodeModel();
        
        public SystemSettings() : base("System Settings", "Manage system settings")
        {
            InitializeComponent();
            _initBasicSize();
            AddHotKeyEvents();

            this.KeyDown += (s, e) => Form_KeyDown(s, e);

            HotkeyButton[] hkButtons = new HotkeyButton[3] { hkPreviousScreen, hkOpenQuickBooks , hkSyncQuickBooks};
            _initializeHKButtons(hkButtons);
            InitInputBox();

            GetSettings();
        }

        private void GetSettings()
        {
            SettingsModelObj.LoadSettings();

            BusinessName.GetTextBox().Text = SettingsModelObj.Settings.businessName;
            businessDescription.GetTextBox().Text = SettingsModelObj.Settings.businessDescription;
            AddressLine1.GetTextBox().Text = SettingsModelObj.Settings.address1;
            AddressLine2.GetTextBox().Text = SettingsModelObj.Settings.address2;
            City.GetTextBox().Text = SettingsModelObj.Settings.city;
            State.GetTextBox().Text = SettingsModelObj.Settings.state;
            Zipcode.GetTextBox().Text = SettingsModelObj.Settings.postalCode;
            Phone.GetTextBox().Text = SettingsModelObj.Settings.phone;
            Fax.GetTextBox().Text = SettingsModelObj.Settings.fax;
            FederalTax.GetTextBox().Text = SettingsModelObj.Settings.ein;
            TargetPrinter.GetComboBox().Text = SettingsModelObj.Settings.targetPrinter;
            TrainingMode.GetCheckBox().Checked = SettingsModelObj.Settings.trainingEnabled;
            ProcessingTax.GetTextBox().Text = SettingsModelObj.Settings.taxCodeId.GetValueOrDefault().ToString();
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
            var taxCode = ProcessingTax.GetTextBox().Text;
            var training = TrainingMode.GetCheckBox().Checked;
            var targetPrinter = TargetPrinter.GetComboBox().Text;
            var authorizationCode = string.Empty;
            var refreshToken = string.Empty;

            int? taxCodeId = null;
            if(int.TryParse(taxCode, out int _taxCodeId))
            {
                taxCodeId = _taxCodeId;
            }

            if (taxCodeId == 0) taxCodeId = null;

            if (taxCodeId != null)
            {
                SalesTaxModelObj.LoadSalesTaxCodeData();
                var salesCode = SalesTaxModelObj.GetSalesTaxCodeData(taxCodeId.GetValueOrDefault());
                if(salesCode == null)
                {
                    // Null the taxCodeId if the code doesn't exist to ensure the settings save.
                    taxCodeId = null;

                    Messages.ShowError("The Processing Tax Code you entered does not exist.\r\n\r\nPlease make sure that the Tax Code exists and try again.");
                }
            }

             
            // We want to continue to save the settings

            try
            {
                SettingsModelObj.SaveSetting(taxCodeId, businessName, description, address1, address2, city, state, zipCode, phone, fax, ein, training, targetPrinter, authorizationCode, refreshToken);
            }
            catch(Exception exception) 
            {
                SentrySdk.CaptureException(exception);

                if(exception.Message.Contains("FOREIGN KEY constraint"))
                {
                    Messages.ShowError("The Processing Tax Code you entered does not exist.\r\n\r\nPlease make sure that the Tax Code exists and try again.");
                }
                else
                {
                    Messages.ShowError("There was an error while saving the settings.");
                }
            }

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
                catch(Exception exception)
                {
                    ShowError("QuickBooks failed to download data.");
                }
            };
        }

        private void InitInputBox()
        {
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
            FormComponents.Add(TrainingMode);
            FormComponents.Add(TargetPrinter);
            FormComponents.Add(ProcessingTax);

            _addFormInputs(FormComponents, 30, 150, 500, 50);

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
        // Function to get printer status
        private string GetPrinterStatusText(string status)
        {
            switch (status)
            {
                case "2":
                    return "Ready";
                case "3":
                    return "Printing";
                case "4":
                    return "Paused";
                case "5":
                    return "Offline";
                default:
                    return "Unknown";
            }
        }
    }
}
