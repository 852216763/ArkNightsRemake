using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class JsonUtilityExtend
    {
        public static string ToJson<T>(List<T> list)
        {
            if (list == null)
            {
                return "";
            }
            Pack<T> pack = new Pack<T>();
            pack.data = list.ToArray();
            return JsonUtility.ToJson(pack);
        }

        public static List<T> FromJson<T>(string json)
        {
            Pack<T> pack = JsonUtility.FromJson<Pack<T>>(json);
            if (pack == null)
            {
                return new List<T>();
            }
            return new List<T>(pack.data);
        }

        [Serializable]
        class Pack<T>
        {
            public T[] data;
        }
    }
}
