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
using MJC.forms.order;
using static System.Windows.Forms.AxHost;
using System.Reflection.Emit;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MJC.forms.login
{
    public partial class Login : GlobalLayout
    {
        private HotkeyButton hkLogin = new HotkeyButton("Enter", "Login", Keys.Enter);
        private FInputBox Username = new FInputBox("Username");
        private FInputBox Password = new FInputBox("Password");
        private UserModel UserModelObj = new UserModel();

        public Login() : base("Login to Account", "Log in with a valid username and password.")
        {
            InitializeComponent();

            HotkeyButton[] hkButtons = new HotkeyButton[] { hkLogin };
            _initializeHKButtons(hkButtons);

            List<dynamic> FormComponents = new List<dynamic>();

            Password.GetTextBox().PasswordChar = '*';

            FormComponents.Add(Username);
            FormComponents.Add(Password);

            _addFormInputs(FormComponents, 30, 150, 500, 50);

            hkLogin.GetButton().Click += Login_Click;
        }

        private void Login_Click(object? sender, EventArgs e)
        {
            if (UserModelObj.Login(Username.GetTextBox().Text, Password.GetTextBox().Text))
            {
                UserData UserData = UserModelObj.getUserDataById(Username.GetTextBox().Text);

                Session.LoggedIn = true;
                Program.permissionOrders = UserData.permissionOrders;
                Program.permissionInventory = UserData.permissionInventory;
                Program.permissionReceivables = UserData.permissionReceivables;
                Program.permissionSetting = UserData.permissionSetting;
                Program.permissionUsers = UserData.permissionUsers;
                Program.permissionQuickBooks = UserData.permissionReceivables;

                Dashboard dashboard = new Dashboard();
                dashboard.Show();

                this.Hide();
            }
            else
            {
                Username.GetTextBox().Text = String.Empty;
                Password.GetTextBox().Text = String.Empty;

                Username.GetTextBox().Focus();

                MessageBox.Show("The username and/or password you entered is not correct.", "Invalid Account and/or Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
