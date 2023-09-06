using MJC.common.components;
using MJC.common;
using MJC.forms.qbo;
using MJC.qbo;

namespace MJC.forms
{
    public partial class SystemSettings : GlobalLayout
    {

        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);
        private HotkeyButton hkOpenQuickBooks = new HotkeyButton("F5", "Auth QuickBooks", Keys.F5);
        private HotkeyButton hkSyncQuickBooks = new HotkeyButton("F8", "Sync QuickBooks", Keys.F8);

        private FInputBox BusinessName = new FInputBox("Business Name");
        private FInputBox AddressLine1 = new FInputBox("Address Line 1");
        private FInputBox AddressLine2 = new FInputBox("Address LIne 2");
        private FInputBox City = new FInputBox("City");
        private FInputBox State = new FInputBox("State");
        private FInputBox Zipcode = new FInputBox("Zipcode");
        private FInputBox Phone = new FInputBox("Phone");
        private FInputBox Fax = new FInputBox("Fax");
        private FInputBox FederalTax = new FInputBox("Federal Tax#");
        private FCheckBox TradingModeOFF = new FCheckBox("Trading Mode OFF");
        private FInputBox TargetPrinter = new FInputBox("Target Printer");
        private FInputBox ProcessingTax = new FInputBox("Processing Tax");

        public SystemSettings() : base("System Settings", "Manage system settings")
        {
            InitializeComponent();
            _initBasicSize();
            AddHotKeyEvents();

            HotkeyButton[] hkButtons = new HotkeyButton[3] { hkPreviousScreen, hkOpenQuickBooks , hkSyncQuickBooks};
            _initializeHKButtons(hkButtons);
            InitInputBox();
        }

        private void AddHotKeyEvents()
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
                    qboClient.LoadCustomers();


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
            FormComponents.Add(AddressLine1);
            FormComponents.Add(AddressLine2);
            FormComponents.Add(City);
            FormComponents.Add(State);
            FormComponents.Add(Zipcode);
            FormComponents.Add(Phone);
            FormComponents.Add(Fax);
            FormComponents.Add(FederalTax);
            FormComponents.Add(TradingModeOFF);
            FormComponents.Add(TargetPrinter);
            FormComponents.Add(ProcessingTax);

            _addFormInputs(FormComponents, 30, 150, 500, 50);
        }
    }
}
