using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MJC.forms;

namespace MJC.forms
{
    public partial class Splash : Form
    {
        private delegate void CloseDelegate();
        public static Splash splash;
        public static void ShowSplashScreen()
        {
            if (splash != null) return;
            splash = new Splash();
            Thread thread = new Thread(new ThreadStart(Splash.ShowForm));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

        }
        private static void ShowForm()
        {
            if (splash != null)
            {
                Application.Run(splash);
            }

        }
        public static void CloseForm()
        {
            splash?.Invoke(new CloseDelegate(Splash.CloseFormInternal));
        }
        private static void CloseFormInternal()
        {
            if (splash != null)
            {
                splash.Close();
                splash = null;
            }
        }
        public Splash()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
        }
    }
}
