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
        public async Task<IActionResult> Delete(int id)
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
        public IActionResult AddComment(int id)
        {
            // send the post id to use it in the http request
            ViewBag.PostId = id;
            return PartialView();
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
        public IActionResult Comments(int id)
        {
            return PartialView();
        }
        public IActionResult ShowAllComments()
        {
            var comments = context.Comments.ToList();
            return View(comments);
        }
    }


}
