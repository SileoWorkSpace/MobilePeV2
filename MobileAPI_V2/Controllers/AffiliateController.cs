using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MobileAPI_V2.Model.BillPayment;
using MobileAPI_V2.Model;
using MobileAPI_V2.Models;
using MobileAPI_V2.Services;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Json;
using System;
using MobileAPI_V2.Model.Fineque;
using static MobileAPI_V2.Model.BillPayment.BillPaymentCommon;
using System.Reflection.PortableExecutable;
using System.Data;
using System.Collections.Generic;
using System.Net;
using MobileAPI_V2.Model.Affiliate;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Linq;

namespace MobileAPI_V2.Controllers
{
    [Route("api/Affiliate")]
    [ApiController]
    public class AffiliateController : ControllerBase
    {
        private readonly LogWrite _logwrite;
        private readonly AffiliateService _AffiliateService;
        IHttpContextAccessor _httpContextAccessor;
        private readonly IDataRepository _dataRepository;

            
        public AffiliateController(AffiliateService affiliateService, IHttpContextAccessor acc, IDataRepository dataRepository, LogWrite logwrite)
        {
            _logwrite = logwrite;
            _dataRepository = dataRepository;
            _AffiliateService = affiliateService;
            this._httpContextAccessor = acc;

        }
        string AESKEY = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build().GetSection("AESKEY").Value;

        [HttpPost("AffiliateFinanceRegistration")]
        public IActionResult AffiliateFinanceRegistration(RequestModel reqModel)
        {

            string EncryptedText = "";
            ResponseModel objres = new ResponseModel();
            CommonResponseDTO<AffiliateFinanceRegistrationDTO> _CommonResponse = new CommonResponseDTO<AffiliateFinanceRegistrationDTO>();
            AffiliateFinanceRegistrationDTO objrequest = new AffiliateFinanceRegistrationDTO();

            try
            {
                string dcdata = ApiEncrypt_Decrypt.DecryptString(AESKEY, reqModel.Body);
                objrequest = JsonConvert.DeserializeObject<AffiliateFinanceRegistrationDTO>(dcdata);
                var response = _AffiliateService.AffiliateFinanceRegistration(objrequest);
                if (response != null)
                {
                    if (response.Status)
                    {
                        _CommonResponse.flag = 1;
                        _CommonResponse.Status = true;
                        _CommonResponse.message = response.message;
                    }
                    else
                    {
                        _CommonResponse.flag = 0;
                        _CommonResponse.Status = false;
                        _CommonResponse.message = response.message;
                    }
                }
                else
                {
                    _CommonResponse.flag = 0;
                    _CommonResponse.Status = false;
                    _CommonResponse.message = "We are facing some technical issues please try after some time";
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                _CommonResponse.flag = 0;
                _CommonResponse.Status = false;
                _CommonResponse.message = "We are facing some technical issues please try after some time. #001";
            }


            objres.Body = JsonConvert.SerializeObject(_CommonResponse);
            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(CommonResponseDTO<AffiliateFinanceRegistrationDTO>));
            ms = new MemoryStream();
            js.WriteObject(ms, _CommonResponse);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(AESKEY, CustData);
            objres.Body = EncryptedText;
            return Ok(objres);
        }

