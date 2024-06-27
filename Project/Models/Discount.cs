using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;

[Table("Discount")]
public class Discount
{
    [Key]
    [Column("Id")]
    public int DiscountId { get; set; }
    
    [MaxLength(50)]
    [Column("Name")]
    public string DiscountName { get; set; }
    
    [Column("PercentageValue", TypeName = "decimal(5, 2)")]
    public double DiscountPercentageValue { get; set; }
    
    [Column("DiscountFrom")]
    public DateTime DiscountFrom { get; set; }
    
    [Column("DiscountUntil")]
    public DateTime DiscountUntil { get; set; }

    [MaxLength(50)]
    [Column("Type")]
    public string DiscountType { get; set; }
    
    [ForeignKey("Software")]
    [Column("SoftwareId")]
    public int SoftwareId { get; set; }
    
    public Software Software { get; set; }
}