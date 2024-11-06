using Blog.Models;
using Blog.Repositories;
using Blog.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Blog.Services
{

	public class AccountService
	{
		private readonly AccountRepository repo;

		public AccountService(AccountRepository repo)
		{
			this.repo = repo;
		}

		public async Task<IdentityResult> RegisterServiceAsync(RegisterViewModel model)
		{
			ApplicationUser user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
			var result = await repo.CreateUserAsync(user, model.RememberMe, model.Password);

			return result;
		}
		#region not Tested
		public async Task<ApplicationUser> GetByUserNameAsync(string userName)
		{
			var result = await repo.GetByUserNameAsync(userName);
			return result;
		}

		public async Task<IdentityResult> UpdateAsync(ApplicationUser UpdatedUser, string oldUserName)
		{
			var result = await repo.UpdateAsync(UpdatedUser, oldUserName);
			return result;
		}

		public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
		{
			var result = await repo.CheckPasswordAsync(user, password);
			return result;
		}

		public async Task<IdentityResult> DeleteAsync(string username, string password)
		{
			var result = await repo.DeleteAsync(username, password);
			return result;
		}
		#endregion

	}
}
