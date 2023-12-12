using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers
{
    [Route("api/company")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    public class CompanyV2Controller : ControllerBase
    {
        private readonly IRepositoryManager _repository;

        public CompanyV2Controller(IRepositoryManager repository)
        {
            _repository = repository;
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges:
           false);
            return Ok(companies);
        }
    }
}
