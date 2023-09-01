using MJC.common.components;
using MJC.common;

namespace MJC.forms
{
    public partial class ZoneChart : GlobalLayout
    {

        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Edits", Keys.Enter);
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);

        public ZoneChart() : base("Zone Chart", "Manage Zones to be used by the system")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[4] { hkAdds, hkDeletes, hkEdits, hkPreviousScreen };
            _initializeHKButtons(hkButtons);
            _addComingSoon();

            foreach (HotkeyButton button in hkButtons)
            {
                if (button != hkPreviousScreen)
                    button.GetButton().Click += new EventHandler(_hotkeyTest);
            }
            hkPreviousScreen.GetButton().Click += new EventHandler(_navigateToPrev);
        }
    }
}
