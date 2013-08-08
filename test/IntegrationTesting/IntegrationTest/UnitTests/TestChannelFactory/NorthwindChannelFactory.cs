using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using IntegrationTest.Common.Domain.Service.Contracts;
using NTier.Client.Domain.Service.ChannelFactory;

namespace TestChannelFactory
{
    [Export(typeof(IChannelFactory<INorthwindDataService>))]
    public class NorthwindChannelFactory : RemoteChannelFactory<INorthwindDataService>, IChannelFactory<INorthwindDataService>
    {
        public NorthwindChannelFactory()
            : base(null as string)
        {
            // configure wcf endpoint
            var endpoint = ChannelFactory.Endpoint;

            // binding
            endpoint.Binding = new WSHttpBinding(SecurityMode.Message, false)
            {
                MaxReceivedMessageSize = 67108864,
                MaxBufferPoolSize = 524288,
                ReceiveTimeout=TimeSpan.MaxValue, // for server debugging
                TransactionFlow = true,
            };

            // address
            endpoint.Address = new EndpointAddress("http://localhost:5000/NorthwindDataService.svc");
        }
    }
}
