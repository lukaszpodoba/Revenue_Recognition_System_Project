using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Project.Models;

[Table("Business")]
public class Business : Client
{
    [MaxLength(50)]
    [Column("Name")]
    public string BusinessName { get; set; }

    [MaxLength(14)]
    [Column("KRS")]
    public string KRS { get; set; }
}