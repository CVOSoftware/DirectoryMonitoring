using System.IO;

namespace DirectoryMonitoring.Studio.Helper
{
    internal static class AssemblyHelper
    {
        public static string GetAssemblyRootPath<T>()
        {
            var type = typeof(T);
            var location = type.Assembly.Location;
            var root = Path.GetDirectoryName(location);

            return root;
        }
    }
}
