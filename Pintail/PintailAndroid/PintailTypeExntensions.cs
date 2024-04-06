using System;
using System.Diagnostics.CodeAnalysis;

public static class PintailTypeExntensions
{
    public static bool IsAssignableTo(this Type type, [NotNullWhen(true)] Type? targetType)
    {
        return targetType?.IsAssignableFrom(type) ?? false;
    }
}
