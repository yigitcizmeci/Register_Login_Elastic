using Nest;

namespace Register_Login_Elasticsearch.DTOs
{
    public record UserLoginDto(string UserName, string Password, string token)
    {
    }   
}
