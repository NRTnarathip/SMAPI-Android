using System.Reflection;

namespace StardewModdingAPI
{
    public static class MethodHelper
    {
        public static string FullName(this MethodBase method) => $"{method.DeclaringType}::{method}";
    }
}