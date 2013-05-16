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
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Tasks;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Shapes;

    public partial class About : PhoneApplicationPage
    {
        private readonly AssemblyName _asmName;
        private StackPanel _changelog;
        private const string ChangelogFile = "changelog.txt";

        public About()
        {
            InitializeComponent();
            Loaded += Settings_Loaded;

            _asmName = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
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
                    var browser = new WebBrowserTask { Uri = App.ContributeUri };
                    browser.Show();
                    break;
                case "Feedback":
                    var email = new EmailComposeTask 
                    { 
                        To = App.FeedbackEmail, 
                        Subject = "[WeightWatch]", 
                        Body = "Version: " + _asmName.Version 
                    };
                    email.Show();
                    break;
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var aboutPivot = (Pivot)sender;
            if (aboutPivot.SelectedIndex > 0 && _changelog == null)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    _changelog = new StackPanel();

                    var sri = Application.GetResourceStream(new Uri(ChangelogFile, UriKind.Relative));
                    if (sri != null)
                    {
                        using (var sr = new StreamReader(sri.Stream))
                        {
                            string line;
                            var isHeader = true;
                            
                            while ( (line = sr.ReadLine()) != null )
                            {
                                if (line != String.Empty)
                                {
                                    var tb = new TextBlock
                                    {
                                        TextWrapping = TextWrapping.Wrap,
                                        Text = line,
                                        Style = (Style)Application.Current.Resources["PhoneTextNormalStyle"],
                                    };

                                    if (!isHeader)
                                    {
                                        tb.Opacity = 0.6;
                                    }
                                    
                                    isHeader = false;
                                    _changelog.Children.Add(tb);
                                }
                                else 
                                { 
                                    // replace empty lines with a rectangle
                                    // mark the following line as a 'header'
                                    var r = new Rectangle
                                    {
                                        Height = 20,
                                    };

                                    isHeader = true;
                                    _changelog.Children.Add(r);
                                }
                            }
                        }
                    }

                    changelogScrollViewer.Content = _changelog;
                });
            }
        }

        #endregion
    }
}