#region Assembly System.Collections, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// C:\Users\narat\.nuget\packages\microsoft.netcore.app.ref\5.0.0\ref\net5.0\System.Collections.dll
#endregion

#nullable enable


namespace System.Collections.Generic
{
    //
    // Summary:
    //     An System.Collections.Generic.IEqualityComparer`1 that uses reference equality
    //     (System.Object.ReferenceEquals(System.Object,System.Object)) instead of value
    //     equality (System.Object.Equals(System.Object)) when comparing two object instances.
    public sealed class ReferenceEqualityComparer : IEqualityComparer<object?>, IEqualityComparer
    {
        //
        // Summary:
        //     Gets the singleton System.Collections.Generic.ReferenceEqualityComparer instance.
        public static ReferenceEqualityComparer Instance { get; }

        //
        // Summary:
        //     Determines whether two object references refer to the same object instance.
        //
        // Parameters:
        //   x:
        //     The first object to compare.
        //
        //   y:
        //     The second object to compare.
        //
        // Returns:
        //     true if both x and y refer to the same object instance or if both are null; otherwise,
        //     false.
        public bool Equals(object? x, object? y);
        //
        // Summary:
        //     Returns a hash code for the specified object. The returned hash code is based
        //     on the object identity, not on the contents of the object.
        //
        // Parameters:
        //   obj:
        //     The object for which to retrieve the hash code.
        //
        // Returns:
        //     A hash code for the identity of obj.
        public int GetHashCode(object? obj);
    }
}