using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenBudgeteer.Core.Data.Contracts.Services;
using OpenBudgeteer.Core.Data.Entities;
using OpenBudgeteer.Core.Data.Entities.Models;
using OpenBudgeteer.Core.Data.Repository;
using OpenBudgeteer.Core.Data.Services.Exceptions;
using OpenBudgeteer.Core.Data.Services.Generic;

namespace OpenBudgeteer.Core.Data.Services.EFCore;

public class EFCoreBankTransactionService : EFCoreBaseService<BankTransaction>, IBankTransactionService
{
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;
    private readonly ILogger<EFCoreBankTransactionService> _logger;

    public EFCoreBankTransactionService(
        IDbContextFactory<DatabaseContext> dbContextFactory, 
        ILogger<EFCoreBankTransactionService> logger) : base(dbContextFactory, logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    protected override GenericBankTransactionService CreateBaseService(DatabaseContext dbContext)
    {
        return new GenericBankTransactionService(
            new BankTransactionRepository(dbContext),
            new BudgetedTransactionRepository(dbContext),
            new TransactionTagRepository(dbContext),
            new GenericTagService(new TagRepository(dbContext)));
    }

    public override BankTransaction Create(BankTransaction entity)
    {
        // New Tags are created before the Transaction: they must not stay behind if the Transaction is not created
        using var dbContext = _dbContextFactory.CreateDbContext();
        using var transaction = dbContext.Database.BeginTransaction();
        var genericBankTransactionService = CreateBaseService(dbContext);
        try
        {
            var result = genericBankTransactionService.Create(entity);
            transaction.Commit();
            return result;
        }
        catch (EntityUpdateException e)
        {
            transaction.Rollback();
            throw new ServiceException($"Unable to create {nameof(BankTransaction)} in database: {e.Message}", _logger);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            _logger.LogError(e, "Error during database update.");
            throw;
        }
    }

    public override BankTransaction Update(BankTransaction entity)
    {
        // Several writes replace the Tags and Bucket assignments: all or none of them must be applied
        using var dbContext = _dbContextFactory.CreateDbContext();
        using var transaction = dbContext.Database.BeginTransaction();
        var genericBankTransactionService = CreateBaseService(dbContext);
        try
        {
            var result = genericBankTransactionService.Update(entity);
            transaction.Commit();
            return result;
        }
        catch (EntityUpdateException e)
        {
            transaction.Rollback();
            throw new ServiceException($"Unable to update {nameof(BankTransaction)} in database: {e.Message}", _logger);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            _logger.LogError(e, "Error during database update.");
            throw;
        }
    }

    public BankTransaction GetWithEntities(Guid id)
    {
        try
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var genericBankTransactionService = CreateBaseService(dbContext);
            return genericBankTransactionService.GetWithEntities(id);
        }
        catch (EntityNotFoundException e)
        {
            throw new ServiceException($"Error on querying database: {e.Message}", _logger);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error on querying database.");
            throw;
        }
    }

    public IEnumerable<BankTransaction> GetAll(DateOnly? periodStart, DateOnly? periodEnd, int limit = 0)
    {
        try
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var genericBankTransactionService = CreateBaseService(dbContext);
            return genericBankTransactionService.GetAll(periodStart, periodEnd, limit);
        }
        catch (EntityNotFoundException e)
        {
            throw new ServiceException($"Error on querying database: {e.Message}", _logger);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error on querying database.");
            throw;
        }
    }

    public IEnumerable<BankTransaction> GetFromAccount(Guid accountId, int limit = 0)
    {
        return GetFromAccount(accountId, null, null, limit);
    }

    public IEnumerable<BankTransaction> GetFromAccount(Guid accountId, DateOnly? periodStart, DateOnly? periodEnd, int limit = 0)
    {
        try
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var genericBankTransactionService = CreateBaseService(dbContext);
            return genericBankTransactionService.GetFromAccount(accountId, periodStart, periodEnd, limit);
        }
        catch (EntityNotFoundException e)
        {
            throw new ServiceException($"Error on querying database: {e.Message}", _logger);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error on querying database.");
            throw;
        }
    }

    public IEnumerable<BankTransaction> ImportTransactions(IEnumerable<BankTransaction> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        using var transaction = dbContext.Database.BeginTransaction();
        var genericBankTransactionService = CreateBaseService(dbContext);
        try
        {
            var results = genericBankTransactionService.ImportTransactions(entities);
            transaction.Commit();
            return results;
        }
        catch (EntityUpdateException e)
        {
            transaction.Rollback();
            throw new ServiceException($"Unable to import Transactions: {e.Message}", _logger);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            _logger.LogError(e, "Error during database update.");
            throw;
        }
    }
}