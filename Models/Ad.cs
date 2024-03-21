namespace AdsApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Ad
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public string OwnerId { get; set; }

    [Required]
    [Column(TypeName = "varchar(65)")]
    public string Title { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Description { get; set; }

    [Required]
    public string ImagePath { get; set; }
}