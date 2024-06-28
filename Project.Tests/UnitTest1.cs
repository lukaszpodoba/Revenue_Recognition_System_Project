using Xunit;
using Moq;
using Project.Services;
using Project.Contexts;
using Project.Models;
using Project.RequestModels;
using System.Threading.Tasks;

public class ClientServiceTests
{
    private readonly Mock<DatabaseContext> _mockContext;
    private readonly IClientService _clientService;

    public ClientServiceTests()
    {
        _mockContext = new Mock<DatabaseContext>();
        _clientService = new ClientService(_mockContext.Object);
    }

    [Fact]
    public async Task AddIndividualClientAsync_ShouldAddClient()
    {
        // Arrange
        var clientData = new CreateIndividualClientModel
        {
            Email = "test@test.com",
            Address = "Test Street 1",
            Phone = "123456789",
            FirstName = "Test",
            LastName = "User",
            PESEL = "12345678901"
        };

        // Act
        await _clientService.AddIndividualClientAsync(clientData);

        // Assert
        _mockContext.Verify(m => m.AddAsync(It.IsAny<Individual>(), default), Times.Once);
        _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
    }
}