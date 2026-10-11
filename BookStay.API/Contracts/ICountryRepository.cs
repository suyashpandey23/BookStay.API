using BookStay.API.Data;

namespace BookStay.API.Data;

public interface ICountryRepository {
    IEnumerable<Country> GetAll();
    Country? GetById(int id);
    Country Add(Country country);
    Country? Update(int id, Country country);
    bool Delete(int id);
}
