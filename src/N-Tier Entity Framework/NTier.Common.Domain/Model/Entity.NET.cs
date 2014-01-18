// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

namespace NTier.Common.Domain.Model
{
    partial class Entity
    {
        #region cloning

        internal Entity DeepCopy()
        {
            return this.Clone();
        }

        internal Entity ShallowCopy()
        {
            var clone = (Entity)MemberwiseClone();
            var changeTracker = _changeTracker;
            clone._changeTracker = changeTracker == null ? null : changeTracker.ShallowCopy();
            return clone;
        }

        #endregion cloning
    }
}
