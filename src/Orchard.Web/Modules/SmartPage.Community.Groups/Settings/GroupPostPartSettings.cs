namespace SmartPage.Community.Groups.Settings {
    public class GroupPostPartSettings {
        public const string FlavorDefaultDefault = "html";
        private string _flavorDefault;
        public string FlavorDefault {
            get { return !string.IsNullOrWhiteSpace(_flavorDefault)
                           ? _flavorDefault
                           : FlavorDefaultDefault; }
            set { _flavorDefault = value; }
        }
    }
}
