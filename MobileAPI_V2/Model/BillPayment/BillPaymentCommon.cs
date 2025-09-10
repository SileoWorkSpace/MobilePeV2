using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Net;
using System.Collections.Generic;

namespace MobileAPI_V2.Model.BillPayment
{
    public class BillPaymentCommon
    {        
        public static string HITMultiRechargeAPI(string APIurl)
        {
            string responseText = "";
            HttpWebRequest request = WebRequest.Create(APIurl) as HttpWebRequest;
           
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            WebHeaderCollection header = response.Headers;

            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseText = reader.ReadToEnd();
            }
            return responseText;
        }
        public class ApiEncrypt_Decrypt
        {
            public static string EncryptString(string key, string plainText)
            {
                byte[] iv = new byte[16];
                byte[] array;
                //string Aeskey = "";

                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                            {
                                streamWriter.Write(plainText);
                            }

                            array = memoryStream.ToArray();
                        }
                    }
                }

                return Convert.ToBase64String(array);
            }
            public static string DecryptString(string key, string cipherText)
            {
                cipherText = cipherText.Replace(" ", "+");
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);
                using (Aes aes = Aes.Create())
                {

                    aes.KeySize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }


            /// New ToPay Api call
            /// 
            //public static string EncryptString(string key, string plainText)
            //{

            //    byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            //    byte[] iv = GenerateRandomIV();
            //    byte[] cipherText;
            //    byte[] tag;

            //    using (AesGcm aesGcm = new AesGcm(keyBytes))
            //    {
            //        cipherText = new byte[plainText.Length];
            //        tag = new byte[16]; // Authentication tag size

            //        aesGcm.Encrypt(iv, Encoding.UTF8.GetBytes(plainText), cipherText, tag);
            //    }

            //    // Concatenate IV and tag with ciphertext for later use in decryption
            //    byte[] encryptedBytes = new byte[iv.Length + cipherText.Length + tag.Length];
            //    Buffer.BlockCopy(iv, 0, encryptedBytes, 0, iv.Length);
            //    Buffer.BlockCopy(cipherText, 0, encryptedBytes, iv.Length, cipherText.Length);
            //    Buffer.BlockCopy(tag, 0, encryptedBytes, iv.Length + cipherText.Length, tag.Length);

            //    // Convert encrypted bytes to Base64 string for safe storage/transmission
            //    return Convert.ToBase64String(encryptedBytes);
            //}

            //public static string DecryptString(string key, string message)
            //{    

            //    byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            //    byte[] encryptedBytes = Convert.FromBase64String(message);

            //    // Extract IV, ciphertext, and tag from the encrypted bytes
            //    byte[] iv = new byte[12]; // IV size
            //    byte[] cipherText = new byte[encryptedBytes.Length - iv.Length - 16]; // IV length + tag length = 28 bytes
            //    byte[] tag = new byte[16]; // Authentication tag size

            //    Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
            //    Buffer.BlockCopy(encryptedBytes, iv.Length, cipherText, 0, cipherText.Length);
            //    Buffer.BlockCopy(encryptedBytes, iv.Length + cipherText.Length, tag, 0, tag.Length);

            //    byte[] decryptedBytes = new byte[cipherText.Length];

            //    using (AesGcm aesGcm = new AesGcm(keyBytes))
            //    {
            //        aesGcm.Decrypt(iv, cipherText, tag, decryptedBytes);
            //    }

            //    return Encoding.UTF8.GetString(decryptedBytes);
            //}

            //static byte[] GenerateRandomIV()
            //{
            //    byte[] iv = new byte[12]; // IV size
            //    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            //    {
            //        rng.GetBytes(iv);
            //    }
            //    return iv;
            //}

        }

    }
    public class ResponseModel
    {
        public string Body { get; set; }
    }
    public class RequestModel
    {
        public string Body { get; set; }

    }
    public class CouponBody
    {
        public string TransactionId { get; set; }
        public string mobile { get; set; }
        public string OperatorTxnID { get; set; }
    }
    public class CouponCodes
    {
        public string CouponCode { get; set; }

    }
    public class ApplyCouponCodes
    {
        public string CouponCode { get; set; }
        public decimal Amount { get; set; }
        public decimal discount { get; set; }
        public decimal finalAmount { get; set; }
        public string result { get; set; }
    }
    public class CPVenusModel
    {
        public string cptransid { get; set; }
        public string status { get; set; }
        public string rechargeupdatetransactionid { get; set; }
    }
    public class TopupResponse
    {
        public ExceptionDataas exception { get; set; }
        public TopupResult result { get; set; }
        public string message { get; set; }
        public int Flag { get; set; }
    }
    public class TopupResult
    {
        public int txId { get; set; }
    }
    public class ExceptionDataas
    {
        public string detailMessage { get; set; }
        public string cause { get; set; }
        public string shortMessage { get; set; }
        public string languageCode { get; set; }
        public string errorCode { get; set; }
        public object fieldErrors { get; set; }
        public List<object> suppressed { get; set; }
    }
    public class TopayWalletTopup
    {
        public string fromEntityId { get; set; }
        public string toEntityId { get; set; }
        public string yapcode { get; set; }
        public string productId { get; set; }
        public string description { get; set; }
        public float amount { get; set; }
        public string transactionType { get; set; }
        public string business { get; set; }
        public string businessEntityId { get; set; }
        public string transactionOrigin { get; set; }
        public string externalTransactionId { get; set; }

        public int TxId { get; set; }

    }
    public class GetPendingData
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string PaymentId { get; set; }
        public string Type { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentMode { get; set; }
        public int IsTopup { get; set; }
        public int totalusers { get; set; }
        public int sn { get; set; }
        public string Amount { get; set; }
        public string AmountFloat { get; set; }
        public string CPTransactionId { get; set; }
        public string OrderId { get; set; }
        public string ReservationId { get; set; }
        public string Remark { get; set; }
        public long FK_MemId { get; set; }

    }

    public class PaymentDetails
    {
        public long FK_MemId { get; set; }
        public string PaymentMode { get; set; }
        public string Type { get; set; }
        public string PaymentId { get; set; }
        public string ServiceType { get; set; }
        public decimal Amount { get; set; }
        public string ClientTransId { get; set; }
        public string CPTransactionId { get; set; }
        public string OrderId { get; set; }
    }
    public class ToPayWalletAmount
    {
        public int flag { get; set; }
        public string msg { get; set; }
        public string name { get; set; }
        public string paymentId { get; set; }
        public long fk_memId { get; set; }
        public float amount { get; set; }
        public string entityId { get; set; }
        public long MemberId { get; set; }
        public string PaymentMode { get; set; }
        public string AddedBy { get; set; }
        public string Type { get; set; }
        public string txn_status { get; set; }



    }

    public class CheckPayment
    {
        public string Flag { get; set; }
        public string entityId { get; set; }
        public int Amount { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentId { get; set; }
        public long fk_memId { get; set; }
        public string OrderId { get; set; }
        public string documentContents { get; set; }

    }

    public class Result
    {
        public string Balance { get; set; }
        public string EntityId { get; set; }
        public string LienBalance { get; set; }
        public string ProductId { get; set; }
    }

    public class WalletBalanceModel
    {
        public List<Result> Result { get; set; }
    }

    public class Credentials
    {
        public static string AESKEY = "MOBILEP@#@132EYZ";
        public static string MobileNo = "9990132968";
        public static string PinNo = "2580";
        public static string ElectricityBillFetch = "https://api.multilinkrecharge.com/ReCharge/billfetchApi.asmx/ElectricityBillFetch?";
        public static string GasBillFetch = "https://api.multilinkrecharge.com/ReCharge/billfetchApi.asmx/GasBillFetch?";
        public static string WaterBillFetch = "https://api.multilinkrecharge.com/ReCharge/billfetchApi.asmx/WaterBillFetch?";
        public static string InsuranceBillFetch = "https://api.multilinkrecharge.com/ReCharge/billfetchApi.asmx/InsuranceBillFetch?";
        public static string LpgBillFetch = "https://api.multilinkrecharge.com/ReCharge/billfetchApi.asmx/LpgBillFetch?";
        public static string CableTvBillFetch = "https://api.multilinkrecharge.com/ReCharge/billfetchApi.asmx/CableTvBillFetch?";
        public static string BillPaymentWithPara = "https://api.multilinkrecharge.com/ReCharge/jsonrechargeapi.asmx/Recharge?";
        public static string CheckBillStatus = "https://api.multilinkrecharge.com/ReCharge/jsonrechargeapi.asmx/StatusCheckByRequestId?";
    }
    public class merchantstatusreq
    {
        public string transactionId { get; set; }
    }

 

    public class TransactionData
    {
        public int Responsecode { get; set; }
        public string Responsedescription { get; set; }
        public decimal CommissioninPersentage { get; set; }
        public decimal TransactionDetails { get; set; }
    }


    //public class ApiResponse
    //{
    //    public string response_code { get; set; }
    //    public string response_message { get; set; }
    //    public string data { get; set; }
    //    public List<TransactionData> TransactionDataList { get; set; }
    //}
    public class ApiResponse
    {
        public string Data { get; set; }
        public string Response_code { get; set; }
        public string Response_message { get; set; } // This is a JSON string
        public List<TransactionData> TransactionDataList { get; set; }
    }


}
