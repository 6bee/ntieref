using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using IntegrationTest.Client.Domain;
using NTier.Client.Domain;
using IntegrationTest.Common.Domain.Model.Northwind;
using NTier.Common.Domain.Model;
using UnitTests.Utils;
using NTier.Client.Domain.Service.ChannelFactory;

namespace UnitTests
{
    [TestFixture]
    public class AcceptRevertTests : AssertionHelper
    {
        private static class RegionId
        {
            public const int Eastern = 1;
            public const int Western = 2;
            public const int Northern = 3;
            public const int Southern = 4;
        }

        private static class TerritoryId
        {
            public const string Boston = "02116";
            public const string Cambridge = "02139";
            public const string Braintree = "02184";
            public const string Hollis = "03049";
        }

        private INorthwindDataContext Context = null;

        [SetUp]
        public void Init()
        {
            Context = Default.OfflineDataContext();

            // load regions
            var east = new Region { RegionID = RegionId.Eastern, RegionDescription = "Eastern" };
            var north = new Region { RegionID = RegionId.Northern, RegionDescription = "Northern" };
            var south = new Region { RegionID = RegionId.Southern, RegionDescription = "Southern" };
            var west = new Region { RegionID = RegionId.Western, RegionDescription = "Western" };

            Context.Regions.Add(east);
            Context.Regions.Add(north);
            Context.Regions.Add(south);
            Context.Regions.Add(west);


            // load territories
            var boston = new Territory { TerritoryID = TerritoryId.Boston, /*RegionID = east.RegionID,*/ Region=east, TerritoryDescription = "Boston" };
            Context.Territories.Add(boston);

            Context.AcceptChanges();
        }

        [TearDown]
        public void CleanUp()
        {
            Context = null;
        }

        [Test]
        public void InitialDataAndStateIsValid()
        {
            Expect(Context.HasChanges, Is.False);
            Expect(Context.Regions.HasChanges, Is.False);
            Expect(Context.Territories.HasChanges, Is.False);

            var regions = Context.Regions.Select(r => r.RegionID);
            Expect(regions, To.Contain(RegionId.Eastern));
            Expect(regions, To.Contain(RegionId.Northern));
            Expect(regions, To.Contain(RegionId.Southern));
            Expect(regions, To.Contain(RegionId.Western));
        }

        [Test]
        public void RevertModifiedProperty()
        {
            var boston = Context.Territories.Single(t => t.TerritoryID == TerritoryId.Boston);
            Expect(boston.HasChanges, Is.False);

            var territoryName = boston.TerritoryDescription;
            var modifiedTerritoryName = new string('A', territoryName.Length);

            Expect(boston.TerritoryDescription, Is.EqualTo(territoryName));

            boston.TerritoryDescription = modifiedTerritoryName;
            Expect(boston.TerritoryDescription, Is.EqualTo(modifiedTerritoryName));
            Expect(boston.HasChanges, Is.True);
            Expect(Context.HasChanges, Is.True);
            Expect(Context.Territories.HasChanges, Is.True);

            boston.RevertChanges("TerritoryDescription");
            Expect(boston.TerritoryDescription, Is.EqualTo(territoryName));
            Expect(boston.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);
            Expect(Context.Territories.HasChanges, Is.False);

            boston.TerritoryDescription = modifiedTerritoryName;
            Expect(boston.TerritoryDescription, Is.EqualTo(modifiedTerritoryName));
            Expect(boston.HasChanges, Is.True);
            Expect(Context.HasChanges, Is.True);
            Expect(Context.Territories.HasChanges, Is.True);

            boston.RevertChanges();
            Expect(boston.TerritoryDescription, Is.EqualTo(territoryName));
            Expect(boston.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);
            Expect(Context.Territories.HasChanges, Is.False);

            boston.TerritoryDescription = modifiedTerritoryName;
            Expect(boston.TerritoryDescription, Is.EqualTo(modifiedTerritoryName));
            Expect(boston.HasChanges, Is.True);
            Expect(Context.HasChanges, Is.True);
            Expect(Context.Territories.HasChanges, Is.True);

            Context.Territories.RevertChanges();
            Expect(boston.TerritoryDescription, Is.EqualTo(territoryName));
            Expect(boston.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);
            Expect(Context.Territories.HasChanges, Is.False);

            boston.TerritoryDescription = modifiedTerritoryName;
            Expect(boston.TerritoryDescription, Is.EqualTo(modifiedTerritoryName));
            Expect(boston.HasChanges, Is.True);
            Expect(Context.HasChanges, Is.True);
            Expect(Context.Territories.HasChanges, Is.True);

            Context.RevertChanges();
            Expect(boston.TerritoryDescription, Is.EqualTo(territoryName));
            Expect(boston.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);
            Expect(Context.Territories.HasChanges, Is.False);
        }

