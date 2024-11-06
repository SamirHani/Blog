using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public string Status { get; set; } = "Published";
        public string? Language { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Tag>? Tags { get; set; } = new List<Tag>();
        public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
        public ICollection<Like>? Likes { get; set; } = new List<Like>();
        public ICollection<Category>? Categories { get; set; } = new List<Category>();
    }
}
