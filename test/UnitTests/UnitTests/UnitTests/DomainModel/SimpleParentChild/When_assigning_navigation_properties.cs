namespace UnitTests.DomainModel.SimpleParentChild
{
    using Test.Common.Domain.Model.SimpleParentChild;

    using Xunit;
    using Xunit.Should;

    public class When_clearing_navigation_properties
    {
        private readonly Parent parent;

        private readonly Child child;

        public When_clearing_navigation_properties()
        {
            parent = new Parent();

            child = new Child() { Parent = parent };
        }

        [Fact]
        public void Parent_should_not_contain_child_after_clearing_childs_parent()
        {
            child.Parent = null;

            parent.Children.ShouldBeEmpty();
        }

        [Fact]
        public void Child_should_have_parent_cleared_after_being_removed_from_parents_child_collection()
        {
            parent.Children.Remove(child);

            child.Parent.ShouldBeNull();
        }

        [Fact]
        public void Child_should_have_parent_cleared_after_clearing_parents_child_collection()
        {
            parent.Children.Clear();

            child.Parent.ShouldBeNull();
        }
    }
}
