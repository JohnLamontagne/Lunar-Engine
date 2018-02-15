/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

using System;

namespace Lunar.Core.Utilities
{
    public class SubjectEventArgs : EventArgs
    {
        private readonly string _eventName;
        private readonly object[] _args;

        public string EventName => _eventName;

        public object[] Args => _args;

        public SubjectEventArgs(string eventName, params object[] args)
        {
            _eventName = eventName;
            _args = args;
        }
    }
}