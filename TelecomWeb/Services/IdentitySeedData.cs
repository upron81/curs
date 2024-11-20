using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using TelecomWeb.Data;

namespace TelecomWeb.Services
{
    public class IdentitySeedData
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public IdentitySeedData(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public void Initialize()
        {
            foreach (var roleName in Enum.GetNames(typeof(Role)))
                if (!_roleManager.RoleExistsAsync(roleName).Result)
                    _roleManager.CreateAsync(new IdentityRole(roleName)).Wait();

            var adminEmail = _configuration["IdentitySeed:AdminEmail"];
            var adminPassword = _configuration["IdentitySeed:AdminPassword"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                throw new InvalidOperationException("Admin email or password is not configured in appsettings.json.");
            }

            var user = _userManager.FindByNameAsync(adminEmail).Result;
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };

                var createResult = _userManager.CreateAsync(user, adminPassword).Result;
                if (createResult.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, Role.admin.ToString()).Wait();
                }
                else
                {
                    throw new Exception($"Failed to create admin user: {string.Join(", ", createResult.Errors)}");
                }
            }
        }
    }
}
