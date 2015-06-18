namespace UnitTests.ClientDataContext.SimpleParentChildDataContext
{
    using Test.Client.Domain;
    using Test.Common.Domain.Model.SimpleParentChild;
    using Test.Common.Domain.Service.Contracts;

    using Xunit;
    using Xunit.Should;

    public class When_detaching_entities
    {
        private readonly SimpleParentChildDataContext context;
        
        private readonly Parent parent;
        
        private readonly Child child;
        
        public When_detaching_entities()
        {
            context = new SimpleParentChildDataContext(() => default(ISimpleParentChildDataService));

            parent = new Parent();
            child = new Child { Parent = parent };
            
            context.Attach(parent);
        }

        [Fact]
        public void Context_should_have_entities_attached_before_detaching()
        {
            context.ParentSet.ShouldContain(parent);
            context.ChildSet.ShouldContain(child);
        }

        [Fact]
        public void Context_should_keep_entities_detached()
        {
            context.Detach(parent);
            context.Detach(child);

            context.ParentSet.ShouldBeEmpty();
            context.ChildSet.ShouldBeEmpty();
        }

        [Fact]
        public void Context_should_keep_entities_detached_upon_revert()
        {
            parent.Children.Clear();

            context.Detach(parent);
            context.Detach(child);


            child.RevertChanges();
            parent.RevertChanges();

            context.ParentSet.ShouldBeEmpty();
            context.ChildSet.ShouldBeEmpty();
        }

        [Fact]
        public void Context_should_keep_all_entities_detached_upon_revert()
        {
            parent.Children.Clear();

            context.ParentSet.DetachAll();
            context.ChildSet.DetachAll();


            child.RevertChanges();
            parent.RevertChanges();

            context.ParentSet.ShouldBeEmpty();
            context.ChildSet.ShouldBeEmpty();
        }
    }
}
