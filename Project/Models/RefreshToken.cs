using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Models;

namespace JWT.Models;

[Table("RefreshToken")]
public class RefreshToken
{
    [Key]
    [Column("Id")]
    public int RefreshTokenId { get; set; }

    [Column("Token")]
    public string RefreshTokenToken { get; set; }

    [Column("ExpiryDate")]
    public DateTime RefreshTokenExpiryDate { get; set; }

    [ForeignKey("AppUserModel")]
    [Column("FK_UserId")]
    public int UserId { get; set; }
    
    public AppUserModel AppUserModel { get; set; }
}