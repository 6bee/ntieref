// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Client.Domain
{
    public sealed class ExceptionEventArgs : EventArgs
    {
        public readonly Exception Exception;

        internal ExceptionEventArgs(Exception e)
        {
            this.Exception = e;
        }
    }
}