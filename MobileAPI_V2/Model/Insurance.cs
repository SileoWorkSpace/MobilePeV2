using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileAPI_V2.Model
{

    public class BankDetailResponse
    {

        public long bankstatus { get; set; }
        public BankDetail data { get; set; }
    }
    
        public class BankDetail
    {
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string IFSC { get; set; }
    }
    public class Insurance
    {
        public string Insuranceurl { get; set; }
    }
    public class BindDropDown
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }
    public class BindDropDown2
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
    public class BindDropDown2Req
    {
        public int ProcId { get; set; }
        public string data { get; set; }
    }
    public class CommonResult
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
    public class ThriweData
    {
        public string Name { get; set; }
        public double Amount { get; set; }
        public string ValidityFrom { get; set; }
        public string ValidityTo { get; set; }
        public string URL { get; set; }
    }
    public class ThriweText
    {
        public string TEXT { get; set; }
        public int SRNO { get; set; }
      
    }
    public class NirmalBangPolicy
    {
        public string InsuredName { get; set; }
        public string Insurer { get; set; }
        public string TypeofPolicy { get; set; }
        public string PolicyNo { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public DateTime PolicyExpiryDate { get; set; }
        public string GrossPremium { get; set; }
        public string? PolicyDoc { get; set; }
        public string MobileNo { get; set; }

    }
    public class NirmalBangPolicyResponse
    {
        public string Message { get; set; }
        public int Status { get; set; }
    }
    public class TblCouponMaster
    {
        public int CouponId { get; set; }
        public string couponCode { get; set; }
        public string type { get; set; }
        public string couponDiscount { get; set; }
        public string validity { get; set; }
        public string DiscountAmount { get; set; }
        public string DiscountRate { get; set; }
        public string MinimumAmont { get; set; }
        public string status { get; set; }
        public string Description { get; set; }
    }

    public class applyCouponMasters
    {
        public string couponCode { get; set; }
        public string amount { get; set; }
    }

}
