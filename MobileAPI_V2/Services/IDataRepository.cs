﻿using Microsoft.AspNetCore.Mvc;
using MobileAPI_V2.Model;
using MobileAPI_V2.Model.BBPS;
using MobileAPI_V2.Model.BillPayment;
using MobileAPI_V2.Model.Fineque;
using MobileAPI_V2.Model.Svaas;
using MobileAPI_V2.Model.Travel;
using MobileAPI_V2.Model.Travel.Bus;
using MobileAPI_V2.Models;
using Nancy;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MobileAPI_V2.Model.FBH_Repository;

namespace MobileAPI_V2.Services
{
    public interface IDataRepository
    {
        Task<List<AffiliateOffer>> GetAffiliateOffers();
        Task<ReferralModelEcomm> GetReferralName(string ReferralCode);
        Task<ResultSet> CheckJWTToken(string Token);
        Task<ResultSet> CheckMobileNo(string mobileno);
        Task<PincodeModel> GetAreaDetailByPincode(string pincode);
        Task<LoginResponse> Login(Login model);
        Task<Common> Insetnotification(string notification);
        //Task<ResultSet> Register(Registration model);
        Task<Registrationresponse> CustomerRegistration(Registration model);
        Task<ResultSet> EmailLog(EmailLog model);
        Task<ResultSet> ForgetPassword(ForgotPassword model);
        Task<OTPResult> OTPProcess(OTPRequest model);
        Task<AppVersionResponse> GetAppVersionDetail(string ostype,string version);
        Task<LoginResponse> GetUserProfile(long MemberId);
        Task<Bankdetails_accountResponse> bankdetails(long memberId);
        Task<ResultSet> ChangePassword(ChangePassword objrequest);
        Task<ResultSet> SignOut(SignOutRequest model);
        Task<ResultSet> EditProfile(ProfileRequest model);
        Task<BusinessLevelResult> BusinessLevel(long MemberId);
        Task<mobileoperator> OperatorDetails(string mobilenumber);

        //Task<OperatorCodeModelName> GetOperatorCodeName(string OperatorCode);

        Task<RechargePlanData> RechargePlan(string operator_id, string circle_id);
        Task<List<Expense>> GETSupper30Expense(string type);
        Task<List<Direct>> GetDirect(long MemberId, long OldmemberId);
        Task<List<Direct>> GetLevelWiseDetail(long MemberId, string level, string type, string searchId, int page);
        //Task<TeamStatusResponse> GetTeamStatus(long MemberId);
        Task<CugItemTemp> GetTeamStatus(long MemberId, string searchId);
        Task<List<RecentRecharge>> GetRecentRecharge(long MemberId, string type = "", int page = 1);
        Task<List<Ministatement>> GetMiniStatement(long MemberId, int page);
        Task<FranchiseDetailsResponse> GetFranchiseDetails(int DistrictId);
        Task<List<OperatorCodeModel>> GetOperatorsCode(int ProcId, string Type, string opt);
        Task<ResultSet> CheckEmail_Mobile(string data);
        Task<Common> SubFranchiseCardRequest(SubFranchiseCardRequest model);
        Task<PaymentOrderResponse> PaymentOrder(PaymentOrderModel model);
        Task<LedgerResult> Ledger(long MemberId, string Type, string level, int monthId, int yearId);
        Task<MobilePeRegularResult> MobilePeRegularEarnings(long MemberId);
        Task<SelfCUGMoreResult> SelfCUGMore(long MemberId, int monthId, int year);
        Task<CUGMoreResponse> CUGMore(CUGMoreRequest model);
        Task<List<CUGMoreDetails>> CUGMoreDetails(CUGMoreRequest model);
        Task<CardActivationMore> cardActivationMore(CardActivationEarningRequest model);
        Task<MobilePeClub> MobilePeClub(long MemberId, int monthId, int year);
        Task<EarningBankTrfResponse> EarningBankTrf(long MemberId, int monthId, int year);
        Task<ResultSet> RedeemRewardPoints(RedeamRewardPoints model);

