using Orchard.ContentManagement.Records;

namespace SmartPage.Contact.Models
{
    public class ContactRecord : ContentPartRecord
    {
        public virtual string Phone { get; set; }
        public virtual string Website { get; set; }
        public virtual string Email { get; set; }
    }
}
