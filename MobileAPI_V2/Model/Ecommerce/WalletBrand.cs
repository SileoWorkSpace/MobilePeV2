namespace MobileAPI_V2.Model.Ecommerce
{
    public class GetBrandwalletLedger_req
    {
        public int Fk_memid { get; set; }
    }
    public class GetWalletBrand_req
    {
        public int mode { get; set; }
        public string Searchvalue { get; set; }
    }

    public class tp_getbalance_req
    {
        public int Fk_memid { get; set; }
        public string Brandproductcode { get; set; }
        public decimal Transactionamount { get; set; }
    }
}