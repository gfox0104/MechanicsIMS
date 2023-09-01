using MJC.common.components;
using MJC.common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MJC.model;
using MJC.forms.order;

namespace MJC.forms
{
    public partial class Users : GlobalLayout
    {

        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Edits", Keys.Enter);
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);

        private GridViewOrigin userGrid = new GridViewOrigin();
        private DataGridView UserGridRefer;

        private UserModel UserModelObj = new UserModel();

        public Users() : base("Users", "Manage users on the system")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[4] { hkAdds, hkDeletes, hkEdits, hkPreviousScreen };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitUserList();

            this.VisibleChanged += (s, e) =>
            {
                LoadUserList();
            };
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                if (UserGridRefer.RowCount > 0)
                {
                    int rowIndex = UserGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = UserGridRefer.Rows[rowIndex];
                    int userId = (int)row.Cells[0].Value;

                    UserDetail userDetailModal = new UserDetail();
                    _navigateToForm(sender, e, userDetailModal);
                    this.Hide();
                }
            };

            hkDeletes.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to delete?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (UserGridRefer.RowCount > 0)
                    {
                        int rowIndex = UserGridRefer.SelectedRows[0].Index;
                        DataGridViewRow row = UserGridRefer.Rows[rowIndex];
                        int userId = (int)row.Cells[0].Value;

                        bool refreshData = UserModelObj.DeleteUser(userId);
                        if (refreshData)
                        {
                            LoadUserList();
                        }
                    }
                }

            };

            hkEdits.GetButton().Click += (sender, e) =>
            {
                if (UserGridRefer.RowCount > 0)
                {
                    int rowIndex = UserGridRefer.SelectedRows[0].Index;

                    UserDetail userDetailModal = new UserDetail();
                    List<UserData> users = UserModelObj.UserDataList;
                    userDetailModal.setDetails(users[rowIndex]);

                    this.Hide();
                    _navigateToForm(sender, e, userDetailModal);
                }
            };
        }

        private void InitUserList()
        {
            UserGridRefer = userGrid.GetGrid();
            UserGridRefer.Location = new Point(0, 95);
            UserGridRefer.Width = this.Width;
            UserGridRefer.Height = this.Height - 295;
            UserGridRefer.VirtualMode = true;
            this.Controls.Add(UserGridRefer);

            LoadUserList();
        }

        private void LoadUserList()
        {
            var refreshData = UserModelObj.LoadUserData();
            
            if (refreshData)
            {
                UserGridRefer.DataSource = UserModelObj.UserDataList;
                UserGridRefer.Columns[0].Visible = false;
                UserGridRefer.Columns[1].HeaderText = "UserName";
                UserGridRefer.Columns[1].Width = 200;
                UserGridRefer.Columns[2].Visible = false;
                UserGridRefer.Columns[3].Visible = false;
                UserGridRefer.Columns[4].Visible = false;
                UserGridRefer.Columns[5].Visible = false;
                UserGridRefer.Columns[6].Visible = false;
                UserGridRefer.Columns[7].Visible = false;
                UserGridRefer.Columns[8].Visible = false;
            }
        }
    }
}
