using System.Text.RegularExpressions;

namespace Blog.Services
{
	public static class CheckEmailService
	{
		private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
		public static bool IsValidEmail(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				return false;
			}
			return Regex.IsMatch(email, EmailPattern);
		}
	}
}