using System.Reflection;
using System.Runtime.Loader;

namespace North.Core.Common
{
    public class CollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        protected CollectibleAssemblyLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
