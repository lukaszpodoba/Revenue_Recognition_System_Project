using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;

[Table("Subscription")]
public class Subscription
{
    [Key]
    [Column("Id")]
    public int SubscriptionId { get; set; }
    
    [Column("Price", TypeName = "decimal(15, 2)")]
    public double SubscriptionPrice { get; set; }
    
    [Column("From")]
    public DateTime SubscriptionFrom { get; set; }
    
    [Column("Until")]
    public DateTime SubscriptionUntil { get; set; }
    
    [Column("CurrentSoftwareVersion")]
    public string SubscriptionCurrentSoftwareVersion { get; set; }
    
    [ForeignKey("Client")]
    [Column("ClientId")]
    public int ClientId { get; set; }
    
    [ForeignKey("Software")]
    [Column("SoftwareId")]
    public int SoftwareId { get; set; }
    
    public Client Client { get; set; }
    
    public Software Software { get; set; }
}