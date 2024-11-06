using Blog.Models;

namespace Blog.ViewModels
{
    public class FeedPostsViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PostImage { get; set; }
        public string? Language { get; set; }
    }
}
