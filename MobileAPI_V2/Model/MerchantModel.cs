//using Microsoft.AspNetCore.Mvc;
using MobileAPI_V2.Model.BillPayment;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using System;
using System.Collections.Generic;
using static MobileAPI_V2.Model.MerchantModel;

namespace MobileAPI_V2.Model
{
    public class MerchantModel
    {    
        public class RequestMerchantModel
        {
            public string Body { get; set; }
        }
        public class RequestMerchantModelsss
        {
            public string mobilenumber { get; set; }
            public string password { get; set; }
        }
        public class MerchantBody
        {
            public string Firstname { get; set; }
            public string Middlename { get; set; }
            public string Lastname { get; set; }
            public string Gender { get; set; }
            public string mobilenumber { get; set; }
            public string Emailid { get; set; }
            public string CurrentAddress1 { get; set; }
            public string CurrentAddress2 { get; set; }
            public string CurrentAddress3 { get; set; }
            public string CurrentPincode { get; set; }
            public string Currentcity { get; set; }
            public string Currentstate { get; set; }
            public string PermanentAddress1 { get; set; }
            public string PermanentAddress2 { get; set; }
            public string PermanentAddress3 { get; set; }
            public string PermanentPincode { get; set; }
            public string Permanentcity { get; set; }
            public string Permanentstate { get; set; }
            public string dob { get; set; }
            public string maritialstatus { get; set; }
            public string password { get; set; }
        }
        public class MerchantCredit
        {
            public string CreditrequestTrasactionid { get; set; }
            public string Merchantid { get; set; }
            public string TransactionAmount { get; set; }
            public string Customername { get; set; }
            public string mobilenumber { get; set; }
            public string CustomerRemarks { get; set; }
        }

        public class MerchantSearchResponse
        {
            public string ResponseCode { get; set; }
            public string ResponseMessage { get; set; }
            public List<MerchantData> Data { get; set; }  
        }

        public class ResponseModelMarchantss
        {
            List<MerchantSearchResponse> Responses { get; set; }
        }
        public class MerchantSearch
        {
            public string Searchoption { get; set; }
            public string Searchvalue { get; set; }
        }

        public class MerchantData
        {
            public string Merchantid { get; set; }
            public string Merchantname { get; set; }
            public string mobilenumber { get; set; }
            public string Emailid { get; set; }
            public string merchantShopname { get; set; }
            public string Shopimage { get; set; }
            public string Merchanttype { get; set; }
            public string Category { get; set; }
            public string BrandProductCode { get; set; }
            public string Brandimage { get; set; }
            public string importantInstruction { get; set; }
            public int Customflag { get; set; }
        }




        public class MerchantRequestModel
        {
            public string Firstname { get; set; }
            public string Middlename { get; set; }
            public string Lastname { get; set; }
            public string Gender { get; set; }
            public string mobilenumber { get; set; }
            public string Emailid { get; set; }
            public string CurrentAddress1 { get; set; }
            public string CurrentAddress2 { get; set; }
            public string CurrentAddress3 { get; set; }
            public string CurrentPincode { get; set; }
            public string Currentcity { get; set; }
            public string Currentstate { get; set; }
            public string PermanentAddress1 { get; set; }
            public string PermanentAddress2 { get; set; }
            public string PermanentAddress3 { get; set; }
            public string PermanentPincode { get; set; }
            public string Permanentcity { get; set; }
            public string Permanentstate { get; set; }
            public string dob { get; set; }
            public string maritialstatus { get; set; }
            public string password { get; set; }
        }

        public class MerchantSearchRequestModel
        {
            public string Searchoption { get; set; }
            public string Searchvalue { get; set; }
        }
        public class MerchantDynamicSearchRequestModel
        {
            public string mode { get; set; }
            public string Searchvalue { get; set; }
        }
        public class MerchantSubCategoryModel
        {
            public string type { get; set; }
            
        }
        public class MerchantSubCategory
        {
            public string CategoryID { get; set; }
            public string CategoryName { get; set; }
            public string check { get; set; }
        }
        public class MerchantService
        {
            public int ServiceID { get; set; }
            public string ServiceName { get; set; }
            public string check { get; set; }
        }
        public class MerchantFiltertypeModel
        {
            public int filterid { get; set; }
            public string filtervalue { get; set; }
        }
        public class ApiResponseModel1234
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public List<MerchantFiltertypeModel> data { get; set; }
        }
        //public class ApiResponsesubcatefory
        //{
        //    public string response_code { get; set; }
        //    public string response_message { get; set; }
        //    public List<MerchantSubCategory> data { get; set; }
        //    public List<MerchantService> data2 { get; set; }
        //}
        public class MerchantSubCategoryResponse
        {
            public string check { get; set; }
            public List<MerchantSubCategory> categories { get; set; }
        }

        // New wrapper class for services
        public class MerchantServiceResponse
        {
            public string check { get; set; }
            public List<MerchantService> services { get; set; }
        }

