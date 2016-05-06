using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoilSimulater.Utilities
{
    public static class ClassUtilities
    {
        /// <summary>
        /// Get first object attribute of give type or null
        /// </summary>
        public static T GetAttribute<T>(ICustomAttributeProvider attributeProvider) where T : Attribute
        {
            // This will return also attributes of derived types
            Object[] attributes = attributeProvider.GetCustomAttributes(typeof(T), true);
            foreach (Object attribute in attributes)
            {
                T attr = attribute as T;
                // ... so we need to also check type
                if ((attr != null) && (attr.GetType() == typeof(T)))
                    return attr;
            }

            return null;
        }

        /// <summary>
        /// Get all object attributes of given type
        /// </summary>
        public static IEnumerable<T> GetAttributes<T>(ICustomAttributeProvider attributeProvider) where T : Attribute
        {
            // This will return also attributes of derived types
            Object[] attributes = attributeProvider.GetCustomAttributes(typeof(T), true);
            foreach (Object attribute in attributes)
            {
                T attr = attribute as T;
                // ... so we need to also check type
                if ((attr != null) && (attr.GetType() == typeof(T)))
                    yield return attr;
            }
        }

        public static T GetPropertyAttribute<T>(Type type, String name) where T : Attribute
        {
            if (type == null)
                return null;

            PropertyInfo property = type.GetProperty(name);
            if (property == null)
                return null;

            Object[] customAttributes = property.GetCustomAttributes(typeof(T), false);
            if ((customAttributes == null) || (customAttributes.Length == 0))
                return null;

            return customAttributes[0] as T;
        }

        public static Type GetGenericType(Type listType)
        {
            if (listType == null)
                return null;

            if (!listType.IsGenericType)
                return listType;

            Type[] genericArguments = listType.GetGenericArguments();
            if ((genericArguments == null) || (genericArguments.Length < 1))
            {
                // No generic types is found -> return original type
                return listType;
            }

            return genericArguments[0];
        }

        public static Boolean IsTypeDerivedFromGenericType(Type typeToCheck, Type genericType)
        {
            if ((typeToCheck == null) || (typeToCheck == typeof(object)))
                return false;

            if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
                return true;

            return IsTypeDerivedFromGenericType(typeToCheck.BaseType, genericType);
        }

        public static Boolean ListContainsValue<T>(IEnumerable<T> list, T value)
        {
            foreach (T listValue in list)
            {
                if (listValue.Equals(value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check if objects in collections are the same
        /// </summary>
        public static Boolean CollectionsEqual(ICollection collection1, ICollection collection2)
        {
            if (collection1 == collection2)
                return true;

            if ((collection1 == null) || (collection2 == null))
                return false;

            if (collection1.Count != collection2.Count)
                return false;

            IEnumerator enum1 = collection1.GetEnumerator();
            IEnumerator enum2 = collection2.GetEnumerator();
            for (int i = 0; i < collection1.Count; i++)
            {
                enum1.MoveNext();
                enum2.MoveNext();

                if (!enum1.Current.Equals(enum2.Current))
                    return false;
            }

            return true;
        }

        public static Boolean ValuesEqual<T>(T value, T newValue) where T : IComparable
        {
            return (((value != null) && value.CompareTo(newValue) == 0) ||
                    ((value == null) && (newValue == null)));
        }

        /// <summary>
        /// Check if types are compatible
        /// </summary>
        public static Boolean CompatibleTypes(Type type1, Type type2)
        {
            if (type1 == type2)
                return true;

            return (type1.IsAssignableFrom(type2) || type2.IsAssignableFrom(type1));
        }

        /// <summary>
        /// Find first compatible type in given list
        /// </summary>
        public static Type GetCompatibleTypeFromList(Type searchType, IEnumerable<Type> list)
        {
            foreach (Type type in list)
            {
                if (CompatibleTypes(searchType, type))
                    return type;
            }
            return null;
        }

        /// <summary>
        /// Returns all compatible types in given list
        /// </summary>
        public static IEnumerable<Type> GetCompatibleTypesFromList(Type searchType, IEnumerable<Type> list)
        {
            foreach (Type type in list)
            {
                if (CompatibleTypes(searchType, type))
                    yield return type;
            }
        }

        public static void CallMethodByName(Object obj, String methodName)
        {
            if ((obj == null) || String.IsNullOrEmpty(methodName))
                return;

            Type objectType = obj.GetType();
            MethodInfo methodInfo = objectType.GetMethod(methodName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
                return;

            try
            {
                methodInfo.Invoke(obj, null);
            }
            catch (Exception ex)
            {
                Logger.AddMessage("Error in CallMethod for: " + obj + " with " + methodName,
                    LogMessageType.Error, LogMessageSeverity.Normal, ex);
            }
        }

        public static void SetPropertyByName(Object obj, String propertyName, Object value)
        {
            if ((obj == null) || String.IsNullOrEmpty(propertyName))
                return;

            Type objectType = obj.GetType();

            try
            {
                PropertyInfo propertyInfo = objectType.GetProperty(propertyName);
                if ((propertyInfo != null) && propertyInfo.CanWrite)
                    propertyInfo.SetValue(obj, value, null);
            }
            catch (Exception ex)
            {
                Logger.AddMessage("Error in SetProperty for: " + obj + " with " + propertyName,
                    LogMessageType.Error, LogMessageSeverity.Normal, ex);
            }
        }

        public static Object GetPropertyByName(Object obj, String propertyName)
        {
            if ((obj == null) || String.IsNullOrEmpty(propertyName))
                return null;

            Type objectType = obj.GetType();

            try
            {
                PropertyInfo propertyInfo = objectType.GetProperty(propertyName);
                if ((propertyInfo == null) || !propertyInfo.CanRead)
                    return null;

                return propertyInfo.GetValue(obj, null);
            }
            catch (Exception ex)
            {
                Logger.AddMessage("Error in GetProperty for: " + obj + " with " + propertyName,
                    LogMessageType.Error, LogMessageSeverity.Normal, ex);
                return null;
            }
        }

        public static Boolean SetField(Object obj, Object value, String fieldName)
        {
            FieldInfo field = obj.GetType().GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);
            if ((field == null) || (value == null))
                return false;

            try
            {
                field.SetValue(obj, value);
            }
            catch (Exception ex)
            {
                Logger.AddMessage("Error in SetField for: " + obj + " field name: " + fieldName,
                    LogMessageType.Error, LogMessageSeverity.Normal, ex);
                return false;
            }

            return true;
        }

        public static Boolean IsNumericType(Type valueType)
        {
            return IsFloatingPointType(valueType) ||
                (valueType == typeof(Int32)) || (valueType == typeof(Int64));
        }

        public static Boolean IsSingleType(Type type)
        {
            return (type == typeof(Single)) || (type == typeof(Single?));
        }

        public static Boolean IsDoubleType(Type type)
        {
            return (type == typeof(Double)) || (type == typeof(Double?));
        }

        public static Boolean IsFloatingPointType(Type type)
        {
            return (type == typeof(Double)) || (type == typeof(Single)) ||
                (type == typeof(Double?)) || (type == typeof(Single?));
        }

        /// <summary>
        /// To validate if typeA is typeB or inherits from typeB.
        /// </summary>
        /// <param name="typeA"></param>
        /// <param name="typeB"></param>
        /// <returns>false will be returned in cases:
        /// <para>typaA/typeB is null</para>
        /// <para>typeA/typeB is neither class nor interface.</para>
        /// <para>typeA is interface, but typeB is NOT inteface</para>
        /// <para>typaA doesn't inherit from typeB</para>
        /// </returns>
        public static bool IsTypeAInheritFromTypeB(Type typeA, Type typeB)
        {
            if (typeA == null || typeB == null ||
                 (typeA.IsClass == false && typeA.IsInterface == false) ||
                (typeB.IsClass == false && typeB.IsInterface == false) ||
                typeA.IsInterface && typeB.IsInterface == false)
            {
                return false;
            }

            if (typeA == typeB)
            {
                return true;
            }

            if ((typeB.IsInterface && typeA.GetInterface(typeB.Name) != null) ||
                (typeB.IsClass && typeA.IsSubclassOf(typeB)))
            {
                return true;
            }

            return false;
        }

        public static bool TypeIsList(Type propType, out Type itemType)
        {
            if (propType.IsGenericType)
            {
                var def = propType.GetGenericTypeDefinition();
                itemType = propType.GetGenericArguments()[0];
                return def.GetInterfaces().Any(i => (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || i == typeof(IEnumerable));
            }
            itemType = null;
            return false;
        }

        public static bool IsTypeNumber(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsTypeDateTime(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            return typeCode == TypeCode.DateTime;
        }
    }
}
