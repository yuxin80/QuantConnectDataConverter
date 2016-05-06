using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CoilSimulater.Utilities
{
    public enum LogMessageType { Information, Start, Exit, Debug, Warning, Error };
    public enum LogMessageSeverity { No = 0, VeryLow = 20, Low = 30, Normal = 50, 
        High = 70, VeryHigh = 80, Critical = 100 };

    public static class Logger
    {
        #region Fields

        private const String DefaultFileName = "CoilSimulater.log";
        private const int MaximumFileSize = 10000000; // 10 MB

        public static String FilePath = String.Empty;
        public static String FileName = DefaultFileName;

#if DEBUG
        public static LogMessageSeverity MinimumEventFireSeverity = LogMessageSeverity.Normal;
#else
        public static LogMessageSeverity MinimumEventFireSeverity = LogMessageSeverity.High;
#endif
        public static event EventHandler<LogMessageEventArgs> MessageAdded;

        private static readonly List<LogMessage> History = new List<LogMessage>();

        #endregion Fields

        #region Properties

        public static String OutputFilePath
        {
            get { return Path.Combine(FilePath, FileName); }
        }

        #endregion Properties

        #region Methods

        private static Boolean CheckFreeSpace(String filePath, long length)
        {
            String pathRoot = Path.GetPathRoot(filePath);
            if (String.IsNullOrEmpty(pathRoot))
                return false;

            DriveInfo driveInfo = new DriveInfo(pathRoot);
            return (length < driveInfo.AvailableFreeSpace);
        }

        private static void MoveIfFileIsTooBig(String filePath)
        {
            FileInfo logFile = new FileInfo(filePath);
            if (logFile.Exists && (logFile.Length > MaximumFileSize))
                logFile.MoveTo(GetNextPossibleFileName(filePath));
        }

        private static String GetNextPossibleFileName(String filePath)
        {
            String newfilePath;
            int i = 0;
            do
            {
                i++;
                newfilePath = filePath + "_" + i;
            }
            while (File.Exists(newfilePath));
            return newfilePath;
        }

        private static void AddMessageToFile(LogMessage logMessage, String filePath)
        {
            if ((logMessage == null) || String.IsNullOrEmpty(filePath))
                return;

            try
            {
                String messageToOutput = logMessage.ToString();
                if (!CheckFreeSpace(filePath, messageToOutput.Length * sizeof(Char)))
                    return;

                MoveIfFileIsTooBig(filePath);
                File.AppendAllText(filePath, messageToOutput);
            }
            catch (Exception)
            {
                // Can't log such errors
            }
        }

        [Conditional("DEBUG")]
        public static void OutputToDebugConsole(LogMessage logMessage)
        {
            if (logMessage == null)
                return;

            if (Debugger.IsAttached && Debugger.IsLogging())
                Debugger.Log((int)logMessage.Severity, logMessage.Type.ToString(), logMessage.ToString());
        }

        public static void AddMessage(String message)
        {
            AddMessage(new LogMessage(message));
        }

        public static void AddMessage(String message, LogMessageType type)
        {
            AddMessage(new LogMessage(message, type));
        }

        public static void AddMessage(String message, LogMessageType type, LogMessageSeverity severity)
        {
            AddMessage(new LogMessage(message, type, severity));
        }

        public static void AddMessage(String message, LogMessageType type, Exception exception)
        {
            AddMessage(new LogMessage(message, type, exception));
        }

        public static void AddMessage(String message, LogMessageType type, LogMessageSeverity severity, Exception exception)
        {
            AddMessage(new LogMessage(message, type, severity, exception));
        }

        public static void AddMessage(LogMessage logMessage)
        {
            AddMessage(logMessage, true);
        }

        public static void AddMessage(LogMessage logMessage, Boolean fireError)
        {
            if (logMessage == null)
                return;

#if !DEBUG
            if (logMessage.Type == LogMessageType.Debug)
                return;
#endif
            try
            {
                lock (History)
                {
                    if (!History.Contains(logMessage))
                        History.Add(logMessage);

                    AddMessageToFile(logMessage, OutputFilePath);
                    OutputToDebugConsole(logMessage);
                }

                if (fireError && ((int)logMessage.Severity > (int)MinimumEventFireSeverity) && (MessageAdded != null))
                    MessageAdded(null, new LogMessageEventArgs(logMessage));
            }
            catch (Exception)
            {
                // Last defense line
            }
        }

        public static void ClearHistory()
        {
            History.Clear();
        }

        #endregion Methods
    }

    #region MessageEventArgs

    #endregion MessageEventArgs
}