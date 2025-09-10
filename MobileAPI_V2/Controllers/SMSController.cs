using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MobileAPI_V2.DataLayer;
using MobileAPI_V2.Model;
using MobileAPI_V2.Services;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MobileAPI_V2.Controllers
{
    [ApiController]
    [Route("api/sms/v3/")]
    public class SMSController : Controller
    {
        private readonly LogWrite _logwrite;
        private readonly ConnectionString _connectionString;
        private readonly IConfiguration _configuration;
        private readonly SendSMSForEcomm sendSMSForEcomm;
        //public SMSController(IConfiguration configuration, LogWrite logwrite)
        public SMSController(ConnectionString connectionString, IConfiguration configuration, LogWrite logwrite)
        {
            _logwrite = logwrite;
            _configuration = configuration;
            _connectionString = connectionString;
            sendSMSForEcomm = new(_connectionString, _configuration, _logwrite);
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("SendSMSMessage")]
        public string SMSSend([FromBody] SendSMSReq sendSMSReq)
        {
            string result = "Failure";
            try
            {
                //result = sendSMSForEcomm.SendSMSMessage(sendSMSReq.to, sendSMSReq.tmplId, sendSMSReq.text);
                Task<string> result1 = sendSMSForEcomm.SendSMSMessage(sendSMSReq);
                result = result1.Result;
                //result = sendSMSForEcomm.SendSMSMessage(sendSMSReq.to,sendSMSReq.tmplId, sendSMSReq.text);
            }
            catch (Exception ex)
            {
                _logwrite.LogRequestException("error in smssend api : " + ex.ToString());
            }
            return result;
        }
    }
}