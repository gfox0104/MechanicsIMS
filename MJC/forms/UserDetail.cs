using MJC.common;
using MJC.common.components;
using MJC.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace MJC.forms
{
    public partial class UserDetail : GlobalLayout
    {
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);

        private FInputBox UserName = new FInputBox("UserName:");
        private FInputBox Password = new FInputBox("Password");
        private FGroupLabel AccessPermissions = new FGroupLabel("Access Permissions");
        private FCheckBox pesmOrder = new FCheckBox("Orders");
        private FCheckBox pesmInventory = new FCheckBox("Inventory");
        private FCheckBox pesmReceivable = new FCheckBox("Receivables");
        private FCheckBox pesmSetting = new FCheckBox("System Settings");
        private FCheckBox pesmUser = new FCheckBox("Users");
        private FCheckBox pesmQBLink = new FCheckBox("QB Links");

        private int userId = 0;
        private UserModel userModelObj = new UserModel();

        public UserDetail() : base("User Details", "Modify a user")
        {
            InitializeComponent();
            _initBasicSize();
            this.KeyDown += (s, e) => Form_KeyDown(s, e);

            HotkeyButton[] hkButtons = new HotkeyButton[1] { hkPreviousScreen };
            _initializeHKButtons(hkButtons);

            InitInputBox();
        }

        private void InitInputBox()
        {
            List<dynamic> FormComponents = new List<dynamic>();

            FormComponents.Add(UserName);
            FormComponents.Add(Password);
            _addFormInputs(FormComponents, 750, 150, 500, 50);

            List<dynamic> FormComponents1 = new List<dynamic>();
            FormComponents1.Add(AccessPermissions);
            FormComponents1.Add(pesmOrder);
            FormComponents1.Add(pesmInventory);
            FormComponents1.Add(pesmReceivable);
            FormComponents1.Add(pesmSetting);
            FormComponents1.Add(pesmUser);
            FormComponents1.Add(pesmQBLink);
            _addFormInputs(FormComponents1, 750, 250, 500, 50);
        }

        public void setDetails(UserData user)
        {
            UserName.GetTextBox().Text = user.username;
            Password.GetTextBox().Text = user.password;

            this.userId = user.id;
            pesmOrder.GetCheckBox().Checked = user.permissionOrders;
            pesmInventory.GetCheckBox().Checked = user.permissionInventory;
            pesmReceivable.GetCheckBox().Checked = user.permissionReceivables;
            pesmSetting.GetCheckBox().Checked = user.permissionSetting;
            pesmUser.GetCheckBox().Checked = user.permissionUsers;
            pesmQBLink.GetCheckBox().Checked = user.permissionQuickBooks;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult result = MessageBox.Show("Do you want to save your changes?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveUser(sender, e);
                }
                else if (result == DialogResult.No)
                {
                    _navigateToPrev(sender, e);
                }
            }
        }

        private void SaveUser(object sender, KeyEventArgs e)
        {
            int userId = this.userId;
            string username = UserName.GetTextBox().Text;
            string password = Password.GetTextBox().Text;

            bool permissionOrders = pesmOrder.GetCheckBox().Checked;
            bool permissionInventory = pesmInventory.GetCheckBox().Checked;
            bool permissionReceivables = pesmReceivable.GetCheckBox().Checked;
            bool permissionSetting = pesmSetting.GetCheckBox().Checked;
            bool permissionUsers = pesmUser.GetCheckBox().Checked;
            bool permissionQbLink = pesmQBLink.GetCheckBox().Checked;

            int createdBy = 1;
            int updatedBy = 1;

            if (userId == 0)
            {
                // create user
                userModelObj.CreateUser(username, password, permissionOrders, permissionInventory, permissionReceivables, permissionSetting, permissionUsers, permissionQbLink, createdBy, updatedBy);
            }
            else
            {
                // update user
                userModelObj.UpdateUser(username, password, permissionOrders, permissionInventory, permissionReceivables, permissionSetting, permissionUsers, permissionQbLink, createdBy, updatedBy, userId);
            }
        }
    }
}
