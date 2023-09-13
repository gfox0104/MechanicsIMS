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

namespace MJC.forms.category
{
    public partial class CategoryDetail : BasicModal
    {
        private ModalButton MBOk = new ModalButton("(Enter) OK", Keys.Enter);
        private Button MBOk_button;

        private FInputBox categoryName = new FInputBox("Name");
        private FComboBox calculateAs = new FComboBox("Calculate As");
        private FInputBox[] priceTiers;

        private PriceTiersModel PriceTiersModelObj = new PriceTiersModel();

        private int categoryId;
        private CategoriesModel CategoriesModelObj = new CategoriesModel();

        public CategoryDetail() : base("Add Category")
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            InitMBOKButton();
            InitInputBox();

            this.Load += (s, e) => CategoryDetail_Load(s, e);
        }

        private void CategoryDetail_Load(object sender, EventArgs e)
        {
            categoryName.GetTextBox().Select();
        }

        private void InitMBOKButton()
        {
            ModalButton_HotKeyDown(MBOk);
            MBOk_button = MBOk.GetButton();
            //            MBOk_button.Location = new Point(425, 150);
            MBOk_button.Click += (sender, e) => ok_button_Click(sender, e);
            this.Controls.Add(MBOk_button);
        }

        private void InitInputBox()
        {
            categoryName.SetPosition(new Point(30, 30));
            this.Controls.Add(categoryName.GetLabel());
            this.Controls.Add(categoryName.GetTextBox());

            calculateAs.SetPosition(new Point(30, 80));
            this.Controls.Add(calculateAs.GetLabel());
            this.Controls.Add(calculateAs.GetComboBox());

            calculateAs.GetComboBox().DropDownStyle = ComboBoxStyle.DropDownList;
            calculateAs.GetComboBox().Items.Add(new FComboBoxItem(1, "Markup"));
            calculateAs.GetComboBox().Items.Add(new FComboBoxItem(2, "Margin"));
            foreach (FComboBoxItem item in calculateAs.GetComboBox().Items)
            {
                if (item.Id == 1)
                {
                    calculateAs.GetComboBox().SelectedItem = item;
                    break;
                }
            }

            string filter = "";
            var refreshData = PriceTiersModelObj.LoadPriceTierData(filter);
            if (refreshData)
            {
                List<PriceTierData> pDatas = PriceTiersModelObj.PriceTierDataList;

                this.Size = new Size(600, 260 + pDatas.Count * 50);
                MBOk_button.Location = new Point(425, pDatas.Count * 50 + 150);

                priceTiers = new FInputBox[pDatas.Count];
                for (int i = 0; i < pDatas.Count; i++)
                {
                    priceTiers[i] = new FInputBox(pDatas[i].Name.ToString(), 200, pDatas[i].Id);

                    priceTiers[i].SetPosition(new Point(30, 130 + 50 * i));
                    this.Controls.Add(priceTiers[i].GetLabel());
                    this.Controls.Add(priceTiers[i].GetTextBox());
                }
            }
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            string name = this.categoryName.GetTextBox().Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("please enter Category Name");
                this.categoryName.GetTextBox().Select();
                return;
            }
            int calculateAs = this.calculateAs.GetComboBox().SelectedItem.ToString() == "Markup" ? 1 : 2;

            Dictionary<int, double> priceTierDict = new Dictionary<int, double>();

            for (int i = 0; i < priceTiers.Length; i++)
            {
                double priceData; bool parse_succeed = double.TryParse(priceTiers[i].GetTextBox().Text, out priceData);
                if (parse_succeed) priceTierDict.Add(priceTiers[i].GetId(), priceData);
            }

            bool refreshData = false;
            if (categoryId == 0)
                refreshData = CategoriesModelObj.AddCategory(name, calculateAs, priceTierDict);
            else refreshData = CategoriesModelObj.UpdateCategory(name, calculateAs, priceTierDict, categoryId);

            string modeText = categoryId == 0 ? "creating" : "updating";

            if (refreshData)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("An Error occured while " + modeText + " the category.");
        }
        public void setDetails(string categoryName, string calculateas, int category_id)
        {
            this.categoryName.GetTextBox().Text = categoryName;

            int itemIndex = calculateas == "Markup" ? 1 : 2;

            foreach (FComboBoxItem item in calculateAs.GetComboBox().Items)
            {
                if (item.Id == itemIndex)
                {
                    calculateAs.GetComboBox().SelectedItem = item;
                    break;
                }
            }

            this.categoryId = category_id;

            List<KeyValuePair<string, double>> priceTierData = new List<KeyValuePair<string, double>>();
            priceTierData = PriceTiersModelObj.GetPriceTierMargin(category_id);

            foreach (KeyValuePair<string, double> pair in priceTierData)
            {
                for (int i = 0; i < priceTiers.Length; i++)
                    if (priceTiers[i].GetLabel().Text == pair.Key)
                        priceTiers[i].GetTextBox().Text = pair.Value.ToString();
            }
        }
    }
}
