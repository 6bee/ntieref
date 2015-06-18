using IntegrationTest.Client.Domain;
using IntegrationTest.Common.Domain.Service.Contracts;
using IntegrationTest.Server.Domain.Repositories;
using IntegrationTest.Server.Domain.Service;
using NTier.Common.Domain.Model;
using System;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Reflection;

namespace UnitTests
{
    public static class Default
    {
        //public readonly static Func<INorthwindDataContext> WcfDataContext;

        public readonly static Func<INorthwindDataContext> DataContext;

        public readonly static Func<INorthwindDataContext> OfflineDataContext;

        static Default()
        {
            //Func<INorthwindDataService> channelFactoryProvider = () =>
            //    {
            //        var binding = new WSHttpBinding(SecurityMode.Message, false)
            //        {
            //            MaxReceivedMessageSize = 67108864,
            //            MaxBufferPoolSize = 524288,
            //            ReceiveTimeout = TimeSpan.MaxValue, // for server debugging
            //            TransactionFlow = true,
            //        };
            //        binding.ReaderQuotas.MaxArrayLength = int.MaxValue;

            //        var address = new EndpointAddress("http://localhost:5000/NorthwindDataService.svc");

            //        var channelFactory = new ChannelFactory<INorthwindDataService>(binding, address);
            //        return channelFactory.CreateChannel();
            //    };

            //WcfDataContext = () => new NorthwindDataContext(channelFactoryProvider);


            Func<INorthwindDataService> offlineChannelFactoryProvider = () =>
            {
                throw new InvalidOperationException("Connot create channel in offline mode.");
            };

            OfflineDataContext = () => new NorthwindDataContext(offlineChannelFactoryProvider);


            //var connectionString = "data source=.;initial catalog=NORTHWND;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
            var connectionString = @"data source=(LocalDB)\v11.0;attachdbfilename=|DataDirectory|\NORTHWND.MDF;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
            DbConnection dbConnection = new SqlConnection(connectionString);
            Assembly edmxAssembly = Assembly.LoadFrom("IntegrationTest.Server.Domain.Edmx.dll");
            MetadataWorkspace metadataWorkspace = new MetadataWorkspace(new[] { "res://*/" }, new[] { edmxAssembly });            
            EntityConnection entityConnection = new EntityConnection(metadataWorkspace, dbConnection);
            Func<ClientInfo, INorthwindRepository> defaultRepositoryProvider = clientInfo => new NorthwindRepository(entityConnection);
            Func<INorthwindDataService> dataServiceProvider = () => new NorthwindDataService(defaultRepositoryProvider);
            DataContext = () => new NorthwindDataContext(dataServiceProvider);
        }
    }
}
