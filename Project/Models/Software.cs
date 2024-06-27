using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;

[Table("Software")]
public class Software
{
    [Key]
    [Column("Id")]
    public int SoftwareId { get; set; }
    
    [MaxLength(50)]
    [Column("Name")]
    public string SoftwareName { get; set; }
    
    [MaxLength(200)]
    [Column("Description")]
    public string SoftwareDescription { get; set; }
    
    [MaxLength(50)]
    [Column("Category")]
    public string SoftwareCategory { get; set; }
    
    [MaxLength(50)]
    [Column("CurrentVersion")]
    public string SoftwareCurrentVersion { get; set; }
    
    [Column("OneTimePrice", TypeName = "decimal(15, 2)")]
    public double? SoftwareOneTimePrice { get; set; }
    
    [Column("SubscriptionPrice", TypeName = "decimal(15, 2)")]
    public double? SoftwareSubscriptionPrice { get; set; }
    
    [Column("IsOneTimePurchase")]
    public bool SoftwareIsOneTimePurchase { get; set; }
    
    [Column("IsSubscriptionPurchase")]
    public bool SoftwareIsSubscriptionPurchase { get; set; }
}