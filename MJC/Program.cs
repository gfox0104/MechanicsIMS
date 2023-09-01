using MJC.forms;
using MJC.forms.login;

namespace MJC
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.EnableVisualStyles();
            ApplicationConfiguration.Initialize();
            
            ShowSplash();

            Login login = new Login();
            Application.Run(login);

        }
        private static void ShowSplash()
        {
            Splash sp = new Splash();
            sp.Show();
            
            Application.DoEvents();
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = 500;
            t.Tick += new EventHandler((sender, ea) =>
            {
                sp.BeginInvoke(new Action(() =>
                {
                    
                    if (sp != null && Application.OpenForms.Count > 1)
                    {
                        sp.Close();
                        sp.Dispose();
                        sp = null;
                        t.Stop();
                        t.Dispose();
                        t = null;
                    }
                }));
            });
            t.Start();
        }
    }
}