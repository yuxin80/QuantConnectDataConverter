using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace CoilSimulater.Utilities
{
    public class ParameterControlManager : ObjectBase
    {
        private readonly Dictionary<Type, Type> m_ControlDatabase = new Dictionary<Type, Type>();

        private ParameterControlManager() { }

        private static ParameterControlManager m_Instance;

        public static ParameterControlManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new ParameterControlManager();

                return m_Instance;
            }
        }

        public void RegisterControl(Type dataType, Type controlType)
        {
            if (!typeof(ParameterControlBase).IsAssignableFrom(controlType))
                throw new ArgumentException("Control type must be inherited from ParameterControlBase", "controlType");

            if (m_ControlDatabase.ContainsKey(dataType))
                throw new ArgumentException("The data type has been registered!", "dataType");

            m_ControlDatabase.Add(dataType, controlType);
        }

        public Type GetControlType(Type dataType)
        {
            if (m_ControlDatabase.ContainsKey(dataType))
                return m_ControlDatabase[dataType];

            return null;
        }

        public ParameterControlBase GetControl(object param)
        {
            var dataType = param.GetType();
            var controlType = GetControlType(dataType);
            if (controlType == null) return null;
            return Activator.CreateInstance(controlType, new object[] { param }) as ParameterControlBase;
        }
    }
}
