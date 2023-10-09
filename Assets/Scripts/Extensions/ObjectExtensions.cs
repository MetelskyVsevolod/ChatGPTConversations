using System.Collections;

namespace Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsCollectionType<T>()
        {
            var type = typeof(T);
            return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
        }
    }
}