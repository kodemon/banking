using Banking.Accounts.Interfaces;
using Banking.Accounts.Repositories.Resources;
using MediatR;

namespace Banking.Accounts.Queries;

internal record GetAccountsByOwnerIdQuery(Guid HolderId) : IRequest<List<Account>>;

internal class GetUserAccountsByOwnerIdHandler(IAccountRepository repository)
    : IRequestHandler<GetAccountsByOwnerIdQuery, List<Account>>
{
    public Task<List<Account>> Handle(GetAccountsByOwnerIdQuery message, CancellationToken token) =>
        repository.GetAllByHolderIdAsync(message.HolderId);
}
