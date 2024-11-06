using Blog.Context;
using Blog.Models;
using Blog.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Blog.Services
{
	public class ProfileService
	{
		private readonly ProfileRepository repo;
		private readonly UserManager<ApplicationUser> user;

		public ProfileService(ProfileRepository repo, UserManager<ApplicationUser> user)
		{
			this.repo = repo;
			this.user = user;
		}

		public async Task<bool> CreateAsync(string UserName)
		{
			ApplicationUser getUserByUsername = await user.FindByNameAsync(UserName);

			Profile profile = new Profile { Name = UserName, Bio = "Hello i'm using Blog", ImgUrl = "/img/av1.jpg", ApplicationUserId = getUserByUsername.Id };

			repo.Create(profile);

			return true;
		}
	}
}
