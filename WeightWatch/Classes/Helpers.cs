using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace WeightWatch.Classes
{
    public static class Helpers
    {
        public static Collection<String> EnumToStringList(Type enumType)
        {
            Collection<String> list = new Collection<String>();
            FieldInfo[] enumDetail = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo item in enumDetail)
            {
                list.Add(item.Name);
            }
            return list;
        }
    }
}
