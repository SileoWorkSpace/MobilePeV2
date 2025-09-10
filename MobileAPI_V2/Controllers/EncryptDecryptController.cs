using Microsoft.AspNetCore.Mvc;
using MobileAPI_V2.Model.BillPayment;
using System.Security.Cryptography;
using System.Text;
using static MobileAPI_V2.Model.BillPayment.BillPaymentCommon;


namespace MobileAPI_V2.Controllers
{
    [ApiController]
    [Route("api/Auth/")]
    public class EncryptDecryptController : ControllerBase
    {
        //// Example of a GET endpoint
        //[HttpGet("test")]
        //public IActionResult Test()
        //{
        //    return Ok("This is a test response from EncryptDecryptController.");
        //}

        [HttpPost("encrypt")]
        public async Task<IActionResult> Encrypt([FromBody] RequestModel ReqData)
        {
            string result = "";
            var re = Request;
            var headers = re.Headers;
            string Aeskey = "";
            if (string.IsNullOrEmpty(headers["Token"].First()))
            {
                result = "Token is invalid";
            }
            else
            {
                string tokenVal = headers["Token"].First();
                string[] split = tokenVal.Split('-');
                Aeskey = split[1];
            }
            result = ApiEncrypt_Decrypt.EncryptString(Aeskey, ReqData.Body);
            return Ok(result);
        }


        [HttpPost("decrypt")]
        public async Task<IActionResult> Decrypt([FromBody] RequestModel ReqData)
        {
            string result = "";
            var re = Request;
            var headers = re.Headers;
            string Aeskey = "";
            if (string.IsNullOrEmpty(headers["Token"].First()))
            {
                result = "Token is invalid";
            }
            else
            {
                string tokenVal = headers["Token"].First();
                string[] split = tokenVal.Split('-');
                Aeskey = split[1];
            }
            result = ApiEncrypt_Decrypt.DecryptString(Aeskey, ReqData.Body);
            return Ok(result);
        }

    }

}
