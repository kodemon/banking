using Banking.Domain.Exceptions;

namespace Banking.Domain.ValueObjects;

public record Address
{
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }
    public string? Region { get; }

    public Address(string street, string city, string postalCode, string country, string? region = null)
    {
        Validate(street, city, postalCode, country);
        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
        Region = region;
    }

    public Address WithStreet(string street) => new(street, City, PostalCode, Country, Region);
    public Address WithCity(string city) => new(Street, city, PostalCode, Country, Region);
    public Address WithPostalCode(string postalCode) => new(Street, City, postalCode, Country, Region);
    public Address WithCountry(string country) => new(Street, City, PostalCode, country, Region);
    public Address WithRegion(string? region) => new(Street, City, PostalCode, Country, region);

    private static void Validate(string street, string city, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            throw new DomainValidationException("Street cannot be empty");
        }
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new DomainValidationException("City cannot be empty");
        }
        if (string.IsNullOrWhiteSpace(postalCode))
        {
            throw new DomainValidationException("Postal code cannot be empty");
        }
        if (string.IsNullOrWhiteSpace(country))
        {
            throw new DomainValidationException("Country cannot be empty");
        }
    }
}