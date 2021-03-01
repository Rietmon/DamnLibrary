﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rietmon.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ToStringContent(this IEnumerable<object> array)
        {
            var result = "";

            foreach (var obj in array)
                result += obj.ToString();

            return result;
        }

        public static T GetObjectByName<T>(this IEnumerable<T> array, string name) where T : Object
        {
            return array.FirstOrDefault(obj => obj.name == name);
        }
    }
}