using Microsoft.DotNet.Scaffolding.Shared;

namespace Blog.ViewModels
{
    public class AddCommentViewModel
    {
        public DateTime CreatedAt { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public IFormFile? Image { get; set; }
    }
}
