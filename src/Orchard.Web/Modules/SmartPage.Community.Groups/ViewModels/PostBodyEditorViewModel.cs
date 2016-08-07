using SmartPage.Community.Groups.Models;

namespace SmartPage.Community.Groups.ViewModels {
    public class PostBodyEditorViewModel {
        public GroupPostPart GroupPostPart { get; set; }

        public string Text {
            get { return GroupPostPart.Record.Text; }
            set { GroupPostPart.Record.Text = string.IsNullOrWhiteSpace(value) ? value : value.TrimEnd(); }
        }

        public string Format {
            get { return GroupPostPart.Record.Format; }
            set { GroupPostPart.Record.Format = value; }
        }

        public string EditorFlavor { get; set; }
    }
}