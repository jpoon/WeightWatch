namespace WeightWatch.Views
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Tasks;
    using WeightWatch.Classes;
    using WeightWatch.Models;
    using GraphMode = WeightWatch.Models.ApplicationSettings.GraphMode;
    using System.Windows.Controls.Primitives;

    public partial class About : PhoneApplicationPage
    {
        private AssemblyName _asmName;

        public About()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Settings_Loaded);

            _asmName = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
        }

        void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            versionTextBlock.Text = _asmName.Version.ToString();
        }

        #region Event Handlers

        private void hyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            string s = ((ButtonBase)sender).Tag as string;

            switch (s)
            {
                case "Review":
                    var task = new MarketplaceReviewTask();
                    task.Show();
                    break;
                case "Contribute":
                    var browser = new WebBrowserTask();
                    browser.URL = Uri.EscapeDataString(App.ContributeUrl);
                    browser.Show();
                    break;
                case "Feedback":
                    var email = new EmailComposeTask();
                    email.To = App.FeedbackEmail;
                    email.Body = "Version: " + _asmName.Version.ToString();
                    email.Show();
                    break;
            }
        }

        #endregion
    }
}