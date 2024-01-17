﻿using Microsoft.EntityFrameworkCore;
using Money.Domain.Entities;
using Money.Domain.Repositories;
using Money.Domain.Responses;

namespace Money.Infrastructure.Repositories;

public class TransactionRepository(MoneyContext context) : ITransactionRepository
{
    private readonly MoneyContext _context = context;

    public async Task Save(Transaction transaction)
    {
        await _context.Transaction.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task Save(List<Transaction> transactions)
    {
        await _context.Transaction.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();
    }

    public async Task<List<StatementTransactionResponse>> GetStatementTransactions(Guid participantId, int page, int pageSize)
    {
        var statement = await _context.Transaction
            .Include(e => e.Transfer)
            .ThenInclude(e => e!.FromParticipant)
            .Include(e => e.Transfer)
            .ThenInclude(e => e!.ToParticipant)
            .OrderByDescending(e => e.CreatedAt)
            .Where(e => e.ParticipantId == participantId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new StatementTransactionResponse(
                e.CreatedAt, 
                e.Type, 
                e.Amount, 
                e.Transfer!.FromParticipantId,
                e.Transfer.FromParticipant.Name,
                e.Transfer.ToParticipantId,
                e.Transfer.ToParticipant.Name))
            .ToListAsync();

        return statement;
    }
}
