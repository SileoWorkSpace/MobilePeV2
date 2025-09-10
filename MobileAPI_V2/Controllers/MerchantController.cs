using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MobileAPI_V2.DataLayer;
using MobileAPI_V2.Services;
using MobileAPI_V2.Model;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static MobileAPI_V2.Model.MerchantModel;
using static MobileAPI_V2.Model.BillPayment.BillPaymentCommon;
using MobileAPI_V2.Model.BillPayment;
using System;
using MobileAPI_V2.Models;
using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using MobileAPI_V2.Model.Fineque;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Reflection;
using MobileAPI_V2.Logs;


namespace MobileAPI_V2.Controllers
{
    [Route("api/MobilePe/")]
    [ApiController]
    public class MerchantController : ControllerBase
    {
        static readonly Logger _log = new();
        private readonly IMerchant _IMerchant;
        private readonly IDataRepository _dataRepository;
        string AESKEYMP = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build().GetSection("AESKEY").Value;
        private readonly LogWrite _logwrite;
        public MerchantController(IMerchant IMerchant, IDataRepository dataRepository, LogWrite logwrite)
        {
            _IMerchant = IMerchant;
            _logwrite = logwrite;
            _dataRepository = dataRepository;

        }

        [HttpPost("MerchantRequest")]
        [Produces("application/json")]
        public async Task<ResponseModel> MerchantRequestss(RequestMerchantModel Body)
        {
            //   _logwrite.LogRequestException("Merchant Controller , MerchantRequest :" + Body.Body);
            _log.WriteToFile("MerchantRequest Process started");
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ApiResponseModel returnResponse = new ApiResponseModel();
            var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/MerchantRequest";            
            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.data = new List<MerchantBody>();
                    returnResponse.response_code = "402";

                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    _log.WriteToFile("MerchantRequest Process Request Input with Encrypt " + Body.Body);
                    string dcdataa = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    _log.WriteToFile("MerchantRequest Process Request Input with Decrypt " + dcdataa);
                    RequestMerchantModelsss dataaa = JsonConvert.DeserializeObject<RequestMerchantModelsss>(dcdataa);
                    MerchantBody data = await _IMerchant.MerchantRequestssData(dataaa.mobilenumber);
                    if (data == null)    
                    {
                        returnResponse.response_message = "Please register your Topay wallet to become a vendor.";
                        returnResponse.data = new List<MerchantBody>();
                        returnResponse.response_code = "404"; 
                    }
                    else
                    {
                        _log.WriteToFile("MerchantRequest Process else part");
                        data.password = dataaa.password;

                        string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                        returnResponse = JsonConvert.DeserializeObject<ApiResponseModel>(res);

                        if (returnResponse.data != null)
                        {
                            _log.WriteToFile("MerchantRequest Process else first if part");
                            var dataObjects = JsonConvert.DeserializeObject<List<dynamic>>(returnResponse.data.ToString());


                            if (dataObjects.Count > 0)
                            {
                                _log.WriteToFile("MerchantRequest Process else second if part");
                                dynamic firstItem = dataObjects[0];
                                if (firstItem.Redirecturl != null)
                                {
                                    _log.WriteToFile("MerchantRequest Process else third if part");
                                    returnResponse.Redirecturl = firstItem.Redirecturl.ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw ex;
            }
            string CustData = "";
            //DataContractJsonSerializer js;
            //MemoryStream ms;
            //js = new DataContractJsonSerializer(typeof(ApiResponseModel));
            //ms = new MemoryStream();
            //js.WriteObject(ms, returnResponse);
            //ms.Position = 0;
            //StreamReader sr = new StreamReader(ms);
            //CustData = sr.ReadToEnd();
            //sr.Close();
            //ms.Close();
            CustData = JsonConvert.SerializeObject(returnResponse);
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            _log.WriteToFile("MerchantRequest Process Ended");
            return Response;
        }

        [HttpPost("MerchantCreditRequest")]
        [Produces("application/json")]
        public async Task<ResponseModel> MerchantCreditRequest(RequestMerchantModel Body)
        {
            // _logwrite.LogRequestException("Merchant Controller , MerchantCreditRequest :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ApiResponseModel returnResponse = new ApiResponseModel();
            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.data = new List<MerchantBody>();
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/MerchantCreditRequest";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    var jsonObject = JsonConvert.DeserializeObject<JObject>(dcdata);
                    var requestJson = jsonObject["request"].ToString();
                    var merchantCreditData = JsonConvert.DeserializeObject<MerchantCredit>(requestJson);
                    string res = WalletCommon.HITMERAPI(url, merchantCreditData, Aeskey);
                    returnResponse = JsonConvert.DeserializeObject<ApiResponseModel>(res);
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw ex;
            }
            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(ApiResponseModel));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("MerchantSearch")]
        [Produces("application/json")]
        public async Task<ResponseModel> MerchantSearch(RequestMerchantModel Body)
        {
            //  _logwrite.LogRequestException("Merchant Controller , MerchantSearch :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ApiResponseModel1 returnResponse = new ApiResponseModel1
            {
                HideDetails = new HideDetails()
            };
            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/MerchantSearch";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    MerchantSearchRequestModel data = JsonConvert.DeserializeObject<MerchantSearchRequestModel>(dcdata);
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                    var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);

                    returnResponse.response_code = tempResponse.response_code;
                    returnResponse.response_message = tempResponse.response_message;
                    returnResponse.Redirecturl = tempResponse.Redirecturl;
                    if (tempResponse.data is JValue && tempResponse.data.Type == JTokenType.String)
                    {
                        string dataString = tempResponse.data.ToString();
                        returnResponse.data = JsonConvert.DeserializeObject<List<MerchantData>>(dataString);
                        returnResponse.HideDetails.NameTrue = true;
                        returnResponse.HideDetails.NumberTrue = false;
                        returnResponse.HideDetails.EmailTrue = false;
                        returnResponse.HideDetails.shopNameTrue = true;
                        returnResponse.HideDetails.DataTrue = true;
                    }
                    else
                    {
                        returnResponse.data = new List<MerchantData>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(ApiResponseModel1));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }



        [HttpPost("MerchantLookUp")]
        [Produces("application/json")]
        public async Task<ResponseModel> MerchantSearchHistory(RequestMerchantModel Body)
        {
            // _logwrite.LogRequestException("Merchant Controller , MerchantLookUp :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ApiResponseModel12 returnResponse = new ApiResponseModel12();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/MerchantLookUp";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    MerchantCreditRequestModel data = JsonConvert.DeserializeObject<MerchantCreditRequestModel>(dcdata);
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                    var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);

