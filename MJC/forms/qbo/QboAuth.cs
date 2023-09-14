using Intuit.Ipp.OAuth2PlatformClient;
using MJC.common;
using QboLib;

namespace MJC.forms.qbo
{
    public partial class QboAuth : Form
    {
        public QboAuth()
        {
            this.InitializeComponent();
        }

        private void QboAuth_Load1(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void QboAuth_Load(object sender, EventArgs e)
        {
            // Initialize the WebView2 control
            // and begin the authorization
            // process inside the WebView2 control.
            this.RunAuthorization();
        }

        public async void RunAuthorization()
        {
            // Use the shared helper library (QBO.Shared)
            // to load the token json data (Local.Tokens)
            // and initialize the OAuth2
            // client (Local.Client).
            QboLocal.Initialize();

            // Initialize the WebView2 control.
            await WebView.EnsureCoreWebView2Async(null);

            // Navigate the WebView2 control to
            // a generated authorization URL.
            WebView.CoreWebView2.Navigate(QboHelper.GetAuthorizationURL(OidcScopes.Accounting));
        }

        private void QboAuth_FormClosing(object sender, FormClosingEventArgs e)
        {
            // When the user closes the form we
            // assume that the operation has
            // completed with success or failure.

            // Get the current query parameters
            // from the current WebView source (page).
            string query = WebView.Source.Query;

            // Use the the shared helper library
            // to validate the query parameters
            // and write the output file.
            if (QboHelper.CheckQueryParamsAndSet(query) == true && QboLocal.Tokens != null)
            {
                QboHelper.WriteTokensAsJson(QboLocal.Tokens);

                Session.SettingsModelObj.Settings.accessToken = QboLocal.Tokens.AccessToken;
                Session.SettingsModelObj.Settings.refreshToken = QboLocal.Tokens.RefreshToken;

                Session.SettingsModelObj.SaveSetting(Session.SettingsModelObj.Settings.taxCodeId, Session.SettingsModelObj.Settings.businessName, Session.SettingsModelObj.Settings.businessDescription, Session.SettingsModelObj.Settings.address1, Session.SettingsModelObj.Settings.address2, Session.SettingsModelObj.Settings.city, Session.SettingsModelObj.Settings.state, Session.SettingsModelObj.Settings.postalCode, Session.SettingsModelObj.Settings.phone, Session.SettingsModelObj.Settings.fax, Session.SettingsModelObj.Settings.ein, Session.SettingsModelObj.Settings.trainingEnabled, Session.SettingsModelObj.Settings.targetPrinter, Session.SettingsModelObj.Settings.accessToken, Session.SettingsModelObj.Settings.refreshToken, Session.SettingsModelObj.Settings.businessFooter, Session.SettingsModelObj.Settings.businessTermsOfService, Session.SettingsModelObj.Settings.invoicePrintQty.GetValueOrDefault(), Session.SettingsModelObj.Settings.holdOrderPrintQty.GetValueOrDefault(), Session.SettingsModelObj.Settings.quotePrintQty.GetValueOrDefault());

            }
            else
            {
                MessageBox.Show("Quickbooks Online failed to authenticate.");
            }
        }
    }
}
