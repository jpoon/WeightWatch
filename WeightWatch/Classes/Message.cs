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

namespace WeightWatch.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using WeightWatch.Models;
    using WeightWatch.ViewModels;

    public static class Message
    {
        private enum MessageType
        {
            Neutral,
            Negative,
            Positive
        }

        private static readonly Dictionary<MessageType, List<String>> MessageDictionary = new Dictionary<MessageType, List<String>>
        {
            {
                MessageType.Negative,
                new List<String>
                {
                    "Lay off the donuts. You've gained [DELTA_WEIGHT] since [START_DATE].",
                    "[DELTA_WEIGHT]! You're packing on the pounds.",
                    "Boom! Boom! Boom!\r\nHear that? That's the sound of [LAST_WEIGHT] walking into the room.",
                    "On [START_DATE] you were [START_WEIGHT], on [LAST_DATE] you were [LAST_WEIGHT].\r\nWhat happened?!",
                    "You've gained [DELTA_WEIGHT] between [START_DATE] and [LAST_DATE]?!",
                    "You've gained [DELTA_WEIGHT]. Go hit the gym!",
                    "If you wish to grow thinner, diminish your dinner.",
                    "It's not the pants that you are wearing that make you look fat. Since [START_DATE] you've gained [DELTA_WEIGHT].",
                }
            },
            {
                MessageType.Positive,
                new List<String> 
                { 
                    "Wow! [DELTA_WEIGHT]!\r\nI am impressed",
                    "What's your secret? You've lost [DELTA_WEIGHT]!",
                    "Since starting on [START_DATE] at [START_WEIGHT], you've lost an amazing [DELTA_WEIGHT]! Keep it up!",
                    "Before: [START_WEIGHT]\r\nAfter: [LAST_WEIGHT]\r\nDifference: [DELTA_WEIGHT]\r\n",
                    "Errr...does this outfit make me look smaller?! It's not the outfit, you've just lost [DELTA_WEIGHT].\r\n",
                }
            },
            {
                MessageType.Neutral,
                new List<String>
                { 
                    "You have made absolutely no progress. Between [START_DATE] and [LAST_DATE], you still weigh [START_WEIGHT].",
                    "Fun Fact #1: The human body is 61.8% water by weight.",
                    "Fun Fact #2: In the average adult, the skin covers 12-20 square feet and accounts for 12% of body weight.",
                    "Fun Fact #3: At a weight of roughly 420 000 lbs, the Blue Whale is the world's heaviest animal.",
                    "Fun Fact #4: An ant can lift 50 times its own weight, can pull 30 times its own weight and always falls over on its right side when intoxicated.",
                    "Fun Fact #5: By weight, the watermelon is the most-consumed melon in the U.S., followed by the cantaloupe and honeydew.",
                    "Fun Fact #6: One pound of body fat is equal to 3500 calories.",
                    "Fun Fact #7: The earth is about 6 588 000 000 000 000 000 tons in weight.",
                    "Fun Fact #8: The total weight of skin for an average adult human is 6 lbs.",
                    "Fun Fact #9: Hummingbirds eat more than their body weight of insects and nectar every day. Due to their high metabolism, these small creatures are in constant danger of starving to death.",
                }
            },
        };

        public static String GetMessage(WeightViewModel first, WeightViewModel last)
        {
            if (first == null || last == null)
            {
                return String.Empty;
            }

            var weightDelta = last.Weight - first.Weight;
            var randomNumber = new Random();
            var measurementSystemAbbr = MeasurementFactory.Get(ApplicationSettings.DefaultMeasurementSystem).Abbreviation;

            var messageType = MessageType.Neutral;
            if (weightDelta > 0)
            {
                messageType = MessageType.Negative;
            }
            else if (weightDelta < 0)
            {
                messageType = MessageType.Positive;
            }

            var messageList = MessageDictionary[messageType];
            var message = messageList[randomNumber.Next(messageList.Count)];
            message = message.Replace("[DELTA_WEIGHT]", Math.Round(weightDelta).ToString("0.#", CultureInfo.InvariantCulture) + " " + measurementSystemAbbr);
            message = message.Replace("[START_DATE]", first.DateStr);
            message = message.Replace("[START_WEIGHT]", first.WeightStr);
            message = message.Replace("[LAST_DATE]", last.DateStr);
            message = message.Replace("[LAST_WEIGHT]", last.WeightStr);
            return message;
        }
    }
}
