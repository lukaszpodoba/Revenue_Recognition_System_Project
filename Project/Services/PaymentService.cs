using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Project.Contexts;
using Project.Exceptions;
using Project.Models;
using Project.RequestModels;
using Project.ResponseModels;

namespace Project.Services;

public interface IPaymentService
{
    Task<GetCreatedPayment> AddPaymentAsync(CreatePaymentModel data, int clientId, int agreementId);
    Task<GetIncomeModel> GetTotalIncomeAsync(string currency);
    Task<GetIncomeModel> GetProductIncomeAsync(int productId, string currency);
}

public class PaymentService(DatabaseContext context, HttpClient httpClient) : IPaymentService
{
    public async Task<GetCreatedPayment> AddPaymentAsync(CreatePaymentModel data, int clientId, int agreementId)
    {
        var client = await context.Clients
            .Where(e => e.ClientId == clientId)
            .FirstOrDefaultAsync();
        
        if (client is null)
        {
            throw new NotFoundException($"Client with given id {clientId} not found");
        }
        
        var agreement = await context.Agreements
            .Where(e => e.AgreementId == agreementId)
            .FirstOrDefaultAsync();

        if (agreement is null)
        {
            throw new NotFoundException($"Agreement with given id {agreementId} not found");
        }

        if (agreement.ClientId != clientId)
        {
            throw new NotFoundException($"Agreement with given id {agreementId} not found for client with id {clientId}");
        }

        if (agreement.AgreementSigned)
        {
            throw new AgreementAlreadySigned($"Agreement with given id {agreementId} is already signed");
        }
        
        var payment = new Payment
        {
            PaymentPrice = data.DepositSize,
            PaymentDate = DateTime.Now,
            AgreementId = agreementId,
            ClientId = clientId
        };

        await context.Payments.AddAsync(payment);
        
        var currentDeposited = agreement.AgreementCurrentDeposited += data.DepositSize;

        if (currentDeposited > agreement.AgreementPrice)
        {
            var exceededValue = currentDeposited - agreement.AgreementPrice;
            throw new AmountHasBeenExceeded($"Amount has been exceeded by {exceededValue} for agreement with id {agreementId}");
        }

        if (currentDeposited == agreement.AgreementPrice)
        {
            agreement.AgreementSigned = true;
        }
        
        await context.SaveChangesAsync();

        return new GetCreatedPayment
        {
            Id = payment.PaymentId,
            DepositSize = payment.PaymentPrice,
            PaymentDate = payment.PaymentDate,
            ClientId = payment.ClientId,
            AgreementId = payment.AgreementId,
            AlreadyPaid = agreement.AgreementCurrentDeposited,
            AgreementPrice = agreement.AgreementPrice
        };
    }

    public async Task<GetIncomeModel> GetTotalIncomeAsync(string currency)
    {
        var actualProfit = context.Agreements
            .Where(e => e.AgreementSigned)
            .Sum(e => e.AgreementPrice);

        var unsignedProfit = context.Agreements
            .Where(e => !e.AgreementSigned && e.AgreementPaymentUntil >= DateTime.Now)
            .Sum(e => e.AgreementPrice);
        
        var expectedProfit = actualProfit + unsignedProfit;

        var exchangeRate = await GetExchangeRateAsync("PLN", currency);
        
        return new GetIncomeModel
        {
            ActualProfit = Math.Round(actualProfit * exchangeRate, 2),
            ExpectedProfit = Math.Round(expectedProfit * exchangeRate, 2)
        };
    }

    public async Task<GetIncomeModel> GetProductIncomeAsync(int productId, string currency)
    {
        var software = await context.Softwares
            .Where(e => e.SoftwareId == productId)
            .FirstOrDefaultAsync();

        if (software is null)
        {
            throw new NotFoundException($"Software with given id {productId} not found");
        }
        
        var actualProfit = context.Agreements
            .Where(e => e.AgreementSigned && e.SoftwareId == productId)
            .Sum(e => e.AgreementPrice);

        var unsignedProfit = context.Agreements
            .Where(e => !e.AgreementSigned && e.AgreementPaymentUntil >= DateTime.Now && e.SoftwareId == productId)
            .Sum(e => e.AgreementPrice);
        
        var expectedProfit = actualProfit + unsignedProfit;

        var exchangeRate = await GetExchangeRateAsync("PLN", currency);
        
        return new GetIncomeModel
        {
            ActualProfit = Math.Round(actualProfit * exchangeRate, 2),
            ExpectedProfit = Math.Round(expectedProfit * exchangeRate, 2)
        };
    }

    public async Task<double> GetExchangeRateAsync(string currency, string toCurrency)
    {
        var apiKey = "28e3e10930b060865b80f1b6";
        var response = await httpClient.GetStringAsync($"https://v6.exchangerate-api.com/v6/{apiKey}/latest/{currency}");
        var rates = JObject.Parse(response)["conversion_rates"];
        if (rates.Value<double>(toCurrency) == 0)
        {
            throw new NotFoundException("Currency not found");
        }
        return rates.Value<double>(toCurrency);
    }
}