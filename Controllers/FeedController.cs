using Blog.Context;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace Blog.Controllers
{
	[Authorize]
	public class FeedController : Controller
	{

		private readonly ApplicationDbContext context;
		private readonly AccountService service;

		public FeedController(ApplicationDbContext context, AccountService service)
		{
			this.context = context;
			this.service = service;
		}

		public async Task<IActionResult> Index(string? id)// id here is the username
		{
			var Posts = new List<Post>();
			if (id != null)
			{
				var user = await service.GetByUserNameAsync(id);
				// we get the posts
				Posts = context.Posts.Where(p => p.UserId == user.Id).OrderByDescending(p => p.CreatedAt).Include(p => p.Comments).Include(p => p.Likes).ToList();
			}
			else
			{
				Posts = context.Posts.OrderByDescending(p => p.CreatedAt).Include(p => p.Comments).Include(p => p.Likes).ToList();
			}

			// Created the feed list 
			List<FeedPostsViewModel> feedPosts = new List<FeedPostsViewModel>();

			// add the posts to the feed
			foreach (var post in Posts)
			{
				var feedView = new FeedPostsViewModel();
				var profile = context.Profiles.FirstOrDefault(p => p.ApplicationUserId == post.UserId);

				if (profile != null)
				{
					feedView.Image = profile.ImgUrl;
					feedView.UserName = profile.Name;
				}

				feedView.Content = post.Content;

				if (post.Title != null)
					feedView.Title = post.Title;

				feedView.CreatedAt = post.CreatedAt;

				// add the number of the comments and likes
				feedView.CommentsNumber = post.Comments?.Count() ?? 0;
				feedView.LikesNumber = post.Likes?.Count() ?? 0;

				// this is for the post image
				if (post.Image != null)
					feedView.PostImage = post.Image;

				feedView.Language = post.Language;
				feedView.UserId = post.UserId;
				feedView.Id = post.Id;
				feedPosts.Add(feedView);
			}
			return View("_index", feedPosts);
		}

		public IActionResult Search(string text)
		{
			if(string.IsNullOrEmpty(text))
			{
				return View("_search", new List<Post>());
			}
				var searchResults = context.Posts.
					Where(p => p.Content.ToLower().Contains(text.ToLower()) || p.Title.ToLower().Contains(text.ToLower())).ToList();
				if (searchResults != null)
					return View("_search", searchResults);
				return View("_search", searchResults);
		}

	}
}
