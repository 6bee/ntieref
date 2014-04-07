// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;

namespace NTier.Common.Domain.Model
{
    public sealed class ObjectStateChangingEventArgs : EventArgs
    {
        public readonly ObjectState NewState;

        public ObjectStateChangingEventArgs(ObjectState newState)
        {
            this.NewState = newState;
        }
    }
}
