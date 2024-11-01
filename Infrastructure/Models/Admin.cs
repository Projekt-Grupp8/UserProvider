
namespace Infrastructure.Models;

public class Admin
{
    public string Email { get; set; } = null!;
    public List<string> Roles { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!; 
    public DateTime Created {  get; set; }
    public DateTime Updated { get; set; }
}
