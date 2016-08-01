using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using SmartPage.Community.Roles.Models;
using SmartPage.Community.Roles.Services;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace SmartPage.Community.Roles.Activities
{
    [OrchardFeature("SmartPage.Community.Roles.Workflows")]
    public class AssignRoleActivity : Task
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IRepository<GroupRolesPartRecord> _repository;
        private readonly IRoleService _roleService;
        private readonly IGroupService _groupService;

        public AssignRoleActivity(
            IWorkContextAccessor workContextAccessor,
            IRepository<GroupRolesPartRecord> repository,
            IRoleService roleService, IGroupService groupService)
        {
            _workContextAccessor = workContextAccessor;
            _repository = repository;
            _roleService = roleService;
            _groupService = groupService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public override string Name
        {
            get { return "AssignRole"; }
        }

        public override LocalizedString Category
        {
            get { return T("Group"); }
        }

        public override LocalizedString Description
        {
            get { return T("Assign specific roles to the current content item if it's a user."); }
        }

        public override string Form
        {
            get { return "SelectRolesGroups"; }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            return new[] { T("Done") };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            var user = workflowContext.Content.As<IGroupRoles>();

            // if the current workflow subject is not a user, use current user
            if (user == null)
            {
                user = _workContextAccessor.GetContext().CurrentUser.As<IGroupRoles>();
            }

            var roles = GetRoles(activityContext).ToArray();
            var groups = GetGroups(activityContext);

            if (user != null)
            {
                foreach (var group in groups)
                {
                    if (!user.Roles.ContainsKey(group))
                    {
                        var groupRecord = _groupService.GetGroupByName(group);
                        if (groupRecord != null)
                        {

                            foreach (var role in roles)
                            {
                                if (!user.Roles[group].Contains(role))
                                {
                                    var roleRecord = _roleService.GetRoleByName(role);
                                    if (roleRecord != null)
                                    {
                                        _repository.Create(new GroupRolesPartRecord { UserId = user.Id, RoleRecord = roleRecord, GroupRecord = groupRecord });
                                    }
                                    else
                                    {
                                        Logger.Debug("Role not found: {0}", role);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Logger.Debug("Group not found: {0}", group);
                        }
                    }
                }
            }

            yield return T("Done");
        }

        private IEnumerable<string> GetRoles(ActivityContext context)
        {
            var roles = context.GetState<string>("Roles");

            if (string.IsNullOrEmpty(roles))
            {
                return Enumerable.Empty<string>();
            }

            return roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
        }

        private IEnumerable<string> GetGroups(ActivityContext context)
        {
            var roles = context.GetState<string>("Groups");

            if (string.IsNullOrEmpty(roles))
            {
                return Enumerable.Empty<string>();
            }

            return roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
        }
    }
}