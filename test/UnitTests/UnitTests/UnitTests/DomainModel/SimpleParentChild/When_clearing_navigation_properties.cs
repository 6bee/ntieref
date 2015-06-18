namespace UnitTests.DomainModel.SimpleParentChild
{
    using Test.Common.Domain.Model.SimpleParentChild;

    using Xunit;
    using Xunit.Should;

    public class When_assigning_navigation_properties
    {
        private readonly Parent parent;

        private readonly Child child;

        public When_assigning_navigation_properties()
        {
            parent = new Parent();
         
            child = new Child();
        }

        [Fact]
        public void Parent_should_contain_child_after_setting_childs_parent()
        {
            child.Parent = parent;

            parent.Children.ShouldContain(child);
        }

        [Fact]
        public void Child_should_have_parent_set_after_being_added_to_parents_child_collection()
        {
            parent.Children.Add(child);

            child.Parent.ShouldBeSameAs(parent);
        }
    }
}
