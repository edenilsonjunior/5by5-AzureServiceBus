using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using Services;

namespace MessageProducer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PeopleController : ControllerBase
{
    const string connectionString = "";
    const string queueName = "peopleQueue";

    private readonly PersonService _service;

    public PeopleController(PersonService service)
    {
        _service = service;
    }


    [HttpPost]
    public async Task<ActionResult<Person>> PostPerson(PersonDTO personDTO)
    {
        Person person = new()
        {
            Id = new Guid().ToString(),
            Name = personDTO.Name,
            Email = personDTO.Email,
            Phone = personDTO.Phone
        };

        await using var client = new ServiceBusClient(connectionString);

        var sender = client.CreateSender(queueName);
        var message = new ServiceBusMessage(JsonConvert.SerializeObject(person));

        await sender.SendMessageAsync(message);

        return CreatedAtAction(nameof(GetPersonById), new { id = person.Id }, person);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Person>> GetPersonById(string id)
    {
        Person p = await _service.GetPersonAsync(id);

        if (p == null)
            return NotFound();
        
        return Ok(p);
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Person>>> GetAll()
    {
        List<Person> people = await _service.GetPeopleAsync();

        if (people.Count == 0)
            return NoContent();

        return Ok(people);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> PutPerson(string id, Person person)
    {
        if (id != person.Id)
            return BadRequest();

        await _service.UpdatePersonAsync(id, person);

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerson(string id)
    {
        await _service.DeletePersonAsync(id);

        return NoContent();
    }
}
