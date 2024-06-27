using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;

[Table("Individual")]
public class Individual : Client
{
    [MaxLength(50)]
    [Column("FirstName")]
    public string IndividualFirstName { get; set; }

    [MaxLength(50)]
    [Column("LastName")]
    public string IndividualLastName { get; set; }

    [MaxLength(11)]
    [Column("PESEL")]
    public string IndividualPesel { get; set; }
    
    [Column("DeletedAt")]
    public DateTime? IndividualDeletedAt { get; set; }
}