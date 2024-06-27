using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Project.Contexts;
using Project.Exceptions;
using Project.Models;
using Project.RequestModels;
using Project.Services;
using Project.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAgreementService, AgreementService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateIndividualClientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBusinessClientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateIndividualClientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateBusinessClientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAgreementValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePaymentValidator>();

builder.Services.AddDbContext<DatabaseContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("api/clients/individualClient", async (
    CreateIndividualClientModel data, IClientService service, IValidator<CreateIndividualClientModel> validator) =>
{
    var validate = await validator.ValidateAsync(data);
    if (!validate.IsValid)
    {
        return Results.ValidationProblem(validate.ToDictionary());
    }
    
    await service.AddIndividualClientAsync(data);
    return Results.Created();
});

app.MapPost("api/clients/businessClient", async (
    CreateBusinessClientModel data, IClientService service, IValidator<CreateBusinessClientModel> validator) =>
{
    var validate = await validator.ValidateAsync(data);
    if (!validate.IsValid)
    {
        return Results.ValidationProblem(validate.ToDictionary());
    }
    
    await service.AddBusinessClientAsync(data);
    return Results.Created();
});

app.MapDelete("api/clients/{id:int}", async (int id, IClientService service) =>
{
    try
    {
        await service.DeleteIndividualClientAsync(id);
    }
    catch (NotFoundException e)
    {
        return Results.NotFound(e.Message);
    }
    
    return Results.NoContent();
});

app.MapPut("api/clients/individualClient/{id:int}", async (int id,
    UpdateIndividualClientModel data, IClientService service, IValidator<UpdateIndividualClientModel> validator) =>
{
    var validate = await validator.ValidateAsync(data);
    if (!validate.IsValid)
    {
        return Results.ValidationProblem(validate.ToDictionary());
    }

    try
    {
        await service.UpdateIndividualClientAsync(id, data);
    }
    catch (Exception e)
    {
        return Results.NotFound(e.Message);
    }
    
    return Results.NoContent();
});

app.MapPut("api/clients/businessClient/{id:int}", async (int id,
    UpdateBusinessClientModel data, IClientService service, IValidator<UpdateBusinessClientModel> validator) =>
{
    var validate = await validator.ValidateAsync(data);
    if (!validate.IsValid)
    {
        return Results.ValidationProblem(validate.ToDictionary());
    }

    try
    {
        await service.UpdateBusinessClientAsync(id, data);
    }
    catch (Exception e)
    {
        return Results.NotFound(e.Message);
    }
    
    return Results.NoContent();
});

app.MapPost("api/clients/{clientId:int}/software/{softwareId:int}/agreement", async (
    int clientId, int softwareId, CreateAgreementModel data, IAgreementService service, 
    IValidator<CreateAgreementModel> validator) =>
{
    var validate = await validator.ValidateAsync(data);
    if (!validate.IsValid)
    {
        return Results.ValidationProblem(validate.ToDictionary());
    }
    
    try
    {
        var result = await service.AddAgreementAsync(data, clientId, softwareId);
        return Results.Created($"/{result}", result);
    }
    catch (NotFoundException e)
    {
        return Results.NotFound(e.Message);
    }
    catch (WrongSoftwareType e)
    {
        return Results.BadRequest(e.Message);
    }
});

app.MapPost("api/clients/{clientId:int}/agreement/{agreementId:int}/payment", async (
    int clientId, int agreementId, CreatePaymentModel data, IPaymentService service, 
    IValidator<CreatePaymentModel> validator) =>
{
    var validate = await validator.ValidateAsync(data);
    if (!validate.IsValid)
    {
        return Results.ValidationProblem(validate.ToDictionary());
    }
    
    try
    {
        var result = await service.AddPaymentAsync(data, clientId, agreementId);
        return Results.Created($"/{result}", result);
    }
    catch (NotFoundException e)
    {
        return Results.NotFound(e.Message);
    }
    catch (AgreementAlreadySigned e)
    {
        return Results.BadRequest(e.Message);
    }
    catch (AmountHasBeenExceeded e)
    {
        return Results.BadRequest(e.Message);
    }
});

app.MapGet("api/totalIncome/{currency}", async (string currency, IPaymentService service) =>
{
    try
    {
        return Results.Ok(await service.GetTotalIncomeAsync(currency));
    }
    catch (NotFoundException e)
    {
        return Results.NotFound(e.Message);
    }
});

app.MapGet("api/productIncome/{currency}/software/{softwareId:int}", async (string currency, int softwareId, IPaymentService service) =>
{
    try
    {
        return Results.Ok(await service.GetProductIncomeAsync(softwareId,currency));
    }
    catch (NotFoundException e)
    {
        return Results.NotFound(e.Message);
    }
});


app.Run();