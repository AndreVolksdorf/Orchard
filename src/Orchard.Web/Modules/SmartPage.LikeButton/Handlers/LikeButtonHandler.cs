using Orchard.ContentManagement.Handlers;
using SmartPage.LikeButton.Models;
using SmartPage.LikeButton.Settings;

namespace SmartPage.LikeButton.Handlers
{
    public class LikeButtonHandler : ContentHandler
    {
        public LikeButtonHandler()
        {
            OnInitializing<LikeButtonPart>((context, part) =>
            {
                part.ShowVoter = part.Settings.GetModel<LikeButtonPartSettings>().ShowVoter;
                part.LikeButtonText = part.Settings.GetModel<LikeButtonPartSettings>().LikeButtonText;
                part.UnlikeButtonText = part.Settings.GetModel<LikeButtonPartSettings>().UnlikeButtonText;
                part.Dimension = part.Settings.GetModel<LikeButtonPartSettings>().Dimension;
            });
        }
    }
}