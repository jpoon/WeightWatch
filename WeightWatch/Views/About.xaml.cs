/*
 * Copyright (C) 2011 by Jason Poon
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

namespace WeightWatch.Views
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Tasks;

    public partial class About : PhoneApplicationPage
    {
        private readonly AssemblyName _asmName;

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

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var s = ((ButtonBase)sender).Tag as string;
            switch (s)
            {
                case "Review":
                    var task = new MarketplaceReviewTask();
                    task.Show();
                    break;
                case "Contribute":
                    var browser = new WebBrowserTask { URL = Uri.EscapeDataString(App.ContributeUrl) };
                    browser.Show();
                    break;
                case "Feedback":
                    var email = new EmailComposeTask { To = App.FeedbackEmail, Body = "Version: " + _asmName.Version.ToString() };
                    email.Show();
                    break;
            }
        }

        #endregion
    }
}