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
using MJC.forms.creditcode;
using static MJC.model.CreditCardModel;
using System.Runtime.CompilerServices;
using System.Net.NetworkInformation;

namespace MJC.forms.creditcard
{
    public partial class CreditCards : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Edits", Keys.Enter);
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);

        private GridViewOrigin creditCardListGrid = new GridViewOrigin();
        private DataGridView CCLGridRefer;
        private CreditCardModel CreditCardModelObj = new CreditCardModel();
        private int customerId = 0;
        private bool readOnly = false;

        public CreditCards(int customerId, bool readOnly = false) : base("Credit Cards for Cust#", "Credit cards on file for the selected customer")
        {
            InitializeComponent();
            _initBasicSize();

            this.readOnly = readOnly;
            HotkeyButton[] hkButtons;

            if (this.readOnly)
            {
                hkButtons = new HotkeyButton[1] {hkPreviousScreen };
            } else
            {
                hkButtons = new HotkeyButton[4] { hkAdds, hkDeletes, hkEdits, hkPreviousScreen };
            }
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();
            this.customerId = customerId;

            InitCreditCardListGrid();

            this.Load += (s, e) =>
            {
                this.LoadCreditCardList();
            };
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                CreditCardsDetail detailModal = new CreditCardsDetail(this.customerId);
                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadCreditCardList();
                }
            };
            hkDeletes.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to delete?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int selectedCreditCardId = 0;
                    if (CCLGridRefer.SelectedRows.Count > 0)
                    {
                        foreach (DataGridViewRow row in CCLGridRefer.SelectedRows)
                        {
                            selectedCreditCardId = (int)row.Cells[0].Value;
                        }
                    }
                    bool refreshData = CreditCardModelObj.DeleteCreditCard(selectedCreditCardId);
                    if (refreshData)
                    {
                        LoadCreditCardList();
                    }
                }
            };
            hkEdits.GetButton().Click += (sender, e) =>
            {
                CreditCardsDetail detailModal = new CreditCardsDetail(customerId);

                int rowIndex = CCLGridRefer.CurrentCell.RowIndex;

                DataGridViewRow row = CCLGridRefer.Rows[rowIndex];

                int id = (int)row.Cells[0].Value;

                detailModal.setDetails(id);

                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadCreditCardList();
                }
            };
        }

        private void InitCreditCardListGrid()
        {
            CCLGridRefer = creditCardListGrid.GetGrid();
            CCLGridRefer.Location = new Point(0, 95);
            CCLGridRefer.Width = this.Width;
            CCLGridRefer.Height = this.Height - 295;
            CCLGridRefer.VirtualMode = true;
            this.Controls.Add(CCLGridRefer);
            this.CCLGridRefer.CellDoubleClick += (sender, e) =>
            {
                CreditCardsDetail detailModal = new CreditCardsDetail(customerId);

                int rowIndex = CCLGridRefer.CurrentCell.RowIndex;

                DataGridViewRow row = CCLGridRefer.Rows[rowIndex];

                int id = (int)row.Cells[0].Value;

                detailModal.setDetails(id);

                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadCreditCardList();
                }
            };
        }

        private void LoadCreditCardList()
        {
            List<CreditCardData> creditCardList = CreditCardModelObj.LoadCreditCardData(this.customerId);

            CCLGridRefer.DataSource = creditCardList;
            CCLGridRefer.Columns[0].Visible = false;
            CCLGridRefer.Columns[1].HeaderText = "Customer Id";
            CCLGridRefer.Columns[1].Visible = false;
            CCLGridRefer.Columns[2].HeaderText = "Card Number";
            CCLGridRefer.Columns[2].Width = 400;
            CCLGridRefer.Columns[3].HeaderText = "Card Type";
            CCLGridRefer.Columns[3].Width = 300;
            CCLGridRefer.Columns[4].HeaderText = "Expires";
            CCLGridRefer.Columns[4].Width = 200;
            CCLGridRefer.Columns[5].HeaderText = "Security Code";
            CCLGridRefer.Columns[5].Width = 300;
            CCLGridRefer.Columns[6].HeaderText = "Priority";
            CCLGridRefer.Columns[6].Width = 300;
        }
    }
}
