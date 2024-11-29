using Blog.Context;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.Security.Claims;

namespace Blog.Controllers
{
	public class ProfileController : Controller
	{
		private readonly ApplicationDbContext context;

		public object ClaimPrincipal { get; private set; }

		public ProfileController(ApplicationDbContext context)
		{
			this.context = context;
		}
		[Route("/Profile/User/{id}")]
		public IActionResult Profile(string id) // id is the username
		{
			var res = context.Profiles.FirstOrDefault(p => p.Name == id);

			if (res == null)
			{
				return NotFound(StatusCode(404));
			}
			// followers count
			var userProfileId = context.Profiles.Where(p => p.Name == id).FirstOrDefault()?.ApplicationUserId;
			var subscriptions = context.Subscriptions.Where(s => s.FollowedId == userProfileId).ToList();
			ViewBag.FollowersCount = subscriptions.Count;

			// followings count
			var peopleHeFollowing = context.Subscriptions.Where(s => s.FollowerId == userProfileId).ToList();
			ViewBag.FollowingsCount = peopleHeFollowing.Count;

			return View("User",res);
		}
		public IActionResult Edit(string id)//id is username here
		{
			var res = context.Profiles.FirstOrDefault(p=>p.Name == id);
			var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (res == null ||claimUserId != res.ApplicationUserId)
				return NotFound();

			return View(res);

		}
		[HttpPost]
		public IActionResult Edit(string userName, string bio)
		{
			var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var oldProfile = context.Profiles.Where(p => p.Name == userName).FirstOrDefault();
			var profileUserId = oldProfile?.ApplicationUserId;

			if ( claimUserId == null || oldProfile == null || claimUserId != profileUserId)
				return BadRequest();

			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Something Went Wrong");
				return View(oldProfile);
			}

			oldProfile.Bio = bio;

			context.Profiles.Update(oldProfile);
			context.SaveChanges();

			return RedirectToAction("User", "Profile", new { id = userName });
		}


	}
}