        [Test]
        public void AcceptModifiedProperty()
        {
            var boston = Context.Territories.Single(t => t.TerritoryID == TerritoryId.Boston);
            Expect(boston.HasChanges, Is.False);

            var territoryName = boston.TerritoryDescription;
            var modifiedTerritoryName = new string('A', territoryName.Length);

            Expect(boston.TerritoryDescription, Is.EqualTo(territoryName));

            boston.TerritoryDescription = modifiedTerritoryName;
            Expect(boston.TerritoryDescription, Is.EqualTo(modifiedTerritoryName));
            Expect(boston.HasChanges, Is.True);
            Expect(Context.HasChanges, Is.True);
            Expect(Context.Territories.HasChanges, Is.True);

            boston.AcceptChanges();
            Expect(boston.TerritoryDescription, Is.EqualTo(modifiedTerritoryName));
            Expect(boston.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);
            Expect(Context.Territories.HasChanges, Is.False);

            modifiedTerritoryName = new string('B', territoryName.Length);
            boston.TerritoryDescription = modifiedTerritoryName;
            Expect(boston.TerritoryDescription, Is.EqualTo(modifiedTerritoryName));
            Expect(boston.HasChanges, Is.True);
            Expect(Context.HasChanges, Is.True);
            Expect(Context.Territories.HasChanges, Is.True);

            Context.Territories.AcceptChanges();
            Expect(boston.TerritoryDescription, Is.EqualTo(modifiedTerritoryName));
            Expect(boston.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);
            Expect(Context.Territories.HasChanges, Is.False);

            modifiedTerritoryName = new string('C', territoryName.Length);
            boston.TerritoryDescription = modifiedTerritoryName;
            Expect(boston.TerritoryDescription, Is.EqualTo(modifiedTerritoryName));
            Expect(boston.HasChanges, Is.True);
            Expect(Context.HasChanges, Is.True);
            Expect(Context.Territories.HasChanges, Is.True);

            Context.AcceptChanges();
            Expect(boston.TerritoryDescription, Is.EqualTo(modifiedTerritoryName));
            Expect(boston.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);
            Expect(Context.Territories.HasChanges, Is.False);
        }

        [Test]
        public void RevertModifiedNavigationProperty()
        {
            var boston = Context.Territories.Single(t => t.TerritoryID == TerritoryId.Boston);
            var eastern = boston.Region;
            var western = Context.Regions.Single(r => r.RegionID == RegionId.Western);
            Expect(boston.HasChanges, Is.False);
            Expect(western.HasChanges, Is.False);
            Expect(eastern.HasChanges, Is.False);
            Expect(eastern.RegionID, Is.EqualTo(RegionId.Eastern));

            boston.Region = western;
            Expect(boston.HasChanges, Is.True);
            Expect(western.HasChanges, Is.True);
            Expect(eastern.HasChanges, Is.True);
            Expect(boston.RegionID, Is.EqualTo(RegionId.Western));

            boston.RevertChanges();
            Expect(boston.HasChanges, Is.False);
            Expect(western.HasChanges, Is.False);
            Expect(eastern.HasChanges, Is.False);
            Expect(boston.RegionID, Is.EqualTo(RegionId.Eastern));

            boston.Region = western;
            Expect(boston.HasChanges, Is.True);
            Expect(western.HasChanges, Is.True);
            Expect(eastern.HasChanges, Is.True);
            Expect(boston.RegionID, Is.EqualTo(RegionId.Western));

            Context.Territories.RevertChanges();
            Expect(boston.HasChanges, Is.False);
            Expect(western.HasChanges, Is.False);
            Expect(eastern.HasChanges, Is.False);
            Expect(boston.RegionID, Is.EqualTo(RegionId.Eastern));

            boston.Region = western;
            Expect(boston.HasChanges, Is.True);
            Expect(western.HasChanges, Is.True);
            Expect(eastern.HasChanges, Is.True);
            Expect(boston.RegionID, Is.EqualTo(RegionId.Western));

            Context.RevertChanges();
            Expect(boston.HasChanges, Is.False);
            Expect(western.HasChanges, Is.False);
            Expect(eastern.HasChanges, Is.False);
            Expect(boston.RegionID, Is.EqualTo(RegionId.Eastern));
        }

