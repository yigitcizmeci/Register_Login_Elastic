 namespace Register_Login_Elasticsearch.DTOs
{
    public record UserCreateDto(string Name, string Surname, string eMail,string UserName, string Password)
    {
    }
}
