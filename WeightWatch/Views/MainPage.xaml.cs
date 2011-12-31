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
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Tasks;
    using WeightWatch.Models;
    using WeightWatch.ViewModels;

    public partial class MainPage : PhoneApplicationPage
    {
        WeightListViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SetupSummaryPivot();
            SetupGraphPivot();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var c = weightLongListSelector.SelectedItem;
            if (c == null)
            {
                _viewModel = new WeightListViewModel();
                DataContext = _viewModel;
            }
        }

        #region Event Handlers

        private void AppBarIconClick_AddWeight(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/WeightEntry.xaml", UriKind.Relative)));
        }

        private void AppBarIconClick_Settings(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.Relative)));
        }

        private void AppBarIconClick_Export(object sender, EventArgs e)
        {
            var weightListModel = new WeightListModel();
            var csv = weightListModel.Export();

            var email = new EmailComposeTask
                            {
                                Body = csv,
                            };
            email.Show();
        }

        private void AppBarIconClick_About(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/About.xaml", UriKind.Relative)));
        }

        private void EditMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as MenuItem).DataContext as WeightViewModel;
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/WeightEntry.xaml?Date=" + item.DateStr, UriKind.Relative)));
        }

        private void DeleteMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as MenuItem).DataContext as WeightViewModel;
            WeightListViewModel.Delete(item);
            SetupSummaryPivot();
        }

        #endregion Event Handlers
    }
}