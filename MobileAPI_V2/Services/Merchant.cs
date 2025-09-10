using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using MobileAPI_V2.Model;
using MobileAPI_V2.Services;
using static MobileAPI_V2.Model.MerchantModel;
using MobileAPI_V2.DataLayer;
using System.Data.SqlClient;
using Dapper;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.IO;
using MobileAPI_V2.Model.BillPayment;
using System.Linq;
using MobileAPI_V2.Model.Travel;
using static MobileAPI_V2.Model.FSCModel;
namespace MobileAPI_V2.Services
{
    public class Merchant : IMerchant
    {
        IDbConnection connection;
        private readonly IConfiguration _configuration;
        private readonly ConnectionString _connectionString;
        public Merchant(IConfiguration configuration, ConnectionString connectionString)
        {

            _configuration = configuration;
            _connectionString = connectionString;
        }

        public async Task<MerchantBody> MerchantRequestssData(string mobilenumber)
        {
            MerchantBody ApplicantList = new MerchantBody();


            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@mobilenumber", mobilenumber);
            //parameters.Add("@password", password);
            using (SqlConnection con = new SqlConnection(_connectionString.Value))
            {
                try
                {
                    con.Open();
                    var result = await con.QueryAsync<MerchantBody>("MerchantRequest", parameters, commandType: CommandType.StoredProcedure);
                    ApplicantList = result.FirstOrDefault();
                }

                catch (Exception ex)
                {

                }
                finally
                {
                    con.Close();
                }
                return ApplicantList;
            }

        }

        public async Task<string> MerchantCreditRequestDB(MerchantCredit data)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString.Value))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CreditrequestTrasactionid", data.CreditrequestTrasactionid);
                parameters.Add("@Merchantid", data.Merchantid);
                parameters.Add("@TransactionAmount", data.TransactionAmount);
                parameters.Add("@Customername", data.Customername);
                parameters.Add("@mobilenumber", data.mobilenumber);
                parameters.Add("@CustomerRemarks", data.CustomerRemarks);

                //string results = "";
                IEnumerable<string> results = null;
                try
                {
                    connection.Open();
                    results = await connection.QueryAsync<String>("MerchantCredit", parameters, commandType: CommandType.StoredProcedure);
                    //var rs=results.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
                return results.FirstOrDefault();
            }
        }

        public async Task<string> MerchantSearch(MerchantSearch model)
        {
            using (var con = new SqlConnection(_connectionString.Value))
            {
                try
                {

                    var parameters = new DynamicParameters();
                    parameters.Add("@Searchoption", model.Searchoption);
                    parameters.Add("@Searchvalue", model.Searchvalue);

                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    await con.ExecuteAsync("InsertMerchantSearchData", parameters, commandType: CommandType.StoredProcedure);


                    return "Insert successful";
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                    return "Error occurred: " + ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }


        public async Task<List<PaymentIdss>> PaymentRes()
        {
            List<PaymentIdss> applicantList = new List<PaymentIdss>();

            using (SqlConnection con = new SqlConnection(_connectionString.Value))
            {
                try
                {
                    con.Open();
                    var result = await con.QueryAsync<PaymentIdss>("GetPaymentOrders", commandType: CommandType.StoredProcedure);
                    applicantList = result.ToList();
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log the error)
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }

            return applicantList;
        }

        public async Task<string> insertPaymentRes(venusModel data, string PaymentId)
        {
            //List<PaymentIdss> applicantList = new List<PaymentIdss>();
            var parameters = new DynamicParameters();
            parameters.Add("@Amount", data.Amount);
            parameters.Add("@PaymentId", PaymentId);
            parameters.Add("@Description", data.Description);
            parameters.Add("@RechargeNumber", data.Mobile);
            parameters.Add("@ResponseStatus", data.ResponseStatus);
            parameters.Add("@OperatorTxnID", data.MerTxnID);
            parameters.Add("@OrderNo", data.OrderNo);

            string resultss = "";
            using (SqlConnection con = new SqlConnection(_connectionString.Value))
            {
                try
                {
                    con.Open();
                    var result = await con.QueryAsync<string>("insResults", parameters, commandType: CommandType.StoredProcedure);
                    //applicantList = result.ToList();
                    resultss = result.ToString();
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log the error)
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }

            return resultss;
        }


    }
}

