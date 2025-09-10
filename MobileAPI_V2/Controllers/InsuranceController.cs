using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MobileAPI_V2.Model.BillPayment;
using MobileAPI_V2.Model;
using MobileAPI_V2.Services;
using static MobileAPI_V2.Model.BillPayment.BillPaymentCommon;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System;

namespace MobileAPI_V2.Controllers
{
    [Route("api/Insurance")]
    [ApiController]
    public class InsuranceController : Controller
    {
        private readonly IDataRepository _dataRepository;
        public InsuranceController(Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IDataRepository dataRepository, IConfiguration configuration)
        {

            _dataRepository = dataRepository;
        }
        [HttpPost("NewPolicy")]
        [Produces("application/json")]
        public async Task<NirmalBangPolicyResponse> NewPolicy(NirmalBangPolicy policy)
        {
                var res = await _dataRepository.SaveNBpolicy(policy);
                return (res);

        }

    }
}
