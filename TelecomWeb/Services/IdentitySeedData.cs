using Microsoft.AspNetCore.Identity;
using TelecomWeb.Data;

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
            foreach (var roleName in Enum.GetNames(typeof(Role)))
                if (!_roleManager.RoleExistsAsync(roleName).Result)
                    _roleManager.CreateAsync(new IdentityRole(roleName)).Wait();

            var user = _userManager.FindByNameAsync("d@d.d").Result;
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
                    _userManager.AddToRoleAsync(user, Role.admin.ToString()).Wait();
                }
            }
        }
    }
}
