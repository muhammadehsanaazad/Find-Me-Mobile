using Find_Me_Mobile.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Find_Me_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        #region Properties

        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly ApplicationDbContext _applicationDbContext;
        protected readonly ApplicationSettings _appSettings;
        #endregion

        #region Constructors

        public BaseController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public BaseController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext, IOptions<ApplicationSettings> appSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
            _appSettings = appSettings.Value;
        }
        #endregion
    }
}