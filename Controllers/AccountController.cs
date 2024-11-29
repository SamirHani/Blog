using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Blog.Repositories;
using Blog.Services;
using Microsoft.AspNetCore.Identity;
using Humanizer;
using Blog.Context;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Controllers
{
	public class AccountController : Controller
	{
		private readonly AccountService service;
		private readonly ApplicationDbContext context;
		private readonly UserManager<ApplicationUser> user;
		private readonly ProfileService profileService;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly RoleManager<IdentityRole> roleManager;

		public AccountController(
			AccountService service,
			ApplicationDbContext _context,
			UserManager<ApplicationUser> user,
			ProfileService profileService,
			SignInManager<ApplicationUser> signInManager,
			RoleManager<IdentityRole> roleManager
			)
		{
			this.service = service;
			this.context = _context;
			this.user = user;
			this.profileService = profileService;
			this.signInManager = signInManager;
			this.roleManager = roleManager;
		}
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Invalid Registration");
				return View(model);
			}

			var result = await service.RegisterServiceAsync(model);

			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
					ModelState.AddModelError("", error.Description.Humanize(LetterCasing.Title));
				return View(model);
			}

			await profileService.CreateAsync(model.UserName);

			return RedirectToAction("ShowAll", "Account");

		}
		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Invalid Login");
				return View(model);
			}

			var user = await service.GetByUserNameAsync(model.UserName);
			bool res = await service.CheckPasswordAsync(user, model.Password);

			if (user == null || res == false)
			{
				ModelState.AddModelError("", "Invalid Login");
				return View(model);
			}

			await signInManager.SignInAsync(user, model.RememberMe);

			return RedirectToAction("Index", "Feed");
		}
		public new async Task<IActionResult> SignOut()
		{
			await signInManager.SignOutAsync();
			return RedirectToAction("Login", "Account");
		}
		public async Task<bool> CheckForUserEmailPattern(string email)
		{
			if (CheckEmailService.IsValidEmail(email) == false)
				return false;

			ApplicationUser FindByEmail = await user.FindByEmailAsync(email);
			if (FindByEmail != null)
				return false;

			return true;
		}
		[Authorize(Roles="Admin")]
		public IActionResult ShowAll()
		{
			List<ShowAllAccountsVM> showAllAccountsVMs = new List<ShowAllAccountsVM>();
			var users = context.Users.ToList();
			foreach (var item in users)
			{
				ShowAllAccountsVM showAllAccountsVM = new ShowAllAccountsVM
				{
					Id = item.Id,
					UserName = item.UserName,
					Email = item.Email,
					PhoneNumber = item.PhoneNumber
				};
				showAllAccountsVMs.Add(showAllAccountsVM);

			}
			var Profiles = context.Profiles.OrderBy(p => p.Name).ToList();
			var orderedAccounts = showAllAccountsVMs.OrderBy(p => p.UserName).ToList();
			ViewBag.Profiles = Profiles;
			return View(orderedAccounts);
		}
		[Authorize(Roles = "Admin")]
		public IActionResult AddRole()
		{
			return View();
		}
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> AddRole(AddRoleVM addRoleVM)
		{
			if (ModelState.IsValid)
			{
				IdentityRole role = new IdentityRole()
				{
					Name = addRoleVM.Role
				};

				var result = await roleManager.CreateAsync(role);
				if (result.Succeeded)
				{
					ViewBag.AddRole = true;
					return RedirectToAction("Index", "Feed");
				}
			}
			ViewBag.AddRole = false;
			return View(addRoleVM);
		}
		[Authorize(Roles = "Admin")]
		public IActionResult Dashboard()
		{
			return View("AdminActions");
		}
		[Authorize(Roles ="Admin")]
		public IActionResult DeleteUser()
		{
			return View();
		}
        [Authorize(Roles ="Admin")]
		[HttpPost]
		public async Task<IActionResult> DeleteUser(string userName)
		{
			var userToDelete = await user.FindByNameAsync(userName);

				if(userToDelete != null)
					await user.DeleteAsync(userToDelete);

			return RedirectToAction("Index" , "Feed");
		}
	}
}
