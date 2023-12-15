namespace Register_Login_Elasticsearch.DTOs
{
    public record UsersUpdateDto(int Id, string ElasticId, string UserName, string Password)
    {
    }
}
