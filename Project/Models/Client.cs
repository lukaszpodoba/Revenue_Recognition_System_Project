using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;

[Table("Client")]
public class Client
{
    [Key]
    [Column("Id")]
    public int ClientId { get; set; }
    
    [MaxLength(50)]
    [Column("Email")]
    public string ClientEmail { get; set; }

    [MaxLength(50)]
    [Column("Address")]
    public string ClientAdress { get; set; }
    
    [MaxLength(9)]
    [Column("Phone")]
    public string ClientPhone { get; set; }

    [Column("Returning")]
    public bool ClientIsReturning { get; set; }
    
    IEnumerable<Agreement> Agreements { get; set; }
    
    public IEnumerable<Payment> Payments { get; set; }
}