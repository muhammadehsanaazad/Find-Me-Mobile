using Find_Me_Mobile.Constants;
using Find_Me_Mobile.Models;
using Find_Me_Mobile.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Find_Me_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : BaseController
    {
        #region Constructors

        public CompaniesController(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
        #endregion

        #region Methods

        [HttpPost]
        [Route("AddCompany")]
        public async Task<IActionResult> AddCompany(CompaniesBindingModel model)
        {
            if (model is null)
                return Ok(new ApplicationResult { IsSuccess = false, Message = ErrorMessages.InvalidInput });

            var account = new Companies()
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                ContactNumber = model.ContactNumber
            };

            await _applicationDbContext.AddAsync(account);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(new ApplicationResult { IsSuccess = true, Message = ErrorMessages.CompanyRegistered });
        }

        [HttpPost]
        [Route("UpdateCompany")]
        public async Task<IActionResult> UpdateCompany(CompaniesBindingModel model)
        {
            if (model is null)
                return Ok(new ApplicationResult { IsSuccess = false, Message = ErrorMessages.InvalidInput });

            var company = await _applicationDbContext.Companies.FirstOrDefaultAsync(e => e.Id == model.Id);

            if (company is null)
                return Ok(new ApplicationResult { IsSuccess = false, Message = ErrorMessages.InvalidInput });

            company.Name = model.Name;
            company.ContactNumber = model.ContactNumber;

            await _applicationDbContext.SaveChangesAsync();

            return Ok(new ApplicationResult { IsSuccess = true, Message = ErrorMessages.CompanyUpdated });
        }

        [HttpGet]
        [Route("GetSingleCompany")]
        public async Task<IActionResult> GetSingle(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Ok(new ApplicationResult { IsSuccess = false, Message = ErrorMessages.InvalidInput });

            var company = await _applicationDbContext.Companies.FirstOrDefaultAsync(e => e.Id == id);

            if (company is null)
                return Ok(new ApplicationResult { IsSuccess = false, Message = ErrorMessages.InvalidInput });

            return Ok(new ApplicationResult { IsSuccess = true, Data = company });
        }

        [HttpGet]
        [Route("GetAllCompanies")]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _applicationDbContext.Companies.ToListAsync();
            return Ok(new ApplicationResult { IsSuccess = true, Data = companies });
        }

        [HttpDelete]
        [Route("DeleteCompany")]
        public async Task<IActionResult> DeleteCompany(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Ok(new ApplicationResult { IsSuccess = false, Message = ErrorMessages.InvalidInput });

            var company = await _applicationDbContext.Companies.FirstOrDefaultAsync(e => e.Id == id);

            if (company is null)
                return Ok(new ApplicationResult { IsSuccess = false, Message = ErrorMessages.InvalidInput });

            _applicationDbContext.Remove(company);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(new ApplicationResult { IsSuccess = true, Message = ErrorMessages.CompanyDeleted });
        }

        #endregion
    }
}