        [HttpPost("GetAffiliateFinanceRegistration")]
        public IActionResult GetAffiliateFinanceRegistration(RequestModel reqModel)
        {
            string EncryptedText = "";
            ResponseModel objres = new ResponseModel();
            CommonResponseDTO<AffiliateFinanceRegistrationDTO> _CommonResponse = new CommonResponseDTO<AffiliateFinanceRegistrationDTO>();
            AffiliateFinanceRegistrationDTO objrequest = new AffiliateFinanceRegistrationDTO();

            try
            {
                string dcdata = ApiEncrypt_Decrypt.DecryptString(AESKEY, reqModel.Body);
                objrequest = JsonConvert.DeserializeObject<AffiliateFinanceRegistrationDTO>(dcdata);
                var response = _AffiliateService.GetAffiliateFinanceRegistration(objrequest);
                if (response != null)
                {
                    _CommonResponse.flag = 1;
                    _CommonResponse.Status = true;
                    _CommonResponse.message = "Success";
                    _CommonResponse.result = response;
                }
                else
                {
                    _CommonResponse.flag = 0;
                    _CommonResponse.Status = false;
                    _CommonResponse.message = "We are facing some technical issues please try after some time";
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                _CommonResponse.flag = 0;
                _CommonResponse.Status = false;
                _CommonResponse.message = "We are facing some technical issues please try after some time. #001";
            }

            objres.Body = JsonConvert.SerializeObject(_CommonResponse);
            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(CommonResponseDTO<AffiliateFinanceRegistrationDTO>));
            ms = new MemoryStream();
            js.WriteObject(ms, _CommonResponse);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(AESKEY, CustData);
            objres.Body = EncryptedText;
            return Ok(objres);
        }

        [HttpPost("GetAllAffiliateFinanceDetailList")]
        public IActionResult GetAllAffiliateFinanceDetailList(RequestModel reqModel)
        {
            string EncryptedText = "";
            ResponseModel objres = new ResponseModel();
            CommonResponseDTO<List<AffiliateFinanceRegistrationReportDTO>> _CommonResponse = new CommonResponseDTO<List<AffiliateFinanceRegistrationReportDTO>>();
            AffiliateFinanceRegistrationFilterDTO objrequest = new AffiliateFinanceRegistrationFilterDTO();          
            try
            {
                string dcdata = ApiEncrypt_Decrypt.DecryptString(AESKEY, reqModel.Body);
                objrequest = JsonConvert.DeserializeObject<AffiliateFinanceRegistrationFilterDTO>(dcdata);
                var response = _AffiliateService.GetAllAffiliateFinanceDetailList(objrequest);
                if (response != null)
                {
                    _CommonResponse.flag = 1;
                    _CommonResponse.Status = true;
                    _CommonResponse.message = "Success";
                    _CommonResponse.result = response;
                }
                else
                {
                    _CommonResponse.flag = 0;
                    _CommonResponse.Status = false;
                    _CommonResponse.message = "No data found.";
                    _CommonResponse.result = new List<AffiliateFinanceRegistrationReportDTO>();
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                _CommonResponse.flag = 0;
                _CommonResponse.Status = false;
                _CommonResponse.message = "We are facing some technical issues please try after some time. #001";
                _CommonResponse.result = new List<AffiliateFinanceRegistrationReportDTO>();
            }
            objres.Body = JsonConvert.SerializeObject(_CommonResponse);
            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(CommonResponseDTO<List<AffiliateFinanceRegistrationReportDTO>>));
            ms = new MemoryStream();
            js.WriteObject(ms, _CommonResponse);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(AESKEY, CustData);
            objres.Body = EncryptedText;
            return Ok(objres);
        }

        [HttpGet("BankCreditCardBusinessDetails")]
        public async Task<string> BankCreditCardBusinessDetails(string NameId, string MobileNumberId, string EmailId, string MarketingExperienceType, string StateType, string DistrictId, string Remarks,string ReferenceType)
        {
            string result = "";
            try
            {
                var res = await _dataRepository.BankCreditCardBusinessDetails(NameId, MobileNumberId, EmailId, MarketingExperienceType, StateType, DistrictId, Remarks, ReferenceType);
                result = res.ToString();
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw ex;
            }

            return result;

        }

        //[HttpPost("CheckMerchantTransactionStatus")]
        //public async Task<IActionResult> CheckMerchantTransactionStatusAsync(RequestModel trans)
        //{
        //    ResponseModel objRes = new ResponseModel();
        //    string responseOutput = "";
        //    try
        //    {
        //        string transID = ApiEncrypt_Decrypt.DecryptString(AESKEY, trans.Body);
        //        if (string.IsNullOrEmpty(transID))
        //        {
        //            objRes.Body = "Transaction ID is null or empty.";
        //            return BadRequest(objRes); 
        //        }

