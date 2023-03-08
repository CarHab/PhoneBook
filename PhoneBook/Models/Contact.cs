using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneBook.Models;
public class Contact
{
    [Required]
    public int Id { get; set; }

    [Required]
    [Column(TypeName="varchar(200)")]
    public string Name { get; set; }

    [Required]
    [Column(TypeName = "varchar(50)")]
    public string PhoneNumber { get; set; }
}
