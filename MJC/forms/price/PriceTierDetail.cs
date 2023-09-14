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

namespace MJC.forms.price
{
    public partial class PriceTierDetail : BasicModal
    {
        private ModalButton MBOk = new ModalButton("(Enter) OK", Keys.Enter);
        private Button MBOk_button;

        private FInputBox priceTierName = new FInputBox("Name");
        private FComboBox priceTierType = new FComboBox("PriceTier Type");
        private FInputBox profitMargin = new FInputBox("Margin");
        private FInputBox priceTierCode = new FInputBox("price tier code");

        private int priceTierId;

        public PriceTierDetail() : base("Add PriceTier")
        {
            InitializeComponent();
            this.Size = new Size(600, 360);
            this.StartPosition = FormStartPosition.CenterScreen;
            InitMBOKButton();
            InitInputBox();

            this.Load += (s, e) => PriceTierDetail_Load(s, e);
        }

        private void PriceTierDetail_Load(object sender, EventArgs e)
        {
            priceTierName.GetTextBox().Select();
        }

        private void InitMBOKButton()
        {
            ModalButton_HotKeyDown(MBOk);
            MBOk_button = MBOk.GetButton();
            MBOk_button.Location = new Point(425, 250);
            MBOk_button.Click += (sender, e) => ok_button_Click(sender, e);
            this.Controls.Add(MBOk_button);
        }

        private void InitInputBox()
        {
            priceTierName.SetPosition(new Point(30, 30));
            this.Controls.Add(priceTierName.GetLabel());
            this.Controls.Add(priceTierName.GetTextBox());
            priceTierName.GetTextBox().KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter)
                {
                    SelectNextControl((Control)s, true, true, true, true);
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.Up || (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Shift))
                {
                    SelectNextControl((Control)s, false, true, true, true);
                    e.Handled = true;
                }
            };
            priceTierType.SetPosition(new Point(30, 80));
            this.Controls.Add(priceTierType.GetLabel());
            this.Controls.Add(priceTierType.GetComboBox());

            List<PriceTierType> priceTierTypeList = new List<PriceTierType>();
            priceTierTypeList.Add(new PriceTierType { Value = 0, Name = "Profit" });
            priceTierTypeList.Add(new PriceTierType { Value = 1, Name = "Discount" });

            priceTierType.GetComboBox().DataSource = priceTierTypeList;
            priceTierType.GetComboBox().DisplayMember = "Name";
            priceTierType.GetComboBox().ValueMember = "Value";

            profitMargin.SetPosition(new Point(30, 130));
            this.Controls.Add(profitMargin.GetLabel());
            this.Controls.Add(profitMargin.GetTextBox());
            profitMargin.GetTextBox().KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter)
                {
                    SelectNextControl((Control)s, true, true, true, true);
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.Up || (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Shift))
                {
                    SelectNextControl((Control)s, false, true, true, true);
                    e.Handled = true;
                }
            };

            priceTierCode.SetPosition(new Point(30, 180));
            this.Controls.Add(priceTierCode.GetLabel());
            this.Controls.Add(priceTierCode.GetTextBox());
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            string name = this.priceTierName.GetTextBox().Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("please enter Price Tier Name");
                this.priceTierName.GetTextBox().Select();
                return;
            }
            if (!double.TryParse(this.profitMargin.GetTextBox().Text, out double profitMargin))
            {
                MessageBox.Show("please enter a number");
                this.profitMargin.GetTextBox().Text = "";
                this.profitMargin.GetTextBox().Focus();
                return;
            }
            string pricetiercode = this.priceTierCode.GetTextBox().Text;

            int type = int.Parse(priceTierType.GetComboBox().SelectedValue.ToString());
            if(type == 0)
            {
                profitMargin = profitMargin * 1;
            } else if(type == 1)
            {
                profitMargin = profitMargin * (-1);
            }

            bool refreshData = false;

            if (priceTierId == 0)
                refreshData = Session.PriceTiersModelObj.AddPriceTier(name, profitMargin, pricetiercode);
            else refreshData = Session.PriceTiersModelObj.UpdatePriceTier(name, profitMargin, pricetiercode, priceTierId);

            string modeText = priceTierId == 0 ? "creating" : "updating";

            if (refreshData)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("An Error occured while " + modeText + " the pricetier.");
        }
        public void setDetails(string name, double profitmargin, string pricetiercode, int pricetierId)
        {
            this.priceTierName.GetTextBox().Text = name;
            if(profitmargin >= 0)
            {
                this.priceTierType.GetComboBox().SelectedValue = 0;
            } else
            {
                this.priceTierType.GetComboBox().SelectedValue = 1;
            }
            this.profitMargin.GetTextBox().Text = Math.Abs(profitmargin).ToString();
            this.priceTierCode.GetTextBox().Text = pricetiercode;
            priceTierId = pricetierId;
        }
    }
}
