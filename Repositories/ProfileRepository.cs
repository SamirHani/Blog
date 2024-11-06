using Blog.Context;
using Blog.Models;

namespace Blog.Repositories
{
	public class ProfileRepository
	{
		private readonly ApplicationDbContext context;

		public ProfileRepository(ApplicationDbContext context)
		{
			this.context = context;
		}

		public bool Create(Profile profile)
		{
			context.Profiles.Add(profile);
			context.SaveChanges();
			return true;
		}

	}
}
