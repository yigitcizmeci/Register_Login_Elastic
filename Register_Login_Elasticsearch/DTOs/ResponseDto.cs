using Register_Login_Elasticsearch.Models;

namespace Register_Login_Elasticsearch.DTOs
{
    public record ResponseDto
    {
       public record LoginResult(Users User , string Token, string Message);
    }
}
