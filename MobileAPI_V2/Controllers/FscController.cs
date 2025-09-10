using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MobileAPI_V2.DataLayer;
using MobileAPI_V2.Services;
using MobileAPI_V2.Model;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static MobileAPI_V2.Model.FSCModel;
using static MobileAPI_V2.Model.BillPayment.BillPaymentCommon;
using MobileAPI_V2.Model.BillPayment;
using System;
using MobileAPI_V2.Models;
using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace MobileAPI_V2.Controllers
{
    [ApiController]
    [Route("api/FSC/")]
    public class FSCController : ControllerBase
    {
        private readonly IFSC _IFSC;
        string AESKEY = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build().GetSection("AESKEY").Value;
        //IFSC _IFSC;
        private readonly LogWrite _logwrite;
        public FSCController(IFSC Ifsc)
        {
            _IFSC = Ifsc;

        }
        
        [HttpGet("GetBankDetails")]

        public async Task<ResponseModel> GetBankDetails()
        {
            string EncryptedText = "";
            //BankResponse bnksss=new BankResponse();
            FSCRes bankDetails = new FSCRes();
            ResponseModel returnResponse = new ResponseModel();
            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {

                    bankDetails.response_message = "Token not Passed";

                }
                else
                {
                    bankDetails.Data = await _IFSC.GetBankDetails();
   
                    if (bankDetails != null)
                    {
                        bankDetails.response_message = "Success";

                    }
                    
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
            
            js = new DataContractJsonSerializer(typeof(FSCRes));
            ms = new MemoryStream();
            js.WriteObject(ms, bankDetails);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(AESKEY, CustData);
            returnResponse.Body = EncryptedText;
            return returnResponse;


        }


        [HttpGet("GetCreditCardDetails")]

        public async Task<ResponseModel> GetCreditCardDetails(string id)
        {
            string EncryptedText = "";
         
            ResponseModel returnResponse = new ResponseModel();
            FSCCardRes CardDetails = new FSCCardRes();
            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    CardDetails.response_message = "Token not Passed";

                }
                else
                {
                    string decryptedId = ApiEncrypt_Decrypt.DecryptString(AESKEY, id);
                    CardDetails.Data = await _IFSC.GetCreditCardDetails(long.Parse(decryptedId));
                    CardDetails.response_message = "Success";
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
            js = new DataContractJsonSerializer(typeof(FSCCardRes));
            ms = new MemoryStream();
          
            js.WriteObject(ms, CardDetails);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(AESKEY, CustData);
            returnResponse.Body = EncryptedText;
            return returnResponse;
           
        }


        [HttpPost("ValidatePinCode")]

        public async Task<ResponseModel> ValidatePinCode(string creditID, string pincode)
        {
            string EncryptedText = "";
            var response = "";
            ValidatePincodeRes res=new ValidatePincodeRes();
            ResponseModel returnResponse = new ResponseModel();
           
            try
            {

                string deCreditId = ApiEncrypt_Decrypt.DecryptString(AESKEY, creditID);
                string dePincode = ApiEncrypt_Decrypt.DecryptString(AESKEY, pincode);
                response = await _IFSC.ValidatePinCode(long.Parse(deCreditId), long.Parse(dePincode));
                if (response == "success")
                {
                    res.Data = response;
                    res.response_message = "success";
                }
                else
                {
                    res.Data = response;
                    res.response_message = "failure";

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
            js = new DataContractJsonSerializer(typeof(ValidatePincodeRes));
            ms = new MemoryStream();

            js.WriteObject(ms, res);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(AESKEY, CustData);
            returnResponse.Body = EncryptedText;
            return returnResponse;
            
           
           
        }


        [HttpPost("SaveFscApplicants")]
        public async Task<ResponseModel> SaveFscApplicants(RequestModel requestModel)
        {
            string resdata = "";
            string EncryptedText = "";
            string Aeskey = "";
            FscResponse objres = new FscResponse();
            ResponseModel returnResponse = new ResponseModel();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    objres.Message = "Please pass token";
                    objres.data = "Bad Request";

                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, requestModel.Body);
                    FscApplicants data = JsonConvert.DeserializeObject<FscApplicants>(dcdata);

                    resdata = await _IFSC.SaveFscApplicants(data);

                    if (resdata =="Saved Successfully")
                       {
                        objres.data=resdata;
                        objres.Message = "Success";
                       }
                    else
                    {
                        objres.data = resdata;
                        objres.Message = "Failure";
                        
                    }
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
            js = new DataContractJsonSerializer(typeof(FscResponse));
            ms = new MemoryStream();
            js.WriteObject(ms, objres);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            returnResponse.Body = EncryptedText;
            return returnResponse;
        }

        [HttpPost("SaveBankFscLogs")]
        public async Task<ResponseModel> SaveBankFscLogs(RequestModel requestModel)
        {


            string EncryptedText = "";
            string Aeskey = "";
            FscResponse objres = new FscResponse();
            ResponseModel returnResponse = new ResponseModel();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    objres.Message = "Please pass token";
                    objres.data = "Bad Request";

                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, requestModel.Body);
                    BankFscLogs data = JsonConvert.DeserializeObject<BankFscLogs>(dcdata);

                    objres.data = await _IFSC.SaveBankFscLogs(data);
                    objres.Message = "Success";
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
            js = new DataContractJsonSerializer(typeof(FscResponse));
            ms = new MemoryStream();
            js.WriteObject(ms, objres);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            returnResponse.Body = EncryptedText;
            return returnResponse;

        }

        [HttpPost("SaveCreditFscLogs")]
        public async Task<ResponseModel> SaveCreditFscLogs(RequestModel requestModel)
        {
       
            string EncryptedText = "";
            string Aeskey = "";
            FscResponse objres = new FscResponse();
            ResponseModel returnResponse = new ResponseModel();

            try
            {
                if (string.IsNullOrEmpty(Request.Headers["Token"]))
                {
                    objres.Message = "Please pass token";
                    objres.data = "Bad Request";

                }
                else
                {
                    string tokenVal = Request.Headers["Token"].ToString();
                    string[] split = tokenVal.Split("-");
                    Aeskey = split[1];
                    string dcdata = ApiEncrypt_Decrypt.DecryptString(Aeskey, requestModel.Body);
                    CreditFscLogs data = JsonConvert.DeserializeObject<CreditFscLogs>(dcdata);

                    objres.data = await _IFSC.SaveCreditFscLogs(data);
                    objres.Message = "Success";
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
            js = new DataContractJsonSerializer(typeof(FscResponse));
            ms = new MemoryStream();
            js.WriteObject(ms, objres);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(Aeskey, CustData);
            returnResponse.Body = EncryptedText;
            return returnResponse;

        }


        [HttpGet("GetFscApplicants")]
        public async Task<ResponseModel> GetFscApplicants(string FSC_MemId)
        {
            string EncryptedText = "";

            ResponseModel returnResponse = new ResponseModel();
            List<GetFscAppl> GetGscDetails = new List<GetFscAppl>();
            try
            {
                string decryptedFSC_MemId = ApiEncrypt_Decrypt.DecryptString(AESKEY, FSC_MemId);
                GetGscDetails = await _IFSC.GetFscApplicants(decryptedFSC_MemId);

            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                throw ex;
            }

            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(List<GetFscAppl>));
            ms = new MemoryStream();
            js.WriteObject(ms, GetGscDetails);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(AESKEY, CustData);
            returnResponse.Body = EncryptedText;
            return returnResponse;

        }


   

        [HttpPost("UpdateSoundFlag")]

        public async Task<ResponseModel> UpdateSoundFlag(string memberId, string flag)
        {
            string EncryptedText = "";
            var response = "";
            ValidatePincodeRes res = new ValidatePincodeRes();
            ResponseModel returnResponse = new ResponseModel();

            try
            {

                string deMemberId = ApiEncrypt_Decrypt.DecryptString(AESKEY, memberId);
                var deflag = ApiEncrypt_Decrypt.DecryptString(AESKEY, flag);
                response = await _IFSC.UpdateSoundFlag(long.Parse(deMemberId), bool.Parse(deflag));
                if (response == "success")
                {
                    res.Data = response;
                    res.response_message = "success";
                }
                else
                {
                    res.Data = response;
                    res.response_message = "failure";

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
            js = new DataContractJsonSerializer(typeof(ValidatePincodeRes));
            ms = new MemoryStream();

            js.WriteObject(ms, res);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(AESKEY, CustData);
            returnResponse.Body = EncryptedText;
            return returnResponse;



        }
    }

}




