using MobileAPI_V2.Model;
using MobileAPI_V2.Model.BillPayment;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MobileAPI_V2.Model.MerchantModel;

namespace MobileAPI_V2.Services
{
    public interface IMerchant
    {
        public Task<MerchantBody> MerchantRequestssData(string mobilenumber);
        public Task<string> MerchantCreditRequestDB(MerchantCredit data);
        public Task<string> MerchantSearch(MerchantSearch model);
        public Task<List<PaymentIdss>> PaymentRes();
        public Task<string> insertPaymentRes(venusModel Data, string PaymentId);

    }
}

