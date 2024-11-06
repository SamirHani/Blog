namespace Blog.ViewModels
{
    public class PostCommentsViewModel
    {
		public string UserName { get; set; }
		public string UserId { get; set; }
		public string ProfileImage { get; set; }
		public int Id { get; set; }
		public string Content { get; set; }
		public string CommentImage { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
