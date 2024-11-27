namespace Blog.ViewModels
{
    public class EditCommentVM
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public string? Image { get; set; }

    }
}
