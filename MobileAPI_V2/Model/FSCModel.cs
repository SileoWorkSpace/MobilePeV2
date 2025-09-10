
﻿using MobileAPI_V2.Model.BillPayment;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using System.Collections.Generic;

﻿using System.Collections.Generic;


namespace MobileAPI_V2.Model
{
    public class FSCModel
    {
        public class BankDetails
        {
            public int Id { get; set; }
            public string BankName { get; set; }
            public string BankImageUrl { get; set; }
            public int Sn { get; set; }
        }
        public class FSCRes
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public List<BankDetails> Data { get; set; }
        }
        public class CreditCardDetails
        {
            public int ID { get; set; }
            public string Benefits { get; set; }
            public string Criteria { get; set; }
            public string Image { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
           
        }
        public class FscApplicants
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string PinCode { get; set; }
            public string FSC_MemId { get; set; }
        }

        public class BankFscLogs
        {
            public string Type { get; set; }
            public string FSC_MemId { get; set; }
            public string BankId { get; set; }
        }

        public class CreditFscLogs
        {
            public string Type { get; set; }
            public string FSC_MemId { get; set; }
            public string CreditCardId { get; set; }

        }


        public class FscResponse
        {
            public string data { get; set; }
            public string Message { get; set; }

        }

        public class ApplyResponseCoupon
        {
            public string Message { get; set; }
            public List<TblCouponMaster> Data { get; set; }
        }
        public class FscResponseCoupon
        {
            public string Message { get; set; }
            public List<ApplyCouponCodes> Data { get; set; }
        }
        

        public class GetFscAppl
        {
            public string FSC_MemId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string PinCode { get; set; }
            public string CreatedDate { get; set; }
            public string Status { get; set; }
        }
       
        public class GetFlagAppl
        {
            public bool flag { get; set; }
           
        }
        public class FSCCardRes
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public List<CreditCardDetails> Data { get; set; }
        }
        public class ValidatePincodeRes
        {
            public string response_code { get; set; }
            public string response_message { get; set; }
            public string Data { get; set; }
        }



    }
}
