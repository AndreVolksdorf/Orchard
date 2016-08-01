using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;

namespace SmartPage.Community.Roles.Models
{
    public interface IGroup : IContent
    {
        string GroupName { get; }
    }
}
