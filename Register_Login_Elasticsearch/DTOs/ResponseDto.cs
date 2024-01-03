using Register_Login_Elasticsearch.Models;
using System.Net;

namespace Register_Login_Elasticsearch.DTOs
{
    public record ResponseDto
    {
        //public T Data { get; set; }
        //public HttpStatusCode Status { get; set; }
        //public string? Message { get; set; }

        public record LoginResult(Users User , string Token, string Message);
       //public static ResponseDto<T> Success(HttpStatusCode status,string message, T data)
       // {
       //     return new ResponseDto<T> { Status = status, Message = message, Data = data };
       // }
       //public static ResponseDto<T> Error(HttpStatusCode status,string message, T data)
       // {
       //     return new ResponseDto<T> { Status = status, Message = message , Data = data};
       // }
    }
    

}
