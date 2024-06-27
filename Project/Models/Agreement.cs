using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;

[Table("Agreement")]
public class Agreement
{
    [Key]
    [Column("Id")]
    public int AgreementId { get; set; }
    
    [Column("Price", TypeName = "decimal(15, 2)")]
    public double AgreementPrice { get; set; }
    
    [Column("CurrentDeposited", TypeName = "decimal(15, 2)")]
    public double AgreementCurrentDeposited { get; set; }
    
    [Column("PaymentFrom")]
    public DateTime AgreementPaymentFrom { get; set; }
    
    [Column("PaymentUntil")]
    public DateTime AgreementPaymentUntil { get; set; }
    
    [Column("Signed")]
    public bool AgreementSigned { get; set; }
    
    [MaxLength(50)]
    [Column("CurrentSoftwareVersion")]
    public string AgreementCurrentSoftwareVersion { get; set; }
    
    [Column("EndOfVersionSupport")]
    public DateTime AgreementEndOfVersionSupport { get; set; }
    
    [ForeignKey("Client")]
    [Column("ClientId")]
    public int ClientId { get; set; }
    
    [ForeignKey("Software")]
    [Column("SoftwareId")]
    public int SoftwareId { get; set; }
    
    public Client Client { get; set; }
    
    public Software Software { get; set; }
    
    public IEnumerable<Payment> Payments { get; set; }
}