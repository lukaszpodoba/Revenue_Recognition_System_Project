using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;

[Table("AppUser")]
public class AppUserModel
{
    [Key] 
    [Column("IdUser")]
    public int IdUser { get; set; }
    
    [MaxLength(100)]
    [Column("Login")]
    public string Login { get; set; }
    
    [Column("PasswordHash")]
    public string PasswordHash { get; set; }

    [Column("Salt")]
    public string Salt { get; set; }
    
    [MaxLength(100)]
    [Column("Role")]
    public string Role { get; set; }
}