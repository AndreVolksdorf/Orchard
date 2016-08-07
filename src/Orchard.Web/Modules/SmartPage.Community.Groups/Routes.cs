using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;
using SmartPage.Community.Groups.Extensions;

namespace SmartPage.Community.Groups {
    public class Routes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Groups/Create/{type}",
                                                         new RouteValueDictionary {
                                                                                      {"area", Constants.LocalArea},
                                                                                      {"controller", "GroupAdmin"},
                                                                                      {"action", "Create"},
                                                                                      {"type", UrlParameter.Optional}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", Constants.LocalArea}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Groups",
                                                         new RouteValueDictionary {
                                                                                      {"area", Constants.LocalArea},
                                                                                      {"controller", "GroupAdmin"},
                                                                                      {"action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", Constants.LocalArea}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Groups/{groupId}",
                                                         new RouteValueDictionary {
                                                                                      {"area", Constants.LocalArea},
                                                                                      {"controller", "DepartmentAdmin"},
                                                                                      {"action", "List"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", Constants.LocalArea}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Groups/{groupId}/Departments/{departmentId}",
                                                         new RouteValueDictionary {
                                                                                      {"area", Constants.LocalArea},
                                                                                      {"controller", "DepartmentAdmin"},
                                                                                      {"action", "Item"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", Constants.LocalArea}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Groups/Create/{type}",
                                                         new RouteValueDictionary {
                                                                                      {"area", Constants.LocalArea},
                                                                                      {"controller", "Group"},
                                                                                      {"action", "Create"},
                                                                                      {"type", UrlParameter.Optional}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", Constants.LocalArea}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Groups",
                                                         new RouteValueDictionary {
                                                                                      {"area", Constants.LocalArea},
                                                                                      {"controller", "Group"},
                                                                                      {"action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", Constants.LocalArea}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                         };
        }
    }
}