using System;
using System.IO;

namespace Pixel3D.Serialization
{
    public class GeneratorReports : IDisposable
    {
        public GeneratorReports(string directory)
        {
            this.Directory = directory;
            System.IO.Directory.CreateDirectory(directory);

            Log = new StreamWriter(Directory + @"\Log.txt");
            TypeDiscovery = new StreamWriter(Directory + @"\Type Discovery.txt");
            DelegateDiscovery = new StreamWriter(Directory + @"\Delegate Discovery.txt");
            DelegateDiscoveryGrouped = new StreamWriter(Directory + @"\Delegate Discovery Grouped.txt");
            DelegateClassification = new StreamWriter(Directory + @"\Delegate Classification.txt");
            DelegateMethods = new StreamWriter(Directory + @"\Delegate Methods.txt");
            TypeClassification = new StreamWriter(Directory + @"\Type Classification.txt");
            CustomMethodDiscovery = new StreamWriter(Directory + @"\Custom Method Discovery.txt");
            Error = new StreamWriter(Directory + @"\Errors.txt");
        }

        public void Dispose()
        {
            Log.Dispose();
            TypeDiscovery.Dispose();
            DelegateDiscovery.Dispose();
            DelegateDiscoveryGrouped.Dispose();
            DelegateClassification.Dispose();
            DelegateMethods.Dispose();
            TypeClassification.Dispose();
            CustomMethodDiscovery.Dispose();
            Error.Dispose();
        }

        public string Directory { get; }

        public StreamWriter Log { get; }
        public StreamWriter TypeDiscovery { get; }
        public StreamWriter DelegateDiscovery { get; }
        public StreamWriter DelegateDiscoveryGrouped { get; }
        public StreamWriter DelegateClassification { get; }
        public StreamWriter DelegateMethods { get; }
        public StreamWriter TypeClassification { get; }
        public StreamWriter CustomMethodDiscovery { get; }
        public StreamWriter Error { get; }
    }
}

