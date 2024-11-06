namespace Blog.ViewModels
{
    public class CreatePostViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Language { get; set; }
        public IFormFile? Image { get; set; }
    }
}
