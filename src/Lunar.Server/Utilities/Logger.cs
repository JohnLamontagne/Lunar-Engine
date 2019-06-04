/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
using System;
using System.IO;
using Lunar.Core;

namespace Lunar.Server.Utilities
{
    public static class Logger
    {
        private static string _previousConsoleLog;
        private static int _previousConsoleLogY;
        private static int _sameErrorCount;

        public static void LogEvent(string eventDetails, LogTypes logType, string stackTrace)
        {
            switch (logType)
            {
                case LogTypes.ERROR:
                    var newConsoleLog = $"Error: {eventDetails}";

                    // If our new console log is the same as the previous log entry, we'll just consolidate the entries and append a [n] at the end where n: the number of duplicate log entries.
                    if (newConsoleLog == _previousConsoleLog)
                    {
                        // We set this variable to -1 so that we can have the correct Y index of the console log to update. Otherwise any additional console activity would break the error logging consolidation feature.
                        if (_previousConsoleLogY == -1)
                        {
                            _previousConsoleLogY = Console.CursorTop - 1;
                        }

                        Console.SetCursorPosition(_previousConsoleLog.Length + 1, _previousConsoleLogY);
                        Console.WriteLine($"[{_sameErrorCount}]");
                        _sameErrorCount++;
                    }
                    else
                    {
                        _previousConsoleLogY = -1;
                        _sameErrorCount = 0;
                        Console.WriteLine(newConsoleLog);
                        _previousConsoleLog = newConsoleLog;
                    }

                    TextLog($"Error: {eventDetails}.", stackTrace, Constants.FILEPATH_LOGS + "Error.txt");
                    break;

                case LogTypes.GAME:
                    TextLog($"Game event: {eventDetails}", stackTrace, Constants.FILEPATH_LOGS + "Game_Event.txt");
                    break;

                case LogTypes.GEN_SERVER:
                    TextLog($"Event: {eventDetails}", stackTrace, Constants.FILEPATH_LOGS + "General_Server.txt");
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