                    returnResponse.response_code = tempResponse.response_code;
                    returnResponse.response_message = tempResponse.response_message;
                    returnResponse.data = tempResponse.Redirecturl;
                    if (tempResponse.data is JValue && tempResponse.data.Type == JTokenType.String)
                    {
                        string dataString = tempResponse.data.ToString();
                        returnResponse.data = JsonConvert.DeserializeObject<List<lookupModel>>(dataString);
                    }
                    else
                    {
                        returnResponse.data = new List<lookupModel>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(ApiResponseModel12));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }


        [HttpPost("MerchantCustomerPaymenthistory")]
        [Produces("application/json")]
        public async Task<ResponseModel> MerchantCustomerPaymenthistory(RequestMerchantModel Body)
        {
            // _logwrite.LogRequestException("Merchant Controller , MerchantCustomerPaymenthistory :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ApiResponseModel123 returnResponse = new ApiResponseModel123();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/MerchantCustomerPaymenthistory";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    MerchantCreditRequestModel data = JsonConvert.DeserializeObject<MerchantCreditRequestModel>(dcdata);
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                    var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);

                    returnResponse.response_code = tempResponse.response_code;
                    returnResponse.response_message = tempResponse.response_message;
                    returnResponse.data = tempResponse.Redirecturl;
                    if (tempResponse.data is JValue && tempResponse.data.Type == JTokenType.String)
                    {
                        string dataString = tempResponse.data.ToString();
                        returnResponse.data = JsonConvert.DeserializeObject<List<searchHistory>>(dataString);
                    }
                    else
                    {
                        returnResponse.data = new List<searchHistory>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(ApiResponseModel123));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("CustomerPaymenthistory")]
        [Produces("application/json")]
        public async Task<ResponseModel> CustomerPaymenthistory(RequestMerchantModel Body)
        {
            // _logwrite.LogRequestException("Merchant Controller , CustomerPaymenthistory :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ApiResponseModel1231 returnResponse = new ApiResponseModel1231
            {
                hideDet = new HideDetail()
            };
            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/CustomertranactionList";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    MerchantCreditRequestModel data = JsonConvert.DeserializeObject<MerchantCreditRequestModel>(dcdata);
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                    var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);

                    returnResponse.response_code = tempResponse.response_code;
                    returnResponse.response_message = tempResponse.response_message;
                    returnResponse.data = tempResponse.Redirecturl;
                    if (tempResponse.data is JValue && tempResponse.data.Type == JTokenType.String)
                    {
                        string dataString = tempResponse.data.ToString();
                        returnResponse.data = JsonConvert.DeserializeObject<List<VendorModel>>(dataString);
                        returnResponse.hideDet.VendornameTrue = true;
                        returnResponse.hideDet.VendormobilenumberTrue = true;
                        returnResponse.hideDet.TransactionAmountTrue = true;
                        returnResponse.hideDet.TransactionidTrue = true;
                        returnResponse.hideDet.CustomerRemarksTrue = true;
                        returnResponse.hideDet.TransactionDatetimeTrue = true;
                    }
                    else
                    {
                        returnResponse.data = new List<VendorModel>();
                    }
                    returnResponse.data = returnResponse.data ?? new List<VendorModel>();

                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(ApiResponseModel1231));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("SettlementRequest")]
        [Produces("application/json")]
        public async Task<ResponseModel> SettlementRequest(RequestMerchantModel Body)
        {
            // _logwrite.LogRequestException("Merchant Controller , CustomerPaymenthistory :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            SettlementReq_Res returnResponse = new();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/SettlementRequest";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    SettlementReq data = JsonConvert.DeserializeObject<SettlementReq>(dcdata);
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                    returnResponse = JsonConvert.DeserializeObject<SettlementReq_Res>(res);
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(SettlementReq_Res));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }


        [HttpPost("AddCharges")]
        [Produces("application/json")]
        public async Task<ResponseModel> AddCharges(RequestMerchantModel Body)
        {
            // _logwrite.LogRequestException("Merchant Controller , AddCharges :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ApiResponseRechargeModel1 returnResponse = new ApiResponseRechargeModel1();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://cprechargeapi.mobilepe.co.in/api/mobilepe/cp/AddCharges";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    RequestAddChargesModelsss data = JsonConvert.DeserializeObject<RequestAddChargesModelsss>(dcdata);
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                    var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);

                    returnResponse.response_code = tempResponse.response_code;
                    returnResponse.response_message = tempResponse.response_message;
                    if (tempResponse.data is JValue && tempResponse.data.Type == JTokenType.String)
                    {
                        string dataString = tempResponse.data.ToString();
                        returnResponse.data = JsonConvert.DeserializeObject<List<ApiResponseRechargeModel1>>(dataString);
                    }
                    else
                    {
                        returnResponse.data = new List<ApiResponseRechargeModel1>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(ApiResponseRechargeModel1));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("StatusCheck")]
        [Produces("application/json")]
        public async Task<ResponseModel> StatusCheck(RequestMerchantModel Body)
        {
            // _logwrite.LogRequestException("Merchant Controller , StatusCheck :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            StatusCheckDataModel1 returnResponse = new StatusCheckDataModel1();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://cprechargeapi.mobilepe.co.in/api/mobilepe/cp/StatusCheck";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    RequestStatusCheckModelsss data = JsonConvert.DeserializeObject<RequestStatusCheckModelsss>(dcdata);
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);

                    var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);

                    returnResponse.response_code = tempResponse.response_code;
                    returnResponse.response_message = tempResponse.response_message;
                    if (tempResponse.data is JValue && tempResponse.data.Type == JTokenType.String)
                    {
                        string dataString = tempResponse.data.ToString();
                        returnResponse.data = JsonConvert.DeserializeObject<List<StatusCheckData>>(dataString);
                    }
                    else
                    {
                        returnResponse.data = new List<StatusCheckData>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(StatusCheckDataModel1));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("MerchantDynamicSearch")]
        [Produces("application/json")]
        public async Task<ResponseModel> MerchantDynamicSearch(RequestMerchantModel Body)
        {
            //  _logwrite.LogRequestException("Merchant Controller , MerchantSearch :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ApiResponseModel1 returnResponse = new ApiResponseModel1
            {
                HideDetails = new HideDetails()
            };
            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];

                    string dcdata123 = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    MRequestModel data111 = JsonConvert.DeserializeObject<MRequestModel>(dcdata123);

                    var searchValue = new StringBuilder();
                    var mode = new StringBuilder();
                    if (!string.IsNullOrEmpty(data111.Pincode))
                        searchValue.Append($"Pincode = {data111.Pincode},");
                    if (!string.IsNullOrEmpty(data111.MobileNumber))
                        searchValue.Append($"Mobilenumber = {data111.MobileNumber},");
                    if (!string.IsNullOrEmpty(data111.MerchantName))
                        searchValue.Append($"merchantname = {data111.MerchantName},");
                    if (!string.IsNullOrEmpty(data111.ShopName))
                        searchValue.Append($"shopname = {data111.ShopName},");
                    if (!string.IsNullOrEmpty(data111.ShopCategory))
                        searchValue.Append($"shopcategory = {data111.ShopCategory},");
                    if (searchValue.Length > 0)
                        searchValue.Length--;
                    string modeValue = !string.IsNullOrEmpty(data111.mode) ? data111.mode : null;


                    var apiRequest = new
                    {
                        mode = modeValue,
                        searchvalue = searchValue.ToString()
                    };

                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/MerchantdynamicSearch";
                    string jsonString = JsonConvert.SerializeObject(apiRequest);
                    MerchantDynamicSearchRequestModel datax = JsonConvert.DeserializeObject<MerchantDynamicSearchRequestModel>(jsonString);
                    //  _logwrite.LogRequestException("Merchant Controller , MerchantDynamicSearch api request time  :" + DateTime.Now);
                    string res = WalletCommon.HITMERAPI(url, datax, Aeskey);
                    //   _logwrite.LogRequestException("Merchant Controller , MerchantDynamicSearch api response time :" + DateTime.Now);
                    var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);

                    returnResponse.response_code = tempResponse.response_code;
                    returnResponse.response_message = tempResponse.response_message;
                    returnResponse.Redirecturl = tempResponse.Redirecturl;
                    if (tempResponse.data is JValue && tempResponse.data.Type == JTokenType.String)
                    {
                        string dataString = tempResponse.data.ToString();
                        returnResponse.data = JsonConvert.DeserializeObject<List<MerchantData>>(dataString);
                        returnResponse.HideDetails.NameTrue = true;
                        returnResponse.HideDetails.NumberTrue = false;
                        returnResponse.HideDetails.EmailTrue = false;
                        returnResponse.HideDetails.shopNameTrue = true;
                        returnResponse.HideDetails.DataTrue = true;
                        returnResponse.HideDetails.BrandProductCode = true;
                        returnResponse.HideDetails.Brandimage = true;
                        returnResponse.HideDetails.Category = true;
                        returnResponse.HideDetails.Service = true;
                        returnResponse.HideDetails.Merchanttype = true;
                    }
                    else
                    {
                        returnResponse.data = new List<MerchantData>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                //_logwrite.LogRequestException("Merchant Controller , MerchantDynamicSearch exception happend:" + DateTime.Now);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(ApiResponseModel1));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            //  _logwrite.LogRequestException("Merchant Controller , MerchantDynamicSearch response to client :" + DateTime.Now);
            return Response;
        }

        [HttpPost("MerchantSubCategory")]
        [Produces("application/json")]
        public async Task<ResponseModel> MerchantSubCategory(MerchantSubCategoryModel body)
        {
            // _logwrite.LogRequestException("Merchant Controller, MerchantSearch:");
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ApiResponsesubcatefory returnResponse = new ApiResponsesubcatefory();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var TEMPTYPE = ApiEncrypt_Decrypt.DecryptString(Aeskey, body.type);
                    MerchantSubCategoryModel tempres = JsonConvert.DeserializeObject<MerchantSubCategoryModel>(TEMPTYPE);
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/SubCategory";
                    List<MerchantSubCategory> allResults123 = new List<MerchantSubCategory>();
                    List<MerchantService> allResults1234 = new List<MerchantService>();
                    int[] bodyValues = { 1, 2 };

                    if (tempres.type == "3")
                    {
                        MerchantSubCategoryModel data = new MerchantSubCategoryModel
                        {
                            type = tempres.type
                        };
                        string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                        var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);
                        returnResponse.response_code = tempResponse.response_code;
                        returnResponse.response_message = tempResponse.response_message;

                        if (tempResponse.response_code == "200")
                        {
                            if (tempResponse.data is JValue && tempResponse.data.Type == JTokenType.String)
                            {
                                string dataString = tempResponse.data.ToString();
                                var resultData = JsonConvert.DeserializeObject<List<MerchantSubCategory>>(dataString);
                                allResults123.AddRange(resultData);
                            }
                        }
                        else
                        {
                            returnResponse.response_message = tempResponse.response_message;
                            returnResponse.response_code = tempResponse.response_code;
                        }
                    }
                    else
                    {
                        foreach (var value in bodyValues)
                        {
                            var Body = value.ToString();
                            MerchantSubCategoryModel data = new MerchantSubCategoryModel
                            {
                                type = Body
                            };
                            string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                            var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);
                            returnResponse.response_code = tempResponse.response_code;
                            returnResponse.response_message = tempResponse.response_message;

                            if (tempResponse.response_code == "200")
                            {
                                if (value == 1)
                                {
                                    if (tempResponse.data is JValue && tempResponse.data.Type == JTokenType.String)
                                    {
                                        string dataString = tempResponse.data.ToString();
                                        var categories = JsonConvert.DeserializeObject<List<MerchantSubCategory>>(dataString);

                                        foreach (var category in categories)
                                        {
                                            category.check = "one";
                                        }

                                        returnResponse.data = new MerchantSubCategoryResponse
                                        {
                                            categories = categories
                                        };
                                        allResults123.AddRange(returnResponse.data.categories);
                                    }
                                }
                                else if (value == 2)
                                {
                                    if (tempResponse.data is JValue && tempResponse.data.Type == JTokenType.String)
                                    {
                                        string dataString = tempResponse.data.ToString();
                                        var services = JsonConvert.DeserializeObject<List<MerchantService>>(dataString);

                                        foreach (var service in services)
                                        {
                                            service.check = "two";
                                        }

                                        returnResponse.data2 = new MerchantServiceResponse
                                        {
                                            services = services
                                        };
                                        allResults1234.AddRange(returnResponse.data2.services);
                                    }
                                }
                            }
                            else
                            {
                                returnResponse.response_message = tempResponse.response_message;
                                returnResponse.response_code = tempResponse.response_code;
                            }
                        }
                    }
                    returnResponse.data = new MerchantSubCategoryResponse
                    {
                        categories = allResults123
                    };
                    returnResponse.data2 = new MerchantServiceResponse
                    {
                        services = allResults1234
                    };
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(ApiResponsesubcatefory));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }




        [HttpGet("Merchantfiltertype")]
        [Produces("application/json")]
        public async Task<ResponseModel> Merchantfiltertype()
        {
            //    _logwrite.LogRequestException("Merchant Controller , Merchantfiltertype :");
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ApiResponseModel1234 returnResponse = new ApiResponseModel1234();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/filtertype";
                    string res = WalletCommon.IRCTCAPI(url);
                    var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);

                    returnResponse.response_code = tempResponse.response_code;
                    returnResponse.response_message = tempResponse.response_message;

                    if (tempResponse.data is JValue && tempResponse.data.Type == JTokenType.String)
                    {
                        string dataString = tempResponse.data.ToString();
                        returnResponse.data = JsonConvert.DeserializeObject<List<MerchantFiltertypeModel>>(dataString);

                    }
                    else
                    {
                        returnResponse.data = new List<MerchantFiltertypeModel>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(ApiResponseModel1234));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }


        [HttpGet("hitVenusThroughAPI1")]
        [Produces("application/json")]
        public async Task<ResponseModel> HitVenusThroughAPI1()
        {
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();

            var paymentIds = await _IMerchant.PaymentRes();
            foreach (var paymentid in paymentIds)
            {
                StatusCheckDataModel111 returnResponse = new StatusCheckDataModel111();

                try
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = $"http://venusrecharge.co.in/TransactionStatus.aspx?authkey=10036&authpass=MOBILEPE@613&ServiceType=&OrderNo=&Merchantrefno=" + paymentid.PaymentId;
                    string res = WalletCommon.HITVENUSPAYAPI(url, tokenVal);
                    var tempResponse = DeserializeXmlResponse(res);
                    string jsonResponse = ConvertXmlToJson(res);
                    var data11 = JsonConvert.DeserializeObject<venusModel12>(jsonResponse);
                    var insresult = await _IMerchant.insertPaymentRes(data11.Response, paymentid.PaymentId);
                    string CustData = JsonConvert.SerializeObject(tempResponse);
                    EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
                    Response.Body += EncryptedText + "; ";

                }
                catch (Exception ex)
                {
                    _logwrite.LogException(ex);
                    throw;
                }
            }

            return Response;
        }
        public static string ConvertXmlToJson(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            return jsonText;
        }
        [NonAction]
        public StatusCheckDataModel111 DeserializeXmlResponse(string xml)
        {
            var serializer = new XmlSerializer(typeof(StatusCheckDataModel111));
            using (var reader = new StringReader(xml))
            {
                return (StatusCheckDataModel111)serializer.Deserialize(reader);
            }
        }
        [XmlRoot("Response")]
        public class StatusCheckDataModel111
        {
            [XmlElement("ResponseStatus")]
            public string ResponseStatus { get; set; }

            [XmlElement("Description")]
            public string Description { get; set; }

            [XmlElement("MerTxnID")]
            public string MerTxnID { get; set; }

            [XmlElement("Mobile")]
            public string Mobile { get; set; }

            [XmlElement("Amount")]
            public string Amount { get; set; }

            [XmlElement("OperatorTxnID")]
            public string OperatorTxnID { get; set; }

            [XmlElement("OrderNo")]
            public string OrderNo { get; set; }
        }



        [HttpPost("GetStock")]
        [Produces("application/json")]
        public async Task<ResponseModel> GetStock(RequestMerchantModel Body)
        {
            //  _logwrite.LogRequestException("Merchant Controller , GetStock :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            GetStockModel returnResponse = new GetStockModel();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/GetStock";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    HitStockListModel data = JsonConvert.DeserializeObject<HitStockListModel>(dcdata);
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                    dynamic tempResponse = JsonConvert.DeserializeObject<dynamic>(res);
                    var stockData = new List<detailsOfStock>();
                    if (tempResponse != null && tempResponse.response_message == "Success")
                    {
                        returnResponse.response_code = tempResponse.response_code;
                        returnResponse.response_message = tempResponse.response_message;
                        if (tempResponse.data != null)
                        {
                            string dataString = tempResponse.data.ToString();
                            stockData = JsonConvert.DeserializeObject<List<detailsOfStock>>(dataString);
                            //dynamic detailStockData = JsonConvert.DeserializeObject<getstockList>(dataString);
                            //JToken tokenObject = JObject.Parse(dataString);
                            //string resp_jdata = (tokenObject != null) ? (tokenObject?.Type == JTokenType.Object) ? "FAILURE" : (tokenObject?.Type == JTokenType.Array && ((JArray)tokenObject).Count > 0) ? "SUCCESS" : "" : "";
                            //string resp_jdata = (tokenObject?["data"] != null) ? (tokenObject?["data"].Type == JTokenType.Object) ? "FAILURE" : (tokenObject?["data"].Type == JTokenType.Array && ((JArray)tokenObject?["data"]).Count > 0) ? "SUCCESS" : "" : "";
                            //var stockData = new List<detailsOfStock>();
                            //if (resp_jdata != "" && resp_jdata == "SUCCESS")
                            //{

                            //}
                            //else
                            //{
                            //    returnResponse.response_code = "000";
                            //    returnResponse.response_message = "Stock not Available";
                            //}
                            //var stockList = new List<getstockList>
                            //{
                            //    new getstockList
                            //    {
                            //        status = tempResponse.data.status.ToString(),
                            //        data = stockData,
                            //        desc = tempResponse.data.desc.ToString(),
                            //        code = tempResponse.data.code.ToString()
                            //    }
                            //};

                            returnResponse.data = stockData;
                        }
                        else
                        {
                            returnResponse.data = new List<detailsOfStock>();
                        }
                    }
                    else // if (tempResponse != null && tempResponse.response_message == "Denomination is not available.")
                    {
                        returnResponse.response_code = tempResponse.response_code;
                        returnResponse.response_message = tempResponse.response_message;

                    }
                    //else
                    //{
                    //    returnResponse.response_code = "000";
                    //    returnResponse.response_message = "Stock not Available";
                    //}

                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(GetStockModel));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("GetStockV1")]
        [Produces("application/json")]
        public async Task<ResponseModel> GetStockV1(RequestMerchantModel Body)
        {
            //  _logwrite.LogRequestException("Merchant Controller , GetStockV1 :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            GetStockModel returnResponse = new GetStockModel();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/GetStockV1";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    HitStockListModelv1 data = JsonConvert.DeserializeObject<HitStockListModelv1>(dcdata);
                    getMobile mobile = await _dataRepository.GetAvailableMobile(data.Fk_MemId);
                    data.mobilenumber = mobile.Mobile.ToString();
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                    dynamic tempResponse = JsonConvert.DeserializeObject<dynamic>(res);
                    var stockData = new List<detailsOfStock>();
                    if (tempResponse != null && tempResponse.response_message == "Success")
                    {
                        returnResponse.response_code = tempResponse.response_code;
                        returnResponse.response_message = tempResponse.response_message;
                        if (tempResponse.data != null)
                        {
                            string dataString = tempResponse.data.ToString();
                            stockData = JsonConvert.DeserializeObject<List<detailsOfStock>>(dataString);
                            //dynamic detailStockData = JsonConvert.DeserializeObject<getstockList>(dataString);
                            //JToken tokenObject = JObject.Parse(dataString);
                            //string resp_jdata = (tokenObject != null) ? (tokenObject?.Type == JTokenType.Object) ? "FAILURE" : (tokenObject?.Type == JTokenType.Array && ((JArray)tokenObject).Count > 0) ? "SUCCESS" : "" : "";
                            //string resp_jdata = (tokenObject?["data"] != null) ? (tokenObject?["data"].Type == JTokenType.Object) ? "FAILURE" : (tokenObject?["data"].Type == JTokenType.Array && ((JArray)tokenObject?["data"]).Count > 0) ? "SUCCESS" : "" : "";
                            //var stockData = new List<detailsOfStock>();
                            //if (resp_jdata != "" && resp_jdata == "SUCCESS")
                            //{

                            //}
                            //else
                            //{
                            //    returnResponse.response_code = "000";
                            //    returnResponse.response_message = "Stock not Available";
                            //}
                            //var stockList = new List<getstockList>
                            //{
                            //    new getstockList
                            //    {
                            //        status = tempResponse.data.status.ToString(),
                            //        data = stockData,
                            //        desc = tempResponse.data.desc.ToString(),
                            //        code = tempResponse.data.code.ToString()
                            //    }
                            //};

                            returnResponse.data = stockData;
                        }
                        else
                        {
                            returnResponse.data = new List<detailsOfStock>();
                        }
                    }
                    else // if (tempResponse != null && tempResponse.response_message == "Denomination is not available.")
                    {
                        returnResponse.response_code = tempResponse.response_code;
                        returnResponse.response_message = tempResponse.response_message;

                    }
                    //else
                    //{
                    //    returnResponse.response_code = "000";
                    //    returnResponse.response_message = "Stock not Available";
                    //}

                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(GetStockModel));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("GetStockV2")]
        [Produces("application/json")]
        public async Task<ResponseModel> GetStockV2(RequestMerchantModel Body)
        {
            //  _logwrite.LogRequestException("Merchant Controller , GetStockV2 :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            GetStockModel returnResponse = new GetStockModel();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/GetStockV2";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    HitStockListModelv2 data = JsonConvert.DeserializeObject<HitStockListModelv2>(dcdata);
                    getMobile mobile = await _dataRepository.GetAvailableMobile(data.Fk_MemId);
                    data.mobilenumber = mobile.Mobile.ToString();
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                    dynamic tempResponse = JsonConvert.DeserializeObject<dynamic>(res);
                    var stockData = new List<detailsOfStock>();
                    if (tempResponse != null && tempResponse.response_message == "Success")
                    {
                        returnResponse.response_code = tempResponse.response_code;
                        returnResponse.response_message = tempResponse.response_message;
                        if (tempResponse.data != null)
                        {
                            string dataString = tempResponse.data.ToString();
                            stockData = JsonConvert.DeserializeObject<List<detailsOfStock>>(dataString);
                            //dynamic detailStockData = JsonConvert.DeserializeObject<getstockList>(dataString);
                            //JToken tokenObject = JObject.Parse(dataString);
                            //string resp_jdata = (tokenObject != null) ? (tokenObject?.Type == JTokenType.Object) ? "FAILURE" : (tokenObject?.Type == JTokenType.Array && ((JArray)tokenObject).Count > 0) ? "SUCCESS" : "" : "";
                            //string resp_jdata = (tokenObject?["data"] != null) ? (tokenObject?["data"].Type == JTokenType.Object) ? "FAILURE" : (tokenObject?["data"].Type == JTokenType.Array && ((JArray)tokenObject?["data"]).Count > 0) ? "SUCCESS" : "" : "";
                            //var stockData = new List<detailsOfStock>();
                            //if (resp_jdata != "" && resp_jdata == "SUCCESS")
                            //{

                            //}
                            //else
                            //{
                            //    returnResponse.response_code = "000";
                            //    returnResponse.response_message = "Stock not Available";
                            //}
                            //var stockList = new List<getstockList>
                            //{
                            //    new getstockList
                            //    {
                            //        status = tempResponse.data.status.ToString(),
                            //        data = stockData,
                            //        desc = tempResponse.data.desc.ToString(),
                            //        code = tempResponse.data.code.ToString()
                            //    }
                            //};

                            returnResponse.data = stockData;
                        }
                        else
                        {
                            returnResponse.data = new List<detailsOfStock>();
                        }
                    }
                    else // if (tempResponse != null && tempResponse.response_message == "Denomination is not available.")
                    {
                        returnResponse.response_code = tempResponse.response_code;
                        returnResponse.response_message = tempResponse.response_message;

                    }
                    //else
                    //{
                    //    returnResponse.response_code = "000";
                    //    returnResponse.response_message = "Stock not Available";
                    //}

                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(GetStockModel));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("GetStoreList")]
        [Produces("application/json")]
        public async Task<ResponseModel> GetStoreList(RequestMerchantModel Body)
        {
            //  _logwrite.LogRequestException("Merchant Controller , GetStoreList :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ResponseModelStore returnResponse = new ResponseModelStore();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/GetStoreList";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    HitStockListModel data = JsonConvert.DeserializeObject<HitStockListModel>(dcdata);
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                    var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);
                    returnResponse.response_code = tempResponse.response_code;
                    returnResponse.response_message = tempResponse.response_message;
                    if (res.Contains("Success"))
                    {
                        var innerDataJson = tempResponse.data.ToString();
                        var innerData = JsonConvert.DeserializeObject<storInnderModel>(innerDataJson);
                        returnResponse.data = innerData;
                    }
                    else
                    {
                        returnResponse = new ResponseModelStore();
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(ResponseModelStore));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("GetBrandss")]
        [Produces("application/json")]
        public async Task<ResponseModel> GetBrandss(RequestMerchantModel Body)
        {
            //  _logwrite.LogRequestException("Merchant Controller , GetBrandss :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ApiBrands returnResponse = new ApiBrands();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/GetBrands";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    ApiGetBrands data = JsonConvert.DeserializeObject<ApiGetBrands>(dcdata);
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                    var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);
                    returnResponse.response_code = tempResponse.response_code;
                    returnResponse.response_message = tempResponse.response_message;

                    if (res.Contains("Success"))
                    {
                        var datastring = tempResponse.data.data.ToString();
                        var stat = tempResponse.data.status.ToString();
                        var brandslist = JsonConvert.DeserializeObject<List<GetBrands>>(datastring);
                        tempBrands tb = new tempBrands();
                        tb.status = stat;
                        tb.data = brandslist;

                        returnResponse.data = tb;
                    }
                    else
                    {
                        returnResponse.data = new tempBrands();
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }


            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(ApiBrands));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("GetVoucherDetails")]
        [Produces("application/json")]
        public async Task<ResponseModel> GetVoucherDetails(RequestMerchantModel Body)
        {
            // _logwrite.LogRequestException("Merchant Controller , GetVoucherDetails :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ApiVoucher returnResponse = new ApiVoucher();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/GetVoucherDetails";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    ApiVoucherList data = JsonConvert.DeserializeObject<ApiVoucherList>(dcdata);
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                    var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);
                    returnResponse.response_code = tempResponse.response_code;
                    returnResponse.response_message = tempResponse.response_message;

                    if (res.Contains("Success"))
                    {
                        var datastring = tempResponse.data.data.ToString();
                        var brandslist = JsonConvert.DeserializeObject<List<Shopdetails>>(datastring);
                        returnResponse.data = brandslist;
                    }
                    else
                    {
                        returnResponse.data = new Shopdetails();
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }


            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(ApiVoucher));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("CheckVoucherStatus")]
        [Produces("application/json")]
        public async Task<ResponseModel> CheckVoucherStatus(RequestMerchantModel Body)
        {
            // _logwrite.LogRequestException("Merchant Controller , CheckVoucherStatus :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            ApiVoucherStatus returnResponse = new ApiVoucherStatus();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    var url = "https://vendorapi.mobilepe.co.in/api/mobilepe/CheckVoucherStatus";
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    APICVS data = JsonConvert.DeserializeObject<APICVS>(dcdata);
                    string res = WalletCommon.HITMERAPI(url, data, Aeskey);
                    var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);
                    returnResponse.response_code = tempResponse.response_code;
                    returnResponse.response_message = tempResponse.response_message;

                    if (returnResponse.response_code == "200" && returnResponse.response_message == "Success")
                    {
                        var datastring = tempResponse.data.data.ToString();
                        var voucherstat = JsonConvert.DeserializeObject<storInnderModel>(datastring);
                        returnResponse.data = voucherstat;
                    }
                    else
                    {
                        returnResponse.data = new storInnderModel();
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }


            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(ApiVoucherStatus));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }



        //[HttpPost("GetOffer")]
        //[Produces("application/json")]
        //public async Task<ResponseModel> GetOffer(RequestMerchantModel Body)
        //{
        //    _logwrite.LogRequestException("Merchant Controller, GetOffer: " + Body.Body);
        //    string EncryptedText = "";
        //    string Aeskey = "";
        //    ResponseModel Response = new ResponseModel();
        //    getOfferResponse returnResponse = new getOfferResponse
        //    {
        //        data = new List<getOffer>()
        //    };

        //    try
        //    {
        //        if (string.IsNullOrEmpty(Request.Headers["Token"]))
        //        {
        //            returnResponse.response_message = "Please pass token";
        //            returnResponse.response_code = "402";
        //        }
        //        else
        //        {
        //            string tokenVal = Request.Headers["Token"].ToString();
        //            string[] split = tokenVal.Split("-");
        //            Aeskey = split[1];
        //            var url = "https://cprechargeapi.mobilepe.co.in/api/mobilepe/cp/StatusCheck";
        //            string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
        //            getOffer data = JsonConvert.DeserializeObject<getOffer>(dcdata);
        //            string res = WalletCommon.HITMERAPI(url, data, Aeskey);
        //            var tempResponse = JsonConvert.DeserializeObject<dynamic>(res);


        //            returnResponse.response_code = tempResponse.response_code;
        //            returnResponse.response_message = tempResponse.response_message;
        //            if (tempResponse.data != null && tempResponse.data is JArray)
        //            {
        //                foreach (var offer in tempResponse.data)
        //                {
        //                    var offerDetails = offer.ToObject<getOffer>();
        //                    decimal initialAmount = decimal.Parse(offerDetails.Initialamount);
        //                    decimal discountPercentage = decimal.Parse(offerDetails.commissionPercentage);
        //                    decimal discountAmount = (initialAmount * discountPercentage) / 100;
        //                    decimal finalAmount = initialAmount - discountAmount;
        //                    var calculatedOffer = new getOffer
        //                    {
        //                        branccode = offerDetails.branccode,
        //                        Initialamount = offerDetails.Initialamount,
        //                        commissionPercentage = offerDetails.commissionPercentage,
        //                        finalamount = finalAmount.ToString("F2"),
        //                        discountamount = discountAmount.ToString("F2")
        //                    };

        //                    returnResponse.data.Add(calculatedOffer);
        //                }
        //            }
        //            else
        //            {
        //                returnResponse.data = new List<getOffer>();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logwrite.LogException(ex);
        //        throw;
        //    }

        //    string CustData = "";
        //    DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(StatusCheckDataModel1));
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        js.WriteObject(ms, returnResponse);
        //        ms.Position = 0;
        //        using (StreamReader sr = new StreamReader(ms))
        //        {
        //            CustData = sr.ReadToEnd();
        //        }
        //    }
        //    EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
        //    Response.Body = EncryptedText;
        //    return Response;
        //}



        [HttpPost("GetOffer")]
        [Produces("application/json")]
        public async Task<ResponseModel> GetOffer([FromBody] RequestMerchantModel Body)
        {
            //_logwrite.LogRequestException("Merchant Controller, GetOffer: " + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            getOfferResponse returnResponse = new getOfferResponse
            {
                data = new List<getOffer>()
            };

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    getOffer data1 = JsonConvert.DeserializeObject<getOffer>(dcdata);
                    decimal initialAmount = decimal.Parse(data1.Initialamount);
                    decimal discountPercentage = 15m;
                    decimal discountAmount = (initialAmount * discountPercentage) / 100;
                    decimal finalAmount = initialAmount - discountAmount;
                    var calculatedOffer = new getOffer
                    {
                        brandcode = "BR123",
                        Initialamount = initialAmount.ToString("F2"),
                        commissionPercentage = discountPercentage.ToString("F2"),
                        finalamount = finalAmount.ToString("F2"),
                        discountamount = discountAmount.ToString("F2")
                    };

                    returnResponse.data.Add(calculatedOffer);
                    returnResponse.response_code = "200";
                    returnResponse.response_message = "Success";
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(getOfferResponse));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }




        [HttpPost("GetMerchantBalance")]
        [Produces("application/json")]
        public async Task<ResponseModel> GetMerchantBalance(RequestMerchantModel Body)
        {
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            MerchantBody_Res returnResponse = new MerchantBody_Res();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    MerchantBalance_Req data = JsonConvert.DeserializeObject<MerchantBalance_Req>(dcdata);
                    List<GetMerchantBalance_Res> getMerchantBalance_ = await _dataRepository.GetMerchantBalance(data.mobile);
                    if (getMerchantBalance_.Count > 0)
                    {
                        if (getMerchantBalance_[0].Responsecode == "100")
                        {
                            returnResponse.response_code = "200";
                            returnResponse.response_message = "Success";
                        }
                        returnResponse.data = JsonConvert.SerializeObject(getMerchantBalance_);
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(MerchantBody_Res));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("GetSettlementTrnxReport")]
        [Produces("application/json")]
        public async Task<ResponseModel> GetSettlementTrnxReport(RequestMerchantModel Body)
        {
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            MerchantBody_Res returnResponse = new MerchantBody_Res();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    SettlementTrnxReport_Req data = JsonConvert.DeserializeObject<SettlementTrnxReport_Req>(dcdata);
                    List<object> getMerchantBalance_ = await _dataRepository.GetSettlementTrnxReport(data.mobilenumber);
                    if (getMerchantBalance_.Count > 0)
                    {
                        returnResponse.response_code = "200";
                        returnResponse.response_message = "Success";
                        returnResponse.data = JsonConvert.SerializeObject(getMerchantBalance_);
                    }

                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(MerchantBody_Res));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }


        [HttpPost("GetAvailableCommission")]
        [Produces("application/json")]
        public async Task<ResponseModel> GetAvailableCommission(RequestMerchantModel Body)
        {
            _log.WriteToFile("GetAvailableCommission Process started");
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            MerchantBody_Res returnResponse = new MerchantBody_Res();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    _log.WriteToFile(" Process Token is missing.");
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    _log.WriteToFile("GetAvailableCommission Process Request Input with Encrypt " + Body.Body);
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    _log.WriteToFile("GetAvailableCommission Process Request Input with Decrypt " + dcdata);
                    GetAvailableCommission_Req data = JsonConvert.DeserializeObject<GetAvailableCommission_Req>(dcdata);
                    List<object> getMerchantDBres_ = await _dataRepository.GetAvailableCommission(data.Fk_memid, data.Brandproductcode);
                    if (getMerchantDBres_.Count > 0)
                    {
                        returnResponse.response_code = "200";
                        returnResponse.response_message = "Success";
                        returnResponse.data = JsonConvert.SerializeObject(getMerchantDBres_);
                    }

                }
            }
            catch (Exception ex)
            {
                _log.WriteToFile("Error occur in GetAvailableCommission : " + ex.ToString());
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(MerchantBody_Res));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            _log.WriteToFile("GetAvailableCommission Process Ended");
            return Response;
        }

        [HttpPost("ApplyCommissionReferal")]
        [Produces("application/json")]
        public async Task<ResponseModel> ApplyCommissionReferal(RequestMerchantModel Body)
        {
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            MerchantBody_Res returnResponse = new MerchantBody_Res();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    ApplyCommissionReferal_Req data = JsonConvert.DeserializeObject<ApplyCommissionReferal_Req>(dcdata);
                    List<object> getMerchantDBres_ = await _dataRepository.ApplyCommissionReferal(data);
                    if (getMerchantDBres_.Count > 0)
                    {
                        returnResponse.response_code = "200";
                        returnResponse.response_message = "Success";
                        returnResponse.data = JsonConvert.SerializeObject(getMerchantDBres_);
                    }

                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(MerchantBody_Res));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("MerchantReferalDebit")]
        [Produces("application/json")]
        public async Task<ResponseModel> MerchantReferalDebit(RequestMerchantModel Body)
        {
            _logwrite.LogRequestException("Merchant Controller , MerchantReferalDebit :" + Body.Body);
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            MerchantBody_Res returnResponse = new MerchantBody_Res();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    MerchantDebitReferal_Req data = JsonConvert.DeserializeObject<MerchantDebitReferal_Req>(dcdata);
                    List<object> getMerchantDBres_ = await _dataRepository.Redeemcommissionwallet(data);
                    if (getMerchantDBres_.Count > 0)
                    {
                        object lsRes = getMerchantDBres_[0];
                        var json = JObject.FromObject(lsRes);
                        if (json["Responsecode"].ToString() == "100")
                        {
                            returnResponse.response_code = "200";
                            returnResponse.response_message = "Success";
                            //JObject walletReqParamReqJson = JObject.Parse(data.Request);
                            if (data.Type.ToLower() == "merchant")
                            {
                                try
                                {
                                    string PaymentId = data.orderid;
                                    string URL = "https://vendorapi.mobilepe.co.in/api/mobilepe/MerchantCreditRequest";
                                    getMobile mobile = await _dataRepository.GetAvailableMobile(data.FK_memid);
                                    //MerchantModelDB requestMobile = JsonConvert.DeserializeObject<MerchantModelDB>(walletTopup.request);
                                    string merId = data.Request;
                                    JObject reqjson = JObject.Parse(merId);
                                    string merchantId = reqjson["Merchantid"].ToString();
                                    string transactionAmount = reqjson["TransactionAmount"].ToString();
                                    string customername = reqjson["Customername"].ToString();
                                    string CustomerRemarks = reqjson["CustomerRemarks"].ToString();
                                    string mobileno = reqjson["mobilenumber"].ToString();
                                    if (CustomerRemarks == null || CustomerRemarks == "")
                                    {
                                        CustomerRemarks = "good";
                                    }
                                    var requestData = new MerchantModelDB
                                    {
                                        CreditrequestTrasactionid = PaymentId,
                                        Merchantid = merchantId,
                                        TransactionAmount = transactionAmount,
                                        Customername = mobile.FirstName.ToString(),
                                        mobilenumber = mobile.Mobile.ToString(),
                                        CustomerRemarks = CustomerRemarks,
                                    };
                                    string jsonRequest = JsonConvert.SerializeObject(requestData);
                                    string operatorName = "MERCHANT";
                                    string OperatorTxnID = "MERCHANT";
                                    var result1 = CommonJsonPostRequest.CommonSendRequest(URL, "POST", jsonRequest);
                                    JObject responseFromMerchant = JObject.Parse(result1);
                                    string status = responseFromMerchant["response_message"].ToString();
                                    // string status = responseMessage;
                                    string Remark = "transaction for merchant";

                                    var transactiondata = await _dataRepository.SaveTransactionData(data.FK_memid, data.orderid.ToString(), "Wallet Debit", Remark, data.Type, data.Amount, OperatorTxnID, status, mobile.Mobile.ToString(), operatorName, result1);
                                }
                                catch (Exception ex)
                                {
                                    _logwrite.LogRequestException("error Merchant Controller - Merchant , MerchantReferalDebit :" + ex.ToString());
                                }

                            }
                            else if (data.Type.ToLower() == "merchantvoucher")
                            {
                                try
                                {
                                    string PaymentId = data.orderid;
                                    getMobile mobile = await _dataRepository.GetAvailableMobile(data.FK_memid);
                                    string URL = "https://vendorapi.mobilepe.co.in/api/mobilepe/MerchantCreditRequestv1";
                                    //MerchantModelDB requestMobile = JsonConvert.DeserializeObject<MerchantModelDB>(walletTopup.request);
                                    string merId = data.Request;
                                    JObject reqjson = JObject.Parse(merId);
                                    string merchantId = reqjson["Merchantid"].ToString();
                                    //string transactionAmount = json["TransactionAmount"].ToString();
                                    string customername = reqjson["Customername"].ToString();
                                    string CustomerRemarks = reqjson["CustomerRemarks"].ToString();
                                    //string qty = json["qty"].ToString();
                                    //string Denomination = json["Denomination"].ToString();
                                    string stockTransactionid = reqjson["stockTransactionid"].ToString();
                                    if (CustomerRemarks == null || CustomerRemarks == "")
                                    {
                                        CustomerRemarks = "good";
                                    }
                                    _logwrite.LogRequestException("Wallet Controller , WalletDebitNew Merchant Before Started");
                                    var requestData = new MerchantVoucherModelDB
                                    {
                                        CreditrequestTrasactionid = PaymentId,
                                        Merchantid = merchantId,
                                        //TransactionAmount = transactionAmount,
                                        Customername = mobile.FirstName.ToString(),
                                        mobilenumber = mobile.Mobile.ToString(),
                                        CustomerRemarks = CustomerRemarks,
                                        //qty = qty,
                                        Emaild = mobile.Email.ToString(),
                                        //Denomination = Denomination,
                                        stockTransactionid = stockTransactionid
                                    };
                                    string jsonRequest = JsonConvert.SerializeObject(requestData);
                                    string operatorName = "MERCHANT";
                                    string OperatorTxnID = "MERCHANT";
                                    var result1 = CommonJsonPostRequest.CommonSendRequest(URL, "POST", jsonRequest);
                                    JObject responseFromMerchant = JObject.Parse(result1);
                                    string status = responseFromMerchant["response_message"].ToString();
                                    // string status = responseMessage;
                                    string Remark = "transaction for Merchant Voucher";

                                    var transactiondata = await _dataRepository.SaveTransactionData(data.FK_memid, data.orderid.ToString(), "Wallet Debit", Remark, data.Type, data.Amount, OperatorTxnID, status, mobile.Mobile.ToString(), operatorName, result1);
                                }
                                catch (Exception ex)
                                {
                                    _logwrite.LogRequestException("error Merchant Controller - MerchantVoucher , MerchantReferalDebit :" + ex.ToString());
                                }

                            }
                        }
                        returnResponse.data = JsonConvert.SerializeObject(getMerchantDBres_);
                    }

                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(MerchantBody_Res));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }




        [HttpPost("UpdateUsedFlag")]
        [Produces("application/json")]
        public async Task<ResponseModel> UpdateUsedFlag(RequestMerchantModel Body)
        {
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            MerchantBody_Res returnResponse = new MerchantBody_Res();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    Updateusedflag_req data = JsonConvert.DeserializeObject<Updateusedflag_req>(dcdata);
                    List<object> getMerchantDBres_ = await _dataRepository.upd_usedflag(data.Transactionid);
                    if (getMerchantDBres_.Count > 0)
                    {
                        object lsRes = getMerchantDBres_[0];
                        var json = JObject.FromObject(lsRes);
                        if (json["Responsecode"].ToString() == "100")
                        {
                            returnResponse.response_code = "200";
                            returnResponse.response_message = "Success";
                        }
                        returnResponse.data = JsonConvert.SerializeObject(getMerchantDBres_);
                    }

                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(MerchantBody_Res));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }

        [HttpPost("CustomertranactionList")]
        [Produces("application/json")]
        public async Task<ResponseModel> CustomertranactionList(RequestMerchantModel Body)
        {
            string EncryptedText = "";
            string Aeskey = "";
            ResponseModel Response = new ResponseModel();
            MerchantBody_Res returnResponse = new MerchantBody_Res();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    returnResponse.response_message = "Please pass token";
                    returnResponse.response_code = "402";
                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, Body.Body);
                    CustomertranactionList_req data = JsonConvert.DeserializeObject<CustomertranactionList_req>(dcdata);
                    List<object> getMerchantDBres_ = await _dataRepository.CustomertranactionList(data);
                    if (getMerchantDBres_.Count > 0)
                    {
                        returnResponse.response_code = "200";
                        returnResponse.response_message = "Success";
                        returnResponse.data = JsonConvert.SerializeObject(getMerchantDBres_);
                    }
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(MerchantBody_Res));
            ms = new MemoryStream();
            js.WriteObject(ms, returnResponse);
            ms.Position = 0;
            using (StreamReader sr = new StreamReader(ms))
            {
                CustData = sr.ReadToEnd();
            }
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            Response.Body = EncryptedText;
            return Response;
        }



    }

}




