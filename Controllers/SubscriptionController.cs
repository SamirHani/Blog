using Blog.Context;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public SubscriptionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        [HttpPost]
        public IActionResult Index(string Id)
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var checkFollowing = context.Subscriptions.Where(s => s.FollowerId == currentUser && s.FollowedId == Id).FirstOrDefault();
            if (checkFollowing != null || currentUser == checkFollowing?.FollowedId)
                return Content("you are already following this person or you can't follow yourself");

            var Subscription = new Subscription() { FollowerId = currentUser, FollowedId = Id, Date = DateOnly.FromDateTime(DateTime.Now)};
            context.Subscriptions.Add(Subscription);
            context.SaveChanges();
            return Content("Following");

        }
		public IActionResult Followers(string Id) // id is username of the person who is followed
		{
            // from the username get the id
            var userProfileId = context.Profiles.Where(p => p.Name == Id).FirstOrDefault()?.ApplicationUserId;
			var subscriptions = context.Subscriptions.Where(s => s.FollowedId == userProfileId).ToList();
			List<Profile> Followers = new List<Profile>();

			if (subscriptions == null)
				return View("showLikers", Followers);

            foreach (var subscription in subscriptions)
            {
                var followerId = subscription.FollowerId;
                var followerProfile = context.Profiles.Where(p=>p.ApplicationUserId ==  followerId).FirstOrDefault();

                if (followerProfile != null)
                            Followers.Add(followerProfile);
            }
			return View("ShowProfiles", Followers);
		}
		public IActionResult Following(string Id) // id is username of the person who is follower
		{
			// from the username get the id
			var userProfileId = context.Profiles.Where(p => p.Name == Id).FirstOrDefault()?.ApplicationUserId;
			var subscriptions = context.Subscriptions.Where(s => s.FollowerId == userProfileId).ToList();
			List<Profile> Followings = new List<Profile>();

			if (subscriptions == null)
				return View("showLikers", Followings);

			foreach (var subscription in subscriptions)
			{
				var followedId = subscription.FollowedId;
				var followedProfile = context.Profiles.Where(p => p.ApplicationUserId == followedId).FirstOrDefault();

				if (followedProfile != null)
					Followings.Add(followedProfile);
			}
			return View("ShowProfiles", Followings);
		}
	}
}
