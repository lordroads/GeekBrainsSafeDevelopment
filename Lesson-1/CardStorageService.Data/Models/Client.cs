using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardStorageService.Data.Models;

[Table("Clients")]
public class Client
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ClientId { get; set; }

    [Column, StringLength(255)]
    public string? Surname { get; set; }

    [Column, StringLength(255)]
    public string? FirstName { get; set; }

    [Column, StringLength(255)]
    public string? Patronymic { get; set; }

    [InverseProperty(nameof(Card.Client))]
    public ICollection<Card> Cards { get; set; } = new HashSet<Card>();
}