namespace Register_Login_Elasticsearch.DTOs
{
    public record UserDto(int DatabaseId,string ElasticId, string Name, string Surname, string Email)
    { 
    }
}
