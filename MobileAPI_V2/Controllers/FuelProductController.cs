using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileAPI_V2.Model.BillPayment;
using MobileAPI_V2.Model.fuel_product;
using MobileAPI_V2.Model;
using MobileAPI_V2.Services;
using static MobileAPI_V2.Model.BillPayment.BillPaymentCommon;
using System.Data;
using System.Runtime.Serialization.Json;
using MobileAPI_V2.Logs;
using System.ComponentModel.DataAnnotations;

namespace MobileAPI_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuelProductController : ControllerBase
    {
        private readonly LogWrite _logwrite;
        private readonly Logger _log;
        private static readonly ConfigurationBuilder configurationBuilder = new();
        private readonly string AESKEY;
        public FuelProductController()
        {
            _logwrite = new LogWrite();
            _log = new Logger();
            var configuration = configurationBuilder.AddJsonFile($"appsettings.json").Build();
            AESKEY = configuration.GetSection("AESKEY").Value ?? throw new InvalidOperationException("AESKEY is not configured in appsettings.json");
        }
        [HttpGet("bpcl_balance")]
        public async Task<ResponseModel> BpclBalanceAsync([Required] string mobile_no)
        {
            _log.WriteToFile("bocl_wallet_balance Process Started");
            string EncryptedText;
            CommonResponseEcomm<bpcl_wallet_response> res = new();
            ResponseModel returnResponse = new();
            bpcl_wallet_response bpcl_Response = new();
            try
            {
                if (string.IsNullOrEmpty(mobile_no))
                {
                    res.message = "Please pass Mobile No";
                    res.Status = 0;
                }

                else
                {
                    mobile_no = mobile_no.Replace(" ", "+");
                    mobile_no = ApiEncrypt_Decrypt.DecryptString(AESKEY, mobile_no);
                    bpcl_wallet_request bpcl_Request = new()
                    {
                        mobile_no = mobile_no
                    };
                    DataSet dataSet = bpcl_Request.get_wallet_balance();
                    if (dataSet != null)
                    {
                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            if (dataSet.Tables[0].Rows[0]["Status"].ToString() == "1")
                            {
                                bpcl_Response.balance = decimal.Parse(dataSet.Tables[0].Rows[0]["balance"].ToString()??"0");

                                res.Status = 1;
                                res.result = bpcl_Response;
                            }
                            else
                            {

                                bpcl_Response.balance = 0;
                                res.Status = 0;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                res.Status = 0;
                res.message = ex.Message;
            }
            string CustData;
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(CommonResponseEcomm<bpcl_wallet_response>));
            ms = new MemoryStream();
            js.WriteObject(ms, res);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = await sr.ReadToEndAsync();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(AESKEY, CustData);
            returnResponse.Body = EncryptedText;
            _log.WriteToFile("bocl_wallet_balance Process Ended");
            return returnResponse;

        }
        [HttpGet("bpcl_transaction")]
        public async Task<ResponseModel> BpclTransactionAsync([Required] string mobile_no, [Required] string page)
        {
            _log.WriteToFile("bpcl_transaction Process Started");
            string EncryptedText;
            CommonResponseEcomm<bpcl_trnasaction_response> res = new();
            ResponseModel returnResponse = new();
            bpcl_trnasaction_response bpcl_trnasaction_response = new();
            try
            {
                if (string.IsNullOrEmpty(mobile_no))
                {
                    res.message = "Please pass Mobile No";
                    res.Status = 0;
                }
                if (string.IsNullOrEmpty(page))
                {
                    res.message = "Please pass page no";
                    res.Status = 0;
                }
                else
                {
                    mobile_no = mobile_no.Replace(" ", "+");
                    mobile_no = ApiEncrypt_Decrypt.DecryptString(AESKEY, mobile_no);
                    page = page.Replace(" ", "+");
                    page = ApiEncrypt_Decrypt.DecryptString(AESKEY, page);

                    bpcl_transaction_request bpcl_transaction_request = new()
                    {
                        mobile_no = mobile_no,
                        page = int.Parse(page)
                    };
                    DataSet dataSet = bpcl_transaction_request.get_bpcl_transaction();
                    if (dataSet != null)
                    {
                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            if (dataSet.Tables[0].Rows[0]["Status"].ToString() == "1")
                            {

                                List<bpcl_trans_data> lstbpcldata = [];
                                for (int k = 0; k <= dataSet.Tables[0].Rows.Count - 1; k++)
                                {
                                    bpcl_trans_data bpcl_trans_data = new()
                                    {
                                        trans_id = dataSet.Tables[0].Rows[k]["trans_id"].ToString(),
                                        amount = decimal.Parse(dataSet.Tables[0].Rows[k]["amount"].ToString()??"0"),
                                        narration = dataSet.Tables[0].Rows[k]["narration"].ToString(),
                                        tran_date = dataSet.Tables[0].Rows[k]["tran_date"].ToString(),
                                        trans_type = dataSet.Tables[0].Rows[k]["trans_type"].ToString()
                                    };

                                    lstbpcldata.Add(bpcl_trans_data);
                                }
                                bpcl_trnasaction_response.transaction_lst = lstbpcldata;
                                bpcl_trnasaction_response.total_record = Int64.Parse(dataSet.Tables[0].Rows[0]["total_record"].ToString() ?? "0");
                                res.Status = 1;
                                res.result = bpcl_trnasaction_response;
                            }
                            else
                            {
                               res.Status = int.Parse(dataSet.Tables[0].Rows[0]["Status"].ToString() ?? "0");
                               res.result = new bpcl_trnasaction_response
                                {
                                    transaction_lst = [],
                                    total_record = 0
                                };
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                res.Status = 0;
                res.message = ex.Message;
            }
            string CustData;
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(CommonResponseEcomm<bpcl_trnasaction_response>));
            ms = new MemoryStream();
            js.WriteObject(ms, res);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = await sr.ReadToEndAsync();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(AESKEY, CustData);
            returnResponse.Body = EncryptedText;
            _log.WriteToFile("bpcl_transaction Process Ended");
            return returnResponse;

        }
    }
}
