using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace SmartPage.Community.Groups.Models {
    public class GroupPostPartRecord : ContentPartRecord {
        public virtual int? RepliedOn { get; set; }

        [StringLengthMax]
        public virtual string Text { get; set; }

        public virtual string Format { get; set; }
    }
}