using MJC.common.components;
using MJC.common;

namespace MJC.forms.customer
{
    public partial class CustomerMemo : BasicModal
    {
        private HotkeyButton hkClose = new HotkeyButton("Esc", "Close", Keys.Escape);

        private FRichTextBox memoTextBox = new FRichTextBox();
        private int priceTierId;

        private int CustomerId = 0;
        private string memo;
        private bool readOnly = false;

        public CustomerMemo() : base()
        {
            InitializeComponent();
        }

        public CustomerMemo(int customerId, string memo, bool readOnly = false) : base()
        {
            InitializeComponent();
            _setModalStyle2();
            this.Size = new Size(420, 320);
            this.readOnly = readOnly;

            this.CustomerId = customerId;
            this.memo = memo;

            InitButton();
            InitInputBox();
        }

        private void InitButton()
        {
            hkClose.SetPosition(new Point(20, 230));
            this.Controls.Add(hkClose.GetButton());
            this.Controls.Add(hkClose.GetLabel());
            hkClose.GetButton().Click += (s, e) => { this.Close(); };
        }

        private void InitInputBox()
        {
            memoTextBox.SetPosition(new Point(20, 20));
            memoTextBox.GetTextBox().Size = new Size(360, 190);
            memoTextBox.GetTextBox().Text = this.memo;
            this.Controls.Add(memoTextBox.GetTextBox());

            if(this.readOnly)
            {
                memoTextBox.GetTextBox().Enabled = false;
            }
        }

        public string getMemo()
        {
            return this.memoTextBox.GetTextBox().Text;
        }
    }
}
