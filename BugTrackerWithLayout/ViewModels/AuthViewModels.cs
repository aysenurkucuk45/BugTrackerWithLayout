using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerWithLayout.ViewModels
{
	public class AuthViewModels
	{
	}
}
namespace BugTrackerWithLayout.ViewModels
{
    public class LoginViewModel
    {
        public string UserOrEmail { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Avatar { get; set; } // Tom | Jerry
    }

    public class ForgotPasswordViewModel
    {
        public string Email { get; set; }
    }
}
