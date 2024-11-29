using Blog.Context;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace Blog.Services
{
    public class PostService
    {

        public IWebHostEnvironment _webHostEnvironment { get; }

        public PostService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string GetUserIdFromClaim(ClaimsPrincipal User)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId;
        }
        public Post BindPostData(CreatePostViewModel model, string userId)
        {
            var post = new Post()
            {
                Content = model.Content,
                Title = model.Title,
                UserId = userId,
                Language = model.Language
            };
            return post;
        }
        public Comment BindCommentData(AddCommentViewModel model, string userId)
        {
            var comment = new Comment()
            {
                Content = model.Content,
                UserId = userId,
                PostId = model.PostId
            };
            return comment;
        }

        public async Task<string> RenameAndSaveImageAsync(IFormFile image)
        {
            if (image != null)
            {
                // Ensure the "img" folder exists in wwwroot
                var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                Directory.CreateDirectory(imgPath);

                // Create a unique filename
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                // combine the img path with file name
                var filePath = Path.Combine(imgPath, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                return fileName;
            }
            return null;
        }

    }
}
