namespace UnitTests.ClientDataContext.SimpleParentChildDataContext
{
    using Test.Client.Domain;
    using Test.Common.Domain.Model.SimpleParentChild;
    using Test.Common.Domain.Service.Contracts;

    using Xunit;
    using Xunit.Should;

    public class When_reverting_changes
    {
        private readonly SimpleParentChildDataContext context;

        private readonly Parent parent;

        public When_reverting_changes()
        {
            context = new SimpleParentChildDataContext(() => default(ISimpleParentChildDataService));

            parent = new Parent { Id = 1 };

            context.ParentSet.Attach(parent);

            context.AcceptChanges();

            var child1 = new Child { Id = 2 };
            var child2 = new Child { Id = 3 };

            context.ChildSet.Add(child1);
            context.ChildSet.Add(child2);

            parent.Children.Add(child1);
            parent.Children.Add(child2);
        }

        [Fact]
        public void Context_should_initially_have_changes()
        {
            context.HasChanges.ShouldBeTrue();
            
            context.ParentSet.Count.ShouldBe(1);
            context.ChildSet.Count.ShouldBe(2);

            parent.Children.Count.ShouldBe(2);
        }

        [Fact]
        public void Context_should_be_unchanged_after_reverting()
        {
            context.RevertChanges();

            context.HasChanges.ShouldBeFalse();

            parent.Children.ShouldBeEmpty();

            context.ParentSet.Count.ShouldBe(1);
            context.ChildSet.Count.ShouldBe(0);
        }
    }
}
