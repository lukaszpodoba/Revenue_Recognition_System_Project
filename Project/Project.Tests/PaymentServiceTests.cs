using Microsoft.EntityFrameworkCore;
using Project.Contexts;
using Project.Exceptions;
using Project.Models;
using Project.RequestModels;
using Project.Services;
using Project.Tests.Dummies;

namespace Project.Tests;

public class PaymentServiceTests
{
    private readonly DbContextOptions<DatabaseContext> _dbContextOptions;

    public PaymentServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private DatabaseContext CreateContext() => new DatabaseContext(_dbContextOptions);

    [Fact]
    public async Task AddPaymentAsync_ShouldAddPayment()
    {
        // Arrange
        await using var context = CreateContext();
        var paymentService = new PaymentService(context, new HttpClientDummy());

        var model = new CreatePaymentModel
        {
            DepositSize = 400
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

        var agreement = new Agreement
        {
            AgreementId = 1,
            AgreementSigned = false,
            AgreementPrice = 400,
            AgreementCurrentDeposited = 0,
            AgreementPaymentFrom = DateTime.Now,
            AgreementPaymentUntil = DateTime.Now.AddDays(20),
            AgreementCurrentSoftwareVersion = "1.0",
            AgreementEndOfVersionSupport = DateTime.Now.AddYears(2),
            ClientId = 1,
            SoftwareId = 1
        };

        await context.Softwares.AddAsync(software);
        await context.Individuals.AddAsync(individual);
        await context.Agreements.AddAsync(agreement);
        await context.SaveChangesAsync();

        // Act
        await paymentService.AddPaymentAsync(model, 1, 1);

        // Assert
        var payment = await context.Payments.FirstOrDefaultAsync();
        Assert.NotNull(payment);
        Assert.Equal(model.DepositSize, payment.PaymentPrice);
    }

    [Fact]
    public async Task AddPaymentAsync_ShouldThrowAmountHasBeenExceeded()
    {
        // Arrange
        await using var context = CreateContext();
        var paymentService = new PaymentService(context, new HttpClientDummy());

        var model = new CreatePaymentModel
        {
            DepositSize = 400000
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

        var agreement = new Agreement
        {
            AgreementId = 1,
            AgreementSigned = false,
            AgreementPrice = 1000,
            AgreementCurrentDeposited = 0,
            AgreementPaymentFrom = DateTime.Now,
            AgreementPaymentUntil = DateTime.Now.AddDays(20),
            AgreementCurrentSoftwareVersion = "1.0",
            AgreementEndOfVersionSupport = DateTime.Now.AddYears(2),
            ClientId = 1,
            SoftwareId = 1
        };

        await context.Softwares.AddAsync(software);
        await context.Individuals.AddAsync(individual);
        await context.Agreements.AddAsync(agreement);
        await context.SaveChangesAsync();
        
        // Act & Assert
        await Assert.ThrowsAsync<AmountHasBeenExceeded>(async () =>
        {
            await paymentService.AddPaymentAsync(model, 1, 1);
        });
    }
    
    [Fact]
    public async Task AddPaymentAsync_ShouldThrowNotFoundExceptionWhenAgreementNotFound()
    {
        // Arrange
        await using var context = CreateContext();
        var paymentService = new PaymentService(context, new HttpClientDummy());

        var model = new CreatePaymentModel
        {
            DepositSize = 400
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
            await paymentService.AddPaymentAsync(model, 1, 1);
        });
    }
    
    [Fact]
    public async Task AddPaymentAsync_ShouldThrowNotFoundExceptionWhenClientNotFound()
    {
        // Arrange
        await using var context = CreateContext();
        var paymentService = new PaymentService(context, new HttpClientDummy());

        var model = new CreatePaymentModel
        {
            DepositSize = 400
        };

        var agreement = new Agreement
        {
            AgreementId = 1,
            AgreementSigned = false,
            AgreementPrice = 1000,
            AgreementCurrentDeposited = 0,
            AgreementPaymentFrom = DateTime.Now,
            AgreementPaymentUntil = DateTime.Now.AddDays(20),
            AgreementCurrentSoftwareVersion = "1.0",
            AgreementEndOfVersionSupport = DateTime.Now.AddYears(2),
            ClientId = 1,
            SoftwareId = 1
        };

        await context.Agreements.AddAsync(agreement);
        await context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await paymentService.AddPaymentAsync(model, 1, 1);
        });
    }

    [Fact]
    public async Task AddPaymentAsync_ShouldThrowNotFoundExceptionWhenAgreementNotFoundForClient()
    {
        // Arrange
        await using var context = CreateContext();
        var paymentService = new PaymentService(context, new HttpClientDummy());

        var model = new CreatePaymentModel
        {
            DepositSize = 400
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
        
        var agreement = new Agreement
        {
            AgreementId = 1,
            AgreementSigned = false,
            AgreementPrice = 1000,
            AgreementCurrentDeposited = 0,
            AgreementPaymentFrom = DateTime.Now,
            AgreementPaymentUntil = DateTime.Now.AddDays(20),
            AgreementCurrentSoftwareVersion = "1.0",
            AgreementEndOfVersionSupport = DateTime.Now.AddYears(2),
            ClientId = 2,
            SoftwareId = 1
        };

        await context.Agreements.AddAsync(agreement);
        await context.Individuals.AddAsync(individual);
        await context.Softwares.AddAsync(software);
        await context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await paymentService.AddPaymentAsync(model, 1, 1);
        });
    }

    [Fact]
    public async Task AddPaymentAsync_ShouldThrowAgreementAlreadySigned()
    {
        // Arrange
        await using var context = CreateContext();
        var paymentService = new PaymentService(context, new HttpClientDummy());

        var model = new CreatePaymentModel
        {
            DepositSize = 400
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

        var agreement = new Agreement
        {
            AgreementId = 1,
            AgreementSigned = true,
            AgreementPrice = 1000,
            AgreementCurrentDeposited = 0,
            AgreementPaymentFrom = DateTime.Now,
            AgreementPaymentUntil = DateTime.Now.AddDays(20),
            AgreementCurrentSoftwareVersion = "1.0",
            AgreementEndOfVersionSupport = DateTime.Now.AddYears(2),
            ClientId = 1,
            SoftwareId = 1
        };
        
        await context.Softwares.AddAsync(software);
        await context.Individuals.AddAsync(individual);
        await context.Agreements.AddAsync(agreement);
        await context.SaveChangesAsync();
        
        // Act & Assert
        await Assert.ThrowsAsync<AgreementAlreadySigned>(async () =>
        {
            await paymentService.AddPaymentAsync(model, 1, 1);
        });
    }

    [Fact]
    public async Task GetTotalIncomeAsync_ShouldReturnTotalIncome()
    {
        // Arrange
        await using var context = CreateContext();
        var paymentService = new PaymentService(context, new HttpClientDummy());

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
        
        var agreement = new Agreement
        {
            AgreementId = 1,
            AgreementSigned = true,
            AgreementPrice = 1000,
            AgreementCurrentDeposited = 0,
            AgreementPaymentFrom = DateTime.Now,
            AgreementPaymentUntil = DateTime.Now.AddDays(20),
            AgreementCurrentSoftwareVersion = "1.0",
            AgreementEndOfVersionSupport = DateTime.Now.AddYears(2),
            ClientId = 1,
            SoftwareId = 1
        };
        
        await context.Softwares.AddAsync(software);
        await context.Individuals.AddAsync(individual);
        await context.Agreements.AddAsync(agreement);
        await context.SaveChangesAsync();
        
        // Act
        var result = await paymentService.GetTotalIncomeAsync("PLN");
        
        // Assert
        Assert.Equal(1000, result.ActualProfit);
        Assert.Equal(1000, result.ExpectedProfit);
    }

    [Fact]
    public async Task GetProductIncomeAsync_ShouldReturnProductIncome()
    {
        // Arrange
        await using var context = CreateContext();
        var paymentService = new PaymentService(context, new HttpClientDummy());

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

        var agreement = new Agreement
        {
            AgreementId = 1,
            AgreementSigned = true,
            AgreementPrice = 1000,
            AgreementCurrentDeposited = 0,
            AgreementPaymentFrom = DateTime.Now,
            AgreementPaymentUntil = DateTime.Now.AddDays(20),
            AgreementCurrentSoftwareVersion = "1.0",
            AgreementEndOfVersionSupport = DateTime.Now.AddYears(2),
            ClientId = 1,
            SoftwareId = 1
        };

        await context.Softwares.AddAsync(software);
        await context.Individuals.AddAsync(individual);
        await context.Agreements.AddAsync(agreement);
        await context.SaveChangesAsync();

        // Act
        var result = await paymentService.GetProductIncomeAsync(1, "PLN");

        // Assert
        Assert.Equal(1000, result.ActualProfit);
        Assert.Equal(1000, result.ExpectedProfit);
    }
    
    [Fact]
    public async Task GetProductIncomeAsync_ShouldThrowNotFoundException()
    {
        // Arrange
        await using var context = CreateContext();
        var paymentService = new PaymentService(context, new HttpClientDummy());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await paymentService.GetProductIncomeAsync(1, "PLN");
        });
    }
    
    [Fact]
    public async Task GetExchangeRateAsync_ShouldReturnNotFoundException()
    {
        // Arrange
        await using var context = CreateContext();
        var paymentService = new PaymentService(context, new HttpClientDummy());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await paymentService.GetExchangeRateAsync("PLN", "LICZENA5");
        });
    }
}