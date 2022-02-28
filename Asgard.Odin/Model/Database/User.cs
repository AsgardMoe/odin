using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Asgard.Odin.Model.Database;

[Comment("Users auth table.")]
public class User
{
    [Key]
    [Required]
    [Comment("Users unique id.")]
    public long Id { get; set; }    // qyl27: Use snowflake algorithm.

    [Required]
    [RegularExpression("[a-zA-Z0-9_]*")]
    [MinLength(6)]
    [MaxLength(30)]
    [Comment("Users name, which is editable.")]
    public string Username { get; set; }
    
    [Required]
    [EmailAddress]
    [Comment("Users bind email.")]
    public string Email { get; set; }
    
    [Required]
    [Comment("Users password, use BCrypt.")]
    public string Password { get; set; }
}