        Task<List<AffiliateModel>> GetAffiliateProgram(long MemberId, string type);
        Task<List<AffilicateCategory>> GetAffiliateCategory();
        Task<Common> EmailConfirmationLink(string Token, string Type, string Mobile, string emailId, string encrypteddata);
        Task<BusinessDashboard> BusinessDashboard(long MemberId);
        Task<DashboardResponse> BusinessTransaction(long MemberId);
        Task<DashboradBannerResponse> DashboradBanner(long MemberId);
        Task<Common> CheckAmountByTransactionId(string paymentId);
        Task<ResultSet> OperatorRecharge_V2(RechargeDeskRequest model);

        Task<ResultSet> ImageUpload(string FilePath, string doctype, long memberId);
        Task<AllLedgerResponse> AllLedger(long MemberId, string type, int monthId, int yearId);
        Task<AllLedgerResponse> LevelWiseLedger(long MemberId, string type, int monthId, int yearId);
        Task<List<FCM>> GetNotification(long MemberId);
        Task<NotificationCount> GetNoticationCount(long MemberId);
        Task<Common> ReadNotification(ReadNotification model);
        Task<List<BindDropDown>> GetDropDown(int ProcId, string dataId);
        Task<List<BindDropDown2>> GetDropDown2(int ProcId, string dataId);
        Task<InsuranceEcomm> GetInsuranceUrl(int serviceId, long memberId);
        Task<Common> FreeCardCostProduct(FREECARDCOSTRequest model);
        Task<List<CardLedgerDetails>> CardLedger(long MemberId, string type);
        Task<Common> BlockCard(BlockCardRequest model);
        Task<CardMasterResponse> GetCardDetails(long MemberId);
        Task<ResultSet> VerifyPhysicalCard(string Cardno, long MemberId);
        Task<ResultSet> CreateRazorpayXContact(CreateRazorPayContactRequest model, int procId);
        Task<ResultSet> UpdateRazorpayXContact(CreateContactResponse model, int procId);
        Task<ResultSet> UpdateRazorpayXFundAccount(FundAccountResponse model);
        Task<ResultSet> insertpayout(payoutsRequest model, int procId);
        Task<List<cardamountdetails>> Cardamountdetails(long Id, string type);
        Task<List<CommissionTransferdetail>> GetCommissionTransfer(long MemberId);
        Task<CommissionDetails> transferedCommissionDetails(long memberId, decimal amount);
        Task<Commissionresponse> transfercommission(int ProcId, long memberId, decimal amount, string accountnumber, string transId, long commissionId
           , long tdsId, long transferchargeId, long transferCommissionId);
        Task<List<Information>> GetInformationMessage(long memberId,string appType,string appVersion);
        Task<List<FAQ>> FAQ(long Id);
        Task<OrderDetailResponse> OrderDetail(long TransId);
        Task<Common> SupportRequest(SupportRequest model);
        Task<List<SupportRequestdata>> SupportRequestStatus(long memberId);
        Task<List<SupportMessageDetails>> SupportRequestByTicket(string TicketNo);
        Task<Common> RepliedMessage(ReplyMessage model);
        Task<List<PendingRechargeRequest>> GetPendingRecharge();
        Task<ResultSet> PineCardHistroy(long memberId, string externalId, string request, string response, string Token, string APIName);
        Task<ResultSet> GetCardReference(long memberId);
        Task<ResultSet> Getpinetoken(int procId, string token, string expirytime);

        Task<List<CommissionModel>> GetGeneratedCommission(long Id);
        Task<DashboardV2Response> DashboardDetailV2(long memberId);
        Task<ProductDetailsResponse> ProductDetail(long productDetailsId);
        Task<Common> AddCartItem(CartRequest model);
        Task<List<CartList>> GetCartItem(long UserId);
        Task<Common> RemoveCartItem(long Pk_CartItemId);
        Task<Common> UpdateProductQuantity(long Pk_CartItemId, string type);
        Task<Common> CheckDeliveryAddress(long ProductId, string pincode);
        Task<Common> AddReview(Review model);

