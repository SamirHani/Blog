namespace Blog.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string FollowerId { get; set; }
        public string FollowedId { get; set; }

        public ApplicationUser Follower { get; set; }

        public ApplicationUser Followed  { get; set; }
        public DateOnly Date { get; set;} 
    }
}
