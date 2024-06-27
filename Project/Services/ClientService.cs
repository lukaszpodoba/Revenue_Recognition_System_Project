using Microsoft.EntityFrameworkCore;
using Project.Contexts;
using Project.Exceptions;
using Project.Models;
using Project.RequestModels;

namespace Project.Services;

public interface IClientService
{
    Task AddIndividualClientAsync(CreateIndividualClientModel data);
    Task AddBusinessClientAsync(CreateBusinessClientModel data);
    Task DeleteIndividualClientAsync(int id);
    Task UpdateIndividualClientAsync(int id, UpdateIndividualClientModel data);
    Task UpdateBusinessClientAsync(int  id, UpdateBusinessClientModel data);
}

public class ClientService(DatabaseContext context) : IClientService
{
    public async Task AddIndividualClientAsync(CreateIndividualClientModel data)
    {
        var individual = new Individual
        {
            ClientEmail = data.Email,
            ClientAdress = data.Address,
            ClientPhone = data.Phone,
            IndividualFirstName = data.FirstName,
            IndividualLastName = data.LastName,
            IndividualPesel = data.PESEL,
            ClientIsReturning = false
        };
        
        await context.Individuals.AddAsync(individual);
        await context.SaveChangesAsync();
    }

    public async Task AddBusinessClientAsync(CreateBusinessClientModel data)
    {
        var business = new Business
        {
            ClientEmail = data.Email,
            ClientAdress = data.Address,
            ClientPhone = data.Phone,
            BusinessName = data.Name,
            KRS = data.KRS,
            ClientIsReturning = false
        };
        
        await context.Businesses.AddAsync(business);
        await context.SaveChangesAsync();
    }

    public async Task DeleteIndividualClientAsync(int id)
    {
        var client = await context.Individuals.Where(e => e.ClientId == id).FirstOrDefaultAsync();

        if (client is null)
        {
            throw new NotFoundException($"Client with given id {id} not found");
        }
        
        client.IndividualDeletedAt = DateTime.Now;
        
        await context.SaveChangesAsync();
    }

    public Task UpdateIndividualClientAsync(Individual individual, UpdateIndividualClientModel data)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateIndividualClientAsync(int id, UpdateIndividualClientModel data)
    {
        var client = await context.Individuals.Where(e => e.ClientId == id).FirstOrDefaultAsync();
        
        if (client is null)
        {
            throw new NotFoundException($"Client with given id {id} not found");
        }
        
        client.IndividualFirstName = string.IsNullOrEmpty(data.FirstName) ? client.IndividualFirstName : data.FirstName;
        client.IndividualLastName = string.IsNullOrEmpty(data.LastName) ? client.IndividualLastName : data.LastName;
        client.ClientEmail = string.IsNullOrEmpty(data.Email) ? client.ClientEmail : data.Email;
        client.ClientAdress = string.IsNullOrEmpty(data.Address) ? client.ClientAdress : data.Address;
        client.ClientPhone = string.IsNullOrEmpty(data.Phone) ? client.ClientPhone : data.Phone;
        
        await context.SaveChangesAsync();
    }

    public async Task UpdateBusinessClientAsync(int id, UpdateBusinessClientModel data)
    {
        var client = await context.Businesses.Where(e => e.ClientId == id).FirstOrDefaultAsync();
        
        if (client is null)
        {
            throw new NotFoundException($"Client with given id {id} not found");
        }
        
        client.BusinessName = string.IsNullOrEmpty(data.Name) ? client.BusinessName : data.Name;
        client.ClientEmail = string.IsNullOrEmpty(data.Email) ? client.ClientEmail : data.Email;
        client.ClientAdress = string.IsNullOrEmpty(data.Address) ? client.ClientAdress : data.Address;
        client.ClientPhone = string.IsNullOrEmpty(data.Phone) ? client.ClientPhone : data.Phone;
        
        await context.SaveChangesAsync();
    }
}