 namespace Register_Login_Elasticsearch.DTOs
{
    public record UserCreateDto(string Name, string Surname, string Email,string UserName, string Password)
    {
    }
}
