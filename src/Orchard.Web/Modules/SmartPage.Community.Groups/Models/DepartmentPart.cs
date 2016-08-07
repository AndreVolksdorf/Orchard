using System;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Common.Utilities;
using Orchard.Security;
using SmartPage.Community.Roles.Models;

namespace SmartPage.Community.Groups.Models
{
    public class DepartmentPart : ContentPart<DepartmentPartRecord>, IDepartmentPart, IGroup
    {
        private readonly LazyField<IUser> _closedBy = new LazyField<IUser>();
        private readonly LazyField<GroupPostPart> _firstPost = new LazyField<GroupPostPart>();
        private readonly LazyField<GroupPostPart> _latestPost = new LazyField<GroupPostPart>();

        public LazyField<IUser> ClosedByField { get { return _closedBy; } }
        public LazyField<GroupPostPart> FirstPostField { get { return _firstPost; } }
        public LazyField<GroupPostPart> LatestPostField { get { return _latestPost; } }

        public string GroupName
        {
            get { return this.GroupPart != null ? GroupPart.GroupName : string.Empty; }
        }

        public string Title
        {
            get { return this.As<ITitleAspect>().Title; }
        }

        [MaxLength(256)]
        public string Description
        {
            get { return Record.Description; }
            set { Record.Description = value; }
        }

        public GroupPart GroupPart
        {
            get { return this.As<ICommonPart>().Container.As<GroupPart>(); }
            set { this.As<ICommonPart>().Container = value; }
        }

        public int PostCount
        {
            get { return Record.PostCount; }
            set { Record.PostCount = value; }
        }

        public bool IsSticky
        {
            get { return Record.IsSticky; }
            set { Record.IsSticky = value; }
        }

        public DateTime? ClosedOnUtc
        {
            get { return Record.ClosedOnUtc; }
            set { Record.ClosedOnUtc = value; }
        }

        public IUser ClosedBy
        {
            get { return _closedBy.Value; }
            set { _closedBy.Value = value; }
        }

        public GroupPostPart FirstPost
        {
            get { return _firstPost.Value; }
            set { _firstPost.Value = value; }
        }

        public GroupPostPart LatestPost
        {
            get { return _latestPost.Value; }
            set { _latestPost.Value = value; }
        }

        public string ClosedDescription
        {
            get { return Record.ClosedDescription; }
            set { Record.ClosedDescription = value; }
        }

        public int ReplyCount
        {
            get { return PostCount; }
        }

        public bool IsClosed
        {
            get { return ClosedOnUtc != null; }
        }
    }
}