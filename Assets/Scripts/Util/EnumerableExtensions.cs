using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions {
    /// <summary>
    /// Returns a string showing all elements of the enumerable
    /// </summary>
    /// <param name="toStrFunc">function to convert elements to string</param>
    /// <param name="includeCount">should prefix enumerable count?</param>
    /// <param name="includeBraces">should braces before and after be included? []</param>
    /// <param name="seperator">seperator between elements. default is ","</param>
    /// <returns>string with all elements</returns>
    public static string ToStringFull<T>(this IEnumerable<T> enumerable, Func<T, string> toStrFunc = null, bool includeCount = false, bool includeBraces = true, string seperator = ",") {
        if (toStrFunc == null) toStrFunc = e => e.ToString();
        // todo? formatting
        System.Text.StringBuilder str = new System.Text.StringBuilder("");
        int cnt = enumerable.Count();
        if (includeCount) str.Append($"{cnt}");
        if (includeBraces) str.Append("[");
        int i = 0;
        foreach (var e in enumerable) {
            str.Append(toStrFunc(e));
            if (i < cnt - 1) {
                str.Append(seperator);
            }
            i++;
        }
        if (includeBraces) str.Append("]");
        return str.ToString();
    }
}