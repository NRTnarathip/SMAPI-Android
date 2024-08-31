using System.Reflection;

namespace StardewModdingAPI
{
    public static class MethodHelper
    {
        public static string FullName(this MethodBase method) => $"{method.DeclaringType}::{method}";
        public static string FullName(this MethodInfo method) => $"{method.DeclaringType}::{method}";
    }
}