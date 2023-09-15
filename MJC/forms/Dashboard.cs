using Microsoft.VisualBasic.ApplicationServices;
using MJC.common.components;
using MJC.common;
using MJC.forms.order;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Timers;
using System.Threading;
using QboLib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace MJC.forms
{
    public partial class Dashboard : GlobalLayout
    {
        private HotkeyButton hkCloseProgramButton = new HotkeyButton("Esc", "Logout", Keys.Escape);
        private HotkeyButton hkOpenSelection = new HotkeyButton("Enter", "Open Selection", Keys.Enter);

        private NavigationButton OrderEntry = new NavigationButton("Order Entry", new OrderEntry());
        private NavigationButton Inventory = new NavigationButton("Inventory", new InventoryDashboard());
        private NavigationButton Receivables = new NavigationButton("Receivables", new ReceivableDashboard());
        private NavigationButton Users = new NavigationButton("Users", new Users());
        //private NavigationButton Accounting = new NavigationButton("Accounting", new Accounting());
        private NavigationButton SystemInformation = new NavigationButton("System Information", new SystemSettings());
        private NavLinkButton OpenQuickbooks = new NavLinkButton("Open Quickbooks", "");

        private PictureBox hkPicture;
        private DateTime lastRefreshedQBO;

        public Dashboard() : base("Dashboard", "Dashboard view")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[2] { hkCloseProgramButton, hkOpenSelection };
            _initializeHKButtons(hkButtons);
            this.AddHKEvents();

            NavigationButton[] navButtons = new NavigationButton[5] { OrderEntry, Inventory, Receivables, Users, SystemInformation };
            _initiallizeNavButtons(navButtons);

            if (!Program.permissionOrders) _disableNavButtons(new NavigationButton[1] { OrderEntry});
            if (!Program.permissionUsers) _disableNavButtons(new NavigationButton[1] { Users });
            if (!Program.permissionInventory) _disableNavButtons(new NavigationButton[1] { Inventory });
            if (!Program.permissionReceivables) _disableNavButtons(new NavigationButton[1] { Receivables });
            if (!Program.permissionSetting) _disableNavButtons(new NavigationButton[1] { SystemInformation });
            if (!Program.permissionQuickBooks) { OpenQuickbooks.GetButton().Enabled = false; }

            SetImage();
            SetLinkButtion();

            this.Activated += Dashboard_Activated;

            RefreshQuickbooks(); 
            
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 5000;
            timer.Tick += new System.EventHandler(RefreshQBOAccessTokens);
            timer.Start();
        }

        private async void RefreshQBOAccessTokens(object? sender, EventArgs e)
        {
            await RefreshQuickbooks();
        }
        
        private async Task RefreshQuickbooks()
        {
            if (QboLocal.Client == null || QboLocal.Tokens == null)
            {
                QboLocal.Initialize();
            }
            if (DateTime.Now.Subtract(lastRefreshedQBO).TotalMinutes > 30)
            {
                if (!string.IsNullOrEmpty(Session.SettingsModelObj.Settings.refreshToken))
                {
                    Console.WriteLine("Refreshing QuickBooks Online Access Token");
                    if (await QboLib.QboLocal.Reauthenticate(Session.SettingsModelObj.Settings.refreshToken))
                    {
                        Session.SettingsModelObj.Settings.accessToken = QboLocal.Tokens.AccessToken;
                        Session.SettingsModelObj.Settings.refreshToken = QboLocal.Tokens.RefreshToken;

                        Session.SettingsModelObj.SaveSetting(Session.SettingsModelObj.Settings.taxCodeId, Session.SettingsModelObj.Settings.businessName, Session.SettingsModelObj.Settings.businessDescription, Session.SettingsModelObj.Settings.address1, Session.SettingsModelObj.Settings.address2, Session.SettingsModelObj.Settings.city, Session.SettingsModelObj.Settings.state, Session.SettingsModelObj.Settings.postalCode, Session.SettingsModelObj.Settings.phone, Session.SettingsModelObj.Settings.fax, Session.SettingsModelObj.Settings.ein, Session.SettingsModelObj.Settings.trainingEnabled, Session.SettingsModelObj.Settings.targetPrinter, Session.SettingsModelObj.Settings.accessToken, Session.SettingsModelObj.Settings.refreshToken, Session.SettingsModelObj.Settings.businessFooter, Session.SettingsModelObj.Settings.businessTermsOfService, Session.SettingsModelObj.Settings.invoicePrintQty.GetValueOrDefault(), Session.SettingsModelObj.Settings.holdOrderPrintQty.GetValueOrDefault(), Session.SettingsModelObj.Settings.quotePrintQty.GetValueOrDefault());
                        lastRefreshedQBO = DateTime.Now;
                    }
                }
            }
        }



        private void Dashboard_Activated(object? sender, EventArgs e)
        {
            base.Activate();

        }

        private void AddHKEvents()
        {
            hkCloseProgramButton.GetButton().Click += new EventHandler(_closeProgram);
            hkOpenSelection.GetButton().Enabled = false;
        }

        private void SetImage()
        {
            hkPicture = new PictureBox();
            hkPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            hkPicture.Image = global::MJC.Properties.Resources.hotkeyview;
            hkPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            hkPicture.TabStop = false;
            hkPicture.Location = new System.Drawing.Point(this.Width - hkPicture.Image.Width - 30, 120);
            this.Controls.Add(hkPicture);
        }

        private void SetLinkButtion()
        {
            OpenQuickbooks.GetButton().Click += (sender, e) =>
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start https://app.qbo.intuit.com/",
                    CreateNoWindow = true
                });
            };

            this.Controls.Add(OpenQuickbooks.GetButton());
            OpenQuickbooks.SetPosition(new Point(this.Width - hkPicture.Image.Width - 30, 120));

            OpenQuickbooks.GetButton().Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            OpenQuickbooks.GetButton().Location = new System.Drawing.Point(this.Width - hkPicture.Image.Width - 30, hkPicture.Image.Height + 130);
            OpenQuickbooks.GetButton().TabIndex = 6;

            this.Controls.Add(hkPicture);
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {

        }

        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
