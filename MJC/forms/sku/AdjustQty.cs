using MJC.common.components;
using MJC.common;
using MJC.model;
using Sentry;

namespace MJC.forms.sku
{
    public partial class AdjustQty : BasicModal
    {
        private ModalButton MBSave = new ModalButton("F1 Save New Qty", Keys.F1);
        private Button MBSave_button;

        private FInputBox SKUName = new FInputBox("SKU#");
        private FInputBox CurrentQty = new FInputBox("CurrentQty");
        private FInputBox NewQty = new FInputBox("New Qty");

        private SKUModel SKUModelObj = new SKUModel();

        private int SKUId = 0;

        public AdjustQty(int _skuId, string _skuName) : base("AdjustQty")
        {
            InitializeComponent();

            this.Size = new Size(360, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.SKUId = _skuId;


            InitButton();
            InitInputBox(_skuId, _skuName);

            this.Load += (s, e) => AdjustQty_Load(s, e);
        }

        private void AdjustQty_Load(object sender, EventArgs e)
        {
            NewQty.GetTextBox().Select();
        }

        private void InitButton()
        {
            ModalButton_HotKeyDown(MBSave);
            MBSave_button = MBSave.GetButton();
            MBSave_button.Location = new Point(140, 190);
            MBSave_button.Click += (sender, e) => {
                saveNewQty();
            };
            this.Controls.Add(MBSave_button);
        }

        private void InitInputBox(int _skuId, string _skuName)
        {
            SKUName.SetPosition(new Point(30, 30));
            SKUName.GetLabel().Text = _skuName;
            this.Controls.Add(SKUName.GetLabel());

            int quantity = SKUModelObj.GetQuantity(_skuId);

            CurrentQty.SetPosition(new Point(30, 75));
            CurrentQty.GetLabel().Text = "Adjust Qty:  " + quantity.ToString();
            this.Controls.Add(CurrentQty.GetLabel());

            NewQty.SetPosition(new Point(30, 120));
            this.Controls.Add(NewQty.GetLabel());
            this.Controls.Add(NewQty.GetTextBox());
            NewQty.GetTextBox().Location = new Point(140, 120);
            NewQty.GetTextBox().Width = 160;
        }

        private void saveNewQty()
        {
            int currentQuantity = SKUModelObj.GetQuantity(SKUId);
            int availableQuantity = SKUModelObj.GetQuantityAvailable(SKUId);
            int quantityAllocated = 2; // SKUModelObj.GetQuantityAllocated(SKUId);

            string cross_ref = NewQty.GetTextBox().Text;
            if (string.IsNullOrWhiteSpace(cross_ref))
            {
                Messages.ShowError("You must enter a SKU Name.");
                NewQty.GetTextBox().Select();
                return;
            }

            int newQty;
            if (!int.TryParse(cross_ref, out newQty))
            {
                Messages.ShowError("Please enter a valid quantity.");
                NewQty.GetTextBox().Select();
                return;
            }

            try
            {
                if (newQty < availableQuantity)
                {
                    // Current quantity brings our available quantity down.
                    if(MessageBox.Show("The new quantity will bring your available quantity to a negative. Are you sure you want to continue?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) 
                    {
                        var qualityDifference = (newQty - currentQuantity);
                        int newAvailableQuantity = (availableQuantity + qualityDifference) - quantityAllocated;

                        SKUModelObj.UpdateQuantity(newQty, newAvailableQuantity, this.SKUId);
                    }
                }
                else if(newQty > currentQuantity)
                {
                    // Increase both quantities when newQty is added.
                    var qualityDifference = (newQty - currentQuantity);
                    availableQuantity = availableQuantity + qualityDifference;

                    SKUModelObj.UpdateQuantity(newQty, availableQuantity, this.SKUId);
                }
                else 
                {
                    // Available Quantity is sufficient with our current quantity
                    // Lower the current quantity but don't touch the available quantity.
                    var qualityDifference = (newQty - currentQuantity);

                    SKUModelObj.UpdateQuantity(newQty, availableQuantity, this.SKUId);
                }
            }
            catch (Exception e)
            {
                Sentry.SentrySdk.CaptureException(e);
                Messages.ShowError("There was a problem updating the quantity.");
            }

            Messages.ShowInformation("The quantity has been updated.");
            this.Close();
        }
    }
}