        Task<List<ProductList>> ProductList(int? CategoryId, string BrandId, int? SizeId, int? ColorId, int? SubategoryId, int Offset, int Page, string Sort);


        Task<ProductData> ProductList_V2(int? CategoryId, string BrandId, int? SizeId, int? ColorId, int? SubategoryId, int Offset, int Page, string Sort, int SearchTypeId, int SearchId);

        Task<ReviewedUser> ReviewUser(long memberId);


        Task<vocherresponse> GetVoucherBrand(string Brand);

        Task<Common> UserAddress(UserAddress model);
        Task<Common> DeleteUserAddress(UserAddress model);

        Task<UserAddressresponse> GetUserAddress(long memberId, string RequestType);
        Task<OrderResponse> CreateOrder(CreateOrder model);

        Task<Common> RedeemPoint(long Id, int point, long orderId, string PaymentId, decimal amount, decimal commpayout);

        Task<MallOrderDetail> MallOrderDetail(string paymentId);
        Task<List<mallOrderStatus>> Getorderstatus(string orderno, string vendorId, long ProductId);
        Task<List<productstatus>> GetorderstatusV3(string orderno, string vendorId, long ProductId);
        Task<CancelOrderResponse> CancelOrder(long pk_OrderItemId, long memberId, string reason);
        Task<Common> CreatePoint(long Id, int point, long orderId);
        Task<VoucherRequest> checkvoucher(long MemberId, long VoucherId);
        Task<Common> UpdateVoucherV2(VoucherResponse model, string voucherdetails);
        Task<Common> AddVoucherItem(vouchercart model);

        Task<MallOrderDetail> GetPaymentMethod(string paymentId);
        Task<Replacement> GetReplacementItem(long pk_OrderItemId);

        Task<Common> ReplacementRequest(ReplacementRequest model);

        Task<Common> GetCartCount(long memberId);
        Task<List<filterdata>> GetFilterData(string type);

        Task<MinistatementV2Response> GetMiniStatement_V2(long MemberId, int page, string transtype, string categorytype);
        Task<LedgerResultV2> LedgerV2(long MemberId, string Type, string commtype, string transtype, string categoryType, int monthId, int yearId, int page);
        Task<AllLedgerResponse> AllLedgerV2(long MemberId, string type, int monthId, int yearId, string Level);
        Task<BusinessDashboardV2> BusinessDashboardV2(long MemberId);
        Task<CustomerHelpResponse> GetHelp(HelpRequest model);

        Task<List<ViewTicket>> ViewTicket(long memberId);
        Task<ViewTicketMessages> ViewTicketMessages(long memberId, string ticketno);

        Task<Common> AddCommissionTransferRequest(long MemberId, decimal Amount, decimal tds, string TransactionId);
        Task<Common> AddActivityDetails(string IPAddress, string RequestData, string controller, string action, string url, string data);
        Task<ResultSet> BillRecharge(VenusBillPayRequest model);

        //////////////////////////Version 3 Start

        Task<BusinessDashboardV3> BusinessDashboardV3(long MemberId);
        Task<DashboardV3Response> DashboardDetailV3(long memberId);
        Task<DashboardV3Response> DashboardDetailV4(long memberId);
        Task<DashboardV5Response> DashboardDetailV5(long memberId, string IsLocal);
        Task<DashboardV5Response> DashboardDetailV6(long memberId, string IsLocal,string appVersion,string appType);
        Task<Registrationresponse> CustomerRegistrationV3(RegistrationV3 model);
        Task<ResultSet> AddCardDispatchDetail(CardDispatchDetail model);
        Task<ResultSet> CheckNewUserCardStatus(long memberId);
        Task<BankDetailResponse> GetBank(long MemberId);
        Task<ResultSet> CheckBankStatus(long memberId);
        Task<ResultSet> SaveWebHookString(string document, long MemberId, long Id);
        Task<PaymentOrderModel> GetPaymentOrderDetail(string OrderId);
        public Task<ResultSet> PaymentStatus(string paymentId, string type, string orderId, string status);
        #region //dainik
        Task<ResultSet> EasyBillRecharge(EasyBillPay model, EasyBillPayResponse billPayResponse);

