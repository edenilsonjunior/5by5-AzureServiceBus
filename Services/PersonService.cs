using Models;
using MongoDB.Driver;
using Services.Utils;

namespace Services;

public class PersonService
{
    private readonly IMongoCollection<Person> _collection;

    public PersonService(IMongoDataBaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        _collection = database.GetCollection<Person>(settings.PersonCollectionName);
    }


    public async Task<Person> CreatePersonAsync(Person person)
    {
        await _collection.InsertOneAsync(person);
        return person;
    }


    public async Task<Person> GetPersonAsync(string id) => 
        await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();


    public async Task<List<Person>> GetPeopleAsync() =>  
        await _collection.Find(p => true).ToListAsync();


    public async Task UpdatePersonAsync(string id, Person person) => 
        await _collection.ReplaceOneAsync(p => p.Id == id, person);


    public async Task DeletePersonAsync(string id) => 
        await _collection.DeleteOneAsync(p => p.Id == id);

}
