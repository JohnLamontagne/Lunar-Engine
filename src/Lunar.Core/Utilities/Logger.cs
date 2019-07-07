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
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using Lunar.Core;

namespace Lunar.Core.Utilities
{
    public class Logger : IService
    {
        private BackgroundWorker _loggerWorker;
        private bool _shuttingDown;
        private ConcurrentQueue<Tuple<string[], string>> _logQueue;

        private string _previousConsoleLog;
        private int _previousConsoleLogY;
        private int _sameErrorCount;

        public bool SuppressErrors { get; set; }

        public string LogPath { get; set; }

        public Logger()
        {
            this.SuppressErrors = true;
        }

        public void Initalize()
        {
        }

        public void LogEvent(string eventDetails, LogTypes logType, Exception exception = null)
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

                    TextLog($"Error: {eventDetails}.", exception.StackTrace, Engine.Services.Get<Logger>().LogPath + "Error.txt");

                    if (!Engine.Services.Get<Logger>().SuppressErrors)
                    {
                        if (exception.InnerException != null)
                            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                        else
                            throw exception;
                    }

                    break;

                case LogTypes.GAME:
                    TextLog($"Game event: {eventDetails}", exception?.StackTrace, Engine.Services.Get<Logger>().LogPath + "Game_Event.txt");
                    break;

                case LogTypes.GEN_SERVER:
                    TextLog($"Event: {eventDetails}", exception?.StackTrace, Engine.Services.Get<Logger>().LogPath + "General_Server.txt");
                    break;
            }
        }

        public void Start()
        {
            _logQueue = new ConcurrentQueue<Tuple<string[], string>>();
            _loggerWorker = new BackgroundWorker();
            _loggerWorker.DoWork += _loggerWorker_DoWork;

            var timer = new System.Timers.Timer(5000);
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_loggerWorker.IsBusy)
                _loggerWorker.RunWorkerAsync();
        }

        private void _loggerWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_logQueue.TryDequeue(out Tuple<string[], string> logs))
            {
                try
                {
                    using (var sw = File.AppendText(logs.Item2))
                    {
                        foreach (var line in logs.Item1)
                            sw.WriteLine(line);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Core error fault! Could not write to error log file.");
                    Console.WriteLine(ex);
                    Console.WriteLine("Original log: ");
                    Console.WriteLine(logs.Item1[2]);
                }
            }
        }

        public void Stop()
        {
            _shuttingDown = true;
        }

        private void TextLog(string logMessage, string stackTrace, string filePath)
        {
            _logQueue.Enqueue(new Tuple<string[], string>(new string[]
            {
                "\r\nLog Entry: ",
                "{0} {1}:", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(),
                "{0}", logMessage,
                stackTrace,
                new string('-', logMessage.Length)
            }, filePath));
        }
    }

    public enum LogTypes
    {
        GAME,
        GEN_SERVER,
        ERROR
    }
}