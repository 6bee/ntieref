// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Client.Domain
{
    internal static class Deployment
    {
        public static System.Windows.Threading.Dispatcher Dispatcher
        {
            get
            {
                return System.Windows.Threading.Dispatcher.CurrentDispatcher;
            }
        }
    }
}
