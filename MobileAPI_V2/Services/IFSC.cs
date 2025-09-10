using MobileAPI_V2.Model.BillPayment;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MobileAPI_V2.Model.FSCModel;

namespace MobileAPI_V2.Services
{
    public interface IFSC
    {
        public Task<List<BankDetails>> GetBankDetails();
        public Task<List<CreditCardDetails>> GetCreditCardDetails(long id);

        public  Task<string> ValidatePinCode(long creditID,long Pincode);
        public Task<string> SaveFscApplicants(FscApplicants requestModel);

        public Task<string> SaveBankFscLogs(BankFscLogs requestModel);

        public Task<string> SaveCreditFscLogs(CreditFscLogs requestModel);
        public Task<List<GetFscAppl>> GetFscApplicants(string FSC_MemId);

        public Task<string> UpdateSoundFlag(long memberId, bool flag);

    }
}
