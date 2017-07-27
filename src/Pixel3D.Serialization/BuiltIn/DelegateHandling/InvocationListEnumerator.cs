using System;

namespace Pixel3D.Serialization.BuiltIn.DelegateHandling
{
    public struct InvocationListEnumerator
    {
        internal InvocationListEnumerator(object[] invocationList, int invocationCount, Delegate theDelegate)
        {
            this.invocationList = invocationList;
            this.invocationCount = invocationCount;
            this.current = theDelegate;
            this.index = 0;
        }

        object[] invocationList;
        int invocationCount;
        Delegate current;
        int index;

        public Delegate Current { get { return current; } }

        public bool MoveNext()
        {
            if(invocationList == null)
            {
                if(index == 0)
                {
                    index++;
                    return true;
                }
            }
            else if(index < invocationCount)
            {
                current = (Delegate)invocationList[index];
                index++;
                return true;
            }

            return false;
        }
    }
}