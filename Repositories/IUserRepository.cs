using BookStoreApi.Models;
using MongoDB.Bson;

namespace BookStoreApi.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(string id);
    Task<User> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task<bool> IsEmailAvailableAsync(string email);
    Task InsertAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}
