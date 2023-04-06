
using BookStoreApi.Models;
using BookStoreApi.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Services;

public class UserService: IUserRepository
{private readonly IMongoCollection<User> _collection;

    public UserService(IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
    {
        var database = new MongoClient(bookStoreDatabaseSettings.Value.OfflineString)
            .GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);
        _collection = database.GetCollection<User>("users");
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<User> GetByIdAsync(string id)
    {
        return await _collection.Find(u => u.Id == id).SingleOrDefaultAsync();
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _collection.Find(u => u.Email == email).SingleOrDefaultAsync();
    }

    public async Task<bool> IsEmailAvailableAsync(string email)
    {
        return await _collection.Find(u => u.Email == email).SingleOrDefaultAsync() is null;
    }

    public async Task InsertAsync(User user)
    {
        await _collection.InsertOneAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        await _collection.ReplaceOneAsync(u => u.Id == user.Id, user);
    }

    public async Task DeleteAsync(User user)
    {
        await _collection.DeleteOneAsync(u => u.Id == user.Id);
    }
}