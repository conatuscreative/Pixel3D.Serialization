using System.Collections.Generic;
using System.Reflection;

namespace Pixel3D.Serialization.Static
{
    internal class StaticModuleTable
    {
        internal static Dictionary<Module, int> moduleToId;
        internal static List<Module> idToModule;

        /// <param name="modules">A list of modules that is sorted by a reproducible sort (order is network-sensitive)</param>
        internal static void SetModuleTable(List<Module> modules)
        {
            moduleToId = new Dictionary<Module, int>();
            idToModule = modules;
            
            for(int i = 0; i < modules.Count; i++)
            {
                moduleToId.Add(modules[i], i);
            }
        }
    }
}
