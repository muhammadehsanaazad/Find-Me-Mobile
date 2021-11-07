using Find_Me_Mobile.Constants;
using Find_Me_Mobile.Models;
using Find_Me_Mobile.ViewModels;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        [HttpPost]
        [Route("GetAllDevices")]
        public async Task<IActionResult> GetAllDevices(GetDevicesBindingModel getDevicesBindingModel)
        {
            if (string.IsNullOrWhiteSpace(getDevicesBindingModel.Company))
                getDevicesBindingModel.Company = null;
            if (string.IsNullOrWhiteSpace(getDevicesBindingModel.OperatingSystem))
                getDevicesBindingModel.OperatingSystem = null;

            var devices = await _applicationDbContext.Devices.Include(e => e.DeviceDetails).ToListAsync();

            if (!string.IsNullOrWhiteSpace(getDevicesBindingModel.Company))
            {
                devices.RemoveAll(e => e.CompanyId != getDevicesBindingModel.Company);
            }
            if (!string.IsNullOrWhiteSpace(getDevicesBindingModel.OperatingSystem))
            {
                devices.RemoveAll(e => e.DeviceDetails.OperatingSystem != getDevicesBindingModel.OperatingSystem);
            }
            if (!string.IsNullOrWhiteSpace(getDevicesBindingModel.Model))
            {
                devices.RemoveAll(e => e.Model != getDevicesBindingModel.Model);
            }
            if (!string.IsNullOrWhiteSpace(getDevicesBindingModel.Battery))
            {
                devices.RemoveAll(e => e.DeviceDetails.Battery != getDevicesBindingModel.Battery);
            }
            if (!string.IsNullOrWhiteSpace(getDevicesBindingModel.Ram))
            {
                devices.RemoveAll(e => e.DeviceDetails.Ram != getDevicesBindingModel.Ram);
            }
            if (!string.IsNullOrWhiteSpace(getDevicesBindingModel.Rom))
            {
                devices.RemoveAll(e => e.DeviceDetails.Rom != getDevicesBindingModel.Rom);
            }
            if (!string.IsNullOrWhiteSpace(getDevicesBindingModel.ScreenType))
            {
                devices.RemoveAll(e => e.DeviceDetails.Type != getDevicesBindingModel.ScreenType);
            }
            if (!string.IsNullOrWhiteSpace(getDevicesBindingModel.Category))
            {
                devices.RemoveAll(e => e.Category != getDevicesBindingModel.Category);
            }

            foreach (var item in devices)
            {
                item.DeviceDetails = null;
            }

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

            var device = await _applicationDbContext.Devices.Include(e => e.Company).Include(e => e.DeviceDetails).FirstOrDefaultAsync(e => e.Id == id);

            if (device is not null)
            {
                var hours = (DateTime.Now - device.UpdationDate).TotalHours;
                if (hours > 24)
                {
                    if (device.Company.Name == CompaniesName.Vivo)
                        await AddOrUpdateVivoDevice(device.Id, device.Model);
                    else if (device.Company.Name == CompaniesName.Oppo)
                        await AddOrUpdateOppoDevice(device.Id, device.DeviceURL);
                    //else if (device.Company.Name == CompaniesName.Huawei)
                    //    await AddOrUpdateHuaweiDevice(device.Id, device.Model);
                    else if (device.Company.Name == CompaniesName.Xiaomi)
                        await AddOrUpdateXiaomiDevice(device.Id, device.DeviceURL);
                    //else if (device.Company.Name == CompaniesName.Samsung)
                    //    await AddOrUpdateSamsungDevice(device.Id, device.Model);

                    device = await _applicationDbContext.Devices.Include(e => e.Company).Include(e => e.DeviceDetails).FirstOrDefaultAsync(e => e.Id == id);
                }

                if (device is not null)
                {
                    if (device.DeviceDetails is not null)
                        device.DeviceDetails.Device = null;

                    if (device.Company.Devices is not null)
                        device.Company.Devices = null;
                }
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
                    if (days > 7)
                    {
                        if (company.Name == CompaniesName.Vivo)
                            await AddOrUpdateVivoDevices(company.Id);
                        else if (company.Name == CompaniesName.Oppo)
                            await AddOrUpdaeOppoDevices(company.Id);
                        else if (company.Name == CompaniesName.Huawei)
                            await AddOrUpdaeHuaweiDevices(company.Id);
                        else if (company.Name == CompaniesName.Xiaomi)
                            await AddOrUpdaeXiaomiDevices(company.Id);
                        else if (company.Name == CompaniesName.Samsung)
                            await AddOrUpdaeSamsungDevices(company.Id);
                    }
                }
                return Ok(new ApplicationResult { IsSuccess = true, Message = ErrorMessages.DevicesRegistered });
            }
            else
                return Ok(new ApplicationResult { IsSuccess = true, Message = ErrorMessages.CompaniesNotFound });
        }

        [HttpGet]
        [Route("GetFilters")]
        public async Task<IActionResult> GetFilters()
        {
            FiltersBindingModel filtersBindingModel = new()
            {
                Company = new(),
                OperatingSystem = new(),
                Model = new(),
                Ram = new(),
                Rom = new(),
                ScreenType = new(),
                Battery = new(),
                Category = new()
            };
            var devices = await _applicationDbContext.Devices.Include(e => e.DeviceDetails).Include(e => e.Company).OrderByDescending(e => e.Price).ToListAsync();

            foreach (var item in devices)
            {
                // Company Filters
                var company = filtersBindingModel.Company.Find(e => e.Id == item.CompanyId);
                if (company is null)
                {
                    FilterBindingModel filterBindingModel = new()
                    {
                        Id = item.CompanyId,
                        Name = item.Company?.Name
                    };

                    filtersBindingModel.Company.Add(filterBindingModel);
                }

                // OperatingSystem Filters
                var operatingSystem = filtersBindingModel.OperatingSystem.Find(e => e.Name == item.DeviceDetails.OperatingSystem || e.DisplayName == item.DeviceDetails.OperatingSystem?.TrimEnd());
                if (operatingSystem is null)
                {
                    FilterBindingModel filterBindingModel = new()
                    {
                        Id = item.DeviceDetails.Id,
                        Name = item.DeviceDetails.OperatingSystem,
                        DisplayName = item.DeviceDetails.OperatingSystem?.TrimEnd()
                    };

                    if (item.DeviceDetails.OperatingSystem is not null)
                    {
                        var index = item.DeviceDetails.OperatingSystem.IndexOf(" (");

                        if (index > 0)
                            filterBindingModel.DisplayName = item.DeviceDetails.OperatingSystem.Substring(0, index).TrimEnd();
                    }
                    operatingSystem = filtersBindingModel.OperatingSystem.Find(e => e.Name?.TrimEnd() == filterBindingModel.DisplayName || e.DisplayName == filterBindingModel.DisplayName);

                    if (operatingSystem is null)
                        filtersBindingModel.OperatingSystem.Add(filterBindingModel);
                };

                // Model Filters
                var model = filtersBindingModel.Model.Find(e => e.Name == item.Model);
                if (model is null)
                {
                    FilterBindingModel filterBindingModel = new()
                    {
                        Id = item.Id,
                        Name = item.Model
                    };

                    filtersBindingModel.Model.Add(filterBindingModel);
                };

                // Ram Filters
                var ram = filtersBindingModel.Ram.Find(e => e.Name == item.DeviceDetails.Ram || e.DisplayName == item.DeviceDetails.Ram.TrimEnd());
                if (ram is null)
                {
                    FilterBindingModel filterBindingModel = new()
                    {
                        Id = item.DeviceDetails.Id,
                        Name = item.DeviceDetails.Ram,
                        DisplayName = item.DeviceDetails.Ram.TrimEnd()
                    };

                    var index = item.DeviceDetails.Ram.IndexOf(" ");

                    if (index > 0)
                        filterBindingModel.DisplayName = item.DeviceDetails.Ram.Substring(0, index).TrimEnd();

                    ram = filtersBindingModel.Ram.Find(e => e.Name.TrimEnd() == filterBindingModel.DisplayName || e.DisplayName == filterBindingModel.DisplayName);

                    if (ram is null)
                        filtersBindingModel.Ram.Add(filterBindingModel);
                };

                // Rom Filters
                var rom = filtersBindingModel.Rom.Find(e => e.Name == item.DeviceDetails.Rom || e.DisplayName == item.DeviceDetails.Rom.TrimEnd());
                if (rom is null)
                {
                    FilterBindingModel filterBindingModel = new()
                    {
                        Id = item.DeviceDetails.Id,
                        Name = item.DeviceDetails.Rom,
                        DisplayName = item.DeviceDetails.Rom
                    };

                    var index = item.DeviceDetails.Rom.IndexOf("<");

                    if (index < 1)
                        index = item.DeviceDetails.Rom.IndexOf(" ");
                    if (index > 0)
                        filterBindingModel.DisplayName = item.DeviceDetails.Rom.Substring(0, index).TrimEnd();

                    ram = filtersBindingModel.Rom.Find(e => e.Name.TrimEnd() == filterBindingModel.DisplayName || e.DisplayName == filterBindingModel.DisplayName);

                    if (ram is null)
                        filtersBindingModel.Rom.Add(filterBindingModel);
                };

                // ScreenType Filters
                var screenType = filtersBindingModel.ScreenType.Find(e => e.Name == item.DeviceDetails.Type || e.DisplayName == item.DeviceDetails.Type?.TrimEnd());
                if (screenType is null)
                {
                    FilterBindingModel filterBindingModel = new()
                    {
                        Id = item.DeviceDetails.Id,
                        Name = item.DeviceDetails.Type,
                        DisplayName = item.DeviceDetails.Type?.TrimEnd()
                    };

                    filtersBindingModel.ScreenType.Add(filterBindingModel);
                }

                // ScreenType Filters
                var battery = filtersBindingModel.Battery.Find(e => e.Name == item.DeviceDetails.Battery || e.Name == item.DeviceDetails.Battery.Trim());
                if (battery is null && item.DeviceDetails.Battery.Contains("mAh"))
                {
                    FilterBindingModel filterBindingModel = new()
                    {
                        Id = item.DeviceDetails.Id,
                        Name = item.DeviceDetails.Battery,
                        DisplayName = item.DeviceDetails.Battery.Trim()
                    };

                    var index = item.DeviceDetails.Battery.IndexOf("(");

                    if (index > 0)
                        filterBindingModel.DisplayName = item.DeviceDetails.Battery.Substring(0, index).Trim();

                    ram = filtersBindingModel.Battery.Find(e => e.Name.Trim() == filterBindingModel.DisplayName || e.DisplayName == filterBindingModel.DisplayName);

                    if (ram is null)
                        filtersBindingModel.Battery.Add(filterBindingModel);
                }

                // ScreenType Filters
                var category = filtersBindingModel.Category.Find(e => e.Name == item.Category || e.Name == item.Category?.TrimEnd());
                if (category is null)
                {
                    FilterBindingModel filterBindingModel = new()
                    {
                        Id = item.Id,
                        Name = item.Category,
                        DisplayName = item.Category?.TrimEnd()
                    };

                    filtersBindingModel.Category.Add(filterBindingModel);
                }
            }

            filtersBindingModel.Company = filtersBindingModel.Company.Where(e => !string.IsNullOrWhiteSpace(e.Name)).Distinct().ToList();
            filtersBindingModel.OperatingSystem = filtersBindingModel.OperatingSystem.Where(e => !string.IsNullOrWhiteSpace(e.DisplayName)).Distinct().ToList();
            filtersBindingModel.Ram = filtersBindingModel.Ram.Where(e => !string.IsNullOrWhiteSpace(e.DisplayName)).Distinct().ToList();
            filtersBindingModel.Rom = filtersBindingModel.Rom.Where(e => !string.IsNullOrWhiteSpace(e.DisplayName)).Distinct().ToList();
            filtersBindingModel.ScreenType = filtersBindingModel.ScreenType.Where(e => !string.IsNullOrWhiteSpace(e.DisplayName)).Distinct().ToList();
            filtersBindingModel.Battery = filtersBindingModel.Battery.Where(e => !string.IsNullOrWhiteSpace(e.DisplayName)).Distinct().ToList();
            filtersBindingModel.Category = filtersBindingModel.Category.Where(e => !string.IsNullOrWhiteSpace(e.DisplayName)).Distinct().ToList();

            return Ok(new ApplicationResult { IsSuccess = true, Data = filtersBindingModel });
        }
        #endregion

        #region Private Methods

        private async Task AddOrUpdateVivoDevices(string companyId)
        {
            HtmlWeb devicesWeb = new();
            HtmlDocument devicesDocument = devicesWeb.Load(Urls.Vivo);
            var devices = devicesDocument?.DocumentNode?.SelectNodes("//a[@class='item no-flip-over']")?.ToList();

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

                    if (string.IsNullOrWhiteSpace(device.Price))
                    {
                        device.Category = "Other";
                    }
                    else
                    {
                        var firstSpaceIndex = device.Price.IndexOf(" ");
                        var price = double.Parse(device.Price.Substring(firstSpaceIndex + 1));

                        //device.Price = price.ToString();

                        if (price <= 25000)
                            device.Category = "Lower Mid-Range";
                        else if (price <= 40000)
                            device.Category = "Mid-Range";
                        else device.Category = "Flagship";
                    }
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

        private async Task AddOrUpdaeOppoDevices(string companyId)
        {
            HtmlWeb devicesWeb = new();
            HtmlDocument devicesDocument = devicesWeb.Load(Urls.Oppo);
            var devices = devicesDocument?.DocumentNode?.SelectNodes("//div[@class='list-item']")?.ToList();

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
                    var heading = new Regex("<a(.*?)>(.*?)</a>");
                    var subHeading = new Regex("<div(.*?)>(.*?)</div>");
                    var a = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);
                    var image = new Regex("<img[^>]+>");

                    var match = a.Match(heading.Match(item.InnerHtml).Groups[0].Value.Replace("[", string.Empty).Replace("]", string.Empty)).Groups[1].ToString();

                    Devices device = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        DeviceURL = a.Match(heading.Match(item.InnerHtml).Groups[0].Value.Replace("[", string.Empty).Replace("]", string.Empty)).Groups[1].ToString(),
                        Image = Regex.Match(Regex.Match(subHeading.Match(item.InnerHtml).Groups[2].Value.Replace("[", string.Empty).Replace("]", string.Empty).ToString(), image.ToString()).Value, "<img.+?data-src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value,
                        CreationDate = DateTime.Now,
                        UpdationDate = DateTime.Now,
                        CompanyId = companyId
                    };

                    var model = device.DeviceURL.Replace(Urls.Oppo, string.Empty);

                    device.Model = model.Substring(model.IndexOf("/") + 1).Replace("/", string.Empty);
                    device.Name = device.Model.Replace("-", " ").ToUpper();

                    await _applicationDbContext.Devices.AddAsync(device);
                }
                await _applicationDbContext.SaveChangesAsync();

                var newDevices = await _applicationDbContext.Devices.Where(e => e.CompanyId == companyId).Include(e => e.DeviceDetails).ToListAsync();

                if (newDevices is not null && newDevices.Count > 0)
                {
                    foreach (var item in newDevices)
                    {
                        await AddOrUpdateOppoDevice(item.Id, item.DeviceURL);
                    }
                }
            }
        }

        private async Task AddOrUpdaeHuaweiDevices(string companyId)
        {
            HtmlWeb devicesWeb = new();
            HtmlDocument devicesDocument = devicesWeb.Load(Urls.Huawei);
            var devices = devicesDocument?.DocumentNode?.SelectNodes("//div[@class='product-col product-col--small product-col--featured-item col-6 col-xl-3 current_page_product']")?.ToList();
            var desvices = devicesDocument?.DocumentNode?.SelectNodes("//div[contains(@class, 'product-block__in js-product-block')]")?.ToList();
            var deviddces = devicesDocument?.DocumentNode?.SelectNodes("//div[contains(@class, 'product-block__in js-product-block')]")?.ToList();

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

        private async Task AddOrUpdaeXiaomiDevices(string companyId)
        {
            HtmlWeb devicesWeb = new();
            HtmlDocument devicesDocument = devicesWeb.Load(Urls.Xiaomi);
            var devices = devicesDocument.DocumentNode.SelectNodes("//div[@class='product-cell']").ToList();

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
                    var heading = new Regex("<h3(.*?)>(.*?)</h3>");
                    var subHeading = new Regex("<p(.*?)>(.*?)</p>");
                    var image = new Regex("<img[^>]+>");
                    var a = new Regex("<a(.*?)>(.*?)</a>");

                    var Price = subHeading.Match(item.InnerHtml);
                    var Price2 = subHeading.Match(item.InnerHtml).Groups;
                    var Pric34e = subHeading.Match(item.InnerHtml).Groups[1].Value;
                    var Price5 = subHeading.Match(item.InnerHtml).Groups[2].Value.Replace("<strong>", string.Empty).Replace("</strong>", string.Empty);
                    var Name = a.Match(heading.Match(item.InnerHtml).Groups[2].Value).Groups;
                    var mmm = Regex.Replace(item.InnerHtml, "<.*?>|&.*?;", string.Empty);

                    var deviceInfo = Regex.Replace(item.InnerHtml, "<.*?>|&.*?;", string.Empty).Split("\n").ToList();

                    List<string> deviceInformation = new();
                    if (deviceInfo is not null)
                    {
                        foreach (var deviceItem in deviceInfo)
                        {
                            if (!string.IsNullOrWhiteSpace(deviceItem) && deviceItem.Trim().ToLower() != "Out Of Stock".Trim().ToLower())
                                deviceInformation.Add(deviceItem);
                        }
                    }

                    Devices device = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = deviceInformation == null || deviceInformation.Count < 1 ? string.Empty : deviceInformation[0].Trim(),
                        Model = deviceInformation == null || deviceInformation.Count < 1 ? string.Empty : deviceInformation[0].Replace(" ", string.Empty).ToLower(),
                        Price = deviceInformation == null || deviceInformation.Count < 3 ? string.Empty : deviceInformation[2].Trim(),
                        Image = Regex.Match(Regex.Match(item.InnerHtml, image.ToString()).Value, "<img.+?data-src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value,
                        CreationDate = DateTime.Now,
                        UpdationDate = DateTime.Now,
                        CompanyId = companyId
                    };

                    if (string.IsNullOrWhiteSpace(device.Price))
                    {
                        device.Category = "Other";
                    }
                    else
                    {
                        var price = double.Parse(device.Price.Substring(device.Price.IndexOf(" ") + 1));

                        if (price <= 25000)
                            device.Category = "Lower Mid-Range";
                        else if (price <= 40000)
                            device.Category = "Mid-Range";
                        else device.Category = "Flagship";
                    }

                    var lastIndex = device.Name.IndexOf("(");

                    if (lastIndex > 1)
                    {
                        device.Name = device.Name.Substring(0, lastIndex).Trim();
                        device.Model = device.Name.Replace(" ", string.Empty).ToLower();
                    }

                    device.DeviceURL = Urls.XiaomiDevice + device.Image.Split("/goods_desktop/")[1].Split("_thumb_M_")[0];

                    await _applicationDbContext.Devices.AddAsync(device);
                }
                await _applicationDbContext.SaveChangesAsync();

                var newDevices = await _applicationDbContext.Devices.Where(e => e.CompanyId == companyId).Include(e => e.DeviceDetails).ToListAsync();

                if (newDevices is not null && newDevices.Count > 0)
                {
                    foreach (var item in newDevices)
                    {
                        await AddOrUpdateXiaomiDevice(item.Id, item.DeviceURL);
                    }
                }
            }
        }

        private async Task AddOrUpdaeSamsungDevices(string companyId)
        {
            HtmlWeb devicesWeb = new();
            HtmlDocument devicesDocument = devicesWeb.Load(Urls.Samsung);
            var devices = devicesDocument?.DocumentNode?.SelectNodes("//div[contains(@class, 'product-card-v2__price-full')]")?.ToList();
            var deviasdce = devicesDocument?.DocumentNode?.SelectNodes("//div[contains(@class, 'product-card-v2__content')]")?.ToList();

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

            var device = deviceDocument?.DocumentNode?.SelectNodes("//div[@class='item-group clearafter']/div[contains(@class, 'clearafter')]")?.OrderBy(e => e.InnerHtml)?.ToList();

            if (device is not null && device.Count > 0)
            {
                var heading = new Regex("<h1(.*?)>(.*?)</h1>");
                var subHeading = new Regex("<p(.*?)>(.*?)</p>");

                var deviceDetails = await _applicationDbContext.DeviceDetails.FirstOrDefaultAsync(e => e.DeviceId == deviceId);
                if (deviceDetails is null)
                {
                    deviceDetails = new()
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
                        Battery = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Battery) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Battery).InnerHtml).Groups[2].Value,
                        Reviews = "https://www.youtube.com/c/ReviewsPK/featured",
                    };
                    await _applicationDbContext.DeviceDetails.AddAsync(deviceDetails);
                }
                else
                {
                    deviceDetails.Processor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Processor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Processor).InnerHtml).Groups[2].Value;
                    deviceDetails.Ram = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.RAM) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.RAM).InnerHtml).Groups[2].Value;
                    deviceDetails.Rom = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ROM) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ROM).InnerHtml).Groups[2].Value;
                    deviceDetails.Charging = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ChargingPower) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ChargingPower).InnerHtml).Groups[2].Value;
                    deviceDetails.Colors = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Color) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Color).InnerHtml).Groups[2].Value;
                    deviceDetails.OperatingSystem = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.OperatingSystem) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.OperatingSystem).InnerHtml).Groups[2].Value;
                    deviceDetails.Screen = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Screen) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Screen).InnerHtml).Groups[2].Value;
                    deviceDetails.Resolution = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Resolution) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Resolution).InnerHtml).Groups[2].Value;
                    deviceDetails.Type = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Type) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Type).InnerHtml).Groups[2].Value;
                    deviceDetails.TouchScreen = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.TouchScreen) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.TouchScreen).InnerHtml).Groups[2].Value;
                    deviceDetails.Camera = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Camera) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Camera).InnerHtml).Groups[2].Value;
                    deviceDetails.Aperture = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Aperture) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Aperture).InnerHtml).Groups[2].Value;
                    deviceDetails.Flash = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Flash) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Flash).InnerHtml).Groups[2].Value;
                    deviceDetails.SceneModes = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SceneModes) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SceneModes).InnerHtml).Groups[2].Value;
                    deviceDetails.WiFi = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.WiFi) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.WiFi).InnerHtml).Groups[2].Value;
                    deviceDetails.Bluetooth = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Bluetooth) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Bluetooth).InnerHtml).Groups[2].Value;
                    deviceDetails.USB = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Usb) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Usb).InnerHtml).Groups[2].Value;
                    deviceDetails.GPS = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Gps) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Gps).InnerHtml).Groups[2].Value;
                    deviceDetails.OTG = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Otg) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Otg).InnerHtml).Groups[2].Value;
                    deviceDetails.FM = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fm) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fm).InnerHtml).Groups[2].Value;
                    deviceDetails.SIMSlotType = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SIMSlotType) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SIMSlotType).InnerHtml).Groups[2].Value;
                    deviceDetails.StandbyMode = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.StandbyMode) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.StandbyMode).InnerHtml).Groups[2].Value;
                    deviceDetails.G2Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G2Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G2Band).InnerHtml).Groups[2].Value;
                    deviceDetails.G3Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G3Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G3Band).InnerHtml).Groups[2].Value;
                    deviceDetails.G4Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G4Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G4Band).InnerHtml).Groups[2].Value;
                    deviceDetails.G5Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G5Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G5Band).InnerHtml).Groups[2].Value;
                    deviceDetails.Fingerprint = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fingerprint) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fingerprint).InnerHtml).Groups[2].Value;
                    deviceDetails.Accelerometer = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Accelerometer) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Accelerometer).InnerHtml).Groups[2].Value;
                    deviceDetails.AmbientLightSensor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AmbientLightSensor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AmbientLightSensor).InnerHtml).Groups[2].Value;
                    deviceDetails.ProximitySensor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ProximitySensor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ProximitySensor).InnerHtml).Groups[2].Value;
                    deviceDetails.ECompass = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ECompass) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ECompass).InnerHtml).Groups[2].Value;
                    deviceDetails.GyroscopeSensor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.GyroscopeSensor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.GyroscopeSensor).InnerHtml).Groups[2].Value;
                    deviceDetails.AudioPlayback = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AudioPlayback) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AudioPlayback).InnerHtml).Groups[2].Value;
                    deviceDetails.VideoPlayback = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoPlayback) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoPlayback).InnerHtml).Groups[2].Value;
                    deviceDetails.VideoRecording = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoRecording) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoRecording).InnerHtml).Groups[2].Value;
                    deviceDetails.Battery = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Battery) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Battery).InnerHtml).Groups[2].Value;
                }
                await _applicationDbContext.SaveChangesAsync();
            }
            else
            {
                var oldDevice = await _applicationDbContext.Devices.Include(e => e.DeviceDetails).FirstOrDefaultAsync(e => e.Id == deviceId);
                if (oldDevice is not null)
                {
                    _applicationDbContext.Remove(oldDevice);
                    await _applicationDbContext.SaveChangesAsync();
                }
            }
        }

        private async Task AddOrUpdateOppoDevice(string deviceId, string deviceUrl)
        {
            HtmlWeb deviceWeb = new();
            HtmlDocument deviceDocument = deviceWeb.Load(deviceUrl + "specs/");

            var device = deviceDocument?.DocumentNode?.SelectNodes("//div[@class='container']/div[contains(@class, 'overview')]")?.OrderBy(e => e.InnerHtml)?.ToList();
            if (device is not null && device.Count > 0)
            {
                var deviceMemory = deviceDocument?.DocumentNode?.SelectNodes("//div[@class='container']/section[contains(@class, 'memory')]/div[contains(@class, 'right')]/span[contains(@class, 'desc')]")?.OrderBy(e => e.InnerHtml)?.ToList();
                var deviceDisplay = deviceDocument?.DocumentNode?.SelectNodes("//div[@class='container']/section[contains(@class, 'display')]/div[contains(@class, 'right')]/span[contains(@class, 'desc')]")?.OrderBy(e => e.InnerHtml)?.ToList();
                var deviceCameraRear = deviceDocument?.DocumentNode?.SelectNodes("//div[@class='container']/section[contains(@class, 'camera')]/div[contains(@class, 'right')]/div[contains(@class, 'wrapper descsection1')]/span[contains(@class, 'desc')]")?.OrderBy(e => e.InnerHtml)?.ToList();
                var deviceCameraFront = deviceDocument?.DocumentNode?.SelectNodes("//div[@class='container']/section[contains(@class, 'camera')]/div[contains(@class, 'right')]/div[contains(@class, 'wrapper descsection2')]/span[contains(@class, 'desc')]")?.OrderBy(e => e.InnerHtml)?.ToList();
                var deviceSim = deviceDocument?.DocumentNode?.SelectNodes("//div[@class='container']/section[contains(@class, 'network')]/div[contains(@class, 'right')]/div[contains(@class, 'wrapper descsection0')]/span[contains(@class, 'desc')]")?.OrderBy(e => e.InnerHtml)?.ToList();
                var deviceNetwork = deviceDocument?.DocumentNode?.SelectNodes("//div[@class='container']/section[contains(@class, 'network')]/div[contains(@class, 'right')]/div[contains(@class, 'wrapper descsection1')]/span[contains(@class, 'desc')]")?.OrderBy(e => e.InnerHtml)?.ToList();
                var deviceWifi = deviceDocument?.DocumentNode?.SelectNodes("//div[@class='container']/section[contains(@class, 'connectivity')]/div[contains(@class, 'right')]/div[contains(@class, 'wrapper descsection1')]/span[contains(@class, 'desc')]")?.OrderBy(e => e.InnerHtml)?.ToList();
                var deviceConnectivity = deviceDocument?.DocumentNode?.SelectNodes("//div[@class='container']/section[contains(@class, 'connectivity')]/div[contains(@class, 'right')]/div[contains(@class, 'wrapper descsection2')]/span[contains(@class, 'desc')]")?.OrderBy(e => e.InnerHtml)?.ToList();
                var deviceSystem = deviceDocument?.DocumentNode?.SelectNodes("//div[@class='container']/section[contains(@class, 'system')]/div[contains(@class, 'right')]/span[contains(@class, 'desc')]")?.OrderBy(e => e.InnerHtml)?.ToList();
                var deviceBiometrics = deviceDocument?.DocumentNode?.SelectNodes("//div[@class='container']/section[contains(@class, 'biometrics')]/div[contains(@class, 'right')]/span[contains(@class, 'desc')]")?.OrderBy(e => e.InnerHtml)?.ToList();
                var deviceSensors = deviceDocument?.DocumentNode?.SelectNodes("//div[@class='container']/section[contains(@class, 'sensors')]/div[contains(@class, 'right')]/span[contains(@class, 'desc')]")?.OrderBy(e => e.InnerHtml)?.ToList();

                var heading = new Regex("<li(.*?)>(.*?)</li>");

                var deviceDetails = await _applicationDbContext.DeviceDetails.FirstOrDefaultAsync(e => e.DeviceId == deviceId);
                if (deviceDetails is null)
                {
                    deviceDetails = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        DeviceId = deviceId,
                        Ram = deviceMemory == null || deviceMemory.Count < 1 ? string.Empty : deviceMemory[0].InnerHtml?.Substring(0, deviceMemory[0].InnerHtml.IndexOf("+")).Replace(" ", string.Empty).Trim(),
                        Rom = deviceMemory == null || deviceMemory.Count < 1 ? string.Empty : deviceMemory[0].InnerHtml?.Substring(deviceMemory[0].InnerHtml.LastIndexOf("+") + 2).Replace(" ", string.Empty).Trim(),
                        OperatingSystem = deviceDisplay == null || deviceDisplay.Count < 1 ? string.Empty : deviceSystem[0].InnerHtml.Trim(),
                        Screen = deviceMemory == null || deviceMemory.Count < 2 ? string.Empty : heading.Match(deviceMemory[1].InnerHtml).Groups[2].Value,
                        Resolution = deviceDisplay == null || deviceDisplay.Count < 8 ? string.Empty : deviceDisplay[7].InnerHtml.ToString().Replace("Resolution: ", string.Empty).Trim(),
                        Camera = (deviceCameraFront.Count == 2 ? "Front " + deviceCameraFront[0].InnerHtml + " & Rear " : string.Empty) + (deviceCameraRear.Count == 1 ? deviceCameraRear[0].InnerHtml : (deviceCameraRear.Count == 2 ? (deviceCameraRear[0].InnerHtml + ", " + deviceCameraRear[1].InnerHtml.Trim()) : (deviceCameraRear.Count == 3 ? (deviceCameraRear[0].InnerHtml + ", " + deviceCameraRear[1].InnerHtml + ", " + deviceCameraRear[2].InnerHtml.Trim()) : (deviceCameraRear[0].InnerHtml + ", " + deviceCameraRear[1].InnerHtml + ", " + deviceCameraRear[2].InnerHtml).Trim()))),
                        WiFi = deviceWifi == null || deviceWifi.Count < 1 ? string.Empty : deviceWifi[0].InnerHtml.Replace("WLAN:", string.Empty).Trim(),
                        Bluetooth = deviceConnectivity == null || deviceConnectivity.Count < 2 ? string.Empty : deviceConnectivity[1].InnerHtml.Replace("Bluetooth Version:", string.Empty).Trim(),
                        USB = deviceMemory == null || deviceMemory.Count < 6 ? string.Empty : deviceMemory[5].InnerHtml.ToString().Replace("USB Version: ", string.Empty).Trim(),
                        OTG = deviceMemory == null || deviceMemory.Count < 5 ? string.Empty : deviceMemory[4].InnerHtml.ToString().Replace("USB OTG: ", string.Empty).Trim(),
                        SIMSlotType = deviceSim == null || deviceSim.Count < 1 ? string.Empty : deviceSim[0].InnerHtml.Trim(),
                        G2Band = deviceNetwork == null || deviceNetwork.Count < 1 ? string.Empty : deviceNetwork[0].InnerHtml.Replace("2G: ", string.Empty).Trim(),
                        G3Band = deviceNetwork == null || deviceNetwork.Count < 2 ? string.Empty : deviceNetwork[1].InnerHtml.Replace("3G: ", string.Empty).Trim(),
                        G4Band = deviceNetwork == null || deviceNetwork.Count < 3 ? string.Empty : deviceNetwork[2].InnerHtml.Replace("4G: ", string.Empty).Trim(),
                        Fingerprint = deviceBiometrics == null || deviceBiometrics.Count < 1 ? string.Empty : deviceBiometrics[0].InnerHtml.Trim(),
                        Battery = deviceMemory == null || deviceMemory.Count < 1 ? string.Empty : heading.Match(deviceMemory[0].InnerHtml).Groups[2].Value.Trim(),
                        AmbientLightSensor = deviceSensors == null ? string.Empty : deviceSensors.Find(e => e.InnerHtml.ToLower().Trim() == "Light sensor".ToLower().Trim()) == null ? string.Empty : "Supported",
                        ProximitySensor = deviceSensors == null ? string.Empty : deviceSensors.Find(e => e.InnerHtml.ToLower().Trim() == "Proximity sensor".ToLower().Trim()) == null ? string.Empty : "Supported",
                        Accelerometer = deviceSensors == null ? string.Empty : deviceSensors.Find(e => e.InnerHtml.ToLower().Trim() == "Accelerometer".ToLower().Trim()) == null ? string.Empty : "Supported",
                        GyroscopeSensor = deviceSensors == null ? string.Empty : deviceSensors.Find(e => e.InnerHtml.ToLower().Trim() == "Geomagnetic sensor".ToLower().Trim()) == null ? string.Empty : "Supported",
                        Reviews = "https://www.youtube.com/c/ReviewsPK/featured",
                    };
                    await _applicationDbContext.DeviceDetails.AddAsync(deviceDetails);
                }
                else
                {
                    deviceDetails.Ram = deviceMemory == null || deviceMemory.Count < 1 ? string.Empty : deviceMemory[0].InnerHtml?.Substring(0, deviceMemory[0].InnerHtml.IndexOf("+")).Replace(" ", string.Empty).Trim();
                    deviceDetails.Rom = deviceMemory == null || deviceMemory.Count < 1 ? string.Empty : deviceMemory[0].InnerHtml?.Substring(deviceMemory[0].InnerHtml.LastIndexOf("+") + 2).Replace(" ", string.Empty).Trim();
                    deviceDetails.OperatingSystem = deviceDisplay == null || deviceDisplay.Count < 1 ? string.Empty : deviceSystem[0].InnerHtml.Trim();
                    deviceDetails.Screen = deviceMemory == null || deviceMemory.Count < 2 ? string.Empty : heading.Match(deviceMemory[1].InnerHtml).Groups[2].Value;
                    deviceDetails.Resolution = deviceDisplay == null || deviceDisplay.Count < 8 ? string.Empty : deviceDisplay[7].InnerHtml.ToString().Replace("Resolution: ", string.Empty).Trim();
                    deviceDetails.Camera = (deviceCameraFront.Count == 2 ? "Front " + deviceCameraFront[0].InnerHtml + " & Rear " : string.Empty) + (deviceCameraRear.Count == 1 ? deviceCameraRear[0].InnerHtml : (deviceCameraRear.Count == 2 ? (deviceCameraRear[0].InnerHtml + ", " + deviceCameraRear[1].InnerHtml.Trim()) : (deviceCameraRear.Count == 3 ? (deviceCameraRear[0].InnerHtml + ", " + deviceCameraRear[1].InnerHtml + ", " + deviceCameraRear[2].InnerHtml.Trim()) : (deviceCameraRear[0].InnerHtml + ", " + deviceCameraRear[1].InnerHtml + ", " + deviceCameraRear[2].InnerHtml).Trim())));
                    deviceDetails.WiFi = deviceWifi == null || deviceWifi.Count < 1 ? string.Empty : deviceWifi[0].InnerHtml.Replace("WLAN:", string.Empty).Trim();
                    deviceDetails.Bluetooth = deviceConnectivity == null || deviceConnectivity.Count < 2 ? string.Empty : deviceConnectivity[1].InnerHtml.Replace("Bluetooth Version:", string.Empty).Trim();
                    deviceDetails.USB = deviceMemory == null || deviceMemory.Count < 6 ? string.Empty : deviceMemory[5].InnerHtml.ToString().Replace("USB Version: ", string.Empty).Trim();
                    deviceDetails.OTG = deviceMemory == null || deviceMemory.Count < 5 ? string.Empty : deviceMemory[4].InnerHtml.ToString().Replace("USB OTG: ", string.Empty).Trim();
                    deviceDetails.SIMSlotType = deviceSim == null || deviceSim.Count < 1 ? string.Empty : deviceSim[0].InnerHtml.Trim();
                    deviceDetails.G2Band = deviceNetwork == null || deviceNetwork.Count < 1 ? string.Empty : deviceNetwork[0].InnerHtml.Replace("2G: ", string.Empty).Trim();
                    deviceDetails.G3Band = deviceNetwork == null || deviceNetwork.Count < 2 ? string.Empty : deviceNetwork[1].InnerHtml.Replace("3G: ", string.Empty).Trim();
                    deviceDetails.G4Band = deviceNetwork == null || deviceNetwork.Count < 3 ? string.Empty : deviceNetwork[2].InnerHtml.Replace("4G: ", string.Empty).Trim();
                    deviceDetails.Fingerprint = deviceBiometrics == null || deviceBiometrics.Count < 1 ? string.Empty : deviceBiometrics[0].InnerHtml.Trim();
                    deviceDetails.Battery = deviceMemory == null || deviceMemory.Count < 1 ? string.Empty : heading.Match(deviceMemory[0].InnerHtml).Groups[2].Value.Trim();
                    deviceDetails.AmbientLightSensor = deviceSensors == null ? string.Empty : deviceSensors.Find(e => e.InnerHtml.ToLower().Trim() == "Light sensor".ToLower().Trim()) == null ? string.Empty : "Supported";
                    deviceDetails.ProximitySensor = deviceSensors == null ? string.Empty : deviceSensors.Find(e => e.InnerHtml.ToLower().Trim() == "Proximity sensor".ToLower().Trim()) == null ? string.Empty : "Supported";
                    deviceDetails.Accelerometer = deviceSensors == null ? string.Empty : deviceSensors.Find(e => e.InnerHtml.ToLower().Trim() == "Accelerometer".ToLower().Trim()) == null ? string.Empty : "Supported";
                    deviceDetails.GyroscopeSensor = deviceSensors == null ? string.Empty : deviceSensors.Find(e => e.InnerHtml.ToLower().Trim() == "Geomagnetic sensor".ToLower().Trim()) == null ? string.Empty : "Supported";
                }
            }
            else
            {
                var oldDevice = await _applicationDbContext.Devices.Include(e => e.DeviceDetails).FirstOrDefaultAsync(e => e.Id == deviceId);
                if (oldDevice is not null)
                    _applicationDbContext.Remove(oldDevice);
            }
            await _applicationDbContext.SaveChangesAsync();
        }

        private async Task AddOrUpdateHuaweiDevice(string deviceId, string model)
        {
            HtmlWeb deviceWeb = new();
            HtmlDocument deviceDocument = deviceWeb.Load(Urls.HuaweiDevice + model);

            var device = deviceDocument.DocumentNode.SelectNodes("//div[@class='item-group clearafter']/div[contains(@class, 'clearafter')]").OrderBy(e => e.InnerHtml).ToList();

            if (device is not null && device.Count > 0)
            {
                var heading = new Regex("<h1(.*?)>(.*?)</h1>");
                var subHeading = new Regex("<p(.*?)>(.*?)</p>");
                // var image = new Regex("<img[^>]+>");

                var deviceDetails = await _applicationDbContext.DeviceDetails.FirstOrDefaultAsync(e => e.DeviceId == deviceId);
                if (deviceDetails is null)
                {
                    deviceDetails = new()
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
                        Battery = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Battery) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Battery).InnerHtml).Groups[2].Value,
                    };
                    await _applicationDbContext.DeviceDetails.AddAsync(deviceDetails);
                }
                else
                {
                    deviceDetails.Processor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Processor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Processor).InnerHtml).Groups[2].Value;
                    deviceDetails.Ram = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.RAM) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.RAM).InnerHtml).Groups[2].Value;
                    deviceDetails.Rom = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ROM) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ROM).InnerHtml).Groups[2].Value;
                    deviceDetails.Charging = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ChargingPower) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ChargingPower).InnerHtml).Groups[2].Value;
                    deviceDetails.Colors = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Color) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Color).InnerHtml).Groups[2].Value;
                    deviceDetails.OperatingSystem = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.OperatingSystem) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.OperatingSystem).InnerHtml).Groups[2].Value;
                    deviceDetails.Screen = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Screen) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Screen).InnerHtml).Groups[2].Value;
                    deviceDetails.Resolution = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Resolution) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Resolution).InnerHtml).Groups[2].Value;
                    deviceDetails.Type = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Type) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Type).InnerHtml).Groups[2].Value;
                    deviceDetails.TouchScreen = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.TouchScreen) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.TouchScreen).InnerHtml).Groups[2].Value;
                    deviceDetails.Camera = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Camera) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Camera).InnerHtml).Groups[2].Value;
                    deviceDetails.Aperture = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Aperture) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Aperture).InnerHtml).Groups[2].Value;
                    deviceDetails.Flash = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Flash) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Flash).InnerHtml).Groups[2].Value;
                    deviceDetails.SceneModes = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SceneModes) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SceneModes).InnerHtml).Groups[2].Value;
                    deviceDetails.WiFi = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.WiFi) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.WiFi).InnerHtml).Groups[2].Value;
                    deviceDetails.Bluetooth = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Bluetooth) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Bluetooth).InnerHtml).Groups[2].Value;
                    deviceDetails.USB = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Usb) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Usb).InnerHtml).Groups[2].Value;
                    deviceDetails.GPS = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Gps) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Gps).InnerHtml).Groups[2].Value;
                    deviceDetails.OTG = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Otg) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Otg).InnerHtml).Groups[2].Value;
                    deviceDetails.FM = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fm) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fm).InnerHtml).Groups[2].Value;
                    deviceDetails.SIMSlotType = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SIMSlotType) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SIMSlotType).InnerHtml).Groups[2].Value;
                    deviceDetails.StandbyMode = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.StandbyMode) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.StandbyMode).InnerHtml).Groups[2].Value;
                    deviceDetails.G2Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G2Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G2Band).InnerHtml).Groups[2].Value;
                    deviceDetails.G3Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G3Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G3Band).InnerHtml).Groups[2].Value;
                    deviceDetails.G4Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G4Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G4Band).InnerHtml).Groups[2].Value;
                    deviceDetails.G5Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G5Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G5Band).InnerHtml).Groups[2].Value;
                    deviceDetails.Fingerprint = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fingerprint) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fingerprint).InnerHtml).Groups[2].Value;
                    deviceDetails.Accelerometer = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Accelerometer) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Accelerometer).InnerHtml).Groups[2].Value;
                    deviceDetails.AmbientLightSensor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AmbientLightSensor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AmbientLightSensor).InnerHtml).Groups[2].Value;
                    deviceDetails.ProximitySensor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ProximitySensor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ProximitySensor).InnerHtml).Groups[2].Value;
                    deviceDetails.ECompass = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ECompass) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ECompass).InnerHtml).Groups[2].Value;
                    deviceDetails.GyroscopeSensor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.GyroscopeSensor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.GyroscopeSensor).InnerHtml).Groups[2].Value;
                    deviceDetails.AudioPlayback = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AudioPlayback) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AudioPlayback).InnerHtml).Groups[2].Value;
                    deviceDetails.VideoPlayback = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoPlayback) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoPlayback).InnerHtml).Groups[2].Value;
                    deviceDetails.VideoRecording = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoRecording) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoRecording).InnerHtml).Groups[2].Value;
                    deviceDetails.Battery = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Battery) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Battery).InnerHtml).Groups[2].Value;
                }
                await _applicationDbContext.SaveChangesAsync();
            }
        }

        private async Task AddOrUpdateXiaomiDevice(string deviceId, string deviceUrl)
        {
            HtmlWeb deviceWeb = new();
            HtmlDocument deviceDocument = deviceWeb.Load(deviceUrl);

            var device = deviceDocument?.DocumentNode?.SelectNodes("//div[contains(@class, 'J_section-box')]")?.ToList();
            if (device is null)
                device = deviceDocument?.DocumentNode?.SelectNodes("//main[contains(@class, 'J_section-box')]")?.ToList();

            if (device is not null && device.Count > 0)
            {
                var deviceDetails = await _applicationDbContext.DeviceDetails.FirstOrDefaultAsync(e => e.DeviceId == deviceId);

                var deviceInformation = device[0].InnerText.Split("\n").ToList();
                deviceInformation = deviceInformation.Where(e => !string.IsNullOrWhiteSpace(e)).Distinct().ToList();

                var featuresIndex = deviceInformation.FindIndex(e => e.ToLower().Contains("Features".ToLower()));
                var displayIndex = deviceInformation.FindIndex(e => e.ToLower().Contains("Display".ToLower()));
                var batteryIndex = deviceInformation.FindIndex(e => e.ToLower().Contains("Battery".ToLower()));
                var rearCameraIndex = deviceInformation.FindIndex(e => e.ToLower().Contains("Rear Camera".ToLower()));
                var frontCameraIndex = deviceInformation.FindIndex(e => e.ToLower().Contains("Front Camera".ToLower()));
                var unlockIndex = deviceInformation.FindIndex(e => e.ToLower().Contains("Unlock".ToLower()));
                var networkConnectivityIndex = deviceInformation.FindIndex(e => e.ToLower().Contains("Network".ToLower()));
                var networkBandsIndex = deviceInformation.FindIndex(e => e.ToLower().Contains("Network bands".ToLower()));
                var wirelessNetworkIndex = deviceInformation.FindIndex(e => e.ToLower().Contains("Wireless network".ToLower()));
                var navigationPositioningIndex = deviceInformation.FindIndex(e => e.ToLower().Contains("Navigation".ToLower()));
                var sensorsIndex = deviceInformation.FindIndex(e => e.ToLower().Contains("Sensors".ToLower()));
                var multimediaIndex = deviceInformation.FindIndex(e => e.ToLower().Contains("Multimedia".ToLower()));

                if (featuresIndex >= 0 && featuresIndex <= 10)
                {
                    if (deviceDetails is null)
                    {
                        deviceDetails = new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            DeviceId = deviceId,
                            Reviews = "https://www.youtube.com/c/ReviewsPK/featured",
                        };

                        await _applicationDbContext.DeviceDetails.AddAsync(deviceDetails);
                    }

                    var features = SubArray.GetSubArray(deviceInformation.ToArray(), featuresIndex + 1, displayIndex - featuresIndex - 1).ToList();
                    var display = SubArray.GetSubArray(deviceInformation.ToArray(), displayIndex + 1, batteryIndex - displayIndex - 1).ToList();
                    var battery = SubArray.GetSubArray(deviceInformation.ToArray(), batteryIndex + 1, rearCameraIndex - batteryIndex - 1).ToList();
                    var rearCamera = SubArray.GetSubArray(deviceInformation.ToArray(), rearCameraIndex + 1, frontCameraIndex - rearCameraIndex - 1).ToList();
                    var frontCamera = SubArray.GetSubArray(deviceInformation.ToArray(), frontCameraIndex + 1, unlockIndex - frontCameraIndex - 1).ToList();
                    var unlock = SubArray.GetSubArray(deviceInformation.ToArray(), unlockIndex + 1, networkConnectivityIndex - unlockIndex - 1).ToList();
                    var networkConnectivity = SubArray.GetSubArray(deviceInformation.ToArray(), networkConnectivityIndex + 1, navigationPositioningIndex - networkConnectivityIndex - 1).ToList();
                    var networkBands = SubArray.GetSubArray(deviceInformation.ToArray(), networkBandsIndex + 1, wirelessNetworkIndex - networkBandsIndex - 1).ToList();
                    var wirelessNetwork = SubArray.GetSubArray(deviceInformation.ToArray(), wirelessNetworkIndex + 1, navigationPositioningIndex - wirelessNetworkIndex - 1).ToList();
                    var navigationPositioning = SubArray.GetSubArray(deviceInformation.ToArray(), navigationPositioningIndex + 1, sensorsIndex - navigationPositioningIndex - 1).ToList();
                    var sensors = SubArray.GetSubArray(deviceInformation.ToArray(), sensorsIndex + 1, deviceInformation.Count - sensorsIndex - 1).ToList();
                    var multimedia = SubArray.GetSubArray(deviceInformation.ToArray(), multimediaIndex + 1, deviceInformation.Count - multimediaIndex - 2).ToList();

                    List<string> emptyList = new();

                    var ramRom = features == null || features.Count < 4 ? emptyList : features[3].Split("/").ToList();

                    deviceDetails.Chipset = features == null || features.Count < 1 ? string.Empty : features[0];
                    deviceDetails.CPU = features == null || features.Count < 2 ? string.Empty : features[1].Replace("CPU:", string.Empty);
                    deviceDetails.GPU = features == null || features.Count < 3 ? string.Empty : features[2].Replace("GPU:", string.Empty);
                    deviceDetails.G2Band = networkBands == null || networkBands.Count < 1 ? string.Empty : networkBands[0];
                    deviceDetails.G3Band = networkBands == null || networkBands.Count < 2 ? string.Empty : networkBands[1];
                    deviceDetails.G4Band = networkBands == null || networkBands.Count < 3 ? string.Empty : networkBands[2];
                    deviceDetails.G5Band = networkBands == null || networkBands.Count < 4 ? string.Empty : networkBands[3];
                    deviceDetails.WiFi = wirelessNetwork == null || wirelessNetwork.Count < 2 ? string.Empty : wirelessNetwork[1].Replace("Supports", string.Empty).Trim();
                    deviceDetails.Bluetooth = wirelessNetwork == null || wirelessNetwork.Count < 3 ? string.Empty : wirelessNetwork[2].Replace("Support", string.Empty).Trim();
                    deviceDetails.FM = wirelessNetwork == null || wirelessNetwork.Count < 4 ? string.Empty : wirelessNetwork[3].Replace("Supports", string.Empty).Trim();
                    deviceDetails.Screen = display == null || display.Count < 1 ? string.Empty : display[0].Substring(0, display[0].IndexOf("+")).Trim() + "+";
                    deviceDetails.Resolution = display == null || display.Count < 2 ? string.Empty : display[1]; deviceDetails.TouchScreen = "Capacitive multi-touch";
                    deviceDetails.SceneModes = display == null ? string.Empty : ((display.Count < 3 ? string.Empty : display[2] + " ") + (display.Count < 4 ? string.Empty : display[3] + " ") + (display.Count < 5 ? string.Empty : display[4] + " ") + (display.Count < 6 ? string.Empty : display[5] + " ") + (display.Count < 7 ? string.Empty : display[6] + " ") + (display.Count < 8 ? string.Empty : display[7] + " "));
                    deviceDetails.Camera = (frontCamera == null || frontCamera.Count < 1 ? string.Empty : "Front " + frontCamera[0] + ", ") + (rearCamera == null ? string.Empty : ((rearCamera.Count < 1 ? string.Empty : " " + rearCamera[0]) + (rearCamera.Count < 2 ? string.Empty : rearCamera[1]) + (rearCamera.Count < 3 ? string.Empty : " " + rearCamera[2]) + (rearCamera.Count < 4 ? string.Empty : rearCamera[3]) + (rearCamera.Count < 5 ? string.Empty : " " + rearCamera[4]) + (rearCamera.Count < 6 ? string.Empty : rearCamera[5])));
                    deviceDetails.GPS = navigationPositioning == null || navigationPositioning.Count < 1 ? string.Empty : (navigationPositioning[0].ToLower().Trim().Contains("GPS".ToLower()) ? "Supported" : string.Empty);
                    deviceDetails.AmbientLightSensor = sensors == null ? string.Empty : (sensors.Find(e => e.Replace("• ", string.Empty).ToLower().Trim() == "Ambient Light Sensor".ToLower().Trim()) == null ? string.Empty : "Supported");
                    deviceDetails.Accelerometer = sensors == null ? string.Empty : (sensors.Find(e => e.Replace("• ", string.Empty).ToLower().Trim() == "Accelerometer".ToLower().Trim()) == null ? string.Empty : "Supported");
                    deviceDetails.ProximitySensor = sensors == null ? string.Empty : (sensors.Find(e => e.Replace("• ", string.Empty).ToLower().Trim() == "Proximity".ToLower().Trim()) == null ? string.Empty : "Supported");
                    deviceDetails.AudioPlayback = sensors == null || sensors.Count < 12 ? string.Empty : sensors[11];
                    deviceDetails.VideoPlayback = sensors == null || sensors.Count < 10 ? string.Empty : sensors[9];
                    deviceDetails.Battery = battery == null || battery.Count < 1 ? string.Empty : battery[0];
                    deviceDetails.USB = battery == null || battery.Count < 4 ? string.Empty : battery[3];
                    deviceDetails.SIMSlotType = networkConnectivity == null || networkConnectivity.Count < 1 ? string.Empty : networkConnectivity[0];
                    deviceDetails.SIM = networkConnectivity == null || networkConnectivity.Count < 2 ? string.Empty : networkConnectivity[1].Replace("①", string.Empty);
                    deviceDetails.Ram = ramRom.Count < 1 ? string.Empty : ramRom[0].Substring(0, ramRom[0].IndexOf("+")).Trim();
                    deviceDetails.Rom = ramRom.Count < 1 ? string.Empty : ramRom[0][(ramRom[0].IndexOf("+") + 1)..].Trim();

                    if (ramRom.Count == 2)
                    {
                        deviceDetails.Ram = deviceDetails.Ram + "/" + ramRom[1].Substring(0, ramRom[1].IndexOf("+")).Trim();
                        deviceDetails.Rom = deviceDetails.Rom + "/" + ramRom[1][(ramRom[1].IndexOf("+") + 1)..].Trim();
                    }
                }
                else
                {
                    var oldDevice = await _applicationDbContext.Devices.Include(e => e.DeviceDetails).FirstOrDefaultAsync(e => e.Id == deviceId);
                    if (oldDevice is not null)
                        _applicationDbContext.Remove(oldDevice);
                }
                await _applicationDbContext.SaveChangesAsync();
            }
            else
            {
                var oldDevice = await _applicationDbContext.Devices.Include(e => e.DeviceDetails).FirstOrDefaultAsync(e => e.Id == deviceId);
                if (oldDevice is not null)
                    _applicationDbContext.Remove(oldDevice);
            }
            await _applicationDbContext.SaveChangesAsync();
        }

        private async Task AddOrUpdateSamsungDevice(string deviceId, string model)
        {
            HtmlWeb deviceWeb = new();
            HtmlDocument deviceDocument = deviceWeb.Load(Urls.SamsungDevice + model);

            var device = deviceDocument.DocumentNode.SelectNodes("//div[contains(@class, 'item-group clearafter')]/div[contains(@class, 'clearafter')]").OrderBy(e => e.InnerHtml).ToList();

            if (device is not null && device.Count > 0)
            {
                var heading = new Regex("<h1(.*?)>(.*?)</h1>");
                var subHeading = new Regex("<p(.*?)>(.*?)</p>");
                // var image = new Regex("<img[^>]+>");

                var deviceDetails = await _applicationDbContext.DeviceDetails.FirstOrDefaultAsync(e => e.DeviceId == deviceId);
                if (deviceDetails is null)
                {
                    deviceDetails = new()
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
                        Battery = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Battery) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Battery).InnerHtml).Groups[2].Value,
                    };
                    await _applicationDbContext.DeviceDetails.AddAsync(deviceDetails);
                }
                else
                {
                    deviceDetails.Processor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Processor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Processor).InnerHtml).Groups[2].Value;
                    deviceDetails.Ram = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.RAM) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.RAM).InnerHtml).Groups[2].Value;
                    deviceDetails.Rom = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ROM) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ROM).InnerHtml).Groups[2].Value;
                    deviceDetails.Charging = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ChargingPower) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ChargingPower).InnerHtml).Groups[2].Value;
                    deviceDetails.Colors = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Color) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Color).InnerHtml).Groups[2].Value;
                    deviceDetails.OperatingSystem = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.OperatingSystem) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.OperatingSystem).InnerHtml).Groups[2].Value;
                    deviceDetails.Screen = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Screen) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Screen).InnerHtml).Groups[2].Value;
                    deviceDetails.Resolution = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Resolution) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Resolution).InnerHtml).Groups[2].Value;
                    deviceDetails.Type = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Type) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Type).InnerHtml).Groups[2].Value;
                    deviceDetails.TouchScreen = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.TouchScreen) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.TouchScreen).InnerHtml).Groups[2].Value;
                    deviceDetails.Camera = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Camera) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Camera).InnerHtml).Groups[2].Value;
                    deviceDetails.Aperture = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Aperture) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Aperture).InnerHtml).Groups[2].Value;
                    deviceDetails.Flash = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Flash) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Flash).InnerHtml).Groups[2].Value;
                    deviceDetails.SceneModes = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SceneModes) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SceneModes).InnerHtml).Groups[2].Value;
                    deviceDetails.WiFi = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.WiFi) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.WiFi).InnerHtml).Groups[2].Value;
                    deviceDetails.Bluetooth = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Bluetooth) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Bluetooth).InnerHtml).Groups[2].Value;
                    deviceDetails.USB = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Usb) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Usb).InnerHtml).Groups[2].Value;
                    deviceDetails.GPS = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Gps) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Gps).InnerHtml).Groups[2].Value;
                    deviceDetails.OTG = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Otg) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Otg).InnerHtml).Groups[2].Value;
                    deviceDetails.FM = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fm) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fm).InnerHtml).Groups[2].Value;
                    deviceDetails.SIMSlotType = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SIMSlotType) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.SIMSlotType).InnerHtml).Groups[2].Value;
                    deviceDetails.StandbyMode = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.StandbyMode) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.StandbyMode).InnerHtml).Groups[2].Value;
                    deviceDetails.G2Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G2Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G2Band).InnerHtml).Groups[2].Value;
                    deviceDetails.G3Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G3Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G3Band).InnerHtml).Groups[2].Value;
                    deviceDetails.G4Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G4Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G4Band).InnerHtml).Groups[2].Value;
                    deviceDetails.G5Band = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G5Band) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.G5Band).InnerHtml).Groups[2].Value;
                    deviceDetails.Fingerprint = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fingerprint) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Fingerprint).InnerHtml).Groups[2].Value;
                    deviceDetails.Accelerometer = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Accelerometer) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Accelerometer).InnerHtml).Groups[2].Value;
                    deviceDetails.AmbientLightSensor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AmbientLightSensor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AmbientLightSensor).InnerHtml).Groups[2].Value;
                    deviceDetails.ProximitySensor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ProximitySensor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ProximitySensor).InnerHtml).Groups[2].Value;
                    deviceDetails.ECompass = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ECompass) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.ECompass).InnerHtml).Groups[2].Value;
                    deviceDetails.GyroscopeSensor = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.GyroscopeSensor) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.GyroscopeSensor).InnerHtml).Groups[2].Value;
                    deviceDetails.AudioPlayback = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AudioPlayback) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.AudioPlayback).InnerHtml).Groups[2].Value;
                    deviceDetails.VideoPlayback = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoPlayback) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoPlayback).InnerHtml).Groups[2].Value;
                    deviceDetails.VideoRecording = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoRecording) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.VideoRecording).InnerHtml).Groups[2].Value;
                    deviceDetails.Battery = subHeading.Match(device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Battery) == null ? "" : device.Find(e => heading.Match(e.InnerHtml).Groups[2].Value == DeviceDetailsList.Battery).InnerHtml).Groups[2].Value;
                }
                await _applicationDbContext.SaveChangesAsync();
            }
        }

        #endregion
    }
    public static class SubArray
    {
        public static T[] GetSubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
    }
}

