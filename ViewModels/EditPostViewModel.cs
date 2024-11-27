namespace Blog.ViewModels
{
    public class EditPostViewModel
    {
        public int PostId { get; set; }
        public string Title {  get; set; }
        public string Content { get; set; }
        public string? Language { get; set; }
        public string? Image { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    }
}
