using Blog.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    public class ApplicationUser : IdentityUser
    {
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<Subscription> FollowerSubscriptions { get; set; } = new List<Subscription>(); // Users who follow this user
    public ICollection<Subscription> FollowedSubscriptions { get; set; } = new List<Subscription>(); // Users this user follows
        public ICollection<Post> posts { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}


