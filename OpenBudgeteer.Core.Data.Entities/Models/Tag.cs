using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace OpenBudgeteer.Core.Data.Entities.Models;

[Index(nameof(Name), IsUnique = true)]
public class Tag : IEntity
{
    // Bounded so that MySQL/MariaDB can index the column (no unique index on longtext)
    public const int NameMaxLength = 100;

    [Key, Column("TagId")]
    public Guid Id { get; set; }

    [Required, MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public ICollection<TransactionTag>? TransactionTags { get; set; }
}
