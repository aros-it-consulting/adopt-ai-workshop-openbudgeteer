using OpenBudgeteer.Core.Data.Contracts.Repositories;
using OpenBudgeteer.Core.Data.Contracts.Services;
using OpenBudgeteer.Core.Data.Entities.Models;
using OpenBudgeteer.Core.Data.Services.Exceptions;

namespace OpenBudgeteer.Core.Data.Services.Generic;

public class GenericBankTransactionService : GenericBaseService<BankTransaction>, IBankTransactionService
{
    private readonly IBankTransactionRepository _bankTransactionRepository;
    private readonly IBudgetedTransactionRepository _budgetedTransactionRepository;
    private readonly ITransactionTagRepository _transactionTagRepository;
    private readonly GenericTagService _tagService;
    
    public GenericBankTransactionService(
        IBankTransactionRepository bankTransactionRepository, 
        IBudgetedTransactionRepository budgetedTransactionRepository,
        ITransactionTagRepository transactionTagRepository,
        GenericTagService tagService) : base(bankTransactionRepository)
    {
        _bankTransactionRepository = bankTransactionRepository;
        _budgetedTransactionRepository = budgetedTransactionRepository;
        _transactionTagRepository = transactionTagRepository;
        _tagService = tagService;
    }

    public BankTransaction GetWithEntities(Guid id)
    {
        var result = _bankTransactionRepository.ByIdWithIncludedEntities(id);
        if (result is null) throw new EntityNotFoundException("Unable to find Bank Transaction with the given id.");
        return result;
    }
    
    public IEnumerable<BankTransaction> GetAll(DateOnly? periodStart, DateOnly? periodEnd, int limit = 0)
    {
        var result = _bankTransactionRepository
            .AllWithIncludedEntities()
            .Where(i =>
                i.TransactionDate >= (periodStart ?? DateOnly.MinValue) &&
                i.TransactionDate <= (periodEnd ?? DateOnly.MaxValue))
            .OrderByDescending(i => i.TransactionDate)
            .ToList();
        return limit > 0
            ? result.Take(limit)
            : result;
    }
    
    public IEnumerable<BankTransaction> GetFromAccount(Guid accountId, int limit = 0)
    {
        return GetFromAccount(accountId, null, null, limit);
    }
    
    public IEnumerable<BankTransaction> GetFromAccount(Guid accountId, DateOnly? periodStart, DateOnly? periodEnd, int limit = 0)
    {
        var result = _bankTransactionRepository
            .AllWithIncludedEntities()
            .Where(i =>
                i.TransactionDate >= (periodStart ?? DateOnly.MinValue) &&
                i.TransactionDate <= (periodEnd ?? DateOnly.MaxValue) &&
                i.AccountId == accountId)
            .OrderByDescending(i => i.TransactionDate)
            .ToList();
        return limit > 0
            ? result.Take(limit)
            : result;
    }
    
    public IEnumerable<BankTransaction> ImportTransactions(IEnumerable<BankTransaction> entities)
    {
        var newTransactions = entities.ToList();
        _bankTransactionRepository.CreateRange(newTransactions);
        return newTransactions;
    }

    public override BankTransaction Create(BankTransaction entity)
    {
        entity.TransactionTags = BuildTransactionTags(entity);
        return base.Create(entity);
    }

    public override BankTransaction Update(BankTransaction entity)
    {
        // Without passed Tags (e.g. an API call), the Tags of the Transaction are kept as they are
        if (entity.TransactionTags is not null) ReplaceTransactionTags(entity);

        if (entity.BudgetedTransactions is not null && entity.BudgetedTransactions.Any())
        {
            // Delete all existing bucket assignments, as they will be replaced by passed assignments
            var deletedIds =
                _budgetedTransactionRepository.All()
                    .Where(i => i.TransactionId == entity.Id)
                    .Select(i => i.Id)
                    .ToList();
                        
            if (deletedIds.Count != 0)
            {
                var result = _budgetedTransactionRepository.DeleteRange(deletedIds);
                if (result != deletedIds.Count) 
                    throw new EntityUpdateException("Unable to delete old Bucket Assignments of that Transaction");
            }
                
            // Ensure that all BudgetedTransaction Guids of incoming entity are empty to enable their (re)creation
            foreach (var budgetedTransaction in entity.BudgetedTransactions)
            {
                budgetedTransaction.Id = Guid.Empty;
            }
        }
            
        // Update BankTransaction including bucket assignments (if available) in DB
        _bankTransactionRepository.Update(entity);
            
        return entity;
    }

    private void ReplaceTransactionTags(BankTransaction entity)
    {
        // Resolve Tags before deleting anything, so that an invalid Tag name keeps the current Tags
        var transactionTags = BuildTransactionTags(entity);
        var deletedIds = _transactionTagRepository.All()
            .Where(transactionTag => transactionTag.TransactionId == entity.Id)
            .Select(transactionTag => transactionTag.Id)
            .ToList();
        if (deletedIds.Count != 0 && _transactionTagRepository.DeleteRange(deletedIds) != deletedIds.Count)
            throw new EntityUpdateException("Unable to delete old Tags of that Transaction");

        entity.TransactionTags = transactionTags;
    }

    /// <summary>
    /// Tags are passed by name, so that a Transaction can be tagged with a Tag which doesn't exist yet
    /// </summary>
    private List<TransactionTag>? BuildTransactionTags(BankTransaction entity)
    {
        if (entity.TransactionTags is null) return null;

        var tagNames = entity.TransactionTags.Select(transactionTag => transactionTag.Tag?.Name);
        return _tagService.GetOrCreate(tagNames)
            .Select(tag => new TransactionTag { TransactionId = entity.Id, TagId = tag.Id })
            .ToList();
    }

    public override void Delete(Guid id)
    {
        var result = _bankTransactionRepository.Delete(id);
        if (result == 0) throw new EntityUpdateException("Unable to delete Bank Transaction");
    }
}