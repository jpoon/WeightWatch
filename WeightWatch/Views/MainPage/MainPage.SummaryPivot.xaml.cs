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
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Microsoft.Phone.Controls;
    using WeightWatch.Classes;
    using WeightWatch.Models;
    using WeightWatch.ViewModels;

    public partial class MainPage : PhoneApplicationPage
    {
        readonly Uri _downArrow = new Uri("/WeightWatch;component/Images/arrow_down.png", UriKind.Relative);
        readonly Uri _upArrow = new Uri("/WeightWatch;component/Images/arrow_up.png", UriKind.Relative);

        private void SetupSummaryPivot()
        {
            startingWeight_textBlock.Text = String.Empty;
            currentWeight_textBlock.Text = String.Empty;
            summary_arrowImage.Source = null;

            summary_messageTextBlock.Text =
                "How to use:\n" +
                "(1) Add your daily weight\n" +
                "(2) Make a mistake? Tap and hold a weight entry on the 'Details' screen to edit or delete\n";

            var currentWeight = _viewModel.WeightHistoryList.FirstOrDefault();
            var startingWeight = _viewModel.WeightHistoryList.LastOrDefault();

            if (startingWeight == null || currentWeight == null)
            {
                startingWeight_textBlock.Text = "N/A";
                currentWeight_textBlock.Text = "N/A";
            }
            else
            {
                SetWeightTextBlock(startingWeight_textBlock, startingWeight);
                SetWeightTextBlock(currentWeight_textBlock, currentWeight);

                var weightDifference = currentWeight.Weight - startingWeight.Weight;
                var runSummaryWeight = new Run
                {
                    Text = weightDifference.ToString("+0.#;-0.#;0", CultureInfo.InvariantCulture),
                    FontSize = (Double)Application.Current.Resources["PhoneFontSizeExtraExtraLarge"],
                };

                var runSummaryMeasurementSystem = new Run
                {
                    Text = " " + MeasurementFactory.Get(ApplicationSettings.DefaultMeasurementSystem).Abbreviation,
                };
                summary_weightTextBlock.Inlines.Clear();
                summary_weightTextBlock.Inlines.Add(runSummaryWeight);
                summary_weightTextBlock.Inlines.Add(runSummaryMeasurementSystem);

                if (weightDifference > 0)
                {
                    summary_arrowImage.Source = new System.Windows.Media.Imaging.BitmapImage(_upArrow);
                }
                else if (weightDifference < 0)
                {
                    summary_arrowImage.Source = new System.Windows.Media.Imaging.BitmapImage(_downArrow);
                }

                summary_messageTextBlock.Text = Message.GetMessage(currentWeight, startingWeight);
            }
        }

        private void SetWeightTextBlock(TextBlock textBlock, WeightViewModel viewModel)
        {
            textBlock.Inlines.Clear();

            textBlock.Inlines.Add(
                new Run
                {
                    Text = viewModel.WeightStr,
                }
            );

            textBlock.Inlines.Add(
                new Run
                {
                    Text = " (" + viewModel.DateStr + ")",
                    FontStyle = FontStyles.Italic,
                    FontSize = (Double)Application.Current.Resources["PhoneFontSizeSmall"],
                    Foreground = (SolidColorBrush)Application.Current.Resources["PhoneSubtleBrush"],
                }
            );
        }
    }
}
