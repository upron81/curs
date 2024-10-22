using Microsoft.AspNetCore.Identity;

namespace TelecomWeb.Services
{
    public class IdentitySeedData
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentitySeedData(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            CreateRoleIfNotExists("admin");
            CreateRoleIfNotExists("manager");

            var user = _userManager.FindByNameAsync("admin").Result;
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = "d@d.d",
                    Email = "d@d.d"
                };
                var createResult = _userManager.CreateAsync(user, "pass").Result;
                if (createResult.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "admin").Wait();
                }
            }
        }

        private void CreateRoleIfNotExists(string roleName)
        {
            if (!_roleManager.RoleExistsAsync(roleName).Result)
            {
                _roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
            }
        }
    }
}
