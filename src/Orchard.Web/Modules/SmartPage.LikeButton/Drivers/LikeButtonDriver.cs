using System;
using System.Collections.Generic;
using Contrib.Voting.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Security;
using SmartPage.LikeButton.Models;
using System.Linq;

namespace SmartPage.LikeButton.Drivers
{
    public class LikeButtonDriver : ContentPartDriver<LikeButtonPart>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IVotingService _votingService;

        public LikeButtonDriver(IAuthenticationService authenticationService, IVotingService votingService)
        {
            _authenticationService = authenticationService;
            _votingService = votingService;
        }

        protected override DriverResult Display(LikeButtonPart part, string displayType, dynamic shapeHelper)
        {

            var results = new List<DriverResult>();

            var displayPart = BuildVoteUpDown(part);

            if (displayType.Equals("Summary", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(ContentShape("Parts_LikeCounter", () => shapeHelper.Parts_LikeCounter(displayPart))
                    );
            }

            if (part.ShowVoter)
            {
                results.Add(ContentShape("Parts_LikeButton", () => shapeHelper.Parts_LikeButton(displayPart))
                    );
            }

            return Combined(results.ToArray());
            

        }

        private LikeButtonPart BuildVoteUpDown(LikeButtonPart part)
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();

            if (currentUser != null)
            {
                var resultRecord = _votingService.GetResult(part.ContentItem.Id, "sum", part.Dimension);

                part.IsFavorite = (resultRecord != null && resultRecord.Value > 0.0);
                part.Value = resultRecord != null ? resultRecord.Value : 0;
                part.NumberOfFavorites = _votingService.Get(vote => vote.Username == currentUser.UserName && vote.Dimension == part.Dimension).Sum(o => o.Value);
            }

            return part;
        }


        protected override DriverResult Editor(LikeButtonPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_LikeButton_Edit",
              () => shapeHelper.EditorTemplate(
                TemplateName: "Parts/LikeButton",
                Model: part,
                Prefix: Prefix));
        }


        protected override DriverResult Editor(LikeButtonPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}