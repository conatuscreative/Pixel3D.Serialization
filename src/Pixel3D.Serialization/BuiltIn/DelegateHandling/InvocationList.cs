using System;

namespace Pixel3D.Serialization.BuiltIn.DelegateHandling
{
    public struct InvocationList
    {
        // Invocation list mode
        internal InvocationList(object[] invocationList, int invocationCount)
        {
            this.invocationList = invocationList;
            this.invocationCount = invocationCount;
            this.theDelegate = null;
        }

        // Single mode
        internal InvocationList(MulticastDelegate theDelegate)
        {
            this.invocationList = null;
            this.invocationCount = 1;
            this.theDelegate = theDelegate;
        }

        object[] invocationList;
        int invocationCount;
        Delegate theDelegate;

        public InvocationListEnumerator GetEnumerator()
        {
            return new InvocationListEnumerator(invocationList, invocationCount, theDelegate);
        }

        public int Count { get { return invocationCount; } }

        public Delegate this[int index]
        {
            get
            {
                if((uint)index >= (uint)invocationCount) // Also check for values < 0 by wrapping them around with uint
                    throw new IndexOutOfRangeException();

                if(invocationList != null)
                    return (Delegate)invocationList[index];
                else
                    return theDelegate;
            }
        }
    }
}