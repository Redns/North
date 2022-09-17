using System.Reflection;
using System.Runtime.Loader;

namespace North.Common
{
    public class CollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        public Assembly Assembly { get; set; }

        public CollectibleAssemblyLoadContext(string assemblyPath) : base(isCollectible: true)
        {
            Assembly = LoadFromAssemblyPath(assemblyPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
