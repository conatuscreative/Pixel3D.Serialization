using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Pixel3D.Serialization.Context
{
    public class SerializeContext
    {
        public SerializeContext(BinaryWriter writer, bool fillObjectTable = false, DefinitionObjectTable definitions = null)
        {
            if(writer == null)
                throw new ArgumentNullException("writer");
            this.writer = writer;

            if(definitions != null)
                this.definitionVisitedObjectIndices = definitions.externalVisitedObjectIndices;
            else
                this.definitionVisitedObjectIndices = new Dictionary<object, int>(ReferenceEqualityComparer<object>.Instance); // Empty for easy code

            visitedObjectIndices = new Dictionary<object, int>(ReferenceEqualityComparer<object>.Instance);

            // Only create this if asked:
            visitedObjectTable = fillObjectTable ? new List<object>() : null;
        }


        public DefinitionObjectTable GetAsDefinitionObjectTable()
        {
            if(visitedObjectTable == null)
                throw new InvalidOperationException("Cannot get definition object table unless fillObjectTable is set");
            return new DefinitionObjectTable(visitedObjectTable, visitedObjectIndices);
        }


        public virtual void Reset(BinaryWriter writer)
        {
            if(writer == null)
                throw new ArgumentNullException("writer");

            visitedObjectCount = 0;
            if(visitedObjectTable != null)
                visitedObjectTable.Clear();

            visitedObjectIndices.Clear();

            lastObjectSerialized = null;
        }




        private BinaryWriter writer;
        public BinaryWriter BinaryWriter { get { return writer; } }


        /// <summary>Optionally build the visited object table (for reconstructing from definitions on deserailization)</summary>
        protected readonly List<object> visitedObjectTable;
        /// <summary>Still need to assign IDs, even if we're not building the full table</summary>
        int visitedObjectCount;

        
        // Allow object graphs to be reconstructed:
        // (MUST use ReferenceEqualityComparer!)
        Dictionary<object, int> visitedObjectIndices;
        Dictionary<object, int> definitionVisitedObjectIndices;


        object lastObjectSerialized = null;


        /// <summary>
        /// Must be called at the start of the serialization of an object.
        /// Should be called at each level of an inheritance hierarchy (ie: call before calling the serializer method for a base type).
        /// </summary>
        public virtual void VisitObject(object obj)
        {
            // Ignore multiple visits on a single object instance (could be traversing an inheritance hierarchy)
            if(ReferenceEquals(obj, lastObjectSerialized))
                return;
            lastObjectSerialized = obj;

            // This will throw an exception if we visit the same object twice (desired behaviour)
            visitedObjectIndices.Add(obj, visitedObjectCount++);

            if(visitedObjectTable != null)
                visitedObjectTable.Add(obj);
        }

        public virtual void LeaveObject()
        {
            // Do nothing (derived types might want to gather statistics)
        }
        

        public bool Walk(object obj)
        {
#if DEBUG
            Debug.Assert(obj == null || !uniqueObjects.Contains(obj));
#endif

            int key;
            if(obj == null)
            {
                writer.Write(Constants.VisitNull);
                return false;
            }
            else if(visitedObjectIndices.TryGetValue(obj, out key))
            {
                DidLinkObject(key);
                writer.Write((uint)key);
                return false;
            }
            else if(definitionVisitedObjectIndices.TryGetValue(obj, out key))
            {
                DidLinkDefinitionObject(key);
                writer.Write((uint)key | Constants.DefinitionVisitFlag);
                return false;
            }
            else
            {
                writer.Write(Constants.FirstVisit);
                return true; // Caller should walk into the object
            }
        }



        // Extension points for statistics:
        protected virtual void DidLinkObject(int id) { }
        protected virtual void DidLinkDefinitionObject(int id) { }




#if DEBUG
        /// <summary>Feedback from the generated serializer about what it's doing (in case a memory-compare mismatch fires somewhere inside, for example)</summary>
        public string DebugLastTrace { get; private set; }

        public void DebugTrace(string trace)
        {
            DebugLastTrace = trace;
        }

#endif


        #region Unique Object Check

#if DEBUG
        HashSet<object> uniqueObjects = new HashSet<object>(ReferenceEqualityComparer<object>.Instance);
#endif

        [Conditional("DEBUG")]
        public void AssertUnique(object obj)
        {
#if DEBUG
            Debug.Assert(obj != null);
            Debug.Assert(!visitedObjectIndices.ContainsKey(obj));
            Debug.Assert(uniqueObjects.Add(obj));
#endif
        }

        #endregion

    }
}
