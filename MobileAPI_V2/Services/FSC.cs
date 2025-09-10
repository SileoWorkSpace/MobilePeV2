using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using MobileAPI_V2.Model;
using MobileAPI_V2.Services;
using static MobileAPI_V2.Model.FSCModel;
using MobileAPI_V2.DataLayer;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
namespace MobileAPI_V2.Services
{
    public class FSC: IFSC
    {
        IDbConnection connection;
        private readonly IConfiguration _configuration;
        private readonly ConnectionString _connectionString;
        public FSC(IConfiguration configuration,ConnectionString connectionString)
        {
           
            _configuration = configuration;
            _connectionString = connectionString;
        }


        public async Task<List<BankDetails>> GetBankDetails()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString.Value))
            {
                List<BankDetails> bankDetails = new List<BankDetails>();
                try
                {
                    connection.Open();
                    var results = await connection.QueryAsync<BankDetails>("GetFscBankMasterData", commandType: CommandType.StoredProcedure);
                    bankDetails.AddRange(results);
                }
                catch (Exception ex)
                {
                    throw ex; // Handle the exception according to your application's requirements
                }
                finally
                {
                    connection.Close();
                }

                return bankDetails;
            }           

        }


        public async Task<List<CreditCardDetails>> GetCreditCardDetails(long id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString.Value))
            {
                List<CreditCardDetails> CardDetails = new List<CreditCardDetails>();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@id", id);
                try
                {
                    connection.Open();
                    var results = await connection.QueryAsync<CreditCardDetails>("GetFscCreditCardMasterData",parameters, commandType: CommandType.StoredProcedure);
                    CardDetails.AddRange(results);
                }
                catch (Exception ex)
                {
                    throw ex; // Handle the exception according to your application's requirements
                }
                finally
                {
                    connection.Close();
                }

                return CardDetails;
            }

        }

        public async Task<string> ValidatePinCode(long CreditID, long Pincode)
        {
            using (SqlConnection con = new SqlConnection(_connectionString.Value))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@credit_id", CreditID); 
                parameters.Add("@@pincode", Pincode); 
                
                var response = "";
                try
                {
                    con.Open();
                    response = await con.QueryFirstOrDefaultAsync<string>("ValidatePincodeData", parameters, commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {

                }
                finally
                {
                    con.Close();
                }
                return response;
            }
        }

        public async Task<string> SaveFscApplicants(FscApplicants model)
        {
            string result = "";

            // Create DynamicParameters object and add parameters
            var parameters = new DynamicParameters();
            parameters.Add("@Name", model.Name);
            parameters.Add("@Email", model.Email);
            parameters.Add("@Phone", model.Phone);
            parameters.Add("@PinCode", model.PinCode);
            parameters.Add("@FSC_MemId", model.FSC_MemId);

            using (var con = new SqlConnection(_connectionString.Value))
            {
                try
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    result = await con.QueryFirstAsync<string>("InsertFscApplicant", parameters, commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {
                }
                finally
                {
                    con.Close();
                }

                return result;
            }
        }

        public async Task<string> SaveBankFscLogs(BankFscLogs model)
        {
            string result = "";

            // Create DynamicParameters object and add parameters
            var parameters = new DynamicParameters();
            parameters.Add("@Type", model.Type);
            parameters.Add("@FSC_MemId", model.FSC_MemId);
            parameters.Add("@BankId", model.BankId);
            using (var con = new SqlConnection(_connectionString.Value))
            {
                try
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    result = await con.QueryFirstAsync<string>("InsertFscLog", parameters, commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {

                }
                finally
                {
                    con.Close();
                }

                return result;
            }
        }

        public async Task<string> SaveCreditFscLogs(CreditFscLogs model)
        {
            string result = "";

            // Create DynamicParameters object and add parameters
            var parameters = new DynamicParameters();
            parameters.Add("@Type", model.Type);
            parameters.Add("@FSC_MemId", model.FSC_MemId);
            parameters.Add("@CreditCardId", model.CreditCardId);

            using (var con = new SqlConnection(_connectionString.Value))
            {
                try
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    result = await con.QueryFirstAsync<string>("InsertFscLog", parameters, commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {

                }
                finally
                {
                    con.Close();
                }

                return result;
            }
        }

        public async Task<List<GetFscAppl>> GetFscApplicants(string FSC_MemId)
        {
            List<GetFscAppl> ApplicantList = new List<GetFscAppl>();


            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@FSC_MemId", FSC_MemId);
            using (SqlConnection con = new SqlConnection(_connectionString.Value))
            {
                try
                {
                    con.Open();
                    var result = await con.QueryAsync<GetFscAppl>("GetFscApplicantsByMemId", parameters, commandType: CommandType.StoredProcedure);
                    ApplicantList = result.ToList();
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


        public async Task<string> UpdateSoundFlag(long memberId, bool flag)
        {
            using (SqlConnection con = new SqlConnection(_connectionString.Value))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@memberid", memberId);
                parameters.Add("@newsoundflag", flag);

                var response = "";
                try
                {
                    con.Open();
                    response = await con.QueryFirstOrDefaultAsync<string>("UpdateisSoundActiveFlag", parameters, commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {

                }
                finally
                {
                    con.Close();
                }
                return response;
            }
        }

    }
}
