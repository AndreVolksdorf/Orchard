using System;
using System.Linq;
using System.Web.Mvc;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using SmartPage.Community.Roles.Services;

namespace SmartPage.Community.Roles.Forms {
    public class SelectRolesGroups : IFormProvider {
        private readonly IGroupService _groupService;
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public SelectRolesGroups(
            IShapeFactory shapeFactory,
            IGroupService groupService) {
            _groupService = groupService;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, dynamic> form =
                shape => {

                    //TODO: Add role select list
                    var f = Shape.Form(
                        Id: "AnyOfGroups",
                        _Parts: Shape.SelectList(
                            Id: "group", Name: "Groups",
                            Title: T("Groups"),
                            Description: T("Select some groups."),
                            Size: 10,
                            Multiple: true
                            )
                        );

                    f._Parts.Add(new SelectListItem { Value = "", Text = T("Any").Text });

                    foreach (var group in _groupService.GetGroups().OrderBy(x => x.Name)) {
                        f._Parts.Add(new SelectListItem { Value = group.Name, Text = group.Name });
                    }

                    return f;
                };

            context.Form("SelectGroups", form);

        }
    }
}