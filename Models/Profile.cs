namespace Blog.Models
{
    public class Profile

    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string ImgUrl { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }


    }
}
