using System;
using IntegrationTest.Common.Domain.Service.Contracts;
using NTier.Client.Domain.Service.ChannelFactory;

namespace TestChannelFactory
{
    public sealed class OfflineChannelFactory : IChannelFactory<INorthwindDataService>
    {
        INorthwindDataService IChannelFactory<INorthwindDataService>.CreateChannel()
        {
            throw new InvalidOperationException("Connot create channel in offline mode.");
        }
    }
}
