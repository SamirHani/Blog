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

		public new IActionResult User(string id)
		{
			var res = context.Profiles.FirstOrDefault(p =>p.Name == id);

			if (res == null)
			{
				return NotFound(StatusCode(404));
			}

			return View(res);
		}
	}
}