        #endregion

        Task<ResultSet> WalletRegistration(WalletCreationRequest model, int ProcId);
        Task<List<SupportTypeList>> SupportType(int Id);
        Task<vouchersummary> GetVoucherSummary(long MemberId);
        Task<VoucherDescription> GetVoucherDescriptionDetails(string Brand, string Product);
        Task<LoginResponse> WebViewLogin(string token);

        Task<ResultSet> MaintainSessionIdForWallet(WalletTransactionHandShakeRequest model);
        Task<ResultSet> WalletRecharge(WalletTop model);

        Task<Name_EmailModel> GetUserName_Email(string MemberId, string type);
        Task<ResultSet> VirtualAccount(virtualaccountsResponse model, long Id);
        Task<ResultSet> GetCardUserMobileNumber(long Id);
        Task<ResultSet> PostWalletTransaction(PostWalletTransactionRequest model);
        Task<List<PostWalletTransactionRequest>> GetDebitWalletTransaction(int ProcId);
        Task<WalletTransactionLog> WalletTransactionAction(WalletTransactionLog model);
        Task<List<PendingWalletTransaction>> GetPendingWalletTransaction();
        Task<ResultSet> TrainTicketBooking(string transactonId, string data, long MemberId,int procId);
        Task<CardDispatchDetail> GetCardDispatchDetailByUser(long MemberId);
        Task<WALLETBALANCEResponse> WalletBalance(int ProcId,string mobileno);
        Task<List<WalletMiniStatement>> WalletStatement(int ProcId, string mobileno,string fromdate,string todate);
        Task<WalletInfoResponse> WalletInfo(int ProcId, string mobileno);
        Task<walletTopUp> ValidateWallet_Topup(walletTopUp model);
        Task<walletTopUp> SaveWallet_Topup(walletTopUp model);
        Task<walletTopUp> UpdateWalletTopUp(walletTopUp model);
        Task<SaveBookingResponse> SaveBookingResponse(BookingResponse bookingResponse);
        Task<walletOTP_Log> SaveWallet_OTPLog(walletOTP_Log model);
        Task<ToPaywalletReg> ValidateWallet_Reg(ToPaywalletReg model);
        Task<ToPaywalletReg> SaveTopayWallet_Reg(WalletCreationRequestV2DTO model);
        Task<ToPaywalletReg> UpdateWalletReg_Response(ToPaywalletReg model);
        Task<WalletBalance> SaveWalletBalance(WalletBalance model);
        Task<WalletSetPreferencesSave> SaveWalletSetPreferences(WalletSetPreferencesSave model);
        Task<WalletBlockUnblock_log> SaveWalletBlockUnblock(WalletBlockUnblock_log model);
        Task<WalletDebit_log> SaveWalletDebit(WalletDebit_log model);
        Task<WalletSetlimit_log> SaveWalletLimitV2(WalletSetlimit_log model);
        Task<WalletSetPIN_log> SaveWalletSetPINV2(WalletSetPIN_log model);
        Task<WalletSetPIN_log> SaveCardinfo(WalletSetPIN_log model);
        Task<WalletDashboard_log> GetDashboard(WalletDashboard_log model);

        Task<ReplaceCardLog> ReplaceCard(ReplaceCardRespone model);

        Task<List<CardData>> GetCardData(CardRequestModelLog model);

        Task<WalletV2WalletCommonResponse> ApplyCard(ApplyCard model);
        Task<UpdateMobileLog> UpdateMobile(UpdateMobileLog model);
        Task<walletResetOTP_Log> SaveWalletResetOTPLog(walletResetOTP_Log model);
        Task<List<CardViewDetailResult>> CardViewInfo(CardViewDetails_log model);


