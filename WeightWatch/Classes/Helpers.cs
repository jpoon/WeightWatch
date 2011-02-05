namespace WeightWatch.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Collections.ObjectModel;

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

        public static Collection<T> GetAllEnum<T>()
        {
            Collection<T> list = new Collection<T>();
            Type enumType = typeof(T);
            FieldInfo[] enumDetail = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo item in enumDetail)
            {
                try
                {
                    list.Add((T)Enum.Parse(enumType, item.Name, true));
                }
                catch (Exception) { }
            }
            return list;
        }
    }
}
