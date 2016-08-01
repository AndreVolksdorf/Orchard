using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Security;

namespace SmartPage.Community.Roles.Models {
    public static class GroupSimulation {

        public static IUser Create(string role)
        {
            var simulationType = new ContentTypeDefinitionBuilder().Named("Group").Build();
            var simulation = new ContentItemBuilder(simulationType)
                .Weld<SimulatedUser>()
                .Weld<SimulatedGroupRoles>()
                .Build();
            simulation.As<SimulatedGroupRoles>().Roles = new Dictionary<string, IEnumerable<string>> { { "SimulatedUserGroup", new[] { role } } };
            return simulation.As<IUser>();
        }

        public static IContent Create()
        {
            var simulationType = new ContentTypeDefinitionBuilder().Named("Group").Build();
            var simulation = new ContentItemBuilder(simulationType)
                .Weld<SimulatedGroupPart>()
                .Build();
            simulation.As<SimulatedGroupPart>().GroupName = "SimulatedUserGroup";
            return simulation.As<IContent>();
        }

        private class SimulatedUser : ContentPart, IUser
        {
            public string UserName{get { return null; }}
            public string Email{get { return null; }}
        }

        private class SimulatedGroupRoles : ContentPart, IGroupRoles
        {
            public IDictionary<string, IEnumerable<string>> Roles { get; set; }
        }

        internal class SimulatedGroupPart : ContentPart, IGroup
        {
            public string GroupName { get; set; }
        }
    }
}
