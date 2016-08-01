using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Common.Utilities;
using SmartPage.Community.Groups.Extensions;
using SmartPage.Community.Groups.Settings;
using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Groups.Models {
    public class GroupPart : ContentPart<GroupPartRecord>, IGroup
    {
        public string Title {
            get { return this.As<ITitleAspect>().Title; }
        }

        [MaxLength(256)]
        public string Description {
            get { return Record.Description; }
            set { Record.Description = value; }
        }

        public int DepartmentCount
        {
            get { return Record.DepartmentCount; }
            set { Record.DepartmentCount = value; }
        }
        
        public int PostCount {
            get { return Record.PostCount; }
            set { Record.PostCount = value; }
        }

        public bool DepartmentedPosts {
            get { return Record.DepartmentedPosts; }
            set { Record.DepartmentedPosts = value; }
        }

        public int Weight {
            get { return Record.Weight; }
            set { Record.Weight = value; }
        }

        public int ReplyCount {
            //get { return PostCount >= DepartmentCount ? PostCount - DepartmentCount : 0; }
            get { return PostCount; }
        }

        public string PostType {
            get {
                var type = Settings.GetModel<GroupPartSettings>().PostType;
                return !string.IsNullOrWhiteSpace(type) ? type : Constants.Parts.Post;
            }
        }

        public string DepartmentType {
            get {
                var type = Settings.GetModel<GroupPartSettings>().DepartmentType;
                return !string.IsNullOrWhiteSpace(type) ? type : Constants.Parts.Department;
            }
        }
        
        public GroupRecord Group
        {
            get { return Record.GroupRecord; }
            set { this.Record.GroupRecord = value; }
        }

        public int? GroupId { get; set; }

        public IEnumerable<GroupRecord> Groups { get; set; } 

        public string GroupName
        {
            get
            {
                return Group != null ? Group.Name : string.Empty;
            }
        }
    }
}