using MJC.forms;
using MJC.forms.login;
using Sentry;
using System.Reflection;


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

            SentrySdk.Init(o =>
            {
                // Tells which project in Sentry to send events to:
                o.Dsn = "https://4b7926db913b708af6e2bdde51bc6243@o382651.ingest.sentry.io/4505844276527104";
                // When configuring for the first time, to see what the SDK is doing:
                o.Debug = true;
                // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                // We recommend adjusting this value in production.
                o.TracesSampleRate = 1.0;
            });
            // Configure WinForms to throw exceptions so Sentry can capture them.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
           
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