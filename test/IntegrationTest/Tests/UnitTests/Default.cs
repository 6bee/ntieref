using IntegrationTest.Client.Domain;
using IntegrationTest.Common.Domain.Service.Contracts;
using IntegrationTest.Server.Domain.Service;
using System;
using System.ServiceModel;

namespace UnitTests
{
    public static class Default
    {
        public readonly static Func<INorthwindDataService> ChannelFactory;
        public readonly static Func<INorthwindDataService> OfflineChannelFactory;


        public readonly static Func<INorthwindDataContext> DataContext;
        public readonly static Func<INorthwindDataContext> OfflineDataContext;



        

        static Default()
        {
            //ChannelFactory = () => new NorthwindDataService();
            ChannelFactory = () =>
                {
                    var binding = new WSHttpBinding(SecurityMode.Message, false)
                    {
                        MaxReceivedMessageSize = 67108864,
                        MaxBufferPoolSize = 524288,
                        ReceiveTimeout = TimeSpan.MaxValue, // for server debugging
                        TransactionFlow = true,
                    };
                    binding.ReaderQuotas.MaxArrayLength = int.MaxValue;

                    var address = new EndpointAddress("http://localhost:5000/NorthwindDataService.svc");

                    var channelFactory = new ChannelFactory<INorthwindDataService>(binding, address);
                    return channelFactory.CreateChannel();
                };

            OfflineChannelFactory = () =>
            {
                throw new InvalidOperationException("Connot create channel in offline mode.");
            };

            DataContext = () => new NorthwindDataContext(ChannelFactory);
            OfflineDataContext = () => new NorthwindDataContext(OfflineChannelFactory);
        }
    }
}
