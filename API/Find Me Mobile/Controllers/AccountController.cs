using Find_Me_Mobile.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Find_Me_Mobile.Constants;
using Find_Me_Mobile.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Find_Me_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        #region Constructors

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext, IOptions<ApplicationSettings> appSettings) : base(userManager, roleManager, applicationDbContext, appSettings)
        {

        }
        #endregion

        #region Methods

        [HttpPost]
        [AllowAnonymous]
        [Route("SignIn")]
        public async Task<IActionResult> SignIn(SignInBindingModel model)
        {
            if (model is null)
                return Ok(new ApplicationResult { IsSuccess = false, Message = ErrorMessages.InvalidInput });

            var user = await _userManager.FindByEmailAsync(model.Email.ToLower());
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (user.IsDisabled)
                    return Ok(new ApplicationResult { Message = ErrorMessages.AccountDisabled });

                var securityDescriptor = new SecurityTokenDescriptor { Subject = new ClaimsIdentity(new Claim[] { new Claim("UserId", user.Id.ToString()), new Claim(ClaimTypes.Name, user.UserName) }), Expires = DateTime.Now.AddDays(90), SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256) };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(securityDescriptor);
                string roleId = await _applicationDbContext.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.RoleId).FirstOrDefaultAsync();
                string role = await _applicationDbContext.Roles.Where(x => x.Id == roleId).Select(x => x.Name).FirstOrDefaultAsync();

                return Ok(new ApplicationResult { IsSuccess = true, Data = new { token = tokenHandler.WriteToken(securityToken), user.Id, user.UserName, user.Email, role = role } });
            }
            else
                return Ok(new ApplicationResult { Message = ErrorMessages.InvalidEmailOrPassword });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp(SignUpBindingModel model)
        {
            if (model is null)
                return Ok(new ApplicationResult { IsSuccess = false, Message = ErrorMessages.InvalidInput });

            var user = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                Domain = model.Domain,
                RegistrationDate = DateTime.Now
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return Ok(new ApplicationResult { IsSuccess = false, Message = "", Data = result });

            var roleExists = await _roleManager.RoleExistsAsync(Roles.User);
            if (!roleExists)
            {
                // Create new role
                IdentityResult roleResult = await _roleManager.CreateAsync(new IdentityRole(Roles.User));
                if (!roleResult.Succeeded)
                    return Ok(new ApplicationResult { IsSuccess = false, Message = "", Data = result });
            }

            IdentityResult addRoleToUserResult = await _userManager.AddToRoleAsync(user, Roles.User);
            if (!addRoleToUserResult.Succeeded)
                return Ok(new ApplicationResult { IsSuccess = false, Message = "", Data = result });

            return Ok(new ApplicationResult { IsSuccess = true, Message = ErrorMessages.UserRegistered });
        }

        #endregion
    }
}
