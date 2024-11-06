using Blog.Context;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
	public class ProfileController : Controller
	{
		private readonly ApplicationDbContext context;
		public ProfileController(ApplicationDbContext context)
		{
			this.context = context;
		}

		public IActionResult User(int id)
		{
			var res = context.Profiles.FirstOrDefault(p => p.Id == id);

			if (res == null)
			{
				return NotFound(StatusCode(404));
			}

			return View(res);
		}
	}
}
