using System.ComponentModel.DataAnnotations;

namespace Project.RequestModels;

public class CreateBusinessClientModel
{
    public string Name { get; set; }
    public string KRS { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
}