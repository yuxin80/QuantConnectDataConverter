using System;
using System.Text;

namespace CoilSimulater.Utilities
{
    public class LogMessage
    {
        #region Fields

        public const LogMessageSeverity DefaultSeverity = LogMessageSeverity.Normal;

        private LogMessageType m_Type = LogMessageType.Information;
        private String m_Message;
        private Exception m_Exception;
        private LogMessageSeverity m_Severity = DefaultSeverity;

        #endregion Fields

        #region Constructors

        public LogMessage() { }

        public LogMessage(String message)
        {
            Message = message;
        }

        public LogMessage(String message, LogMessageType type)
        {
            Message = message;
            Type = type;
        }

        public LogMessage(String message, LogMessageType type, LogMessageSeverity severity)
        {
            Message = message;
            Type = type;
            Severity = severity;
        }

        public LogMessage(String message, LogMessageType type, Exception exception)
        {
            Message = message;
            Type = type;
            Exception = exception;
        }

        public LogMessage(String message, LogMessageType type, LogMessageSeverity severity, Exception exception)
        {
            Message = message;
            Type = type;
            Severity = severity;
            Exception = exception;
        }

        #endregion Constructors

        #region Properties

        public String Title
        {
            get
            {
                switch (Type)
                {
                    case LogMessageType.Start:
                        return "starting";
                    case LogMessageType.Exit:
                        return "exiting";
                    case LogMessageType.Information:
                        return "information";
                    case LogMessageType.Warning:
                        return "warning";
                    case LogMessageType.Error:
                        return "error";
                    case LogMessageType.Debug:
                        return String.Empty;
                    default:
                        return String.Empty;
                }
            }
        }

        private String LineStarterString
        {
            get
            {
                switch (Type)
                {
                    case LogMessageType.Start:
                        return ">>> ";
                    case LogMessageType.Exit:
                        return "<<< ";
                    case LogMessageType.Information: 
                        return "    ";
                    case LogMessageType.Warning:
                        return "!   ";
                    case LogMessageType.Error:
                        return "!!! ";
                    case LogMessageType.Debug:
                        return "-   ";
                    default:
                        return "    ";
                }
            }
        }

        public LogMessageType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        public String Message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }

        public Exception Exception
        {
            get { return m_Exception; }
            set { m_Exception = value; }
        }

        public LogMessageSeverity Severity
        {
            get { return m_Severity; }
            set { m_Severity = value; }
        }

        #endregion Properties

        #region Methods

        public override String ToString()
        {
            DateTime dateTime = DateTime.Now;
            String dateTimeString = String.Concat(dateTime.ToString(TimeUtilities.DateFormat), " ", dateTime.ToString(TimeUtilities.FullTimeFormat));

            StringBuilder output = new StringBuilder();
            if (m_Type == LogMessageType.Start)
                output.Append(Environment.NewLine);

            String buildId = ApplicationUtilities.FormattedBuildId;
            buildId += String.IsNullOrEmpty(buildId) ? String.Empty : ", ";

            output.Append(String.Concat(LineStarterString, dateTimeString, " ", ApplicationUtilities.BuiltType,
                buildId, m_Message, Environment.NewLine));

            if (m_Type == LogMessageType.Exit)
                output.Append(Environment.NewLine);

            Exception ex = Exception;
            if (ex != null)
            {
                String exceptionMessage = ex.Message;
                if (!String.IsNullOrEmpty(exceptionMessage))
                    exceptionMessage = exceptionMessage.Trim();
                output.Append(String.Concat("Exception raised (", ex.GetType().Name, "): ", exceptionMessage, Environment.NewLine));
                output.Append(String.Concat("Stack trace: ", Environment.NewLine, ex.StackTrace, Environment.NewLine));

                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    exceptionMessage = ex.Message;
                    if (!String.IsNullOrEmpty(exceptionMessage))
                        exceptionMessage = exceptionMessage.Trim();
                    output.Append(String.Concat(ex.GetType().Name, ": ", exceptionMessage, Environment.NewLine));
                }

                output.Append(Environment.NewLine);
            }

            return output.ToString();
        }

        public String FormatSimple()
        {
            String title = Title;
            if (!String.IsNullOrEmpty(title))
                title += ": ";

            title += Message;
            return (Exception != null) ? title + " (" + Exception.Message.Trim() + ")" : title;
        }

        public String BriefOutput()
        {
            String output = Message;
            return (Exception != null) ? output + ": " + Exception.Message.Trim() : output;
        }

        #endregion Methods
    }

    public sealed class LogMessageEventArgs : EventArgs
    {
        #region Fields

        private LogMessage m_Message;

        #endregion Fields

        #region Constructors

        public LogMessageEventArgs() { }

        public LogMessageEventArgs(LogMessage message)
        {
            Message = message;
        }

        #endregion Constructors

        #region Properties

        public LogMessage Message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }

        #endregion Properties
    }
}