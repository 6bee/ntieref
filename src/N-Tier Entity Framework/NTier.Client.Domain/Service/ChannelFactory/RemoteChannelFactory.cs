// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using System.ServiceModel;

namespace NTier.Client.Domain.Service.ChannelFactory
{
    public abstract class RemoteChannelFactory<T> : IChannelFactory<T>
    {
        #region Fields and Properties

        private readonly ChannelFactory<T> _channelFactory;

        protected ChannelFactory<T> ChannelFactory
        {
            get { return _channelFactory; }
        }

        #endregion Fields and Properties

        #region Constructor

        protected RemoteChannelFactory(ChannelFactory<T> channelFactory)
        {
            this._channelFactory = channelFactory ?? new ChannelFactory<T>();
        }

        protected RemoteChannelFactory(string endpointConfigurationName)
        {
            this._channelFactory = string.IsNullOrEmpty(endpointConfigurationName) ? new ChannelFactory<T>() : new ChannelFactory<T>(endpointConfigurationName);
        }

        #endregion Constructor

        #region Factory Method

        public virtual T CreateChannel()
        {
            return _channelFactory.CreateChannel();
        }

        #endregion Factory Method
    }
}
