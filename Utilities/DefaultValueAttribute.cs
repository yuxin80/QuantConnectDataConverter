using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoilSimulater.Utilities
{
    public class DefaultValueAttribute : Attribute
    {
        #region Fields

        private readonly Object m_Value;

        #endregion

        #region Constructors

        public DefaultValueAttribute(Object value)
        {
            m_Value = value;
        }

        public DefaultValueAttribute(Type type, String value)
        {
            Debug.Assert(type != null, "type should not be null");

            if (type.IsEnum)
            {
                m_Value = Enum.Parse(type, value);
            }
            else if (type.Equals(typeof(DateTime)))
            {
                m_Value = DateTime.Parse(value);
            }
        }

        public DefaultValueAttribute(ValueType valueType, Type type, String value)
        {
            Debug.Assert(type != null, "type should not be null");

            switch (valueType)
            {
                case ValueType.ListClass:
                    PropertyInfo prop = type.GetProperty(value);
                    m_Value = prop.GetValue(null, null);
                    break;
                default:
                    if (type.IsEnum)
                    {
                        m_Value = Enum.Parse(type, value);
                    }
                    else if (type.Equals(typeof(DateTime)))
                    {
                        m_Value = DateTime.Parse(value);
                    }
                    break;
            }
        }

        #endregion

        #region Properties

        public Object Value
        {
            get { return m_Value; }
        }

        #endregion

        public enum ValueType
        {
            Normal,
            ListClass
        }
    }
}
