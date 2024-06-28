using Microsoft.EntityFrameworkCore;
using Project.Contexts;
using Project.Exceptions;
using Project.Models;
using Project.RequestModels;
using Project.Services;

namespace Project.Tests;

public class ClientServiceTests
{
    private readonly DbContextOptions<DatabaseContext> _dbContextOptions;

    public ClientServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private DatabaseContext CreateContext() => new DatabaseContext(_dbContextOptions);

    [Fact]
    public async Task AddIndividualClientAsync_ShouldAddClient()
    {
        // Arrange
        await using var context = CreateContext();
        var clientService = new ClientService(context);

        var model = new CreateIndividualClientModel
        {
            FirstName = "test",
            LastName = "test",
            Email = "test@gmail.com",
            Address = "test 123",
            Phone = "123456789",
            PESEL = "12345678901"
        };

        // Act
        await clientService.AddIndividualClientAsync(model);

        // Assert
        var client = await context.Individuals.FirstOrDefaultAsync();
        Assert.NotNull(client);
        Assert.Equal(model.Email, client.ClientEmail);
        Assert.Equal(model.Address, client.ClientAdress);
        Assert.Equal(model.Phone, client.ClientPhone);
        Assert.Equal(model.FirstName, client.IndividualFirstName);
        Assert.Equal(model.LastName, client.IndividualLastName);
        Assert.Equal(model.PESEL, client.IndividualPesel);
    }

    [Fact]
    public async Task AddBusinessClientAsync_ShouldAddClient()
    {
        // Arrange
        await using var context = CreateContext();
        var clientService = new ClientService(context);

        var model = new CreateBusinessClientModel
        {
            Email = "test@gmail.com",
            Address = "test 123",
            Phone = "123456789",
            Name = "test",
            KRS = "123456789"
        };

        // Act
        await clientService.AddBusinessClientAsync(model);

        // Assert
        var client = await context.Businesses.FirstOrDefaultAsync();
        Assert.NotNull(client);
        Assert.Equal(model.Email, client.ClientEmail);
        Assert.Equal(model.Address, client.ClientAdress);
        Assert.Equal(model.Phone, client.ClientPhone);
        Assert.Equal(model.Name, client.BusinessName);
        Assert.Equal(model.KRS, client.KRS);
    }

    [Fact]
    public async Task DeleteIndividualClientAsync_ShouldDeleteClient()
    {
        // Arrange
        await using var context = CreateContext();
        var clientService = new ClientService(context);

        var individual = new Individual
        {
            ClientId = 1,
            IndividualFirstName = "test",
            IndividualLastName = "test",
            IndividualPesel = "12345678901",
            ClientEmail = "test@example.com",
            ClientPhone = "123456789",
            ClientAdress = "test 123",
            ClientIsReturning = false
        };
        
        await context.Individuals.AddAsync(individual);
        await context.SaveChangesAsync();

        // Act
        await clientService.DeleteIndividualClientAsync(1);

        // Assert
        var client = await context.Individuals.FirstOrDefaultAsync(e => e.ClientId == 1);
        Assert.NotNull(client);
        Assert.NotNull(client.IndividualDeletedAt);
    }
    
    [Fact]
    public async Task DeleteIndividualClientAsync_ShouldThrowNotFoundException()
    {
        // Arrange
        await using var context = CreateContext();
        var clientService = new ClientService(context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await clientService.DeleteIndividualClientAsync(10);
        });
    }
    
    [Fact]
    public async Task UpdateIndividualClientAsync_ShouldUpdateClient()
    {
        // Arrange
        await using var context = CreateContext();
        var clientService = new ClientService(context);

        var individual = new Individual
        {
            ClientId = 1,
            IndividualFirstName = "test",
            IndividualLastName = "test",
            IndividualPesel = "12345678901",
            ClientEmail = "test@example.com",
            ClientPhone = "123456789",
            ClientAdress = "test 123",
            ClientIsReturning = false
        };
        
        await context.Individuals.AddAsync(individual);
        await context.SaveChangesAsync();

        var updateModel = new UpdateIndividualClientModel
        {
            FirstName = "test1test",
            LastName = "test1test",
            Email = "test@example.com1",
            Address = "test 1231",
            Phone = "123456789",
        };

        // Act
        await clientService.UpdateIndividualClientAsync(1, updateModel);

        // Assert
        var client = await context.Individuals.FirstOrDefaultAsync(e => e.ClientId == 1);
        Assert.NotNull(client);
        Assert.Equal(updateModel.FirstName, client.IndividualFirstName);
        Assert.Equal(updateModel.LastName, client.IndividualLastName);
        Assert.Equal(updateModel.Email, client.ClientEmail);
        Assert.Equal(updateModel.Address, client.ClientAdress);
        Assert.Equal(updateModel.Phone, client.ClientPhone);
    }
    
    [Fact]
    public async Task UpdateIndividualClientAsync_ShouldThrowNotFoundException()
    {
        // Arrange
        await using var context = CreateContext();
        var clientService = new ClientService(context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await clientService.UpdateIndividualClientAsync(10, new UpdateIndividualClientModel());
        });
    }
    
    [Fact]
    public async Task UpdateBusinessClientAsync_ShouldUpdateClient()
    {
        // Arrange
        await using var context = CreateContext();
        var clientService = new ClientService(context);

        var business = new Business
        {
            ClientId = 1,
            ClientEmail = "test@gmail.com",
            BusinessName = "test",
            ClientAdress = "test 123",
            ClientPhone = "123456789",
            KRS = "123456789",
            ClientIsReturning = false
        };
        await context.Businesses.AddAsync(business);
        await context.SaveChangesAsync();

        var updateModel = new UpdateBusinessClientModel
        {
            Name = "test1test",
            Email = "test@gmail.com",
            Address = "test 123",
            Phone = "123456789"
        };

        // Act
        await clientService.UpdateBusinessClientAsync(1, updateModel);

        // Assert
        var client = await context.Businesses.FirstOrDefaultAsync(e => e.ClientId == 1);
        Assert.NotNull(client);
        Assert.Equal(updateModel.Name, client.BusinessName);
        Assert.Equal(updateModel.Email, client.ClientEmail);
        Assert.Equal(updateModel.Address, client.ClientAdress);
        Assert.Equal(updateModel.Phone, client.ClientPhone);
    }
    
    [Fact]
    public async Task UpdateBusinessClientAsync_ShouldThrowNotFoundException()
    {
        // Arrange
        await using var context = CreateContext();
        var clientService = new ClientService(context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await clientService.UpdateBusinessClientAsync(10, new UpdateBusinessClientModel());
        });
    }
}