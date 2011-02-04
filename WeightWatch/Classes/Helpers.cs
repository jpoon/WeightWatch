namespace WeightWatch.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class Helpers
    {
        public static List<String> EnumToStringList(Type enumType)
        {
            List<String> list = new List<String>();
            FieldInfo[] enumDetail = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo item in enumDetail)
            {
                list.Add(item.Name);
            }
            return list;
        }
    }
}