        public class ApiResponsesubcatefory
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public MerchantSubCategoryResponse data { get; set; } // Updated to use new wrapper
            public MerchantServiceResponse data2 { get; set; } // Updated to use new wrapper
        }
        public class ApiBrands
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public tempBrands data { get; set; }
        }
        public class tempBrands
        {
            public string status { get; set; }
            public List<GetBrands> data { get; set; }
        }
        public class ApiGetBrands
        {
            public string BrandProductCode { get; set; }
            public ApiBrands data { get; set; }

        }

        public class GetBrands
        {
            public string BrandName { get; set; }
            public string BrandType { get; set; }
            public string OnlineRedemptionUrl { get; set; }
            public string BrandImage { get; set; }
            public string denominationList { get; set; }
            public string stockAvailable { get; set; }
            public string Category { get; set; }
            public string Descriptions { get; set; }
        }

    }
    public class MerchantCreditRequestModel
        {
            public string CreditrequestTrasactionid { get; set; }
            public string Merchantid { get; set; }
            public string TransactionAmount { get; set; }
            public string Customername { get; set; }
            public string mobilenumber { get; set; }
            public string CustomerRemarks { get; set; }
        }

        public class ApiResponseModel
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public object data { get; set; }
            public string Redirecturl { get; set; }
        }
        public class HideDetails
        {
            public bool NameTrue { get; set; }
            public bool NumberTrue { get; set; }
            public bool DataTrue { get; set; }
            public bool EmailTrue { get; set; }
            public bool shopNameTrue { get; set; }
            public bool Service { get; set; }
            public bool Category { get; set; }
            public bool Brandimage { get; set; }
            public bool BrandProductCode { get; set; }
            public bool Merchanttype { get; set; }
           
        }
        public class ApiResponseModel1
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public List<MerchantData> data { get; set; }
            public HideDetails HideDetails { get; set; }
            public string Redirecturl { get; set; }
        }
    public class MRequestModel
    {
       // public string Body { get; set; }
        public string Pincode { get; set; }
        public string MobileNumber { get; set; }
        public string MerchantName { get; set; }
        public string ShopName { get; set; }
        public string ShopCategory { get; set; }
        public string mode { get; set; }
    }
    public class ApiResponseModel12
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public List<lookupModel> data { get; set; }
        }
        public class lookupModel
        {
            public string Responsecode { get; set; }
            public string Responsedescription { get; set; }
            public string Redirecturl { get; set; }
            public string Currentkycstatus { get; set; }
            public string Balance { get; set; }
        }
        public class ApiResponseModel123
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public List<searchHistory> data { get; set; }
        }
        public class ApiResponseModel1231
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public HideDetail hideDet { get; set; }
            public List<VendorModel> data { get; set; }
        }
        public class HideDetail
        {
        public bool VendornameTrue { get; set; }
        public bool VendormobilenumberTrue { get; set; }
        public bool TransactionAmountTrue { get; set; }
        public bool TransactionidTrue { get; set; }
        public bool CustomerRemarksTrue { get; set; }
        public bool TransactionDatetimeTrue { get; set; }
        }
        public class VendorModel
        {
            public string Vendorname { get; set; }
            public string Vendormobilenumber { get; set; }
            public decimal TransactionAmount { get; set; }
            public string Transactionid { get; set; }
            public string CustomerRemarks { get; set; }
            public string TransactionDatetime { get; set; }
            public string Shopimage { get; set; }
            public string Response { get; set; }
            public string VoucherNumber { get; set; }
            public string VoucherPin { get; set; }
            public string ExpiryDate { get; set; }

        }

        public class searchHistory
        {
            public string Merchantname { get; set; }
            public string CreditrequestTrasactionid { get; set; }
            public string Transactionid { get; set; }
            public string Transactiondate { get; set; }
            public string TransactionAmount { get; set; }
            public string Vendorname { get; set; }
        }

        public class RequestAddChargesModelsss
        {
            public string Operatorname { get; set; }
            public string mobilenumber { get; set; }
            public string emailid { get; set; }
            public string pincode { get; set; }
            public decimal amount { get; set; }
            public int cpid { get; set; }
            public string CprequestTransid { get; set; }
        }

        public class ApiResponseRechargeModel1
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public object data { get; set; }
        }

        public class RequestStatusCheckModelsss
        {
            public int cprequesttranid { get; set; }
        }

        public class StatusCheckData
        {
            public string Transactionrefnumber { get; set; }
        }

        public class StatusCheckDataModel1
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public List<StatusCheckData> data { get; set; }

        }
        public class getOfferResponse
    {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public List<getOffer> data { get; set; }

        }
    public class getOffer
        {
            public string brandcode { get; set; }
            public string Initialamount { get; set; }
            public string commissionPercentage { get; set; }
            public string finalamount { get; set; }
            public string discountamount { get; set; }
        }
        public class PaymentIdss
        {
            public string PaymentId { get; set; }
        }



    public class venusModel
        {
            public string ResponseStatus { get; set; }
            public string Description { get; set; }
            public string MerTxnID { get; set; }
            public string Mobile { get; set; }
            public string Amount { get; set; }
            public string OperatorTxnID { get; set; }
            public string OrderNo { get; set; }

        }
        public class GetStockModel
        {
            public string response_message { get; set; }
            public string response_code { get; set; }
            public List<detailsOfStock> data { get; set; }
        }
        public class ResponseModelStore
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public storInnderModel data { get; set; }
        }
        public class storInnderModel
        {
            public string status { get; set; }
            public string desc { get; set; }
            public string code { get; set; }
            public List<detailsOfstore> data { get; set; }
        }
        public class getstockList
        {
            public string status { get; set; }
            public string data { get; set; }
            public string desc { get; set; }
            public string code { get; set; }
        }


        public class detailsOfStock
        {
             public string Responsecode { get; set; }
            public string Responsedescription { get; set; }
             public string AvailableQuantity { get; set; }
            public string BrandName { get; set; }
            public string TransactionID { get; set; }
            public string TotalDenominationValue { get; set; }
            public string balance { get; set; }
            public string DisplayMessage { get; set; }
        }

        public class HitStockListModel
        {
            public string BrandProductCode { get; set; }
            public string Denomination { get; set; }
            public string shop { get; set; }
        }
    public class HitStockListModelv1
    {
        public string BrandProductCode { get; set; }
        public string Denomination { get; set; }
        public string shop { get; set; }
        public long Fk_MemId { get; set; }
        public string mobilenumber { get; set; }
    }

    public class HitStockListModelv2
    {
        public string BrandProductCode { get; set; }
        public string Denomination { get; set; }
        public string shop { get; set; }
        public long Fk_MemId { get; set; }
        public string mobilenumber { get; set; }
        public string Stockchecktransid { get; set; }
    }

    public class MerchantBalance_Req
    {
        public string mobile { get; set; }
    }

    public class SettlementTrnxReport_Req
    {
        public string mobilenumber { get; set; }
    }
    public class MerchantBody_Res
    {
        public string? response_code { get; set; } = "000";
        public string? response_message { get; set; } = "Failure";
        public string? data { get; set; }
    }
    public class GetAvailableCommission_Req
    {
        public int Fk_memid { get; set; }
        public string Brandproductcode { get; set; }
    }
    public class Updateusedflag_req
    {
        public string Transactionid { get; set; }
    }
    public class CustomertranactionList_req
    {
        public string mobilenumber { get; set; }
        public int Usedflag { get; set; }
    }
    public class ApplyCommissionReferal_Req
    {
        public int Fk_memid { get; set; }
        public decimal Transamount { get; set; }
    }
    public class MerchantDebitReferal_Req
    {
        public int FK_memid { get; set; }
        public string Paymentid { get; set; }
        public string Type { get; set; }
        public int Merchantid { get; set; }
        public string MerchantTransactionid { get; set; }
        public decimal Amount { get; set; }
        public string Paymenttype { get; set; }
        public string Request { get; set; }
        public string orderid { get; set; }
        public decimal TRNSACTIONAMOUNT { get; set; }
        public decimal Redeemcommission { get; set; }
        public int Redeemflag { get; set; }

    }

    public class ApiVoucherStatus
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public storInnderModel data { get; set; }
        }


        public class APICVS
        {
            public string sv_ex_order_id { get; set; }
        }
    public class detailsOfstore
        {
            public string brand_id { get; set; }
            public string brand_name { get; set; }
            public string brand_status { get; set; }
            public List<Shopdetails> shop_details { get; set; }

        }
        //public class ApiVoucher
        //{
        //    public string response_code { get; set; }
        //    public string response_message { get; set; }
        //    public APIVoucherList data { get; set; }
        //}
        //public class APIVoucherListDetails
        //{
        //    public string status { get; set; }
        //    public List<>
        //}
        public class ApiVoucherList
        {
            public string externalOrderId { get; set; }
            public string mobileNo { get; set; }
            public string emailId { get; set; }
        }
    public class ApiVoucher
    {
        public string response_code { get; set; }
        public string response_message { get; set; }
        public Shopdetails data { get; set; }
    }
    public class Shopdetails
        {
            public string shop_id { get; set; }
            public string shop_name { get; set; }
            public string shop_guid { get; set; }
            public string shop_code { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string shop_contact { get; set; }
            public string shop_status { get; set; }

        }

        public class venusModel12
        {
            public venusModel Response { get; set; }
        }

        public class SettlementReq_Res
        {
            public string? response_code { get; set; }
            public string? response_message { get; set; }
            public object? data { get; set; }
        }
        public class SettlementReq
        {
            public string? mobilenumber { get; set; }
           
        }

}


