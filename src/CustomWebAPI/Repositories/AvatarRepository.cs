using CustomWebAPI.Data;
using CustomWebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CustomWebAPI.Repositories
{
  public class AvatarRepository
  {
    private readonly AppDbContext _context;

    public AvatarRepository(AppDbContext context) => _context = context;

    public async Task<Avatar> CreateAsync(Avatar avatar)
    {
      _context.Avatars.Add(avatar);
      await _context.SaveChangesAsync();
      return avatar;
    }

    public async Task<Avatar?> GetByUserSizeAndTypeAsync(int userId, string size, string type)
    {
      return await _context.Avatars
          .Where(a => a.UserId == userId && a.Size == size && a.ImageType == type)
          .OrderByDescending(a => a.CreatedAt)
          .FirstOrDefaultAsync();
    }

    public async Task<List<Avatar>> GetGeneratedByUserAsync(int userId, string? prompt = null)
    {
      var query = _context.Avatars
          .Where(a => a.UserId == userId && a.ImageType == "generated");

      if (!string.IsNullOrEmpty(prompt))
      {
        query = query.Where(a => a.Prompt.Contains(prompt));
      }

      return await query.ToListAsync();
    }
  }
}