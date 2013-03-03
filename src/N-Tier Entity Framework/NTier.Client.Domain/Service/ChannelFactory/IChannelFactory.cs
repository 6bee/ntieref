// Copyright (c) Trivadis. All rights reserved. See license.txt in the project root for license information.

namespace NTier.Client.Domain.Service.ChannelFactory
{
    public interface IChannelFactory<T>
    {
        T CreateChannel();
    }
}
