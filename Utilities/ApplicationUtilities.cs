using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoilSimulater.Utilities
{
    public delegate Object InvokeFunction(Delegate method, Object[] parameters);
    public delegate void UpdateStatusMessageFunction(String message);

    public static class ApplicationUtilities
    {
        #region Public fields

        public static String Name;
        public static String CompactVersion;
        public static String Version;
        public static Boolean HasLicense = true;
        public static Boolean IsNemoActivated = true;
        public static Boolean IsReportActivated = true;

        public const String RegistryRoot = @"Software\CoilSimulater";

        public static Type ProgressDisplayType;
        public static Boolean IsInRemote;

        public const String BuiltType =
#if !DEBUG
            "";
#else
 "Debug ";
#endif

        //public static InvokeFunction ThreadSafeInvoke = ControlUtil.MainFormInvoke;
        public static UpdateStatusMessageFunction UpdateStatusMessage;

        #endregion

        #region Private fields

        private static String m_Revision;
        private static String m_FileVersion;
        private readonly static CultureInfo m_CultureInfo = CultureInfo.InstalledUICulture;
        private readonly static NumberFormatInfo m_NumberFormatInfo1 = (NumberFormatInfo)m_CultureInfo.NumberFormat.Clone();
        private readonly static NumberFormatInfo m_NumberFormatInfo2 = (NumberFormatInfo)m_CultureInfo.NumberFormat.Clone();

        private static readonly String[] m_FileSizeFormat = new String[] {
            "{0} bytes", "{0} KB", "{0} MB", "{0} GB", "{0} TB", "{0} PB", "{0} EB", "{0} ZB", "{0} YB" };

        #endregion

        #region Properties

        public static String Revision
        {
            get
            {
                if (m_Revision == null)
                {
                    String revisionStr = "$WCREV$";
                    int intRevision;
                    if (Int32.TryParse(revisionStr, out intRevision))
                        return revisionStr;

                    revisionStr = "$Rev$";
                    String revision = String.Empty;
                    for (int i = 0; i < revisionStr.Length; i++)
                    {
                        if (Char.IsDigit(revisionStr[i]))
                            revision += revisionStr[i];
                    }

                    m_Revision = revision;
                }

                return m_Revision;
            }
        }

        public static String BuildNumber
        {
            get
            {
                if (m_FileVersion == null)
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    m_FileVersion = String.IsNullOrEmpty(assembly.Location) ? String.Empty :
                        FileVersionInfo.GetVersionInfo(assembly.Location).ProductPrivatePart.ToString();
                }

                return m_FileVersion;
            }
        }

        public static String FormattedBuildId
        {
            get
            {
                String revision = Revision;
                if (!String.IsNullOrEmpty(revision))
                    return "[" + revision + "]";

                return "#" + BuildNumber;
            }
        }

        public static String BuildDate
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                DateTime dateBuild = String.IsNullOrEmpty(assembly.Location) ? DateTime.Now :
                    File.GetLastWriteTime(assembly.Location);

                return dateBuild.ToString("d", CultureInfo.InvariantCulture);
            }
        }

        public static Boolean IsIn64BitProcess
        {
            get { return IntPtr.Size == 8; }
        }

        public static String StatusMessage
        {
            set
            {
                if (UpdateStatusMessage != null)
                    UpdateStatusMessage(value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Trying to parse double value from string using "." and "," decimal separators
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <param name="value">Output value</param>
        /// <returns>true if double value was successfully parsed, false otherwise</returns>
        public static Boolean DoubleSafeParse(String str, out Double value)
        {
            m_NumberFormatInfo1.NumberDecimalSeparator = ",";
            if (Double.TryParse(str, NumberStyles.Any, m_NumberFormatInfo1, out value))
                return true;

            m_NumberFormatInfo2.NumberDecimalSeparator = ".";
            return Double.TryParse(str, NumberStyles.Any, m_NumberFormatInfo2, out value);
        }

        public static String DataSizeString(long size)
        {
            return DataSizeString(Convert.ToDecimal(size));
        }

        public static String DataSizeString(Decimal size)
        {
            if (size == 0)
                return "0";

            int i = 0;
            while ((i < m_FileSizeFormat.Length - 1) && (size >= 1024))
            {
                size = Math.Round(100 * size / 1024) / 100;
                i++;
            }

            return String.Format(CultureInfo.InvariantCulture, m_FileSizeFormat[i],
                size.ToString("###,###,###.##", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Returns memory available to be allocated by current process
        /// </summary>
        /// <returns>Available memory in bytes, or -1 in case of an error</returns>
        public static Int64 GetFullAvailableMemory()
        {
            try
            {
                using (PerformanceCounter pc = new PerformanceCounter("Memory", "Available Bytes"))
                {
                    return (Int64)pc.NextValue();
                }
            }
            catch (Exception ex)
            {
                Logger.AddMessage("Error while counting memory available",
                    LogMessageType.Error, LogMessageSeverity.Low, ex);
                return -1;
            }
        }

        /// <summary>
        /// Returns memory available to be allocated by current process
        /// </summary>
        /// <returns>Available memory in bytes, or -1 in case of an error</returns>
        public static Int64 GetAvailableMemory()
        {
            Int64 fullAvailableMemory = GetFullAvailableMemory();
            if (IsIn64BitProcess)
                return fullAvailableMemory;

            const Int64 TwoGb = 2147483648;
            return (fullAvailableMemory < TwoGb) ?
                fullAvailableMemory : TwoGb;
        }

        public static String GetResourceText(Type type, String name)
        {
            return (type == null) ? null : GetResourceText(type.Assembly, name);
        }

        public static String GetResourceText(Assembly assembly, String name)
        {
            if (assembly == null)
                return null;

            String[] names = assembly.FullName.Split(',');
            if (names.Length <= 0)
                return null;

            try
            {
                Stream stream = assembly.GetManifestResourceStream(names[0] + ".Resources." + name);
                if (stream == null)
                    return null;

                using (StreamReader textStreamReader = new StreamReader(stream))
                    return textStreamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Logger.AddMessage("Error while getting the text " + name + " from " + assembly,
                    LogMessageType.Error, LogMessageSeverity.High, ex);

                return null;
            }
        }

        #endregion Methods
    }
}
