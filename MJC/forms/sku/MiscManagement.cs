using MJC.common.components;
using MJC.common;
using MJC.forms.customer;
using MJC.forms.vendor;

namespace MJC.forms.sku
{
    public partial class MiscManagement : BasicModal
    {
        private FInputBox VenderCosts = new FInputBox("1) Vendor Costs");
        private FInputBox CrossReferences = new FInputBox("2) Cross References");
        private FInputBox SubAssemblies = new FInputBox("3) Sub-assemblies");
        private FInputBox SKUCostQty = new FInputBox("4) SKU Cost/Qty");
        private FInputBox SerialLotNumbers = new FInputBox("5) Serial/Lot Numbers");
        private FInputBox QuantityDiscounts = new FInputBox("6) Quantity Discounts");
        private FInputBox SKUHistory = new FInputBox("7) SKU History");

        private HotkeyButton hkClose = new HotkeyButton("Esc", "Close", Keys.Escape);

        private int saveFlage = 0;

        public MiscManagement()
        {
            InitializeComponent();
            //_setModalStyle2();
            this.Size = new Size(420, 420);

            InitForms();

            this.KeyDown += MiscActions_KeyDown;
        }

        private void InitForms()
        {
            int xPos = 100;
            int yPos = 20;
            int yDistance = 40;

            VenderCosts.SetPosition(new Point(xPos, yPos));
            this.Controls.Add(VenderCosts.GetLabel());

            yPos += yDistance;
            CrossReferences.SetPosition(new Point(xPos, yPos));
            this.Controls.Add(CrossReferences.GetLabel());

            yPos += yDistance;
            SubAssemblies.SetPosition(new Point(xPos, yPos));
            this.Controls.Add(SubAssemblies.GetLabel());

            yPos += yDistance;
            SKUCostQty.SetPosition(new Point(xPos, yPos));
            this.Controls.Add(SKUCostQty.GetLabel());

            yPos += yDistance;
            SerialLotNumbers.SetPosition(new Point(xPos, yPos));
            this.Controls.Add(SerialLotNumbers.GetLabel());

            yPos += yDistance;
            QuantityDiscounts.SetPosition(new Point(xPos, yPos));
            this.Controls.Add(QuantityDiscounts.GetLabel());

            yPos += yDistance;
            SKUHistory.SetPosition(new Point(xPos, yPos));
            this.Controls.Add(SKUHistory.GetLabel());

            xPos += 40;
            yPos += yDistance + 10;
            hkClose.SetPosition(new Point(xPos, yPos));
            this.Controls.Add(hkClose.GetButton());
            this.Controls.Add(hkClose.GetLabel());

        }

        private void MiscActions_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D1:
                case Keys.NumPad1:
                    this.saveFlage = 1;
                    this.Close();
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    this.saveFlage = 2;
                    this.Close();
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    this.saveFlage = 3;
                    this.Close();
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    this.saveFlage = 4;
                    this.Close();
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    this.saveFlage = 5;
                    this.Close();
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    this.saveFlage = 6;
                    this.Close();
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    this.saveFlage = 7;
                    this.Close();
                    break;
            }
        }

        public int GetSaveFlage()
        {
            return this.saveFlage;
        }
    }
}
