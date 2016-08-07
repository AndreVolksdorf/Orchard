using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace SmartPage.Contact.Models
{
    public class ContactPart : ContentPart<ContactRecord>
    {

        [DataType(DataType.PhoneNumber)]
        public string Phone
        {
            get { return Retrieve(r => r.Phone); }
            set { Store(r => r.Phone, value); }
        }

        [DataType(DataType.Url)]
        public string Website
        {
            get { return Retrieve(r => r.Website); }
            set { Store(r => r.Website, value); }
        }

        [DataType(DataType.EmailAddress)]
        public string Email
        {
            get { return Retrieve(r => r.Email); }
            set { Store(r => r.Email, value); }
        }

    }
}