// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace NTier.Client.Domain
{
    [Serializable]
    partial class UpdateException
    {
        protected UpdateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            StateEntries = (ReadOnlyCollection<StateEntry>)info.GetValue("StateEntries", typeof(ReadOnlyCollection<StateEntry>));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StateEntries", StateEntries);
        }
    }
}