        Task<AddedOnRequest> AddedOnRequest(AddedOnRequest model);
        Task<BlockandUnblockStatusRequest> BlockandUnblockStatus(BlockandUnblockStatusRequest model);
        Task<CheckCardTypeResponse> CheckKitType(CheckCardTypeRequest model);
        Task<TokenResponse> GetToken();
        Task<KitNoResponse> AssignKit_No(KitNoResponse model);
        Task<List<KitsAvailabilityList>> GetKitAvailability();

        Task<CheckCardTypeResponse> GetAvailableKit(int OldCardId, int NewCardId,long Fk_MemId,string entityid);


        Task<CheckCardTypeResponse> SaveTravelRequest(string Request,string Type,int Fk_MemId);
        Task<CheckCardTypeResponse> SaveBBPSRequest(string Request,string Response,string Type,int Fk_MemId,string OrderId);

        Task<CheckCardTypeResponse> SaveBookingResponse(TravelSaveResponse bookTicketResponse);
        Task<CheckCardTypeResponse> SaveBookingResponseSuccess(TravelSaveResponse bookTicketResponse);
        Task<List<BookingList>> GetBookingList(int FK_MemId);

        Task<List<PaymentData>> GetPaymentOption(int Fk_MemId,string Type);

        Task<CheckMobileTransfer> CheckMobileForTransfer(string MobileNo);

        Task<List<TrainBookingList>> GetTrainBookingList(int FK_MemId);

        Task<CheckCardTypeResponse> CancellationResponse(string ClientTransactionId,string Response);

        Task<CheckCardTypeResponse> UpdateNonLCCBookingResponse(TravelSaveResponse bookingResponse); 
        Task<CheckCardTypeResponse> SaveErrorBookingResponse(TravelSaveResponse bookTicketResponse);

        Task<ResultTravel> SaveTransactionData(long FK_MemId, string TransactionId, string PaymentType, string Remark, string ActionType, decimal Amount, string OperatorTxnID, string status,string Number, string OperatorCode,string Response);


        Task<WalletSetPin> WalletSetPin(WalletSetPin model);
        Task<BillPayRequest_log> SaveBillPayRequsest(BillPayRequest_log model);


        Task<List<BillerModel>> GetBiller();
        Task<DashboardV5Response> NewDashboard(long memberId);

        Task<List<PinCodeData>> GetSvaasAvailbility(string Pincode);


        Task<SvaasSaveResponse> SvaasPaymentReponse(SvaaspaymentResponse svaaspaymentResponse);


        Task<CheckCardTypeResponse> SaveBillPaymentResponse(BillPaymentResponse billPaymentResponse);
        Task<CheckCardTypeResponse> UpdateBillpayemnt(string accountid, string transtype,string txid);

        Task<List<GetBillPayProvider>> GetBilPayBillers(string Type);
        Task<TokenResponse> getdata();

        Task<List<OrderData>> GetSvaasOrders(string Fk_MemId);

        Task<ResultTravel> GetBillPaymentRequest(string OrderId);

        Task<ResultSet> UpdateMemberCard(string paymentId, string OrderId, int OpCode, string KitNo, int ToPayCardId);

        Task<CheckCardTypeResponse> UpdateOpertortxnId(string accountid, string transtype, string txid);

        Task<CheckMobileTransfer> RecievedCard(RecivedCardRequest recivedCardRequest);

        Task<AvailableKitForVirtual> GetAvailableKitForVirtual(long Fk_MemId);
        Task<AvailableKitForVirtual> UpdateRefundPaymentStaus(string PaymentId,string RefundId,string PaymentType,string refundstatus);
        Task<LoginResponse> ViewProfile(ViewProfile viewProfile);

        Task<ResultSet> CheckExtraAddedAmountInPineCard(int ProcId, long memberId, decimal amount);
        Task<pinecard> getpinecarddetails(long customerId);

