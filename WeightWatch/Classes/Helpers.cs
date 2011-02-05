namespace WeightWatch.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Collections.ObjectModel;

    public static class Helpers
    {
        public static ReadOnlyCollection<String> EnumToStringList(Type enumType)
        {
            List<String> list = new List<String>();
            FieldInfo[] enumDetail = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo item in enumDetail)
            {
                list.Add(item.Name);
            }
            return new ReadOnlyCollection<string>(list);
        }

        public static ReadOnlyCollection<T> GetAllEnum<T>()
        {
            List<T> list = new List<T>();
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
            return new ReadOnlyCollection<T>(list);
        }
    }
}
