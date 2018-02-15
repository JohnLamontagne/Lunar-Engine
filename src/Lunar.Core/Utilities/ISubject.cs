/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

using System;

namespace Lunar.Core.Utilities
{
    public interface ISubject
    {
        event EventHandler<SubjectEventArgs> EventOccured;
    }
}