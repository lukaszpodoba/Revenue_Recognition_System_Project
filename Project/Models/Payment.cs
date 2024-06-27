using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;

[Table("Payment")]
public class Payment
{
    [Key]
    [Column("Id")]
    public int PaymentId { get; set; }
    
    [Column("Price", TypeName = "decimal(15, 2)")]
    public double PaymentPrice { get; set; }
    
    [Column("Date")]
    public DateTime PaymentDate { get; set; }
    
    [ForeignKey("Client")]
    [Column("ClientId")]
    public int ClientId { get; set; }
    
    [ForeignKey("Agreement")]
    [Column("AgreementId")]
    public int AgreementId { get; set; }
    
    public Client Client { get; set; }
    
    public Agreement Agreement { get; set; }
}