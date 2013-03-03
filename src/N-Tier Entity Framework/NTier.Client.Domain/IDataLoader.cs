// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.ComponentModel;
using NTier.Common.Domain.Model;

namespace NTier.Client.Domain
{
    public interface IDataLoader<T> where T : Entity
    {
        void Load(IEntityCollectionView<T> view);

        event EventHandler DataLoading;
        event EventHandler<DataLoadedEventArgs<T>> DataLoaded;
    }

    public class DataLoadedEventArgs<T> : AsyncCompletedEventArgs where T : Entity
    {
        public readonly IEntitySet<T> Result;

        public DataLoadedEventArgs(IEntitySet<T> result)
            : this(null, false, null, result)
        {
        }

        public DataLoadedEventArgs(Exception error, bool cancelled, object userState, IEntitySet<T> result)
            : base(error, cancelled, userState)
        {
            this.Result = result;
        }
    }
}
