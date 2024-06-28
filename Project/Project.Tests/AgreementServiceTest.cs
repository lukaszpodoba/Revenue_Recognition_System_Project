using Microsoft.EntityFrameworkCore;
using Project.Contexts;
using Project.Exceptions;
using Project.Models;
using Project.RequestModels;
using Project.Services;

namespace Project.Tests;

public class AgreementServiceTest
{
    private readonly DbContextOptions<DatabaseContext> _dbContextOptions;
    
    public AgreementServiceTest()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }
    
    private DatabaseContext CreateContext() => new DatabaseContext(_dbContextOptions);

    [Fact]
    public async Task AddAgreementAsync_ShouldAddAgreement()
    {
        // Arrange
        await using var context = CreateContext();
        var agreementService = new AgreementService(context);

        var model = new CreateAgreementModel
        {
            PaymentFrom = DateTime.Now,
            PaymentUntil = DateTime.Now.AddDays(20),
            YearsOfVersionSupport = 2
        };

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

        var software = new Software
        {
            SoftwareId = 1,
            SoftwareName = "Kaspersky",
            SoftwareCategory = "Antivirus",
            SoftwareDescription = "Antivirus software",
            SoftwareCurrentVersion = "1.0",
            SoftwareSubscriptionPrice = null,
            SoftwareIsSubscriptionPurchase = false,
            SoftwareOneTimePrice = 1000,
            SoftwareIsOneTimePurchase = true
        };

        await context.Individuals.AddAsync(individual);
        await context.Softwares.AddAsync(software);
        await context.SaveChangesAsync();

        // Act
        await agreementService.AddAgreementAsync(model, 1, 1);

        // Assert
        var agreement = await context.Agreements.FirstOrDefaultAsync();
        var getClient = await context.Individuals.FirstOrDefaultAsync();
        var getSoftware = await context.Softwares.FirstOrDefaultAsync();
        Assert.NotNull(agreement);
        Assert.Equal(getClient!.ClientId, agreement.ClientId);
        Assert.Equal(getSoftware!.SoftwareId, agreement.SoftwareId);
        Assert.Equal(model.PaymentFrom, agreement.AgreementPaymentFrom);
        Assert.Equal(model.PaymentUntil, agreement.AgreementPaymentUntil);
        Assert.Equal(model.PaymentFrom.AddYears(model.YearsOfVersionSupport), agreement.AgreementEndOfVersionSupport);
    }
    
    [Fact]
    public async Task AddAgreementAsync_ShouldThrowNotFoundExceptionWhenClientNotFound()
    {
        // Arrange
        await using var context = CreateContext();
        var agreementService = new AgreementService(context);

        var model = new CreateAgreementModel
        {
            PaymentFrom = DateTime.Now,
            PaymentUntil = DateTime.Now.AddDays(20),
            YearsOfVersionSupport = 2
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await agreementService.AddAgreementAsync(model, 1, 1);
        });
    }

    [Fact]
    public async Task AddAgreementAsync_ShouldThrowNotFoundExceptionWhenSoftwareNotFound()
    {
        // Arrange
        await using var context = CreateContext();
        var agreementService = new AgreementService(context);

        var model = new CreateAgreementModel
        {
            PaymentFrom = DateTime.Now,
            PaymentUntil = DateTime.Now.AddDays(20),
            YearsOfVersionSupport = 2
        };
        
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
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await agreementService.AddAgreementAsync(model, 1, 1);
        });
    }

    [Fact]
    public async Task AddAgreementAsync_ShouldThrowWrongSoftwareTypeException()
    {
        // Arrange
        await using var context = CreateContext();
        var agreementService = new AgreementService(context);

        var model = new CreateAgreementModel
        {
            PaymentFrom = DateTime.Now,
            PaymentUntil = DateTime.Now.AddDays(20),
            YearsOfVersionSupport = 2
        };

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

        var software = new Software
        {
            SoftwareId = 1,
            SoftwareName = "Kaspersky",
            SoftwareCategory = "Antivirus",
            SoftwareDescription = "Antivirus software",
            SoftwareCurrentVersion = "1.0",
            SoftwareSubscriptionPrice = 200,
            SoftwareIsSubscriptionPurchase = true,
            SoftwareOneTimePrice = null,
            SoftwareIsOneTimePurchase = false
        };

        await context.Individuals.AddAsync(individual);
        await context.Softwares.AddAsync(software);
        await context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<WrongSoftwareType>(async () =>
        {
            await agreementService.AddAgreementAsync(model, 1, 1);
        });
    }

    [Fact]
    public async Task GetSoftwareDiscountAsync_ShouldReturnBiggestDiscount()
    {
        // Arrange
        await using var context = CreateContext();
        var agreementService = new AgreementService(context);

        var software = new Software
        {
            SoftwareId = 1,
            SoftwareName = "Kaspersky",
            SoftwareCategory = "Antivirus",
            SoftwareDescription = "Antivirus software",
            SoftwareCurrentVersion = "1.0",
            SoftwareSubscriptionPrice = null,
            SoftwareIsSubscriptionPurchase = false,
            SoftwareOneTimePrice = 1000,
            SoftwareIsOneTimePurchase = true
        };
        
        await context.Softwares.AddAsync(software);
        await context.SaveChangesAsync();

        var discount1 = new Discount
        {
            DiscountId = 1,
            DiscountName = "Student",
            DiscountFrom = DateTime.Parse("2023-01-01"),
            DiscountUntil = DateTime.Parse("2024-12-31"),
            DiscountPercentageValue = 50.0,
            DiscountType = "Agreement",
            SoftwareId = 1
        };
        
        var discount2 = new Discount
        {
            DiscountId = 2,
            DiscountName = "Sale",
            DiscountFrom = DateTime.Parse("2023-01-01"),
            DiscountUntil = DateTime.Parse("2024-12-31"),
            DiscountPercentageValue = 30.0,
            DiscountType = "Agreement",
            SoftwareId = 1
        };
        
        var discount3 = new Discount
        {
            DiscountId = 3,
            DiscountName = "Summer",
            DiscountFrom = DateTime.Parse("2022-01-01"),
            DiscountUntil = DateTime.Parse("2022-12-31"),
            DiscountPercentageValue = 80.0,
            DiscountType = "Agreement",
            SoftwareId = 1
        };
        
        await context.Discounts.AddAsync(discount1);
        await context.Discounts.AddAsync(discount2);
        await context.Discounts.AddAsync(discount3);
        await context.SaveChangesAsync();

        // Act
        var result = await agreementService.GetSoftwareDiscountAsync(1);

        // Assert
        Assert.Equal(50.0, result);
    }
}