using System.Reflection;

namespace ShareInvest.Analysis
{
    public static class UtilityToFindByCode
    {
        public static T FindByCode<T>(this object target, string name) where T : class => target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public).GetValue(target) as T;
        public static T FindByCode<T>(this string name, object target) where T : class => target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public).GetValue(target) as T;
        public static OpenAPI.Collect Collect(string name, object target) => FindByCode<OpenAPI.Collect>(target, name);
        public static OpenAPI.Collect IsCollect(this string name, object target) => FindByCode<OpenAPI.Collect>(target, name);
    }
}