using Find_Me_Mobile.Constants;
using Find_Me_Mobile.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
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

        #region Public Methods

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

            var device = await _applicationDbContext.Devices.Include(e => e.DeviceDetails).FirstOrDefaultAsync(e => e.Id == id);

            if (device is not null)
            {
                if (device.DeviceDetails is not null)
                    device.DeviceDetails.Device = null;

                return Ok(new ApplicationResult { IsSuccess = true, Data = device });
            }
            return Ok(new ApplicationResult { IsSuccess = false, Message = ErrorMessages.InvalidInput });
        }

        [HttpGet]
        [Route("AddDevices")]
        public async Task<IActionResult> AddDevices()
        {
            var companies = await _applicationDbContext.Companies.ToListAsync();

            if (companies is not null && companies.Count > 0)
            {
                foreach (var company in companies)
                {
                    var days = (DateTime.Now - company.UpdationDate).TotalDays;
                    if (days > 1)
                    {
                        if (company.Name == CompaniesName.Vivo)
                            await AddOrUpdateVivoDevices(company.Id);
                    }
                }
                return Ok(new ApplicationResult { IsSuccess = true, Message = ErrorMessages.DevicesRegistered });
            }
            else
                return Ok(new ApplicationResult { IsSuccess = true, Message = ErrorMessages.CompaniesNotFound });
        }
        #endregion

        #region Private Methods

        private async Task AddOrUpdateVivoDevices(string companyId)
        {
            HtmlWeb devicesWeb = new();
            HtmlDocument devicesDocument = devicesWeb.Load(Urls.VivoDevices);
            var devices = devicesDocument.DocumentNode.SelectNodes("//a[@class='item no-flip-over']").ToList();

            if (devices is not null && devices.Count > 0)
            {
                var company = await _applicationDbContext.Companies.FirstOrDefaultAsync(e => e.Id == companyId);
                var oldDevices = await _applicationDbContext.Devices.Where(e => e.CompanyId == companyId).ToListAsync();

                if (oldDevices is not null && oldDevices.Count > 0)
                {
                    foreach (var item in oldDevices)
                    {
                        var oldDeviceDetails = await _applicationDbContext.DeviceDetails.FirstOrDefaultAsync(e => e.DeviceId == item.Id);

                        if (oldDeviceDetails is not null)
                            _applicationDbContext.Remove(oldDeviceDetails);
                    }

                    if (company is not null)
                        company.UpdationDate = DateTime.Now;

                    _applicationDbContext.RemoveRange(oldDevices);
                }

                foreach (var item in devices)
                {
                    var heading = new Regex("<h1(.*?)>(.*?)</h1>");
                    var subHeading = new Regex("<p(.*?)>(.*?)</p>");
                    var image = new Regex("<img[^>]+>");

                    Devices device = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = heading.Match(item.InnerHtml).Groups[2].Value.Replace("[", string.Empty).Replace("]", string.Empty),
                        Model = Regex.Replace(heading.Match(item.InnerHtml).Groups[2].Value, @"\s+", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).ToLower(),
                        Price = subHeading.Match(item.InnerHtml).Groups[2].Value,
                        Image = Regex.Match(Regex.Match(item.InnerHtml, image.ToString()).Value, "<img.+?data-original=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value,
                        CreationDate = DateTime.Now,
                        UpdationDate = DateTime.Now,
                        CompanyId = companyId
                    };
                    await _applicationDbContext.Devices.AddAsync(device);
                }
                await _applicationDbContext.SaveChangesAsync();

                var newDevices = await _applicationDbContext.Devices.Where(e => e.CompanyId == companyId).Include(e => e.DeviceDetails).ToListAsync();

                if (newDevices is not null && newDevices.Count > 0)
                {
                    foreach (var item in newDevices)
                    {
                        await AddOrUpdateVivoDevice(item.Id, item.Model);
                    }
                }
            }
        }

        private async Task AddOrUpdateVivoDevice(string deviceId, string model)
        {
            HtmlWeb deviceWeb = new();
            HtmlDocument deviceDocument = deviceWeb.Load(Urls.VivoDevice + model);
            //  htmlDocument.Load(stream);
            var device = deviceDocument.DocumentNode.SelectNodes("//div[@class='item-group clearafter']/div[contains(@class, 'clearafter')]").OrderBy(e => e.InnerHtml).ToList();

            if (device is not null && device.Count > 0)
            {
                var heading = new Regex("<h1(.*?)>(.*?)</h1>");
                var subHeading = new Regex("<p(.*?)>(.*?)</p>");
                var image = new Regex("<img[^>]+>");

                DeviceDetails deviceDetails = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    DeviceId = deviceId,
                    Processor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Processor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Processor).InnerHtml).Groups[2].Value,
                    Ram = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.RAM) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.RAM).InnerHtml).Groups[2].Value,
                    Rom = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ROM) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ROM).InnerHtml).Groups[2].Value,
                    Charging = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ChargingPower) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ChargingPower).InnerHtml).Groups[2].Value,
                    Colors = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Color) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Color).InnerHtml).Groups[2].Value,
                    OperatingSystem = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.OperatingSystem) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.OperatingSystem).InnerHtml).Groups[2].Value,
                    Screen = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Screen) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Screen).InnerHtml).Groups[2].Value,
                    Resolution = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Resolution) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Resolution).InnerHtml).Groups[2].Value,
                    Type = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Type) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Type).InnerHtml).Groups[2].Value,
                    TouchScreen = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.TouchScreen) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.TouchScreen).InnerHtml).Groups[2].Value,
                    Camera = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Camera) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Camera).InnerHtml).Groups[2].Value,
                    Aperture = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Aperture) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Aperture).InnerHtml).Groups[2].Value,
                    Flash = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Flash) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Flash).InnerHtml).Groups[2].Value,
                    SceneModes = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SceneModes) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SceneModes).InnerHtml).Groups[2].Value,
                    WiFi = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.WiFi) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.WiFi).InnerHtml).Groups[2].Value,
                    Bluetooth = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Bluetooth) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Bluetooth).InnerHtml).Groups[2].Value,
                    USB = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Usb) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Usb).InnerHtml).Groups[2].Value,
                    GPS = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Gps) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Gps).InnerHtml).Groups[2].Value,
                    OTG = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Otg) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Otg).InnerHtml).Groups[2].Value,
                    FM = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fm) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fm).InnerHtml).Groups[2].Value,
                    SIMSlotType = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SIMSlotType) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SIMSlotType).InnerHtml).Groups[2].Value,
                    StandbyMode = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.StandbyMode) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.StandbyMode).InnerHtml).Groups[2].Value,
                    G2Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G2Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G2Band).InnerHtml).Groups[2].Value,
                    G3Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G3Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G3Band).InnerHtml).Groups[2].Value,
                    G4Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G4Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G4Band).InnerHtml).Groups[2].Value,
                    G5Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G5Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G5Band).InnerHtml).Groups[2].Value,
                    Fingerprint = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fingerprint) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fingerprint).InnerHtml).Groups[2].Value,
                    Accelerometer = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Accelerometer) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Accelerometer).InnerHtml).Groups[2].Value,
                    AmbientLightSensor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AmbientLightSensor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AmbientLightSensor).InnerHtml).Groups[2].Value,
                    ProximitySensor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ProximitySensor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ProximitySensor).InnerHtml).Groups[2].Value,
                    ECompass = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ECompass) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ECompass).InnerHtml).Groups[2].Value,
                    GyroscopeSensor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.GyroscopeSensor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.GyroscopeSensor).InnerHtml).Groups[2].Value,
                    AudioPlayback = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AudioPlayback) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AudioPlayback).InnerHtml).Groups[2].Value,
                    VideoPlayback = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoPlayback) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoPlayback).InnerHtml).Groups[2].Value,
                    VideoRecording = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoRecording) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoRecording).InnerHtml).Groups[2].Value,
                    VoiceRecording = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VoiceRecording) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VoiceRecording).InnerHtml).Groups[2].Value,
                };

                var oldDevices = _applicationDbContext.DeviceDetails.Where(e => e.DeviceId == deviceId);
                if (oldDevices is not null)
                    _applicationDbContext.DeviceDetails.RemoveRange(oldDevices);

                await _applicationDbContext.DeviceDetails.AddAsync(deviceDetails);
                await _applicationDbContext.SaveChangesAsync();

                //try
                //{

                //}
                //catch (Exception ex)
                //{

                //}
            }
        }

        #endregion
    }
}

