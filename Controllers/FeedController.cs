using Blog.Context;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        private readonly ApplicationDbContext context;

        public FeedController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            // we get the posts
            var Posts = context.Posts.ToList();

            // Created the feed list 
            List<FeedPostsViewModel> feedPosts = new List<FeedPostsViewModel>();

            // add the posts to the feed
            foreach (var post in Posts)
            {
                var feed = new FeedPostsViewModel();
                var profile = context.Profiles.FirstOrDefault(p => p.ApplicationUserId == post.UserId);

                if (profile != null)
                {
                    feed.Image = profile.ImgUrl;
                    feed.UserName = profile.Name;
                }

                feed.Content = post.Content;

                if (post.Title != null)
                    feed.Title = post.Title;

                feed.CreatedAt = post.CreatedAt;

                // this is for the post image
                if (post.Image != null)
                    feed.PostImage = post.Image;

                feed.Language = post.Language;
                feed.UserId = post.UserId;
                feed.Id = post.Id;
                feedPosts.Add(feed);
            }
            return View(feedPosts);
        }
    }
}
