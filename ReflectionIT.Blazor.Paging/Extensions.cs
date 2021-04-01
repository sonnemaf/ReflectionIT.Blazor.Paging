using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ReflectionIT.Blazor.Paging {
    public static class Extensions {

        [return: MaybeNull]
        internal static T GetOrDefault<T>(this Dictionary<string, StringValues> dict, string key, [AllowNull] T defaultValue) {
            return dict.TryGetValue(key, out var value) ? (T)Convert.ChangeType(value.First(), typeof(T)) : defaultValue;
        }
    }
}
