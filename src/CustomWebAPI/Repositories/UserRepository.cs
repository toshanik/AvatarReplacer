using CustomWebAPI.Data;
using CustomWebAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CustomWebAPI.Repositories
{
  public class UserRepository
  {
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> FindByNormalizedNameAsync(string normalizedName)
    {
      return await _context.Users.FirstOrDefaultAsync(u => u.NormalizedName == normalizedName);
    }

    public async Task<User> CreateAsync(User user)
    {
      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      return user;
    }
  }
}