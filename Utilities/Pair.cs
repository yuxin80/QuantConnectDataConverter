using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoilSimulater.Utilities
{
    [Serializable]
    public class Pair<T, U> : ISerializable
    {
        #region Fields

        private T m_First;
        private U m_Second;

        #endregion

        #region Constructors

        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            First = first;
            Second = second;
        }

        public Pair(SerializationInfo info, StreamingContext context)
        {
            m_First = (T)info.GetValue("First", typeof(T));
            m_Second = (U)info.GetValue("Second", typeof(U));
        }

        #endregion

        #region Properties

        public T First
        {
            get { return m_First; }
            set { m_First = value; }
        }

        public U Second
        {
            get { return m_Second; }
            set { m_Second = value; }
        }

        #endregion

        public static bool operator ==(Pair<T, U> first, Pair<T, U> second)
        {
            if ((object)first == null && (object)second == null) return true;

            if ((object)first == null || (object)second == null) return false;

            return (first.Second.Equals(second.Second) && first.First.Equals(second.First));
        }

        public static bool operator !=(Pair<T, U> first, Pair<T, U> second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Pair<T, U>)) return false;

            Pair<T, U> tmpObj = obj as Pair<T, U>;

            return this == tmpObj;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + First.GetHashCode();
            hash = hash * 31 + Second.GetHashCode();
            return hash;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("First", m_First);
            info.AddValue("Second", m_Second);
        }

        public override string ToString()
        {
            return string.Format("{0},\t{1}", First, Second);
        }
    }
}
