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
    using System.Globalization;
    using Microsoft.Phone.Controls;
    using WeightWatch.Classes;
    using WeightWatch.Models;

    public partial class MainPage : PhoneApplicationPage
    {
        readonly Uri _downArrow = new Uri("/WeightWatch;component/Images/downarrow.png", UriKind.Relative);
        readonly Uri _upArrow = new Uri("/WeightWatch;component/Images/uparrow.png", UriKind.Relative);
        readonly Uri _noChange = new Uri("/WeightWatch;component/Images/nochange.png", UriKind.Relative);

        private void SetupSummaryPivot()
        {
            summary_arrowImage.Source = new System.Windows.Media.Imaging.BitmapImage(_noChange);
            summary_weightTextBlock.Text = "0";
            summary_systemTextBlock.Text = "[" + MeasurementFactory.Get(ApplicationSettings.DefaultMeasurementSystem).Abbreviation + "]";
            summary_messageTextBlock.Text =
                "How to use:\n" +
                "(1) Add your daily weight\n" +
                "(2) Make a mistake? Tap and hold a weight entry on the 'Details' screen to edit or delete\n";

            var first = _viewModel.FirstWeightEntry;
            var last = _viewModel.LastWeightEntry;

            if (first != null && last != null)
            {
                var weightDelta = last.Weight - first.Weight;
                if (weightDelta > 0)
                {
                    summary_arrowImage.Source = new System.Windows.Media.Imaging.BitmapImage(_upArrow);
                }
                else if (weightDelta < 0)
                {
                    summary_arrowImage.Source = new System.Windows.Media.Imaging.BitmapImage(_downArrow);
                }

                summary_weightTextBlock.Text = weightDelta.ToString("+#.#;-#.#;0", CultureInfo.InvariantCulture);
                summary_messageTextBlock.Text = Message.GetMessage(first, last);
            }
        }
    }
}
