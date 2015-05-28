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

    public class When_loading_entities
    {
        private readonly Mock<ISimpleParentChildDataService> dataServiceMock;
        
        private readonly SimpleParentChildDataContext context;
        
        private readonly Parent parent;
        
        private readonly Child child;

        public When_loading_entities()
        {
            dataServiceMock = new Mock<ISimpleParentChildDataService>();
            
            context = new SimpleParentChildDataContext(() => dataServiceMock.Object);

            parent = new Parent();

            child = new Child { Parent = parent };
        }

        [Fact]
        public void Context_should_have_all_entities_attached_upon_loading_parent_entity()
        {
            dataServiceMock
                .Setup(x => x.GetParentSet(null, It.IsAny<Query>()))
                .Returns(new QueryResult<Parent>() { Data = new[] { parent } });

            context.ParentSet.AsQueryable().ToList();

            context.ParentSet.ShouldContain(parent);
            context.ChildSet.ShouldContain(child);
        }

        [Fact]
        public void Context_should_have_all_entities_attached_upon_loading_child_entities()
        {
            dataServiceMock
                .Setup(x => x.GetChildSet(null, It.IsAny<Query>()))
                .Returns(new QueryResult<Child>() { Data = new[] { child } });

            context.ChildSet.AsQueryable().ToList();

            context.ParentSet.ShouldContain(parent);
            context.ChildSet.ShouldContain(child);
        }
    }
}
