using System.Collections.Generic;

namespace Pixel3D.Serialization.Context
{
    public class DefinitionObjectTable
    {
        internal DefinitionObjectTable(List<object> visitedObjectTable, Dictionary<object, int> externalVisitedObjectIndices)
        {
            this.visitedObjectTable = visitedObjectTable;
            this.externalVisitedObjectIndices = externalVisitedObjectIndices;
        }

        internal List<object> visitedObjectTable;
        internal Dictionary<object, int> externalVisitedObjectIndices;

        public bool ContainsObject(object o)
        {
            return externalVisitedObjectIndices.ContainsKey(o);
        }
    }
}
