using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace SmartPage.Community.Roles.Activities {
    [OrchardFeature("SmartPage.Community.Roles.Workflows")]
    public class IsInGroupActivity : Task {
        private readonly IWorkContextAccessor _workContextAccessor;

        public IsInGroupActivity(IWorkContextAccessor workContextAccessor) {
            _workContextAccessor = workContextAccessor;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override string Name {
            get { return "IsInRole"; }
        }

        public override LocalizedString Category {
            get { return T("Conditions"); }
        }

        public override LocalizedString Description {
            get { return T("Whether the current user is in a specific role.");  }
        }

        public override string Form {
            get { return "SelectGroups"; }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext) {
            return new[] {T("Yes"), T("No")};
        }

        public override bool CanExecute(WorkflowContext workflowContext, ActivityContext activityContext) {
            return true;
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext) {

            if (UserIsInGroup(activityContext)) {
                yield return T("Yes");
            }
            else {
                yield return T("No");
            }
        }

        private bool UserIsInGroup(ActivityContext context) {

            // checking if user is in an accepted role
            var workContext = _workContextAccessor.GetContext();
            var user = workContext.CurrentUser;
            var roles = GetGroups(context);

            return UserIsInGroup(user, roles);
        }

        public static bool UserIsInGroup(IUser user, IEnumerable<string> groups) {
             bool isInRole = false;
            
            if (user == null) {
                isInRole = groups.Contains("Anonymous");
            }
            else {
                dynamic dynGroup = user.ContentItem;

                if (dynGroup.GroupRolesPart != null) {
                    IEnumerable<string> userRoles = dynGroup.GroupRolesPart.Roles;
                    isInRole = userRoles.Any(groups.Contains);
                }
            }

            return isInRole;
        }

        private IEnumerable<string> GetGroups(ActivityContext context) {
            string groups = context.GetState<string>("Groups");

            if (string.IsNullOrEmpty(groups)) {
                return Enumerable.Empty<string>();
            }

            return groups.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
        }
    }
}