using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data;
using Orchard.Data.Migration;
using Orchard.Localization;
using Orchard.Projections.Models;
using SmartPage.Contact.Models;

namespace SmartPage.Contact
{
    public class Migrations : DataMigrationImpl
    {
        public Localizer T { get; set; }

        private readonly IRepository<MemberBindingRecord> _memberBindingRepository;

        public Migrations(IRepository<MemberBindingRecord> memberBindingRepository)
        {
            _memberBindingRepository = memberBindingRepository;
            T = NullLocalizer.Instance;
        }

        public int Create()
        {
            // Creating table MapRecord
            SchemaBuilder.CreateTable("ContactRecord", table => table
                .ContentPartRecord()
                .Column("Phone", DbType.String)
                .Column("Website", DbType.String)
                .Column("Email", DbType.String)
            );

            ContentDefinitionManager.AlterPartDefinition(typeof(ContactPart).Name, part => part
                .WithDescription(T("Add fields to enter contact information").Text)
                .Attachable()
                );

            return 1;
        }

        public int UpdateFrom1() {

            _memberBindingRepository.Create(new MemberBindingRecord
            {
                Type = typeof(ContactRecord).FullName,
                Member = "Phone",
                DisplayName = T("Phone number").Text,
                Description = T("The given phone number").Text
            });
            _memberBindingRepository.Create(new MemberBindingRecord
            {
                Type = typeof(ContactRecord).FullName,
                Member = "Email",
                DisplayName = T("Email").Text,
                Description = T("The given email address").Text
            });
            _memberBindingRepository.Create(new MemberBindingRecord
            {
                Type = typeof(ContactRecord).FullName,
                Member = "Website",
                DisplayName = T("Website").Text,
                Description = T("The given url").Text
            });

            return 2;
        }
    }
}