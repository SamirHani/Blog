using Blog.Context;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace Blog.Controllers
{
	[Authorize]
	public class PostController : Controller
	{
		private readonly ApplicationDbContext context;
		private readonly PostService postService;
		public PostController(ApplicationDbContext context, PostService postService)
		{
			this.context = context;
			this.postService = postService;
		}
		
		//Post CRUD Operations
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(CreatePostViewModel model)
		{
			var userId = postService.GetUserIdFromClaim(User); // return string

			if (ModelState.IsValid)
			{
				var post = postService.BindPostData(model, userId); // return Post


				var fileName = await postService.RenameAndSaveImageAsync(model.Image);
				// bind the image to the post to save it in the database
				if (fileName != null)
					post.Image = "/img/" + fileName;

				context.Posts.Add(post);
				await context.SaveChangesAsync();
				return RedirectToAction("Index", "Feed");
			}
			ModelState.AddModelError("", "Invalid Post");
			return View(model);
		}
		public IActionResult Delete(int id)
		{
			var post = context.Posts.Include(p => p.Comments)
									.Include(p => p.Likes)
									.FirstOrDefault(p => p.Id == id);
			if (post == null)
			{
				return NotFound();
			}
			if (post.UserId != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
			{
				return Unauthorized();
			}
			context.Comments.RemoveRange(post.Comments);
			context.Likes.RemoveRange(post.Likes);
			context.Posts.Remove(post);
			context.SaveChanges();
			return RedirectToAction("Index", "Feed");
		}
		public IActionResult Update(int Id)
		{
			if (Id == 0 || Id == null)
				return RedirectToAction("index", "Feed");

            // get the old post from the database
            var oldPost = context.Posts.Where(p => p.Id == Id).FirstOrDefault();
			if (oldPost == null)
				return Unauthorized();
            var userId = postService.GetUserIdFromClaim(User); // return string

            // check if this the Creator of the Post
            if (userId != oldPost.UserId)
                return Unauthorized();

            // edit post view model to be able to edit on it
            EditPostViewModel oldPostModel = new EditPostViewModel();
			oldPostModel.Title = oldPost.Title;
			oldPostModel.Content = oldPost.Content;
			oldPostModel.Language = oldPost.Language;
			oldPostModel.PostId = Id;

			if(oldPost.Image != null)
			oldPostModel.Image = oldPost.Image;

            return View(oldPostModel);
		}
		[HttpPost]
		public IActionResult Update(EditPostViewModel newVersionPost)
		{
            if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Something Went Wrong");
				return View(newVersionPost);
			}
			var oldPost = context.Posts.Where(p=>p.Id == newVersionPost.PostId).FirstOrDefault();

            if (oldPost == null)
                return Unauthorized();

            var userOfThePost = oldPost.UserId;

			if (postService.GetUserIdFromClaim(User) != userOfThePost)
				return Unauthorized();

			oldPost.Title = newVersionPost.Title;
			oldPost.Content = newVersionPost.Content;
			oldPost.Language = newVersionPost.Language;

			context.Posts.Update(oldPost);
			context.SaveChanges();

			return RedirectToAction("index", "Feed");
		}

		//Comment CRUD Operations
		public IActionResult AddComment(int id)
		{
			// send the post id to use it in the http request
			ViewBag.PostId = id;
			return PartialView();
		}
        [HttpPost]
        public async Task<IActionResult> AddComment(AddCommentViewModel model)
        {
            var userId = postService.GetUserIdFromClaim(User);
            if (ModelState.IsValid)
            {
                var comment = postService.BindCommentData(model, userId);
                var fileName = await postService.RenameAndSaveImageAsync(model.Image);

                // bind the image to the post to save it in the database if exist
                if (fileName != null)
                    comment.Image = "/img/" + fileName;

                context.Comments.Add(comment);
                await context.SaveChangesAsync();
                return RedirectToAction("Index", "Feed");
            }
            ModelState.AddModelError("", "Invalid Comment");
            return View(model);
        }
        public IActionResult ShowPostComments(int id)
		{
			var comments = context.Comments.Where(c => c.PostId == id).ToList();
			if (comments.Count == 0)
			{
				return Content("<h3>there's no comment's here</h3>");
			}
			List<PostCommentsViewModel> postComments = new List<PostCommentsViewModel>();
			ViewBag.Post = context.Posts.FirstOrDefault(p => p.Id == comments[0].PostId);
			foreach (var item in comments)
			{
				var comment = new PostCommentsViewModel();
				var profile = context.Profiles.FirstOrDefault(p => p.ApplicationUserId == item.UserId);

				if (profile != null)
				{
					comment.ProfileImage = profile.ImgUrl;
					comment.UserName = profile.Name;
				}
				// feed => comment , Post => item
				comment.Content = item.Content;

				comment.CreatedAt = item.CreatedAt;

				// this is for the post image
				if (item.Image != null)
					comment.CommentImage = item.Image;

				comment.UserId = item.UserId;
				comment.Id = item.Id;
				postComments.Add(comment);
			}
			return PartialView(postComments);
		}
		public IActionResult EditComment(int Id)
		{
            if (Id == 0 || Id == null)
                return RedirectToAction("index", "Feed");
            // get the old post from the database
            var oldComment = context.Comments.Where(c => c.Id == Id).FirstOrDefault();

            if (oldComment == null)
                return Unauthorized();

            var userId = postService.GetUserIdFromClaim(User); // return string

            // check if this the Creator of the Post
            if (userId != oldComment.UserId)
                return Unauthorized();

            // edit post view model to be able to edit on it
            EditCommentVM oldPostModel = new EditCommentVM();
            oldPostModel.Content = oldComment.Content;
            oldPostModel.CommentId = Id;
			if (oldComment.Image != null)
				oldPostModel.Image = oldComment.Image;

            return View(oldPostModel);
        }
		[HttpPost]
		public IActionResult Editcomment(EditCommentVM newVersionComment)
		{
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Something Went Wrong");
                return View(newVersionComment);
            }

            var oldComment = context.Comments.Where(c => c.Id == newVersionComment.CommentId).FirstOrDefault();

            if (oldComment == null)
                return Unauthorized();

            var userOfTheComment = oldComment.UserId;

            if (postService.GetUserIdFromClaim(User) != userOfTheComment)
                return Unauthorized();

            oldComment.Content = newVersionComment.Content;

            context.Comments.Update(oldComment);
            context.SaveChanges();

            return RedirectToAction("index", "Feed");
        }
		// you didn't add button for deleing the comment yet and the delete also
        public IActionResult DeleteComment(int id) // comment id
        {
            var comment = context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
                return NotFound();

            if (comment.UserId != postService.GetUserIdFromClaim(User))
                return Unauthorized();

            context.Comments.Remove(comment);
            context.SaveChanges();

            return RedirectToAction("Index", "Feed");
        }
        public IActionResult ShowAllComments()
		{
			var comments = context.Comments.ToList();
			return View(comments);
		}

        //Like CRUD Operations
        [HttpPost]
		public async Task<IActionResult> AddLike(int Id, string userId)
		{
			var color = "black";
			var Liked = context.Likes.FirstOrDefault(L => L.PostId == Id && L.UserId == userId);
			if (Liked == null)
			{
				context.Add(new Like { PostId = Id, UserId = userId });
				color = "blue";
			}
			else
			{
				context.Likes.Remove(Liked);
			}
			await context.SaveChangesAsync();
			var res = UpdateLikesCount(Id);
			return Json(new { success = true, likes = res, color = color });
		}
		public int UpdateLikesCount(int Id)
		{
			var Post = context.Posts.Include(p => p.Likes).FirstOrDefault(p => p.Id == Id);
			var num = Post.Likes?.Count() ?? 0;
			return num;
		}

		// link for Sharing posts and Show it
		public IActionResult Post(int id) // id is for post id
		{
			var post = context.Posts.Where(p => p.Id == id).Include(p => p.Comments).Include(p => p.Likes).FirstOrDefault();
			if (post == null)
				return NotFound();

			var postViewModel = new List<FeedPostsViewModel> { MapPostToViewModel(post) };

			return View("_index", postViewModel);
		}
		public FeedPostsViewModel MapPostToViewModel(Post post)
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
			return feedView;
		}
	}
}
