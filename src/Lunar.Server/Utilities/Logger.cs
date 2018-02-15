using System;
using System.IO;

namespace Lunar.Server.Utilities
{
    public static class Logger
    {
        public static void LogEvent(string eventDetails, LogTypes logType)
        {
            switch (logType)
            {
                case LogTypes.ERROR:
                    Console.WriteLine($"Error: {eventDetails}.");
                    TextLog($"Error: {eventDetails}.", Constants.FILEPATH_LOGS + "Error.txt");
                    break;

                case LogTypes.GAME:
                    TextLog($"Game event: {eventDetails}.", Constants.FILEPATH_LOGS + "Game_Event.txt");
                    break;

                case LogTypes.GEN_SERVER:
                    TextLog($"Event: {eventDetails}.", Constants.FILEPATH_LOGS + "General_Server.txt");
                    break;
            }
        }

        private static void TextLog(string logMessage, string filePath)
        {
            using (var sw = File.AppendText(filePath))
            {
                sw.Write("\r\nLog Entry: ");
                sw.WriteLine("{0} {1}:", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                sw.WriteLine("{0}", logMessage);
                sw.WriteLine(new string('-', logMessage.Length));
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
