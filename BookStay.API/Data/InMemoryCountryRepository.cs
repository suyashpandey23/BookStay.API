namespace BookStay.API.Data;

public class InMemoryCountryRepository : ICountryRepository {
    private static List<Country> countries = new List<Country> {
        new Country {
            Id = 1, Name = "United States", Code = "US", Continent = "North America"
        },
        new Country {
            Id = 2, Name = "United Kingdom", Code = "UK", Continent = "Europe"
        },
        new Country {
            Id = 3, Name = "Japan", Code = "JP", Continent = "Asia"
        },
        new Country {
            Id = 4, Name = "Australia", Code = "AU", Continent = "Oceania"
        },
        new Country {
            Id = 5, Name = "Germany", Code = "DE", Continent = "Europe"
        }
    };

    public IEnumerable<Country> GetAll() {
        return countries;
    }

    public Country? GetById(int id) {
        return countries.FirstOrDefault(c => c.Id == id);
    }

    public Country Add(Country country) {
        countries.Add(country);
        return country;
    }

    public Country? Update(int id, Country country) {
        var existingCountry = countries.FirstOrDefault(c => c.Id == id);
        if (existingCountry == null) {
            return null;
        }

        existingCountry.Name = country.Name;
        existingCountry.Code = country.Code;
        existingCountry.Continent = country.Continent;

        return existingCountry;
    }

    public bool Delete(int id) {
        var country = countries.FirstOrDefault(c => c.Id == id);
        if (country == null) {
            return false;
        }

        countries.Remove(country);
        return true;
    }
}
