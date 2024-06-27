using Microsoft.EntityFrameworkCore;
using Project.Contexts;
using Project.Exceptions;
using Project.Models;
using Project.RequestModels;
using Project.ResponseModels;

namespace Project.Services;

public interface IAgreementService
{
    Task<GetCreatedAgreement> AddAgreementAsync(CreateAgreementModel data, int clientId, int softwareId);
    Task<double> GetSoftwareDiscountAsync(int id);
}

public class AgreementService(DatabaseContext context) : IAgreementService
{
    public async Task<GetCreatedAgreement> AddAgreementAsync(CreateAgreementModel data, int clientId, int softwareId)
    {
        var client = await context.Clients
            .Where(e => e.ClientId == clientId)
            .FirstOrDefaultAsync();
        
        if (client is null)
        {
            throw new NotFoundException($"Client with given id {clientId} not found");
        }
        
        var software = await context.Softwares
            .Where(e => e.SoftwareId == softwareId)
            .FirstOrDefaultAsync();

        if (software is null)
        {
            throw new NotFoundException($"Software with given id {softwareId} not found");
        }

        if (!software.SoftwareIsOneTimePurchase)
        {
            throw new WrongSoftwareType($"Software with given id {softwareId} is not one-time purchase software");
        }
        
        var discount = await GetSoftwareDiscountAsync(softwareId);

        var isReturningClient = await context.Clients
            .Where(e => e.ClientId == clientId)
            .Select(e => e.ClientIsReturning).FirstOrDefaultAsync();

        discount += isReturningClient ? 5.0 : 0.0;

        var supportPrice = 0;
        for (var i = 0; i < data.YearsOfVersionSupport; i++)
        {
            if (i != 0)
            {
                supportPrice += 1000;
            }
        }
        
        var finalPrice = (double)(software.SoftwareOneTimePrice + supportPrice)!;
        finalPrice -= finalPrice * discount / 100;
        
        var agreement = new Agreement
        {
            AgreementPrice = finalPrice,
            AgreementCurrentDeposited = 0,
            AgreementPaymentFrom = data.PaymentFrom,
            AgreementPaymentUntil = data.PaymentUntil,
            AgreementSigned = false,
            AgreementCurrentSoftwareVersion = software.SoftwareCurrentVersion,
            AgreementEndOfVersionSupport = data.PaymentFrom.AddYears(data.YearsOfVersionSupport),
            ClientId = clientId,
            SoftwareId = softwareId
        };
        
        await context.Agreements.AddAsync(agreement);
        await context.SaveChangesAsync();

        return new GetCreatedAgreement
        {
            Id = agreement.AgreementId,
            Price = agreement.AgreementPrice,
            CurrentDeposited = agreement.AgreementCurrentDeposited,
            PaymentFrom = agreement.AgreementPaymentFrom,
            PaymentUntil = agreement.AgreementPaymentUntil,
            Signed = agreement.AgreementSigned,
            CurrentSoftwareVersion = agreement.AgreementCurrentSoftwareVersion,
            EndOfVersionSupport = agreement.AgreementEndOfVersionSupport,
            ClientId = clientId,
            SoftwareId = softwareId
        };
    }

    public async Task<double> GetSoftwareDiscountAsync(int id)
    {
        var discounts = await context.Discounts
            .Where(e => e.SoftwareId == id 
                        && e.DiscountFrom <= DateTime.Now 
                        && e.DiscountUntil >= DateTime.Now 
                        && e.DiscountType == "Agreement").ToListAsync();

        if (discounts.Count == 0)
        {
            return 0;
        }
        
        var maxDiscount = discounts.Max(e => e.DiscountPercentageValue);

        return maxDiscount;
    }
}