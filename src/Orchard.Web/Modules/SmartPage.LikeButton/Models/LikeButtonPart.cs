using Orchard.ContentManagement;

namespace SmartPage.LikeButton.Models
{
    public class LikeButtonPart : ContentPart
    {
        public string UnlikeButtonText { get; set; }

        public string LikeButtonText { get; set; }

        public string Dimension { get; set; }

        public bool ShowVoter { get; set; }

        public double Value { get; set; }

        public bool IsFavorite { get; set; }

        public double NumberOfFavorites { get; set; }
    }
}