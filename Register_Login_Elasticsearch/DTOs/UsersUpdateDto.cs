namespace Register_Login_Elasticsearch.DTOs
{
    public record UsersUpdateDto(string ElasticId, string UserName, string Password)
    {
    }

}
