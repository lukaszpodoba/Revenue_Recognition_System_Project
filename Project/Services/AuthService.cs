using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JWT.Helpers;
using JWT.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project.Contexts;
using Project.Models;
using Project.RequestModels;
using Project.ResponseModels;

namespace JWT.Services;

public interface IAuthServices
{
    Task<LoginResponseModel> LoginAsync(LoginAndRegisterRequestModel model);
    Task<bool> RegisterAsync(LoginAndRegisterRequestModel model, string userRole);
    Task<LoginResponseModel> RefreshTokenAsync(string refreshToken);
}

public class AuthService : IAuthServices
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _config;

    public AuthService(DatabaseContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }
    
    public async Task<LoginResponseModel> LoginAsync(LoginAndRegisterRequestModel model)
    {
        var user = await _context.AppUserModels.FirstOrDefaultAsync(u => u.Login == model.UserName);
        if (user is null || !SecurityHelpers.CheckIfPasswordIsCorrect(user.PasswordHash, model.Password, user.Salt))
        {
            return null;
        }
        
        var token = SecurityHelpers.GenerateJwtToken(user, _config);
        var refreshToken = SecurityHelpers.GenerateRefreshToken();
            
        _context.RefreshTokens.Add(new RefreshToken
        {
            RefreshTokenToken = refreshToken,
            RefreshTokenExpiryDate = DateTime.Now.AddDays(3),
            UserId = user.IdUser
        });
        await _context.SaveChangesAsync();

        return new LoginResponseModel
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }
    
    public async Task<bool> RegisterAsync(LoginAndRegisterRequestModel model, string userRole)
    {
        if (await _context.AppUserModels.AnyAsync(u => u.Login == model.UserName))
            return false;

        var (hashedPassword, salt) = SecurityHelpers.GetHashedPasswordAndSalt(model.Password);

        var user = new AppUserModel
        {
            Login = model.UserName,
            PasswordHash = hashedPassword,
            Salt = salt,
            Role = userRole
        };

        _context.AppUserModels.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<LoginResponseModel> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _context.RefreshTokens.Include(rt => rt.AppUserModel)
            .SingleOrDefaultAsync(rt => rt.RefreshTokenToken == refreshToken);
        
        if (storedToken == null || storedToken.RefreshTokenExpiryDate <= DateTime.Now) return null;
        
        var user = storedToken.AppUserModel;
        var newJwtToken = SecurityHelpers.GenerateJwtToken(user, _config);
        var newRefreshToken = SecurityHelpers.GenerateRefreshToken();

        storedToken.RefreshTokenToken = newRefreshToken;
        storedToken.RefreshTokenExpiryDate = DateTime.Now.AddDays(3);
        _context.RefreshTokens.Update(storedToken);
        await _context.SaveChangesAsync();

        return new LoginResponseModel
        {
            Token = newJwtToken,
            RefreshToken = newRefreshToken
        };

    }
}