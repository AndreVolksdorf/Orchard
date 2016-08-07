using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;
using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Roles.Activities {
    [OrchardFeature("SmartPage.Community.Groups.Workflows")]
    public class UserTaskActivity : Event {
        private readonly IWorkContextAccessor _workContextAccessor;

        public UserTaskActivity(IWorkContextAccessor workContextAccessor) {
            _workContextAccessor = workContextAccessor;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override string Name {
            get { return "UserTask"; }
        }

        public override LocalizedString Category {
            get { return T("Tasks"); }
        }

        public override LocalizedString Description {
            get { return T("Wait for a user to execute a specific task.");  }
        }

        public override string Form {
            get { return "ActivityUserTask"; }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext) {
            return GetActions(activityContext).Select(action => T(action));
        }

        public override bool CanExecute(WorkflowContext workflowContext, ActivityContext activityContext) {
            return ActionIsValid(workflowContext, activityContext) && UserIsInGroup(activityContext);
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext) {

            if (ActionIsValid(workflowContext, activityContext) && UserIsInGroup(activityContext)) {
                yield return T(workflowContext.Tokens["UserTask.Action"].ToString());
            }
        }

        private bool UserIsInGroup(ActivityContext context) {

            // checking if user is in an accepted group
            var workContext = _workContextAccessor.GetContext();
            var user = workContext.CurrentUser;
            var groups = GetGroups(context).ToArray();

            if (!groups.Any()) {
                return true;
            }

            return UserIsInGroup(user, groups);
        }

        public static bool UserIsInGroup(IUser user, IEnumerable<string> groups) {
             bool isInGroup = false;
            
            if (user == null) {
                isInGroup = groups.Contains("Anonymous");
            }
            else {

                if (user.ContentItem.Has(typeof(GroupRolesPart))) {
                    isInGroup = user.ContentItem.As<GroupRolesPart>().Roles.Keys.Any(groups.Contains);
                }
            }

            return isInGroup;
        }

        private bool ActionIsValid(WorkflowContext workflowContext, ActivityContext activityContext) {
            
            // checking if user has triggered an accepted action

            // triggered action
            var userAction = workflowContext.Tokens["UserTask.Action"];

            var actions = GetActions(activityContext);
            bool isValidAction = actions.Contains(userAction);

            return isValidAction;    
        }

        private IEnumerable<string> GetGroups(ActivityContext context) {

            var groups = context.GetState<string>("Groups");

            if (string.IsNullOrEmpty(groups)) {
                return Enumerable.Empty<string>();
            }

            return groups.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
        }

        private IEnumerable<string> GetActions(ActivityContext context) {

            var actions = context.GetState<string>("Actions");

            if (String.IsNullOrEmpty(actions)) {
                return Enumerable.Empty<string>();
            }

            return actions.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            
        }
    }
}