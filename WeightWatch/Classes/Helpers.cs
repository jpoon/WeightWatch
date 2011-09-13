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
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Linq;

    public static class Helpers
    {
        public static ReadOnlyCollection<String> EnumToStringList(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }

            var enumDetail = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            var list = enumDetail.Select(item => item.Name).ToList();
            return new ReadOnlyCollection<string>(list);
        }

        public static ReadOnlyCollection<T> GetAllEnum<T>()
        {
            var list = new List<T>();
            var enumType = typeof(T);
            var enumDetail = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var item in enumDetail)
            {
                try
                {
                    list.Add((T)Enum.Parse(enumType, item.Name, true));
                }
                catch
                { }
            }
            return new ReadOnlyCollection<T>(list);
        }
    }
}
