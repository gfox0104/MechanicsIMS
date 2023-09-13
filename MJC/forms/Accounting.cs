using MJC.common.components;
using MJC.common;
using MJC.model;
using static MJC.model.AccountingModel;
using MJC.forms.customer;

namespace MJC.forms
{
    public partial class Accounting : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Edits", Keys.Enter);
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);

        private GridViewOrigin accountingGrid = new GridViewOrigin();
        private DataGridView AccountingGridRefer;

        public Accounting() : base("Accounting", "Manage accounts on the system")
        {
            InitializeComponent();
            HotkeyButton[] hkButtons = new HotkeyButton[4] { hkAdds, hkDeletes, hkEdits, hkPreviousScreen };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitAccountingList();
            //this.VisibleChanged += (s, e) =>
            //{
            //    LoadAccountList();
            //};
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                //if (UserGridRefer.RowCount > 0)
                //{
                //    int rowIndex = UserGridRefer.SelectedRows[0].Index;
                //    DataGridViewRow row = UserGridRefer.Rows[rowIndex];
                //    int userId = (int)row.Cells[0].Value;

                //    UserDetail userDetailModal = new UserDetail();
                //    _navigateToForm(sender, e, userDetailModal);
                //    this.Hide();
                //}
            };
            hkEdits.GetButton().Click += (sender, e) =>
            {
                AccountingDetail detailModal = new AccountingDetail();

                if (AccountingGridRefer.Rows.Count > 0)
                {
                    int rowIndex = AccountingGridRefer.CurrentCell.RowIndex;
                    DataGridViewRow row = AccountingGridRefer.Rows[rowIndex];

                    int id = (int)row.Cells[0].Value;
                    this.Hide();
                    detailModal.setDetails(id);
                    _navigateToForm(sender, e, detailModal);

                    //if (detailModal.ShowDialog() == DialogResult.OK)
                    //{
                    //   LoadCustomerList();
                    //}
                }
            };
        }

        private void InitAccountingList()
        {
            AccountingGridRefer = accountingGrid.GetGrid();
            AccountingGridRefer.Location = new Point(0, 95);
            AccountingGridRefer.Width = this.Width;
            AccountingGridRefer.Height = this.Height - 295;
            AccountingGridRefer.AllowUserToAddRows = false;
            this.Controls.Add(AccountingGridRefer);

            AccountingGridRefer.Columns.Clear();

            AccountingGridRefer.Columns.Add("Id", "Id");
            AccountingGridRefer.Columns["Id"].Visible = false;

            AccountingGridRefer.Columns.Add("Name", "Accounting Name");
            AccountingGridRefer.Columns["Name"].Width = 500;

            AccountingGridRefer.Columns.Add("AccountType", "Account Type");
            AccountingGridRefer.Columns["AccountType"].Width = 400;
            AccountingGridRefer.Columns.Add("SubAcctType", "Detail Type");
            AccountingGridRefer.Columns["SubAcctType"].Width = 400;
            AccountingGridRefer.Columns.Add("CurrentBalance", "Current Balance");
            AccountingGridRefer.Columns["CurrentBalance"].Width = 300;

            LoadAccountingList();
        }

        private void LoadAccountingList()
        {
            List<model.Accounting> accountingList = Session.accountingModelObj.LoadAccountingList();
            int index = 0;
            
            foreach (model.Accounting accountData in accountingList)
            {
                int colonCount = accountData.FullyQualifiedName.Split(":").Length - 1;
                string prefixStr = AddSpace(colonCount);
                string tempStr = prefixStr + accountData.Name;
                accountingList[index].Name = tempStr;
                index++;
            }

            foreach (model.Accounting accouning in accountingList)
            {
                int rowIndex = AccountingGridRefer.Rows.Add();
                DataGridViewRow newRow = AccountingGridRefer.Rows[rowIndex];
                newRow.Cells["Id"].Value = accouning.Id;
                newRow.Cells["Name"].Value = accouning.Name;
                newRow.Cells["AccountType"].Value = accouning.AccountType;
                newRow.Cells["SubAcctType"].Value = accouning.SubAcctType;
                newRow.Cells["CurrentBalance"].Value = accouning.CurrentBalance;
            }
        }

        private string AddSpace(int count)
        {
            string prefixStr = "";
            while(count > 0)
            {
                prefixStr = prefixStr + "    ";
                count--;
            }
            return prefixStr;
        }
    }
}
