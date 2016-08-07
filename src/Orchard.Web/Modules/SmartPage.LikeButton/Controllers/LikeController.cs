using Contrib.Voting.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using SmartPage.LikeButton.Models;
using System.Linq;
using System.Web.Mvc;

namespace SmartPage.LikeButton.Controllers
{
    public class LikeController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        private readonly IVotingService _votingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LikeController(IOrchardServices orchardServices,
            IContentManager contentManager,
            IVotingService votingService,
            IHttpContextAccessor httpContextAccessor)
        {
            _orchardServices = orchardServices;
            _contentManager = contentManager;
            _votingService = votingService;
            _httpContextAccessor = httpContextAccessor;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        [HttpPost]
        public ActionResult Apply(int contentId, string returnUrl)
        {
            var content = _contentManager.Get(contentId);
            if (content == null || !content.Has<LikeButtonPart>() || !content.As<LikeButtonPart>().ShowVoter)
                return this.RedirectLocal(returnUrl, "~/");

            var currentUser = _orchardServices.WorkContext.CurrentUser;
            if (currentUser == null)
                return this.RedirectLocal(returnUrl, "~/");

            var currentVote = _votingService.Get(vote =>
                vote.Username == currentUser.UserName &&
                vote.ContentItemRecord == content.Record &&
                vote.Dimension == content.As<LikeButtonPart>().Dimension).FirstOrDefault();

            if (currentVote != null)
            {
                _votingService.RemoveVote(currentVote);
            }
            else
            {
                _votingService.Vote(content, currentUser.UserName, _httpContextAccessor.Current().Request.UserHostAddress, 1, content.As<LikeButtonPart>().Dimension);
            }

            return this.RedirectLocal(returnUrl, "~/");
        }
    }
}