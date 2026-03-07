namespace Banking.Principals;

internal interface IPrincipalIdentityRepository
{
    Task<bool> HasIdentityAsync(string provider, string externalId);
}
