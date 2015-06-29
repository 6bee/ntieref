namespace UnitTests.ClientDataContext.SimpleParentChildDataContext
{
    using Moq;
    
    using NTier.Common.Domain.Model;

    using System.Linq;

    using Test.Client.Domain;
    using Test.Common.Domain.Model.SimpleParentChild;
    using Test.Common.Domain.Service.Contracts;
    
    using Xunit;
    using Xunit.Should;

    public class When_loading_entities_with_include
    {
        private readonly Mock<ISimpleParentChildDataService> dataServiceMock;
        
        private readonly SimpleParentChildDataContext context;
        
        public When_loading_entities_with_include()
        {
            dataServiceMock = new Mock<ISimpleParentChildDataService>();
            
            context = new SimpleParentChildDataContext(() => dataServiceMock.Object);

            dataServiceMock.Setup(x => x.GetChildSet(null, It.IsAny<Query>()))
                .Returns<ClientInfo, Query>((ci, q) =>
                {
                    var child = new Child { Id = 20, Parent = new Parent { Id = 10 } };
                    child.AcceptChanges();
                    child.Parent.AcceptChanges();

                    return new QueryResult<Child> { Data = new[] { child } };
                });

            dataServiceMock.Setup(x => x.GetParentSet(null, It.IsAny<Query>()))
                .Returns<ClientInfo, Query>((ci, q) =>
                {
                    var parent = new Parent { Id = 10 };
                    parent.AcceptChanges();

                    return new QueryResult<Parent> { Data = new[] { parent } };
                });
        }

        [Fact]
        public void Query_result_should_be_substituted_with_local_instance()
        {
            var parent0 = context.ParentSet.AsQueryable().Single();

            var parent1 = context.ParentSet.AsQueryable().Single();

            parent0.ShouldBeSameAs(parent1);

            dataServiceMock.Verify(x => x.GetParentSet(null, It.IsAny<Query>()), Times.Exactly(2));
        }

        [Fact]
        public void Query_result_should_be_substituted_with_local_instance_loaded_via_include()
        {
            var client = context.ChildSet.AsQueryable().Include("Parent").Single();

            var parent0 = client.Parent;

            var parent1 = context.ParentSet.Single();

            var parent2 = context.ParentSet.AsQueryable().Single();

            parent0.ShouldBeSameAs(parent1);
            parent0.ShouldBeSameAs(parent2);

            dataServiceMock.Verify(x => x.GetChildSet(null, It.IsAny<Query>()), Times.Once);
            dataServiceMock.Verify(x => x.GetParentSet(null, It.IsAny<Query>()), Times.Once);
        }

        [Fact]
        public void Query_result_with_include_reference_should_be_substituted_with_local_instance()
        {
            var parent0 = context.ParentSet.AsQueryable().Single();

            var client = context.ChildSet.AsQueryable().Include("Parent").Single();

            var parent1 = client.Parent;

            parent0.ShouldBeSameAs(parent1);

            dataServiceMock.Verify(x => x.GetChildSet(null, It.IsAny<Query>()), Times.Once);
            dataServiceMock.Verify(x => x.GetParentSet(null, It.IsAny<Query>()), Times.Once);
        }

        [Fact]
        public void Query_result_with_include_collection_should_be_substituted_with_local_instance()
        {
            dataServiceMock.Setup(x => x.GetChildSet(null, It.IsAny<Query>()))
                .Returns<ClientInfo, Query>((ci, q) =>
                {
                    var child = new Child { Id = 20 };
                    child.AcceptChanges();

                    return new QueryResult<Child> { Data = new[] { child } };
                });

            dataServiceMock.Setup(x => x.GetParentSet(null, It.IsAny<Query>()))
                .Returns<ClientInfo, Query>((ci, q) =>
                {
                    var child = new Child { Id = 20, Parent = new Parent { Id = 10 } };
                    child.AcceptChanges();
                    child.Parent.AcceptChanges();

                    return new QueryResult<Parent> { Data = new[] { child.Parent } };
                });

            var child0 = context.ChildSet.AsQueryable().Single();

            var parent = context.ParentSet.AsQueryable().Include("Children").Single();

            var child1 = parent.Children.Single();

            child0.ShouldBeSameAs(child1);

            dataServiceMock.Verify(x => x.GetChildSet(null, It.IsAny<Query>()), Times.Once);
            dataServiceMock.Verify(x => x.GetParentSet(null, It.IsAny<Query>()), Times.Once);
        }
    }
}
