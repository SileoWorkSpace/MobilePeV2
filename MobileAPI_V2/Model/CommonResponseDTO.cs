using System.Collections.Generic;

namespace MobileAPI_V2.Model
{
    public class CommonResponseDTO
    {
        public bool Status { get; set; }
        
        public int flag { get; set; }
        public string message { get; set; }  
        public string response { get; set; }
    }
    public class CommonResponseDTO<T>
    {
        public bool Status { get; set; }
        public int flag { get; set; }
        public string message { get; set; }
        public T result { get; set; }
    }
    public class CommonListResponseDTO<T>
    {
        public bool Status { get; set; }
        public int flag { get; set; }
        public string message { get; set; }
        public List<T> result { get; set; }
    }

    public class CommonResponseBody_Res
    {
        public string? response_code { get; set; } = "000";
        public string? response_message { get; set; } = "Failure";
        public string? data { get; set; }
    }

}
   
