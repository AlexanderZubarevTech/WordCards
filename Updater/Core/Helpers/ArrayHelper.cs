
namespace Updater.Core.Helpers
{
    public static class ArrayHelper
    {
        public static T[] AsArray<T>(this T value)
        {
            return new T[] { value };
        }
    }
}
