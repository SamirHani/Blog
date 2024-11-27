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
            if (checkFollowing != null)
                return Content("you are already following this person");

            var Subscription = new Subscription() { FollowerId = currentUser, FollowedId = Id, Date = DateOnly.FromDateTime(DateTime.Now)};
            context.Subscriptions.Add(Subscription);
            context.SaveChanges();
            return Content("Following");

        }
    }
}