        Task<List<PassengersList>> GePassengersList(string fk_memid);
        Task<PassengerResponse> SavePassengersDetail(PassengersList passengermodel);
        Task<PassengerResponse> UpdatePassengersDetail(PassengersList passengermodel);
        Task<PassengerResponse> DeletePassengersDetail(PassengersList passengermodel);
        Task<BBSPBillPayReq> SaveBBPSBillPayReq(BBSPBillPayReq model);
        Task<BBPSBillPayres> SaveBBPSBillPayResponse(BBPSBillPayres model,long Fk_MemId,string Request,string Response);
        Task<LoginHeader> GetFinequeHeader();
        Task<FinequeReqRes> SaveReqRes(FinequeReq model,string request);
        Task<AuthResponse> SaveBusToken(string TokenId);
        Task<TokenResponse> GetBUSToken();
        Task<FinequeLoginRes> SaveFinqueLoginToken(string token, string userName, string createdAt, string partnerId, string result);
        Task<LoginToken> GetFinequeAuthToken();
        Task<NirmalBangPolicyResponse> SaveNBpolicy(NirmalBangPolicy policy);

        Task<string> BankCreditCardBusinessDetails(string Name, string Mobile_Number, string Email_Id, string Marketing_Experience, string State, string District, string Remarks,string ReferenceType);
        Task<List<TblCouponMaster>> GetCouponMaster();
        Task<string> CheckStatus(string TransactionId, string mobile, string OperatorTxnID);
        Task<TblCouponMaster> CheckCouponCodeMaster(string couponCode);
        Task<ApplyCouponCodes> ApplyCouponCodeMaster(string couponCode, decimal amount);
        List<GetPendingData> GetPendingPayment(string search, int page, int pagesize, string order, string fromDate, string Todate, string Type);
        public CheckPayment GetPaymentDetails(string PaymentId);
        public ToPayWalletAmount UpdateTopayWalletAmount(string paymentId, string txtmobile, string txtremark, decimal Amount, string AddedBy, string externalTransactionId, string Response);
        // public List<PaymentDetails> UpdatePendingRecharge(string TransactionNo, string Status, string RechargeNumber, string Amount, string ActionType, string OperatorTxnID, string Remark, string Response);
        public Task<List<PaymentDetails>> UpdatePendingRechargeAsync(string TransactionNo,string Status,string RechargeNumber,string Amount,string ActionType,string OperatorTxnID,string Remark,string Response);
        public Task<getTypeFromCP> CPorVenus();
        public Task<ExtraDataForCP> exDataWantToUpdateRechargeAsync(string TransactionId, string Status);
        public Task<string> CommissionMerchant(decimal CommissionPercentage, string paymentId);
        public Task<getMobile> GetAvailableMobile(long Fk_MemId);
        public Task<List<GetMerchantBalance_Res>> GetMerchantBalance(string mobile);
        public Task<List<object>> GetSettlementTrnxReport(string mobilenumber);
        public Task<List<object>> GetAvailableCommission(int Fk_memid, string Brandproductcode);
        public Task<List<object>> ApplyCommissionReferal(ApplyCommissionReferal_Req applyCommissionReferal_);
        public Task<List<object>> upd_usedflag(string tranxId);
        public Task<List<object>> CustomertranactionList(CustomertranactionList_req req);
        public Task<List<object>> Redeemcommissionwallet(MerchantDebitReferal_Req merchantDebitReferal_Req);
        public Task<ins_WalletdebitRequest_res> _repo_ins_WalletdebitRequest(Ins_walleDebitrequest_req walletTop);
        public Task<IEnumerable<MerchantModelDB>> GetPendingMerchants();
        public Task SaveMerchantData(long FK_MemId, string TransactionId, string PaymentType, string Remark, string ActionType, decimal Amount, string OperatorTxnID, string status, string Number, string OperatorCode, string Response);

        }
}
