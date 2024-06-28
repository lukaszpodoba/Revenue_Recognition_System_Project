using System.Text;
using FluentValidation;
using JWT.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Project.Contexts;
using Project.Exceptions;
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
builder.Services.AddScoped<IAuthServices, AuthService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateIndividualClientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBusinessClientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateIndividualClientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateBusinessClientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAgreementValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePaymentValidator>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "My API", 
        Version = "v1" 
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header, 
        Description = "Please insert \"Bearer {Token}\" into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey 
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { 
            new OpenApiSecurityScheme 
            { 
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" 
                } 
            },
            new string[] { } 
        } 
    });
});

builder.Services.AddDbContext<DatabaseContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});


builder.Services.AddAuthorizationBuilder()
    .AddPolicy("UserPolicy", policy => policy.RequireRole("User", "Admin"))
    .AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.MapPost("/api/login", async (LoginAndRegisterRequestModel model, IAuthServices authService) =>
{
    var result = await authService.LoginAsync(model);
    if (result == null)
    {
        return Results.Unauthorized();
    }
    return Results.Ok(result);
});

app.MapPost("/api/register/User", async (LoginAndRegisterRequestModel model, IAuthServices authService) =>
{
    var result = await authService.RegisterAsync(model, "User");
    if (!result)
    {
        return Results.BadRequest("User already exists.");
    }
    return Results.Ok("User registered successfully.");
});

app.MapPost("/api/register/Admin", async (LoginAndRegisterRequestModel model, IAuthServices authService) =>
{
    var result = await authService.RegisterAsync(model, "Admin");
    if (!result)
    {
        return Results.BadRequest("User already exists.");
    }
    return Results.Ok("User registered successfully.");
});

app.MapPost("/api/refreshToken", async (string refreshToken, IAuthServices authService) =>
{
    var result = await authService.RefreshTokenAsync(refreshToken);
    if (result == null)
    {
        return Results.Unauthorized();
    }
    return Results.Ok(result);
});

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
}).RequireAuthorization("UserPolicy");

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
}).RequireAuthorization("UserPolicy");

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
}).RequireAuthorization("AdminPolicy");

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
}).RequireAuthorization("AdminPolicy");

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
}).RequireAuthorization("AdminPolicy");

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
}).RequireAuthorization("UserPolicy");

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
}).RequireAuthorization("UserPolicy");

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
}).RequireAuthorization("UserPolicy");

app.MapGet("api/productIncome/{currency}/software/{softwareId:int}", 
    async (string currency, int softwareId, IPaymentService service) => 
{
    try 
    {
        return Results.Ok(await service.GetProductIncomeAsync(softwareId, currency));
    }
    catch (NotFoundException e) 
    { 
        return Results.NotFound(e.Message);
    }
}).RequireAuthorization("UserPolicy");


app.Run();