namespace Blog.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

       // public Post Post { get; set; }
        public ICollection<Post>? Posts { get; set; }


    }
}