        [Test]
        public void AcceptModifiedNavigationProperty()
        {
            var boston = Context.Territories.Single(t => t.TerritoryID == TerritoryId.Boston);
            var eastern = boston.Region;
            var western = Context.Regions.Single(r => r.RegionID == RegionId.Western);
            var southern = Context.Regions.Single(r => r.RegionID == RegionId.Southern);
            var northern = Context.Regions.Single(r => r.RegionID == RegionId.Northern);
            Expect(Context.HasChanges, Is.False);
            Expect(boston.HasChanges, Is.False);
            Expect(northern.HasChanges, Is.False);
            Expect(southern.HasChanges, Is.False);
            Expect(western.HasChanges, Is.False);
            Expect(eastern.HasChanges, Is.False);
            Expect(eastern.RegionID, Is.EqualTo(RegionId.Eastern));

            boston.Region = western;
            Expect(Context.HasChanges, Is.True);
            Expect(boston.HasChanges, Is.True);
            Expect(northern.HasChanges, Is.False);
            Expect(southern.HasChanges, Is.False);
            Expect(western.HasChanges, Is.True);
            Expect(eastern.HasChanges, Is.True);
            Expect(boston.RegionID, Is.EqualTo(RegionId.Western));

            boston.AcceptChanges();
            Expect(Context.HasChanges, Is.True);
            Expect(boston.HasChanges, Is.False);
            Expect(northern.HasChanges, Is.False);
            Expect(southern.HasChanges, Is.False);
            Expect(western.HasChanges, Is.True);
            Expect(eastern.HasChanges, Is.True);
            Expect(boston.RegionID, Is.EqualTo(RegionId.Western));

            Context.AcceptChanges();
            Expect(Context.HasChanges, Is.False);
            Expect(eastern.HasChanges, Is.False);
            Expect(western.HasChanges, Is.False);

            boston.Region = northern;
            Expect(Context.HasChanges, Is.True);
            Expect(boston.HasChanges, Is.True);
            Expect(northern.HasChanges, Is.True);
            Expect(southern.HasChanges, Is.False);
            Expect(western.HasChanges, Is.True);
            Expect(eastern.HasChanges, Is.False);
            Expect(boston.RegionID, Is.EqualTo(RegionId.Northern));

            Context.Territories.AcceptChanges();
            Expect(Context.HasChanges, Is.True);
            Expect(boston.HasChanges, Is.False);
            Expect(northern.HasChanges, Is.True);
            Expect(southern.HasChanges, Is.False);
            Expect(western.HasChanges, Is.True);
            Expect(eastern.HasChanges, Is.False);
            Expect(boston.RegionID, Is.EqualTo(RegionId.Northern));

            Context.AcceptChanges();
            Expect(Context.HasChanges, Is.False);
            Expect(northern.HasChanges, Is.False);
            Expect(western.HasChanges, Is.False);

            boston.Region = southern;
            Expect(Context.HasChanges, Is.True);
            Expect(boston.HasChanges, Is.True);
            Expect(northern.HasChanges, Is.True);
            Expect(southern.HasChanges, Is.True);
            Expect(western.HasChanges, Is.False);
            Expect(eastern.HasChanges, Is.False);
            Expect(boston.RegionID, Is.EqualTo(RegionId.Southern));

            Context.AcceptChanges();
            Expect(Context.HasChanges, Is.False);
            Expect(boston.HasChanges, Is.False);
            Expect(northern.HasChanges, Is.False);
            Expect(southern.HasChanges, Is.False);
            Expect(western.HasChanges, Is.False);
            Expect(eastern.HasChanges, Is.False);
            Expect(boston.RegionID, Is.EqualTo(RegionId.Southern));
        }

        [Test]
        public void RevertAdditionAndRemoalToContext()
        {
            Expect(Context.Territories.Count, Is.EqualTo(1));
            Expect(Context.Territories.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);

            Context.Territories.Add(new Territory());
            Expect(Context.Territories.Count, Is.EqualTo(2));
            Expect(Context.Territories.HasChanges, Is.True);
            Expect(Context.HasChanges, Is.True);

            Context.Territories.RevertChanges();
            Expect(Context.Territories.Count, Is.EqualTo(1));
            Expect(Context.Territories.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);

            Context.Territories.Add(new Territory());
            Expect(Context.Territories.Count, Is.EqualTo(2));
            Expect(Context.Territories.HasChanges, Is.True);
            Expect(Context.HasChanges, Is.True);

            Context.RevertChanges();
            Expect(Context.Territories.Count, Is.EqualTo(1));
            Expect(Context.Territories.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);

            var boston = Context.Territories.First();
            Context.Detach(boston);
            Expect(Context.Territories.Count, Is.EqualTo(0));
            Expect(Context.Territories.HasChanges, Is.True);
            Expect(Context.HasChanges, Is.True);

            Context.Territories.RevertChanges();
            Expect(Context.Territories.Count, Is.EqualTo(1));
            Expect(Context.Territories.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);

            Context.Detach(boston);
            Expect(Context.Territories.Count, Is.EqualTo(0));
            Expect(Context.Territories.HasChanges, Is.True);
            Expect(Context.HasChanges, Is.True);

            Context.RevertChanges();
            Expect(Context.Territories.Count, Is.EqualTo(1));
            Expect(Context.Territories.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);
        }

        [Test]
        public void AcceptAdditionAndRemoalToContext()
        {
            Expect(Context.Territories.Count, Is.EqualTo(1));
            Expect(Context.Territories.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);

            Context.Territories.Add(new Territory());
            Expect(Context.Territories.Count, Is.EqualTo(2));
            Expect(Context.Territories.HasChanges, Is.True);
            Expect(Context.HasChanges, Is.True);

            Context.Territories.AcceptChanges();
            Expect(Context.Territories.Count, Is.EqualTo(2));
            Expect(Context.Territories.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);

            Context.Territories.Attach(new Territory());
            Expect(Context.Territories.Count, Is.EqualTo(3));
            Expect(Context.Territories.HasChanges, Is.True);
            Expect(Context.HasChanges, Is.True);

            Context.AcceptChanges();
            Expect(Context.Territories.Count, Is.EqualTo(3));
            Expect(Context.Territories.HasChanges, Is.False);
            Expect(Context.HasChanges, Is.False);
        }
    }
}
