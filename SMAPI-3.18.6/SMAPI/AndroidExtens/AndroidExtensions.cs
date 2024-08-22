using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public static class AndroidExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>
    {
        return span.IndexOf(value) >= 0;
    }
    public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
    {
        return source.ToDictionary(keySelector, elementSelector, null);
    }
}
public static class AndroidLog
{
    public static void Log(string msg) => Android.Util.Log.Debug("NRT Debug", "[NRT Debug] " + msg);
    public static void Fixbug(string msg) => Android.Util.Log.Debug("NRT Fixbug", "[NRT Fixbug] " + msg);
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
internal sealed class MemberNotNullWhenAttribute : Attribute
{
    public bool ReturnValue { get; }

    public string[] Members { get; }

    public MemberNotNullWhenAttribute(bool returnValue, string member)
    {
        ReturnValue = returnValue;
        Members = new string[1] { member };
    }

    public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
    {
        ReturnValue = returnValue;
        Members = members;
    }
}

