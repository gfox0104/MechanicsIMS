using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJC.common.components
{
    public class CustomDataGridView : DataGridView
    {
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x0100;
            const int VK_RETURN = 0x0D;

            if (msg.Msg == WM_KEYDOWN && keyData != Keys.None && (int)keyData == VK_RETURN)
            {
                OnKeyDown(new KeyEventArgs(Keys.Enter));
                return true;  // prevent default processing
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
