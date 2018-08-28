using System;
using System.IO;
using System.Threading;

namespace Lunar.Core.Utilities
{
    public static class Logger
    {
        public static void LogEvent(string eventDetails, LogTypes logType, string stackTrace)
        {
            switch (logType)
            {
                case LogTypes.ERROR:
                    Console.WriteLine($"Error: {eventDetails}");
                    TextLog($"Error: {eventDetails}.", stackTrace, EngineConstants.FILEPATH_LOGS + "Error.txt");
                    break;

                case LogTypes.GAME:
                    TextLog($"Game event: {eventDetails}", stackTrace, EngineConstants.FILEPATH_LOGS + "Game_Event.txt");
                    break;

                case LogTypes.GEN_SERVER:
                    TextLog($"Event: {eventDetails}", stackTrace, EngineConstants.FILEPATH_LOGS + "General_Server.txt");
                    break;
            }
            
        }

        private static void TextLog(string logMessage, string stackTrace, string filePath)
        {
            try
            {
                using (var sw = File.AppendText(filePath))
                {
                    sw.Write("\r\nLog Entry: ");
                    sw.WriteLine("{0} {1}:", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    sw.WriteLine("{0}", logMessage);
                    sw.WriteLine(stackTrace);

                    sw.WriteLine(new string('-', logMessage.Length));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Core error fault! Could not write to error log file.");
                Console.WriteLine(ex);
                Console.WriteLine("Original Error: ");
                Console.WriteLine(logMessage);
            }
        }
    }

    public enum LogTypes
    {
        GAME,
        GEN_SERVER,
        ERROR
    }
}
