using System.IO;
using System.Linq;
using System.Reflection;

namespace Parcel.Neo.Base
{
    public static class Helpers
    {
        public static Stream ReadBinaryResource(string name)
        {
            // Determine path
            var assembly = Assembly.GetCallingAssembly();
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            if (!name.StartsWith(nameof(Parcel)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));
            }

            Stream stream = assembly.GetManifestResourceStream(resourcePath);
            return stream;
        }
        
        public static string ReadTextResource(string name)
        {
            // Determine path
            var assembly = Assembly.GetCallingAssembly();
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            if (!name.StartsWith(nameof(Parcel)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));
            }

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}