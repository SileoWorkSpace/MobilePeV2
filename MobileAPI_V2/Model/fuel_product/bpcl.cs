using System.Data.SqlClient;
using System.Data;

namespace MobileAPI_V2.Model.fuel_product
{
    public class bpcl_wallet_response
    {
        public decimal balance { get; set; }
    }
    public class bpcl_wallet_request
    {
        public string? mobile_no { get; set; }
      
        public DataSet get_wallet_balance()
        {
            try
            {
                SqlParameter[] para = {
                                      new SqlParameter("@mobile_no", mobile_no)

                                  };

                DataSet ds = Connection.ExecuteQuery("get_bpcl_balance", para);
                return ds;


            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class bpcl_transaction_request
    {
        public string? mobile_no { get; set; }
        public int? page { get; set; }

        public DataSet get_bpcl_transaction()
        {
            try
            {
                SqlParameter[] para = {
                                      new SqlParameter("@mobile_no", mobile_no),
                                      new SqlParameter("@page", page),

                                  };

                DataSet ds = Connection.ExecuteQuery("get_bpcl_transaction", para);
                return ds;


            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    public class bpcl_trnasaction_response
    {
        public List<bpcl_trans_data>? transaction_lst { get; set; }
        public Int64? total_record { get; set; }
    }
    public class bpcl_trans_data
    {
        public string? trans_id { get; set; }
        public decimal amount { get; set; }
        public string? narration { get; set; }

        public string? tran_date { get; set; }
        public string? trans_type { get;  set; }
    }
}

