using Find_Me_Mobile.Constants;
using Find_Me_Mobile.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Find_Me_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class DevicesController : BaseController
    {
        #region Constructors

        public DevicesController(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
        #endregion

        #region Methods

        [HttpGet]
        [Route("GetAllDevices")]
        public async Task<IActionResult> GetAllDevices()
        {
            var devices = await _applicationDbContext.Devices.ToListAsync();
            return Ok(new ApplicationResult { IsSuccess = true, Data = devices });
        }

        [HttpGet]
        [Route("GetCompanyDevices")]
        public async Task<IActionResult> GetCompanyDevices(string companyId)
        {
            var devices = await _applicationDbContext.Devices.Where(e => e.CompanyId == companyId).ToListAsync();
            return Ok(new ApplicationResult { IsSuccess = true, Data = devices });
        }

        [HttpGet]
        [Route("GetSingleDevice")]
        public async Task<IActionResult> GetSingleDevice(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Ok(new ApplicationResult { IsSuccess = false, Message = ErrorMessages.InvalidInput });

            var device = await _applicationDbContext.Devices.Include(e => e.DeviceDetails).Include(e => e.DeviceImages).FirstOrDefaultAsync(e => e.Id == id);

            if (device is not null)
            {
                device.DeviceDetails.Device = null;
                if (device.DeviceImages is not null && device.DeviceImages.Count > 0)
                    foreach (var item in device.DeviceImages)
                    {
                        item.Device = null;
                    }
                return Ok(new ApplicationResult { IsSuccess = true, Data = device });
            }
            return Ok(new ApplicationResult { IsSuccess = false, Message = ErrorMessages.InvalidInput });
        }

        #endregion
    }
}

