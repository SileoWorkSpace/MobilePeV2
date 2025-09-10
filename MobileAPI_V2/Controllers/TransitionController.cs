using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using Newtonsoft.Json;
using Razorpay.Api;
using System.Net;
using MobileAPI_V2.Model.Travel;
using System.Collections.Generic;
using System.Data;
using Nancy;
using System.Xml;
using MobileAPI_V2.Filter;
using MobileAPI_V2.Models;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using MobileAPI_V2.Logs;

namespace MobileAPI_V2.Controllers
{

    //[ServiceFilter(typeof(CheckUser))]
    //[ApiVersion("1.1")]
    //[Route("api/{v:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    public class TransitionController : ControllerBase
    {
        private readonly LogWrite _logwrite;
        private readonly Logger _log;
        private readonly IDataRepositoryEcomm _dataRepository;
        public string razorpayupikey = string.Empty;
        public string razorpayupisecret = string.Empty;
        public string razorpaydebitcardkey = string.Empty;
        public string razorpaydebitcardsecret = string.Empty;
        public string razorpaycreditcardkey = string.Empty;
        public string razorpaycreditcardsecret = string.Empty;
        public string VenusRechargeURL = string.Empty;
        string AESKEY = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build().GetSection("AESKEY").Value;
        private readonly IConfiguration _configuration;
        public TransitionController(Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IDataRepositoryEcomm dataRepository, IConfiguration configuration, LogWrite logwrite, Logger log)
        {
            _logwrite = logwrite;
            _log = log;
            _dataRepository = dataRepository;
            _configuration = configuration;
            razorpayupikey = _configuration["razorpayupikey"];
            razorpayupisecret = _configuration["razorpayupisecret"];
            razorpaydebitcardkey = _configuration["razorpaydebitcardkey"];
            razorpaydebitcardsecret = _configuration["razorpaydebitcardsecret"];
            razorpaycreditcardkey = _configuration["razorpaycreditcardkey"];
            razorpaycreditcardsecret = _configuration["razorpaycreditcardsecret"];
            VenusRechargeURL = _configuration["VenusRecharge"];

        }

        [HttpPost("PaymentOrder")]
        [Produces("application/json")]
        public async Task<ResponseModel> PaymentOrder(RequestModel reqModel)
        {
            _log.WriteToFile("PaymentOrder Process Started");
            string EncryptedText = "";
            CommonResponseEcomm<PaymentOrderModel> objres = new CommonResponseEcomm<PaymentOrderModel>();
            ResponseModel returnResponse = new ResponseModel();
            try
            {
                Order objorder = null;
                PaymentOrderModel paymentReq = new PaymentOrderModel();
                EncryptedText = ApiEncrypt_Decrypt.DecryptString(AESKEY, reqModel.Body);

                // Log the decrypted request body
                _log.WriteToFile("PaymentOrder Request (Decrypted): " + EncryptedText);

                paymentReq = JsonConvert.DeserializeObject<PaymentOrderModel>(EncryptedText);
                if (paymentReq.IsLocal == 1)
                {
                    razorpaydebitcardkey = "rzp_test_uF6itBiYzVL13D";
                    razorpaydebitcardsecret = "hVXLLndzYmZZNMTE4yuvSrNf";
                }
                string OrderId = "", Status = "";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                RazorpayClient client = null;
               

                if (paymentReq.Type.ToLower() == "mobile recharge" || paymentReq.Type.ToLower() == "dth recharge" || paymentReq.Type.ToLower() == "voucher")
                {
                    //paymentReq.Amount = (paymentReq.Amount - (paymentReq.Amount * 1) / 100);
                    //paymentReq.Amount = (paymentReq.Amount - (paymentReq.Amount * 0.75m) / 100);

                    paymentReq.Amount = Math.Round(
                        paymentReq.Amount - (paymentReq.Amount * 0.75m) / 100,
                        2,
                        MidpointRounding.AwayFromZero
                    );

                    _log.WriteToFile("PaymentOrder mobile recharge amount is " + paymentReq.Amount.ToString());

                    //var res1 = await _dataRepository.LevelWiseLedger(paymentReq.MemberId, "Cashpoint", 0, 0);

                    //if (res1 != null && res1.summarydata != null && res1.summarydata.Count > 0)
                    //{
                    //    var points = decimal.Parse(res1.summarydata[0].Value);
                    //    if (points >= (paymentReq.Amount * 1) / 100)
                    //    {
                    //        paymentReq.Amount = (paymentReq.Amount - (paymentReq.Amount * 1) / 100);
                    //    }
                    //}
                }


                if (paymentReq.PaymentMode.ToLower() == "upi" || paymentReq.PaymentMode.ToLower() == "rupay" || paymentReq.PaymentMode.ToLower() == "rupaycard")
                {

                    _log.WriteToFile("PaymentOrder mobile recharge PaymentMode is " + paymentReq.PaymentMode.ToString());

                    if (paymentReq.Type.ToLower() == "bankverification")
                    {
                        _log.WriteToFile("PaymentOrder mobile recharge type is " + paymentReq.Type.ToString());
                        paymentReq.Amount = 10;
                    }


                    client = new RazorpayClient(razorpayupikey, razorpayupisecret);
                }
                if (paymentReq.PaymentMode.ToLower() == "debitcard")
                {
                    client = new RazorpayClient(razorpaydebitcardkey, razorpaydebitcardsecret);
                    if (paymentReq.Type.ToLower() == "bankverification")
                    {
                        paymentReq.Amount = 10.06M;
                    }
                    if (paymentReq.Type.ToLower() == "cardcost")
                    {
                        paymentReq.Amount = 1005.90M;
                    }


                }
                if (paymentReq.PaymentMode.ToLower() == "creditcard")
                {
                    client = new RazorpayClient(razorpaycreditcardkey, razorpaycreditcardsecret);
                    if (paymentReq.Type.ToLower() == "bankverification")
                    {
                        paymentReq.Amount = 10.22M;
                    }
                    if (paymentReq.Type.ToLower() == "cardcost")
                    {
                        paymentReq.Amount = 1022.40M;
                    }

                }
                if (paymentReq.ProcId == 1)
                {

                    _log.WriteToFile("PaymentOrder mobile recharge procid is 1");
                    _log.WriteToFile("PaymentOrder mobile recharge before order create amount is " + paymentReq.Amount.ToString());

                    //paymentReq.Amount = paymentReq.Amount - (paymentReq.Amount * 0.75m / 100);

                    //if (paymentReq.Type.ToLower() == "mobile recharge" || paymentReq.Type.ToLower() == "dth recharge" || paymentReq.Type.ToLower() == "voucher")
                    //{
                    //    //paymentReq.Amount = Math.Round(paymentReq.Amount - (paymentReq.Amount * 0.75m / 100), 2, MidpointRounding.AwayFromZero);
                    //    paymentReq.Amount = paymentReq.Amount - (paymentReq.Amount * 0.75m / 100);
                    //}

                    objres.result = new PaymentOrderModel();
                    Dictionary<string, object> options = new Dictionary<string, object>();
                    options.Add("amount", Convert.ToInt64(paymentReq.Amount * 100));
                    options.Add("receipt", "");
                    options.Add("currency", "INR");
                    options.Add("payment_capture", 1);
                    objorder = client.Order.Create(options);
                    OrderId = objorder["id"].ToString();
                    objres.result.OrderId = OrderId;
                    paymentReq.OrderId = OrderId;

                    _log.WriteToFile("PaymentOrder mobile recharge OrderId is " + OrderId);

                }
                if (paymentReq.Type.ToLower() == "mobile" || paymentReq.Type.ToLower() == "dth" || paymentReq.Type.ToLower() == "electricity"
                    || paymentReq.Type.ToLower() == "lic"
                    )
                {
                    if (paymentReq.dict != null)
                    {
                        if (paymentReq.dict.Count > 0)
                        {
                            foreach (var item in paymentReq.dict)
                            {
                                if (item.Key == "Mobile")
                                {
                                    paymentReq.Mobile = item.Value;
                                }
                                else if (item.Key == "CustomerId")
                                {
                                    paymentReq.CustomerId = item.Value;
                                }
                                else
                                {
                                    paymentReq.OperatorCode = item.Value;
                                }
                            }
                        }

                    }

                }
                if (paymentReq.Type.ToLower() == "mobilepemall")
                {
                    if (paymentReq.dict.Count > 0)
                    {
                        foreach (var item in paymentReq.dict)
                        {
                            if (item.Key == "CartItemId")
                            {
                                paymentReq.CartItemId = item.Value;
                            }
                            else
                            {
                                paymentReq.AddressId = item.Value;
                            }
                        }
                    }
                }
                if (paymentReq.Type.ToLower() == "flightbooking")
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("JourneyType");
                    dt.Columns.Add("TripIndicator");
                    dt.Columns.Add("PNR");
                    dt.Columns.Add("Origin");
                    dt.Columns.Add("Destination");
                    dt.Columns.Add("AirlineCode");
                    dt.Columns.Add("LastTicketDate");
                    dt.Columns.Add("NonRefundable");


                    DataTable dtSegment = new DataTable();
                    dtSegment.Columns.Add("AirportCode");
                    dtSegment.Columns.Add("AirportName");
                    dtSegment.Columns.Add("Terminal");
                    dtSegment.Columns.Add("CityCode");
                    dtSegment.Columns.Add("CityName");
                    dtSegment.Columns.Add("CountryCode");
                    dtSegment.Columns.Add("CountryName");
                    dtSegment.Columns.Add("DepTime");
                    dtSegment.Columns.Add("Destination");
                    dtSegment.Columns.Add("ArrivalTime");


                    DataTable dtSegmentR = new DataTable();
                    dtSegmentR.Columns.Add("AirportCode");
                    dtSegmentR.Columns.Add("AirportName");
                    dtSegmentR.Columns.Add("Terminal");
                    dtSegmentR.Columns.Add("CityCode");
                    dtSegmentR.Columns.Add("CityName");
                    dtSegmentR.Columns.Add("CountryCode");
                    dtSegmentR.Columns.Add("CountryName");
                    dtSegmentR.Columns.Add("DepTime");

                    TravelSaveResponse travelSaveResponse = new TravelSaveResponse();
                    ResultTravel resultTravel = JsonConvert.DeserializeObject<ResultTravel>(paymentReq.Request);
                    for (int i = 0; i <= resultTravel.data.Count - 1; i++)
                    {
                        string JourneyType = "";
                        if (resultTravel.data.Count == 2)
                        {
                            JourneyType = "2";
                        }
                        else
                        {
                            JourneyType = "1";
                        }


                        if (resultTravel.data[i].isLcc == true)
                        {

                            ResultTravelForLCC resultTravelForLCC = JsonConvert.DeserializeObject<ResultTravelForLCC>(paymentReq.Request);
                            string ResultIndex = resultTravelForLCC.data[i].ResultIndex;

                            travelSaveResponse.IsSuccess = 2;
                            travelSaveResponse.Message = "Flight booking is pending";
                            travelSaveResponse.TraceId = resultTravelForLCC.data[i].TraceId;
                            travelSaveResponse.Fk_MemId = int.Parse(paymentReq.MemberId.ToString());
                            travelSaveResponse.IsLcc = "1";
                            travelSaveResponse.JourneyDate = resultTravelForLCC.data[i].Flight[0].JourneyDate;
                            travelSaveResponse.Class = resultTravelForLCC.data[i].Flight[0].Class;
                            int BaggageCount = resultTravelForLCC.data[i].Passengers[0].Baggage.Count;
                            string Origin = resultTravelForLCC.data[i].Flight[0].Origin;
                            string Desitination = resultTravelForLCC.data[i].Flight[0].Destination;
                            dt.Rows.Add(JourneyType, "", "", Origin, Desitination, "", "", "");
                            travelSaveResponse.tblFlightItinerary = dt;

                            for (int k = 0; k < resultTravelForLCC.data[i].Flight.Count; k++)
                            {
                                dtSegment.Rows.Add(resultTravelForLCC.data[i].Flight[k].AirlineCode, resultTravelForLCC.data[i].Flight[k].FlightNumber, "", resultTravelForLCC.data[i].Flight[k].Origin, resultTravelForLCC.data[i].Flight[k].Origin, "", "", resultTravelForLCC.data[i].Flight[k].DepartureTime, resultTravelForLCC.data[i].Flight[k].Destination,
                                    resultTravelForLCC.data[i].Flight[k].ArrivalTime);
                                //dtSegmentR.Rows.Add(resultTravelForLCC.data[i].Flight[k].AirlineCode, resultTravelForLCC.data[i].Flight[k].FlightNumber, "", resultTravelForLCC.data[i].Flight[k].Destination, resultTravelForLCC.data[i].Flight[k].Destination, "", "", resultTravelForLCC.data[i].Flight[k].DepartureTime);

                            }

                            travelSaveResponse.dtSegment = dtSegment;
                            travelSaveResponse.dtSegmentR = dtSegmentR;
                            travelSaveResponse.OrderId = paymentReq.OrderId;
                            var SaveBookingResponse = _dataRepository.SaveBookingResponse(travelSaveResponse);





                        }
                        else
                        {
                            ResultTravelForLCC resultTravelForLCC = JsonConvert.DeserializeObject<ResultTravelForLCC>(paymentReq.Request);
                            string ResultIndex = resultTravelForLCC.data[i].ResultIndex;
                            travelSaveResponse.IsSuccess = 2;
                            travelSaveResponse.Message = "Flight booking is pending";
                            travelSaveResponse.TraceId = resultTravelForLCC.data[i].TraceId;
                            travelSaveResponse.Fk_MemId = int.Parse(paymentReq.MemberId.ToString());
                            travelSaveResponse.IsLcc = "0";
                            travelSaveResponse.JourneyDate = resultTravelForLCC.data[i].Flight[0].JourneyDate;
                            travelSaveResponse.Class = resultTravelForLCC.data[i].Flight[0].Class;
                            int BaggageCount = resultTravelForLCC.data[i].Passengers[0].Baggage.Count;
                            string Origin = resultTravelForLCC.data[i].Passengers[0].Baggage[0].Origin;
                            string Desitination = resultTravelForLCC.data[i].Passengers[BaggageCount].Baggage[0].Destination;
                            dt.Rows.Add(JourneyType, "", "", Origin, Desitination, "", "", "");
                            travelSaveResponse.tblFlightItinerary = dt;

                            for (int k = 0; k < resultTravelForLCC.data[i].Flight.Count; k++)
                            {
                                dtSegment.Rows.Add(resultTravelForLCC.data[i].Flight[k].AirlineCode, resultTravelForLCC.data[i].Flight[k].FlightNumber, "", resultTravelForLCC.data[i].Flight[k].Origin, resultTravelForLCC.data[i].Flight[k].Origin, "", "", resultTravelForLCC.data[i].Flight[k].DepartureTime);
                                //dtSegmentR.Rows.Add(resultTravelForLCC.data[i].Flight[k].AirlineCode, resultTravelForLCC.data[i].Flight[k].FlightNumber, "", resultTravelForLCC.data[i].Flight[k].Destination, resultTravelForLCC.data[i].Flight[k].Destination, "", "", resultTravelForLCC.data[i].Flight[k].DepartureTime);

                            }

                            travelSaveResponse.dtSegment = dtSegment;
                            travelSaveResponse.dtSegmentR = dtSegmentR;
                            travelSaveResponse.OrderId = paymentReq.OrderId;
                            var SaveBookingResponse = _dataRepository.SaveBookingResponse(travelSaveResponse);

                        }
                    }
                }
                paymentReq.ipAddress = paymentReq.DeviceInfo == null ? "" : paymentReq.DeviceInfo.ipAddress;
                paymentReq.os = paymentReq.DeviceInfo == null ? "" : paymentReq.DeviceInfo.os;
                paymentReq.deviceId = paymentReq.DeviceInfo == null ? "" : paymentReq.DeviceInfo.deviceId;
                paymentReq.appversion = paymentReq.DeviceInfo == null ? "" : paymentReq.DeviceInfo.appversion;
                paymentReq.Longitude = paymentReq.DeviceInfo == null ? "" : paymentReq.DeviceInfo.Longitude;
                paymentReq.Latitude = paymentReq.DeviceInfo == null ? "" : paymentReq.DeviceInfo.Latitude;

                string walletReqParamReq =(!string.IsNullOrEmpty(paymentReq.Request)) ? paymentReq.Request : string.Empty;
                string merchantTxnId = string.Empty;
                string mercht_id = "";
                decimal TxnAmt = paymentReq.Amount;
                int Redeemflag = 0; decimal Redeemcommission = 0;
                if (paymentReq.Type.ToLower() == "merchantvoucher" || paymentReq.Type.ToLower() == "merchant")
                {
                    try
                    {
                        walletReqParamReq = paymentReq.Request;
                        JObject walletReqParamReqJson = JObject.Parse(walletReqParamReq);
                        mercht_id = (walletReqParamReqJson["Merchantid"].ToString());
                        merchantTxnId = walletReqParamReqJson["stockTransactionid"].ToString();

                        _log.WriteToFile("PaymentOrder mobile recharge mercht_id is " + mercht_id);
                        _log.WriteToFile("PaymentOrder mobile recharge merchantTxnId is " + merchantTxnId);

                        if (paymentReq.Type.ToLower() == "merchant")
                        {
                            TxnAmt = Convert.ToDecimal(walletReqParamReqJson["TransactionAmount"].ToString());
                        }
                        if (walletReqParamReqJson.ContainsKey("Redeemflag"))
                        {
                            Redeemflag = Convert.ToInt32(walletReqParamReqJson["Redeemflag"].ToString());
                            Redeemcommission = Convert.ToDecimal(walletReqParamReqJson["Redeemcommission"].ToString());

                            _log.WriteToFile("PaymentOrder if condition Redeemflag : " + Redeemflag);
                            _log.WriteToFile("PaymentOrder if condition Redeemcommission : " + Redeemcommission);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logwrite.LogRequestException("Transition PaymentOrder , PaymentOrder request input paramater is missing from merchantvoucher or merchnat failure:" + ex.ToString());
                    }
                }
                else if (paymentReq.Type.ToLower() == "mobile recharge" || paymentReq.Type.ToLower() == "dth recharge")
                {
                    try
                    {
                        walletReqParamReq = paymentReq.Request;
                        JObject walletReqParamReqJson = JObject.Parse(walletReqParamReq);
                        TxnAmt = Convert.ToDecimal(walletReqParamReqJson["Amount"].ToString());
                        _log.WriteToFile("PaymentOrder mobile recharge TxnAmt is " + TxnAmt);
                    }
                    catch (Exception ex)
                    {
                        _logwrite.LogRequestException("Transition PaymentOrder , PaymentOrder request input paramater is missing mobile or dth failure:" + ex.ToString());
                    }
                }
                else if (paymentReq.Type.ToLower() == "topaywallet")
                {
                    try
                    {
                        walletReqParamReq = paymentReq.Request;
                        JObject walletReqParamReqJson = JObject.Parse(walletReqParamReq);
                        TxnAmt = Convert.ToDecimal(walletReqParamReqJson["amount"].ToString());
                    }
                    catch (Exception ex)
                    {
                        _logwrite.LogRequestException("Transition PaymentOrder , PaymentOrder request input paramater is missing amount failure:" + ex.ToString());
                    }
                }

                try
                {
                    _log.WriteToFile("PaymentOrder Ins_walleDebitrequest_req Redeemflag : " + Redeemflag);
                    _log.WriteToFile("PaymentOrder Ins_walleDebitrequest_req Redeemcommission : " + Redeemcommission);

                    Ins_walleDebitrequest_req WalletReqInsert = new()
                    {
                        FK_memid = Convert.ToInt32(paymentReq.MemberId),
                        Paymentid = paymentReq.PaymentId,
                        Type = paymentReq.Type,
                        Merchantid = mercht_id,
                        MerchantTransactionid = merchantTxnId,
                        Amount = paymentReq.Amount,
                        Paymenttype = paymentReq.PaymentMode,
                        Request = walletReqParamReq,
                        orderid = paymentReq.OrderId,
                        Transactionamount = TxnAmt,
                        Redeemflag = Redeemflag,
                        Redeemcommission = Redeemcommission
                    };

                    // Log the object as a plain string (no JSON)
                    string walletReqStr =
                        "WalletReqInsert Details:" +
                        "\nFK_memid = " + WalletReqInsert.FK_memid +
                        "\nPaymentid = " + WalletReqInsert.Paymentid +
                        "\nType = " + WalletReqInsert.Type +
                        "\nMerchantid = " + WalletReqInsert.Merchantid +
                        "\nMerchantTransactionid = " + WalletReqInsert.MerchantTransactionid +
                        "\nAmount = " + WalletReqInsert.Amount +
                        "\nPaymenttype = " + WalletReqInsert.Paymenttype +
                        "\nRequest = " + WalletReqInsert.Request +
                        "\norderid = " + WalletReqInsert.orderid +
                        "\nTransactionamount = " + WalletReqInsert.Transactionamount +
                        "\nRedeemflag = " + WalletReqInsert.Redeemflag +
                        "\nRedeemcommission = " + WalletReqInsert.Redeemcommission;

                    _log.WriteToFile("WalletReqInsert JSON:\n" + walletReqStr);

                    ins_WalletdebitRequest_res walletbetitRes = await _dataRepository._repo_ins_WalletdebitRequest(WalletReqInsert);
                }
                catch (Exception ex)
                {
                    _logwrite.LogRequestException("Transition PaymentOrder, PaymentOrder ins_WalletdebitRequest SP failure:" + ex.ToString());
                }

                paymentReq.Redeemcommission = Redeemcommission;
                paymentReq.Redeemflag = Redeemflag;
                DataSet dataset = paymentReq.GenerateOrderId(paymentReq);

                // var res = await _dataRepository.PaymentOrder(model);
                if (dataset != null && dataset.Tables[0].Rows[0]["flag"].ToString() == "1")
                {
                    if (paymentReq.PaymentMode.ToLower() == "upi" || paymentReq.PaymentMode.ToLower() == "rupay" || paymentReq.PaymentMode.ToLower() == "rupaycard")
                    {

                        objres.result.key = razorpayupikey;
                        objres.result.secret = razorpayupisecret;
                    }
                    if (paymentReq.PaymentMode.ToLower() == "debitcard")
                    {
                        objres.result.key = razorpaydebitcardkey;
                        objres.result.secret = razorpaydebitcardsecret;
                    }
                    if (paymentReq.PaymentMode.ToLower() == "creditcard")
                    {
                        objres.result.key = razorpaycreditcardkey;
                        objres.result.secret = razorpaycreditcardsecret;
                    }
                    objres.result.Amount1 = Convert.ToDecimal(paymentReq.Amount);
                    objres.response = "success";
                    objres.message = "success";
                }
                else
                {

                    objres.response = "error";
                    objres.message = dataset.Tables[0].Rows[0]["message"].ToString();
                    objres.result = new Model.PaymentOrderModel();
                }
            }

            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                objres.response = "error";
                objres.message = ex.Message;
            }
            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(CommonResponseEcomm<PaymentOrderModel>));
            ms = new MemoryStream();
            js.WriteObject(ms, objres);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            CustData = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            EncryptedText = ApiEncrypt_Decrypt.EncryptString(AESKEY, CustData);
            returnResponse.Body = EncryptedText;
            _log.WriteToFile("PaymentOrder final encrypted response is : " + EncryptedText);
            _log.WriteToFile("PaymentOrder Process Ended");
            return returnResponse;
        }


        [HttpPost("Recharge")]
        [Produces("application/json")]
        public async Task<ResponseModel> Recharge(RequestModel reqModel)
        {
            string EncryptedText = "";
            CommonResponseEcomm<Common> objres = new CommonResponseEcomm<Common>();
            VenusRechargeResponse response = new VenusRechargeResponse();
            ResponseModel returnResponse = new ResponseModel();
            try
            {
                Order objorder = null;
                RechargeDeskRequest rechargeReq = new RechargeDeskRequest();
                EncryptedText = ApiEncrypt_Decrypt.DecryptString(AESKEY, reqModel.Body);

                rechargeReq = JsonConvert.DeserializeObject<RechargeDeskRequest>(EncryptedText);

                var check = await _dataRepository.CheckAmountByTransactionId(rechargeReq.TransactionId);

                if (rechargeReq.amount >= decimal.Parse(check.message))
                {
                    rechargeReq.amount = (Convert.ToDecimal(rechargeReq.amount) - ((Convert.ToDecimal(rechargeReq.amount)) * 0.01m));
                }
                if (check.message != null && decimal.Parse(check.message) >= rechargeReq.amount)
                {

                    rechargeReq.amount = Math.Round(rechargeReq.amount * 1.01m);
                    rechargeReq.ProcId = 1;
                    // model.merchantInfoTxn = DateTime.UtcNow.Ticks.ToString().Substring(0, 14);

                    var res = await _dataRepository.OperatorRecharge_V2(rechargeReq);
                    if (res != null && res.Id > 0)
                    {
                        rechargeReq.merchantInfoTxn = res.TransactionId;
                        string serviceType = string.Empty;
                        if (rechargeReq.Type.ToLower() == "mobile" || rechargeReq.Type.ToLower() == "mobile recharge")
                        {
                            serviceType = "MR";
                        }
                        else if (rechargeReq.Type.ToLower() == "dth" || rechargeReq.Type.ToLower() == "dth recharge")
                        {
                            serviceType = "DH";
                        }

                        VenusRechargeURL = VenusRechargeURL.Replace("[number]", rechargeReq.number).Replace("[recharge_amount]", rechargeReq.amount.ToString()).Replace("[operator_code]", rechargeReq.OperatorCode).Replace("[transaction_id]", rechargeReq.merchantInfoTxn).Replace("[ServiceType]", serviceType);
                        var result = CommonJsonPostRequest.CommonSendRequest(VenusRechargeURL, "GET", null);
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(result);

                        string json = JsonConvert.SerializeXmlNode(doc);
                        response = JsonConvert.DeserializeObject<VenusRechargeResponse>(json);
                        if (response != null && response.Response != null)
                        {
                            rechargeReq.ProcId = 2;
                            rechargeReq.Id = res.Id;
                            rechargeReq.status = response.Response.ResponseStatus;
                            rechargeReq.description = response.Response.Description;
                            if (response.Response.ResponseStatus.ToLower() == "success")
                            {
                                rechargeReq.optr_txn_id = response.Response.OperatorTxnID;
                            }
                            rechargeReq.response = result;

                            var res1 = _dataRepository.OperatorRecharge_V2(rechargeReq);
                            if (res1 != null && res1.Id > 0)
                            {
                                objres.message = "RECHARGE " + response.Response.ResponseStatus.ToLower();
                                objres.response = response.Response.ResponseStatus.ToLower();
                            }
                            else
                            {
                                objres.message = "Error";
                                objres.response = "error";

                            }

                        }
                        else
                        {
                            objres.message = "Error";
                            objres.response = "error";

                        }
                    }
                    else
                    {
                        objres.message = res.msg;
                        objres.response = "error";

                    }

                }
                else
                {
                    objres.message = "Payment is pending";
                    objres.response = "error";

                }
            }

            catch (Exception ex)
            {
                _logwrite.LogException(ex);
                objres.response = "error";
                objres.message = ex.Message;
            }
            string CustData = "";
            DataContractJsonSerializer js;
            MemoryStream ms;
            js = new DataContractJsonSerializer(typeof(CommonResponseEcomm<Common>));
            ms = new MemoryStream();
            js.WriteObject(ms, objres);
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
