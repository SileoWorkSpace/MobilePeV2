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
            string EncryptedText = string.Empty;
            var res = new CommonResponseEcomm<bpcl_wallet_response>();
            var returnResponse = new ResponseModel();
            var bpcl_Response = new bpcl_wallet_response();
            try
            {
                if (string.IsNullOrEmpty(mobile_no))
                {
                    res = new CommonResponseEcomm<bpcl_wallet_response>
                    {
                        message = "Please pass Mobile No",
                        Status = 0
                    };
                }
                else
                {
                    bpcl_wallet_request bpcl_Request = new()
                    {
                        mobile_no = ApiEncrypt_Decrypt.DecryptString(AESKEY, mobile_no.Replace(" ", "+")),
                    };
                    DataSet dataSet = bpcl_Request.get_wallet_balance();
                    if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        DataRow row = dataSet.Tables[0].Rows[0];

                        if (row["Status"]?.ToString() == "1")
                        {
                            bpcl_Response.balance = decimal.TryParse(row["balance"]?.ToString(), out var bal) ? bal : 0;
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
            string EncryptedText = string.Empty;
            var res = new CommonResponseEcomm<bpcl_trnasaction_response>();
            var returnResponse = new ResponseModel();
            var bpcl_trnasaction_response = new bpcl_trnasaction_response();
            try
            {
                if (string.IsNullOrEmpty(mobile_no))
                {
                    res = new CommonResponseEcomm<bpcl_trnasaction_response>
                    {
                        message = "Please pass Mobile No",
                        Status = 0
                    };
                }
                if (string.IsNullOrEmpty(page))
                {
                    res = new CommonResponseEcomm<bpcl_trnasaction_response>
                    {
                        message = "Please pass page no",
                        Status = 0
                    };          
                }
                else
                {
                    bpcl_transaction_request bpcl_transaction_request = new()
                    {
                        mobile_no = ApiEncrypt_Decrypt.DecryptString(AESKEY, mobile_no.Replace(" ", "+")),
                        page = int.Parse(ApiEncrypt_Decrypt.DecryptString(AESKEY, page.Replace(" ", "+")))
                    };
                    DataSet dataSet = bpcl_transaction_request.get_bpcl_transaction();
                    if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        DataTable table = dataSet.Tables[0];
                        DataRow firstRow = table.Rows[0];
                        if (firstRow["Status"]?.ToString() == "1")
                        {
                            var lstbpcldata = table.AsEnumerable().Select(row => new bpcl_trans_data
                            {
                                trans_id = row["trans_id"]?.ToString(),
                                amount = decimal.TryParse(row["amount"]?.ToString(), out var amt) ? amt : 0,
                                narration = row["narration"]?.ToString(),
                                tran_date = row["tran_date"]?.ToString(),
                                trans_type = row["trans_type"]?.ToString()
                            }).ToList();
                            bpcl_trnasaction_response.transaction_lst = lstbpcldata;
                            bpcl_trnasaction_response.total_record = long.TryParse(firstRow["total_record"]?.ToString(), out var total) ? total : 0;
                            res.Status = 1;
                            res.result = bpcl_trnasaction_response;
                        }
                        else
                        {
                            res.Status = int.TryParse(firstRow["Status"]?.ToString(), out var status) ? status : 0;
                            res.result = new bpcl_trnasaction_response
                            {
                                transaction_lst = new List<bpcl_trans_data>(),
                                total_record = 0
                            };
                        }
                    }
                    else
                    {
                        res.Status = 0;
                        res.result = new bpcl_trnasaction_response
                        {
                            transaction_lst = new List<bpcl_trans_data>(),
                            total_record = 0
                        };
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
