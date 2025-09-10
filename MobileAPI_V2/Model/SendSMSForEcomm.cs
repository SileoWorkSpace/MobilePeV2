using Microsoft.Extensions.Configuration;
using MobileAPI_V2.Services;
using System.Net;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Dapper;
using MobileAPI_V2.DataLayer;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MobileAPI_V2.Model
{
    public class SendSMSForEcomm
    {

        IConfiguration _configuration;
        IDbConnection con;
        private readonly ConnectionString _connectionString;
        private readonly LogWrite _logwrite;
        string EQUENCE_SMS_USERNAME = "";
        string EQUENCE_SMS_PASSWORD = "";
        string EQUENCE_SMS_URL = "";
        string EQUENCE_SMS_FROM = "";
        string SMSAPI = "";
        public string EQUENCE_SMS_FLAG { get; set; } = "";
        private readonly IDataRepositoryEcomm _dataRepository;

        public SendSMSForEcomm(ConnectionString connectionString, IConfiguration configuration, LogWrite logwrite)
        {
            _connectionString = connectionString;
            _configuration = configuration;
            _logwrite = logwrite;
            //_dataRepository = dataRepository;
            SMSAPI = configuration["SMSAPI"];
            EQUENCE_SMS_URL = configuration["EQUENCE_SMS_URL"];
            EQUENCE_SMS_USERNAME = configuration["EQUENCE_SMS_USERNAME"];
            EQUENCE_SMS_PASSWORD = configuration["EQUENCE_SMS_PASSWORD"];
            EQUENCE_SMS_FROM = configuration["EQUENCE_SMS_FROM"];
            EQUENCE_SMS_FLAG = configuration["EQUENCE_SMS_FLAG"];
        }
        public string SendSMSMessage(string mobileno, string tmplId, string text)
        {
            string responseText = "Failure";
            string body = "";
            try
            {
                SendSMSRequestTemplate template = new()
                {
                    username = EQUENCE_SMS_USERNAME,
                    password = EQUENCE_SMS_PASSWORD,
                    tmplId = tmplId,
                    from = EQUENCE_SMS_FROM,
                    to = mobileno,
                    text = text
                };
                body = JsonConvert.SerializeObject(template);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var request = (HttpWebRequest)WebRequest.Create(EQUENCE_SMS_URL);
                if (request != null)
                {


                    request.Timeout = 50000;
                    request.Method = "POST";
                    request.KeepAlive = true;
                    request.UseDefaultCredentials = true;
                    request.Credentials = CredentialCache.DefaultCredentials;
                    request.ContentType = "application/json";
                    using (Stream s = request.GetRequestStream())
                    {
                        using (StreamWriter sw = new StreamWriter(s))
                            sw.Write(body);
                    }
                }

                using (Stream s = request.GetResponse().GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(s))
                        responseText = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                _logwrite.LogRequestException("error occur in SendSmsMessage : " + ex.ToString());
            }
            return responseText;

        }
        public async Task<string> SendSMSMessage(SendSMSReq inputData)
        {
            string responseText = "Failure";
            string mobileno = inputData.to; string tmplId = inputData.tmplId; string text = inputData.text;
            string pattern = @"^91\d{10}$"; // Matches 91 followed by 10 digits
            List<object> result = new List<object>();
            //DynamicParameters parameters = new DynamicParameters();
            DynamicParameters parameters = new DynamicParameters();
            using (con = new SqlConnection(_connectionString.Value))
            {
                try
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    //var result1 = await con.QueryAsync<object>("", parameters, commandType: CommandType.StoredProcedure);
                    var result1 = await con.QueryAsync<object>("get_smsprovider", parameters, commandType: CommandType.StoredProcedure);
                    result = result1.ToList();

                }
                catch (Exception ex)
                {
                    _logwrite.LogException(ex);
                }
                finally
                {
                    con.Close();
                }
            }

            if (result.Count > 0)
            {
                object lsRes = result[0];
                var json = JObject.FromObject(lsRes);
                //if (json["flag"].ToString() == "1")
                //{
                //    SendSMSMessage(mobileno, tmplId, text);
                //}
                if (json.ContainsKey("id") && json["id"].ToString() == "2")
                {
                    if (!Regex.IsMatch(mobileno, pattern))
                    {
                        mobileno = "91" + mobileno;
                    }
                    text = text.Replace("<%23>", "");
                    SendSMSMessage(mobileno, tmplId, text);
                    responseText = "Success";
                }
                else
                {
                    SMSAPI = SMSAPI.Replace("[AND]", "&");
                    SMSAPI = SMSAPI.Replace("[MOBILE]", mobileno);
                    SMSAPI = SMSAPI.Replace("[MESSAGE]", text);
                    SMSAPI = SMSAPI.Replace("[TEMID]", tmplId);
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(SMSAPI, false));
                    HttpWebResponse httpResponse = (HttpWebResponse)(httpReq.GetResponse());
                    responseText = "Success";
                }

            }

            return responseText;
        }
    }

    public class SendSMSReq
    {
        public string to { get; set; }
        public string tmplId { get; set; }
        public string text { get; set; }
    }
    public class SendSMSRequestTemplate
    {
        public string username { get; set; }
        public string password { get; set; }
        //public string peId { get; set; }
        public string tmplId { get; set; }
        public string to { get; set; }
        public string from { get; set; }
        public string text { get; set; }
    }


    public class SendSmsResponse
    {
        public List<Response> response { get; set; }
    }

    public class Response
    {
        public string destination { get; set; }
        public string mrid { get; set; }
        public int segment { get; set; }
        public string status { get; set; }
    }

    public class SendSMSError
    {
        public int errorCode { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }

}