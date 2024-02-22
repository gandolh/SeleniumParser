using System.Reflection;

namespace SeleniumParser.Lib
{
    internal static class ReflectionExtensions
    {
        private static IEnumerable<Type> BaseTypes(this Type type)
        {
            while (type.BaseType != null)
            {
                yield return type.BaseType;
                type = type.BaseType;
            }
        }

        internal static FieldInfo GetFieldInfo(this object value, string memberName, BindingFlags? bindingFlags = null)
        {
            bindingFlags ??= BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var type = value.GetType();
            return new[] { type }.Concat(type.BaseTypes()).Select(t => t.GetField(memberName, bindingFlags.Value)).First(
                f => f != null);
        }

        internal static TValue GetFieldValue<TValue>(this object instance, string fieldName) =>
            instance == null
                ? throw new ArgumentNullException(nameof(instance))
                : (TValue)instance.GetFieldInfo(fieldName).GetValue(instance);

        internal static void SetFieldValue<TValue>(this object instance, string fieldName, TValue value) =>
            instance.GetFieldInfo(fieldName).SetValue(instance, value);
    }
}
