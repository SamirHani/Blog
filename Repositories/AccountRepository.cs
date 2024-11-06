using Blog.Models;
using Microsoft.AspNetCore.Identity;

namespace Blog.Repositories
{
	public class AccountRepository
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;
		private IdentityResult delete;

		public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, bool RememberMe, string password)
		{
			IdentityResult result = await userManager.CreateAsync(user, password);
			if (result.Succeeded)
			{
				await signInManager.SignInAsync(user, RememberMe);
			}
			return result;
		}

		public async Task<ApplicationUser> GetByUserNameAsync(string userName)
		{
			var result = await userManager.FindByNameAsync(userName);
			return result;
		}

		public async Task<IdentityResult> UpdateAsync(ApplicationUser UpdatedUser, string oldUserName)
		{
			var oldUser = await GetByUserNameAsync(oldUserName);
			if (oldUser == null)
				return IdentityResult.Failed(new IdentityError { Description = "User Not Found" });

			var checkIfEmailUsed = await userManager.FindByEmailAsync(UpdatedUser.Email);
			if (checkIfEmailUsed != null && checkIfEmailUsed.Id != oldUser.Id)
				return IdentityResult.Failed(new IdentityError { Description = "Email is already Used by another account" });

			var checkIfUserNameUsed = await GetByUserNameAsync(UpdatedUser.UserName);
			if (checkIfUserNameUsed != null && checkIfUserNameUsed.Id != oldUser.Id)
				return IdentityResult.Failed(new IdentityError { Description = "Username is already Used by another account" });

			oldUser.UserName = UpdatedUser.UserName;
			oldUser.Email = UpdatedUser.Email;
			oldUser.PhoneNumber = UpdatedUser.PhoneNumber;

			var res = await userManager.UpdateAsync(oldUser);
			return res;
		}

		public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
		{
			var result = await userManager.CheckPasswordAsync(user, password);
			return result;
		}

		public async Task<IdentityResult> DeleteAsync(string username, string password)
		{
			var result = await GetByUserNameAsync(username);
			if (result == null)
				return IdentityResult.Failed(new IdentityError { Description = "User not found" });

			var decision = await CheckPasswordAsync(result, password);
			if (decision == false)
				return IdentityResult.Failed(new IdentityError { Description = "Wrong Password" });
			try
			{
				var deleteResult = await userManager.DeleteAsync(result);
				return deleteResult;
			}
			catch (Exception ex)
			{
				return IdentityResult.Failed(new IdentityError { Description = ex.Message });
			}
		}
	}
}