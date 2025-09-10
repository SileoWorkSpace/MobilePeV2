using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileAPI_V2.Model
{
    public class RechargeDesk
    {
        public string error_code { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public string ref_no { get; set; }
        public decimal after_balance { get; set; }
        public string number { get; set; }
        public decimal amount { get; set; }
        public string optr_txn_id { get; set; }


    }
    public class RechargeDeskRequest : RechargeDesk
    {
        public int ProcId { get; set; }
        public string TransactionId { get; set; }
        public string orderId { get; set; }
        public string transdate { get; set; }
       
        public string OperatorCode { get; set; }
        public string Type { get; set; }
        public long MemberId { get; set; }
        public long Id { get; set; }
        public string merchantInfoTxn { get; set; }
        public string response { get; set; }
    }
    public class VenusRecharge
    {
        public string ResponseStatus { get; set; }

        public string Description { get; set; }

        public string MerTxnID { get; set; }

        public string Mobile { get; set; }
        public string Amount { get; set; }

        public string OperatorTxnID { get; set; }
        public string OrderNo { get; set; }
    }
    public class MerchantResponse
    {
        public MerchantResponsebody Response { get; set; }
    }
    public class MerchantResponsebody
    {
        public string response_message { get; set; }
        public string response_code { get; set; }
        public string data { get; set; }
        public string OperatorTxnID { get; set; }
    }
    public class MerchantModelDB
    {
        public string CustomerRemarks { get; set; }
        public string Customername { get; set; }
        public string amount { get; set; }
        public string CreditrequestTrasactionid { get; set; }
        public string Merchantid { get; set; }
        public string TransactionAmount { get; set; }
        public string mobilenumber { get; set; }
        public long Fk_MemId { get; set; }
    }
    public class MerchantVoucherModelDB
    {
        public string CustomerRemarks { get; set; }
        public string Customername { get; set; }
        public string amount { get; set; }
        public string CreditrequestTrasactionid { get; set; }
        public string Merchantid { get; set; }
        public string TransactionAmount { get; set; }
        public string mobilenumber { get; set; }
        public long Fk_MemId { get; set; }
        public string qty { get; set; }
        public string Emaild { get; set; }
        public string Denomination { get; set; }
        public string stockTransactionid { get; set; }
    }

    public class VenusRechargeResponse
    {
        public VenusRecharge Response { get; set; }
    }
    public class VenusBillInfo
    {
        public string ResponseStatus { get; set; }

        public string Description { get; set; }

        public string MerTxnID { get; set; }

        public string ConsumerID { get; set; }
        public double DueAmount { get; set; }
       // public decimal? DueAmount { get; set; }
        public string OrderId { get; set; }
        public string ConsumerName { get; set; }
        public string DueDate { get; set; }
    }

    public class VenusBillInfoResponse 
    {

        public VenusBillInfo Response { get; set; }
    }
    public class VenusBillInfoRequest
    {
        public string OpertorCode { get; set; }
        public string TransactionNo { get; set; }
        public string CustomerId { get; set; }
        public string MobileNo { get; set; }
    }

    public class VenusBillPayRequest
    {
        public string OpertorCode { get; set; }
        public string TransactionNo { get; set; }
        public string CustomerId { get; set; }
        public string MobileNo { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string merchantInfoTxn { get; set; }
        public int ProcId { get; set; }
        public long Id { get; set; }
        public long MemberId { get; set; }
        public string Type { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public string optr_txn_id { get; set; }
        public string response { get; set; }
    }

    public class VenusBillPay
    {
        public string ResponseStatus { get; set; }
        public string Description { get; set; }
        public string ConsumerID { get; set; }
        public string OrderId { get; set; }
        public string ConsumerName { get; set; }
        public decimal DueAmount { get; set; }
        public string DueDate { get; set; }
        public string BillDate { get; set; }
        public string OperatorTxnId { get; set; }
    }
    public class VenusBillPayResponse
    {
        public VenusBillPay response { get; set; }

    }

    public class ElectricityBillData
    {
        public double bill_amount { get; set; }
        public List<ElectricityBillFetch> key_value { get; set; }
        public string success { get; set; }
        public string msg { get; set; }
    }

    public class ElectricityBillFetch
    {
        public string label { get; set; }
        public string value { get; set; }
    }

    public class ElectricityBillDataResponse
    {
        public ElectricityBillData data { get; set; }
    }
    public class CPRechargeResponse
    {
        public CPRechargess Response { get; set; }
    }
    public class CPRechargess
    {
        public string data { get; set; }
        public string response_code { get; set; }
        public string response_message { get; set; }
    }
    public class getTypeFromCP
    {
        public string type { get; set; }
      
    }
    public class ExtraDataForCP
    {
        public string Type { get; set; }
        public string TransactionDate { get; set; }
        public string MemberId { get; set; }
        public string PaymentMode { get; set; }
        public string Amount { get; set; }
        public string Remark { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Number { get; set; }
        public string optr_txn_id { get; set; }
    }
    public class CPRecharge
    {
        public string OperatorCode { get; set; }
        public string Number { get; set; }
        public string emailid { get; set; }
        public string pincode { get; set; }
        public string amount { get; set; }
        public string cpid { get; set; }
        public string TransactionId { get; set; }
    }
    public class CPRechargeRequest
    {
        public string Operatorname { get; set; }
        public string mobilenumber { get; set; }
        public string emailid { get; set; }
        public string pincode { get; set; }
        public string amount { get; set; }
        public string cpid { get; set; }
        public string CprequestTransid { get; set; }
    }
    public class CPRechargessCheckStatus
    {
        public string cprequesttranid { get; set; }
    }

}
