using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoilSimulater.Utilities
{
    public class ParameterCustomizeUIAttribute : Attribute
    {
        private Type m_UiType = null;
        private string m_PropertyName = string.Empty;

        public ParameterCustomizeUIAttribute(Type typeofUi, string propertyName = null)
        {
            m_UiType = typeofUi;
            m_PropertyName = propertyName;
        }

        public Type UiType
        {
            get { return m_UiType; }
        }

        /// <summary>
        /// The property name of the object which will be set for the customized control. If this property is empty, then the object itself will be set to the customized UI
        /// </summary>
        public string PropertyName
        {
            get { return m_PropertyName; }
        }
    }

    public class ParameterRangeAttribute : Attribute
    {
        private double m_Min;
        private double m_Max;

        public double Min
        {
            get { return m_Min; }
            set { m_Min = value; }
        }

        public double Max
        {
            get { return m_Max; }
            set { m_Max = value; }
        }

        public ParameterRangeAttribute(double min, double max)
        {
            m_Min = Math.Min(min, max);
            m_Max = Math.Max(min, max);
        }
    }

    public class ParameterAttribute : Attribute
    {
        public ParameterAttribute() { }
    }

    public class ParameterNameAttribute : Attribute
    {
        private string m_ParameterName;

        public string ParameterName
        {
            get { return m_ParameterName; }
        }

        public ParameterNameAttribute(string name)
        {
            m_ParameterName = name;
        }
    }

    public class ParameterFromListAttribute : Attribute
    {
        private string m_ListName;
        private Type m_ObjectType;

        public string ListName
        {
            get { return m_ListName; }
        }

        public Type Type
        {
            get { return m_ObjectType; }
        }

        public ParameterFromListAttribute(string listName, Type type)
        {
            m_ListName = listName;

            m_ObjectType = type;
        }
    }

    public class ParameterEnabledFromPropertyAttribute : Attribute
    {
        private string m_PropertyName;
        private Boolean m_NotOperation = false;

        public string PropertyName
        {
            get { return m_PropertyName; }
        }

        public Boolean NotOperation
        {
            get { return m_NotOperation; }
        }

        public ParameterEnabledFromPropertyAttribute(string propertyName, Boolean notOperation = false)
        {
            m_PropertyName = propertyName;
            m_NotOperation = notOperation;
        }
    }

    public class ParameterTipAttribute : Attribute
    {
        private string m_Message;

        public string Message
        {
            get { return m_Message; }
        }

        public ParameterTipAttribute(string message)
        {
            m_Message = message;
        }
    }

    public class ParameterRelationToAttribute : Attribute
    {
        #region fields

        private Relationship m_Relationship;
        private string m_PropertyName;

        #endregion

        #region property

        public Relationship Relationship
        {
            get { return m_Relationship; }
        }

        public string PropertyName
        {
            get { return m_PropertyName; }
        }

        #endregion

        public ParameterRelationToAttribute(string propertyName, Relationship relation)
        {
            m_Relationship = relation;
            m_PropertyName = propertyName;
        }
    }

    public class ReadOnlyAttribute : Attribute { }

    public enum Relationship
    {
        EqualTo,
        LargerThan,
        SmallerThan,
        LargerEqualTo,
        SmallerEqualTo
    }

    public class IsFileNameAttribute : Attribute { }

    public class IsFolderNameAttribute : Attribute { }
}
