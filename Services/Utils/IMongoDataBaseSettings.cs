namespace Services.Utils;

public interface IMongoDataBaseSettings
{
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
    string PersonCollectionName { get; set; }
}
