using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace OpenBudgeteer.Core.Data.Entities.Models;

[Index(nameof(TransactionId), nameof(TagId), IsUnique = true)]
public class TransactionTag : IEntity
{
    [Key, Column("TransactionTagId")]
    public Guid Id { get; set; }

    [Required]
    public Guid TransactionId { get; set; }

    [JsonIgnore]
    public BankTransaction Transaction { get; set; } = null!;

    [Required]
    public Guid TagId { get; set; }

    public Tag? Tag { get; set; }
}
