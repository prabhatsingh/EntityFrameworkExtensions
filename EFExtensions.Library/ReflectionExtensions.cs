namespace EFExtensions.Library
{
    public static class ReflectionExtensions
    {
        public static T GetPropertyValue<T>(this object obj, string propname)
        {
            if (obj == null || obj.GetType().GetProperty(propname) == null)
                return default;

            return (T)obj.GetType().GetProperty(propname).GetValue(obj);
        }

        public static void SetPropertyValue(this object obj, string propertyname, object value)
        {
            obj.GetType().GetProperty(propertyname).SetValue(obj, value);
        }
    }
}
