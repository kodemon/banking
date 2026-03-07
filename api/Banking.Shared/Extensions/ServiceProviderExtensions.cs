using Microsoft.Extensions.DependencyInjection;

namespace Banking.Shared.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task RunInScope<TService>(
        this IServiceProvider sp,
        Func<TService, Task> action
    )
        where TService : notnull
    {
        await using var scope = sp.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<TService>();
        await action(service);
    }
}
