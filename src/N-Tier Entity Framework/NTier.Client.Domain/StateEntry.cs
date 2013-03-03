// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public sealed class StateEntry
    {
        public readonly Entity StoreValue;
        public readonly Entity Entity;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public StateEntry(Entity entity, Entity storeValue)
        {
            this.Entity = entity;
            this.StoreValue = storeValue;
        }
    }
}
