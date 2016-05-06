using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using System.Windows;
using System.Linq.Expressions;

namespace CoilSimulater.Utilities
{
    [Serializable]
    public abstract class ObjectBase : ISerializable, IDisposable, INotifyPropertyChanged, ITransaction, ICloneable
    {
        #region fields

        private readonly Dictionary<string, object> m_PropertyCache = new Dictionary<string, object>();

        protected Boolean m_IsLoading = false;

        private String m_Id = null;

        private int m_InTransaction = 0;

        #endregion

        #region constructor

        public ObjectBase()
        {
            Initialize();
            SetDefaultValue();

            m_Id = Guid.NewGuid().ToString();
        }

        //serrilizable
        protected ObjectBase(SerializationInfo info, StreamingContext context)
        {
            m_IsLoading = true;
            try
            {
                SetDefaultValue();

                Type classType = this.GetType();
                foreach (PropertyInfo propInfo in classType.GetProperties())
                {
                    object[] attributes = propInfo.GetCustomAttributes(typeof(XmlAttributeAttribute), true);
                    foreach (var attribute in attributes)
                    {
                        if (attribute == null)
                            continue;

                        try
                        {
                            Object obj = info.GetValue(propInfo.Name, propInfo.PropertyType);
                            propInfo.SetValue(this, obj, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
                        }
                        catch
                        {

                        }
                        break;
                    }
                }
            }
            finally
            {
                if (string.IsNullOrEmpty(m_Id))
                {
                    m_Id = Guid.NewGuid().ToString();
                }
                m_IsLoading = false;
            }
        }

        #endregion

        #region Properties

        [XmlAttribute]
        [CloneIgnore]
        public string Id
        {
            get { return m_Id; }
            protected set { m_Id = value; }
        }

        [Parameter]
        [XmlAttribute]
        [DefaultValue("Default name")]
        public string Name
        {
            get { return GetValue(() => Name); }
            set { SetValue(() => Name, value); }
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            if (Disposed != null)
            {
                Disposed(this, new EventArgs());
            }
        }

        public event EventHandler Disposed;

        #endregion

        #region methods

        public virtual void Initialize()
        {

        }

        protected virtual void TransactionCommittedHandler()
        {

        }

        public void SetDefaultValue()
        {
            Type classType = this.GetType();
            foreach (PropertyInfo propInfo in classType.GetProperties())
            {
                Object defaultValue = GetDefaultValue(propInfo);
                if (defaultValue == null) continue;
                try
                {
                    if (propInfo.CanWrite)
                    {
                        if (propInfo.PropertyType == typeof(double?))
                        {
                            double? value = new double?(Convert.ToDouble(defaultValue));
                            propInfo.SetValue(this, value, null);
                        }
                        else
                        {
                            propInfo.SetValue(this, defaultValue, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private object GetDefaultValue(PropertyInfo propInfo)
        {
            Debug.Assert(propInfo != null, "propInfo can not be null in GetDefaultValue method.");

            Object[] attributes = propInfo.GetCustomAttributes(typeof(DefaultIgnoreAttribute), true);
            if (attributes.Length > 0)
                return null;

            attributes = propInfo.GetCustomAttributes(typeof(DefaultValueAttribute), true);
            foreach (Object att in attributes)
            {
                DefaultValueAttribute valueAttr = att as DefaultValueAttribute;
                if (valueAttr == null)
                    continue;
                if (valueAttr.Value is Type)
                {
                    var type = valueAttr.Value as Type;

                    return Activator.CreateInstance(type);
                }
                return valueAttr.Value;
            }
            if (propInfo.PropertyType.IsValueType)
            {
                return Activator.CreateInstance(propInfo.PropertyType);
            }
            return null;
        }

        public object GetValue(string propname)
        {
            Type type = this.GetType();
            var propInfo = type.GetProperty(propname);
            if (propInfo != null)
            {
                return propInfo.GetValue(this, null);
            }

            return null;
        }

        public T GetValue<T>(Expression<Func<T>> property)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            if (propertyInfo == null)
                throw new Exception("Get property information failed.");

            return (T)GetValue(propertyInfo);
        }

        private object GetValue(PropertyInfo propertyInfo)
        {
            if (!m_PropertyCache.ContainsKey(propertyInfo.Name))
            {
                m_PropertyCache.Add(propertyInfo.Name, GetDefaultValue(propertyInfo));
            }

            return m_PropertyCache[propertyInfo.Name];
        }

        protected void FirePropertyChangedEvent(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected Boolean SetValue<T>(Expression<Func<T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            if (propertyInfo == null)
                throw new Exception("Get property information failed.");

            var currentValue = GetValue(propertyInfo);
            if (currentValue != null && currentValue.Equals(value)) return false;
            m_PropertyCache[propertyInfo.Name] = value;
            FirePropertyChangedEvent(propertyInfo.Name);
            return true;
        }

        #endregion

        #region ISerializable

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Type classType = this.GetType();
            foreach (PropertyInfo propInfo in classType.GetProperties())
            {
                object[] attributes = propInfo.GetCustomAttributes(typeof(XmlAttributeAttribute), true);
                foreach (var attribute in attributes)
                {
                    if (attribute == null)
                        continue;

                    info.AddValue(propInfo.Name, propInfo.GetValue(this, null));
                    break;
                }
            }
        }

        #endregion

        protected bool SetParameter<T>(ref T value, T newValue) where T : IComparable
        {
            if (ClassUtilities.ValuesEqual(value, newValue))
                return false;

            value = newValue;
            //InvokeChanged();
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //current implementation is not thread safe
        #region ITransaction Members

        public void StartTransaction()
        {
            m_InTransaction++;
        }

        public void CommitTransaction()
        {
            m_InTransaction--;
            if (m_InTransaction < 0)
                m_InTransaction = 0;
            if (m_InTransaction <= 0)
                TransactionCommittedHandler();
        }

        public bool IsInTransaction()
        {
            return m_InTransaction > 0;
        }

        #endregion

        #region IClonable Members

        public virtual object Clone()
        {
            var type = this.GetType();
            var result = Activator.CreateInstance(type, new object[0]);

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in properties)
            {
                if (IsCloneIgnorable(prop))
                    continue;
                var setMethod = prop.GetSetMethod();
                if (setMethod == null) continue;
                if (typeof(ICloneable).IsAssignableFrom(prop.PropertyType) && (!IsReferenceType(prop)))
                {
                    var source = prop.GetValue(this) as ICloneable;
                    prop.SetValue(result, source.Clone());
                }
                else
                {
                    prop.SetValue(result, prop.GetValue(this));
                }
            }

            return result;
        }

        private bool IsCloneIgnorable(PropertyInfo prop)
        {
            var attrs = prop.GetCustomAttributes(true);
            return attrs.Any(i => i is CloneIgnoreAttribute);
        }

        private bool IsReferenceType(PropertyInfo prop)
        {
            var attrs = prop.GetCustomAttributes(true);
            return attrs.Any(i => i is ReferenceTypeAttribute);
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion
    }

    public class DefaultIgnoreAttribute : Attribute
    { }
}
