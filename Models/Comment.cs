namespace Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt{ get; set; } = DateTime.Now;
        public string? Image { get; set; }
        //-----------------------------------
        public int PostId { get; set; }
        public Post Post { get; set; }

        //-----------------------------------
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
