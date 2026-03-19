using Banking.Accounts.Interfaces;
using Banking.Accounts.Repositories.Resources;
using MediatR;

namespace Banking.Accounts.Queries;

internal record GetAccountByIdQuery(Guid Id) : IRequest<Account?>;

internal class GetAccountByIdQueryHandler(IAccountRepository repository)
    : IRequestHandler<GetAccountByIdQuery, Account?>
{
    public Task<Account?> Handle(GetAccountByIdQuery message, CancellationToken token) =>
        repository.GetByIdAsync(message.Id);
}