        //        var url = "https://uatmerchant.mobilepefintech.com/api/mobilepe/MerchantTransactionStatus";

        //        var requestBody = new
        //        {
        //            transactionid = transID
        //        };

        //        var apiResponse = await HitTopayApiAsync(url, requestBody);
        //         responseOutput = JsonConvert.SerializeObject(apiResponse);
        //        JObject json = JObject.Parse(responseOutput);

        //        string responseCode = json["ResponseCode"]?.ToString();
        //        string responseMessage = json["ResponseMessage"]?.ToString();
        //        JArray transactionDataArray = (JArray)json["TransactionDataList"];
        //        List<TransactionData> transactionDataList = transactionDataArray?.ToObject<List<TransactionData>>();

        //        if (transactionDataList != null && transactionDataList.Count > 0)
        //        {
        //            objRes.Body = JsonConvert.SerializeObject(transactionDataList); 
        //        }
        //        else
        //        {
        //            objRes.Body = "No transaction data found.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logwrite.LogException(ex);
        //        objRes.Body = "We are facing some technical issues. Please try again later. #001";
        //    }
        //    string encryptedResponse = ApiEncrypt_Decrypt.EncryptString(AESKEY, responseOutput);
        //    return Ok(new ResponseModel { Body = encryptedResponse }); 
        //}

        [HttpPost("CheckMerchantTransactionStatus")]
        public async Task<IActionResult> CheckMerchantTransactionStatusAsync(string trans)
        {
            if (string.IsNullOrEmpty(trans))
            {
                return BadRequest("Transaction ID is null or empty.");
            }

            string url = "https://vendorapi.mobilepe.co.in/api/mobilepe/MerchantTransactionStatus";
            var requestBody = new
            {
                transactionid = trans
            };

            try
            {
                ApiResponse apiResponse = await HitTopayApiAsync(url, requestBody);
               
                if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.Data))
                {
                    var dataList = JsonConvert.DeserializeObject<List<TransactionData>>(apiResponse.Data);
                    var Responsedescription = dataList[0].Responsedescription;
                    var commissionPercentage = dataList[0].CommissioninPersentage;
                    if (Responsedescription == "Transaction Not Available")
                    {

                        var transactionData = new TransactionData
                        {
                            Responsecode = 101,
                            Responsedescription = "Failure", 
                            CommissioninPersentage = 0, 
                            TransactionDetails = 0 
                        };

                        var failureResponse = new ApiResponse
                        {
                            Data = JsonConvert.SerializeObject(new List<TransactionData> { transactionData }), 
                            Response_code = "200", // Success code
                            Response_message = "Success", // Keep original success message
                            TransactionDataList = new List<TransactionData> { transactionData } 
                        };

                        return Ok(failureResponse);


                    }
                    var updateValue = await _dataRepository.CommissionMerchant(commissionPercentage, trans);
                }
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                return StatusCode(500, "We are facing some technical issues. Please try again later. #001");
            }
        }

        public static async Task<ApiResponse> HitTopayApiAsync(string url, object body)
        {
            using (var client = new HttpClient())
            {
                var jsonBody = JsonConvert.SerializeObject(body);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                try
                {
                    response = await client.PostAsync(url, content);
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("Error connecting to the API.", ex);
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"API request failed with status code: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
                if (apiResponse != null)
                {
                    if (!string.IsNullOrEmpty(apiResponse.Data))
                    {
                        apiResponse.TransactionDataList = JsonConvert.DeserializeObject<List<TransactionData>>(apiResponse.Data);
                    }
                    else
                    {
                        apiResponse.TransactionDataList = new List<TransactionData>();
                    }
                }

                return apiResponse;
            }
        }
    }
}
