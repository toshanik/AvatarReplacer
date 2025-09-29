using CustomWebAPI.Data;
using CustomWebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Select(u => new User { Id = u.Id, Name = u.Name })
            .ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        return new User { Id = user.Id, Name = user.Name };
    }

    public async Task<bool> CreateUserAsync(User model)
    {
        var user = new User { Name = model.Name }; // User Ч сущность Ѕƒ
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }
}