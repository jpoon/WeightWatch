﻿/*
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

namespace WeightWatch.MeasurementSystem
{
    using System;
    using System.Globalization;

    class Stone : IMeasurementSystem
    {
        public MeasurementUnit MeasurementSystem
        {
            get { return MeasurementUnit.Stone; }
        }

        public string Abbreviation
        {
            get { return "st"; }
        }

        public Double ConvertTo(MeasurementUnit system, Double weight)
        {
            switch (system)
            {
                case MeasurementUnit.Imperial:
                    return weight * 14;
                case MeasurementUnit.Metric:
                    return weight * 6.35029318;
                case MeasurementUnit.Stone:
                    return weight;
                default:
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Measurement system of type {0} cannot be found",
                            Enum.GetName(typeof(MeasurementUnit), system)));
            }
        }
    }
}
