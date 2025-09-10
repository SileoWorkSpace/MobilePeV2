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

namespace MobileAPI_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuelProductController : ControllerBase
    {
        private readonly LogWrite _logwrite;
        private readonly Logger _log;
        string AESKEY = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build().GetSection("AESKEY").Value;
        public FuelProductController()
        {
            _logwrite = new LogWrite();
            _log = new Logger();
        }
        [HttpGet("bpcl_balance")]
        public async Task<ResponseModel> bpcl_balance(string mobile_no)
        {
            _log.WriteToFile("bocl_wallet_balance Process Started");
            string EncryptedText = "";
            CommonResponseEcomm<bpcl_wallet_response> res = new CommonResponseEcomm<bpcl_wallet_response>();
            ResponseModel returnResponse = new ResponseModel();
            bpcl_wallet_response bpcl_Response = new bpcl_wallet_response();
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

                    bpcl_wallet_request bpcl_Request = new bpcl_wallet_request();
                    bpcl_Request.mobile_no = mobile_no;
                    DataSet dataSet = bpcl_Request.get_wallet_balance();
                    if (dataSet != null)
                    {
                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            if (dataSet.Tables[0].Rows[0]["Status"].ToString() == "1")
                            {
                                bpcl_Response.balance = decimal.Parse(dataSet.Tables[0].Rows[0]["balance"].ToString());

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
            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(CommonResponseEcomm<bpcl_wallet_response>));
            ms = new MemoryStream();
            js.WriteObject(ms, res);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(AESKEY, CustData);
            returnResponse.Body = EncryptedText;
            _log.WriteToFile("bocl_wallet_balance Process Ended");
            return returnResponse;

        }
        [HttpGet("bpcl_transaction")]
        public async Task<ResponseModel> bpcl_transaction(string mobile_no)
        {
            _log.WriteToFile("bpcl_transaction Process Started");
            string EncryptedText = "";
            CommonResponseEcomm<bpcl_trnasaction_response> res = new CommonResponseEcomm<bpcl_trnasaction_response>();
            ResponseModel returnResponse = new ResponseModel();
            bpcl_trnasaction_response bpcl_trnasaction_response = new bpcl_trnasaction_response();
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

                    bpcl_transaction_request bpcl_transaction_request = new bpcl_transaction_request();
                    bpcl_transaction_request.mobile_no = mobile_no;
                    DataSet dataSet = bpcl_transaction_request.get_bpcl_transaction();
                    if (dataSet != null)
                    {
                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            if (dataSet.Tables[0].Rows[0]["Status"].ToString() == "1")
                            {
                                List<bpcl_trans_data> lstbpcldata = new List<bpcl_trans_data>();
                                for (int k = 0; k <= dataSet.Tables[0].Rows.Count - 1; k++)
                                {
                                    bpcl_trans_data bpcl_trans_data = new bpcl_trans_data();
                                    bpcl_trans_data.trans_id = dataSet.Tables[0].Rows[k]["trans_id"].ToString();
                                    bpcl_trans_data.amount = decimal.Parse(dataSet.Tables[0].Rows[k]["amount"].ToString());
                                    bpcl_trans_data.narration = dataSet.Tables[0].Rows[k]["narration"].ToString();
                                    bpcl_trans_data.tran_date = dataSet.Tables[0].Rows[k]["tran_date"].ToString();
                                    bpcl_trans_data.trans_type = dataSet.Tables[0].Rows[k]["trans_type"].ToString();
                                    lstbpcldata.Add(bpcl_trans_data);
                                }
                                bpcl_trnasaction_response.transaction_lst = lstbpcldata;
                                res.Status = 1;
                                res.result = bpcl_trnasaction_response;
                            }
                            else
                            {
                               res.Status = int.Parse(dataSet.Tables[0].Rows[0]["Status"].ToString());
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
            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(CommonResponseEcomm<bpcl_trnasaction_response>));
            ms = new MemoryStream();
            js.WriteObject(ms, res);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(AESKEY, CustData);
            returnResponse.Body = EncryptedText;
            _log.WriteToFile("bpcl_transaction Process Ended");
            return returnResponse;

        }
    }
}
