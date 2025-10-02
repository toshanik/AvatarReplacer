using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomWebAPI.Data;
using CustomWebAPI.Models;
using CustomWebAPI.Repositories;

namespace CustomWebAPI.Services
{
  public class AvatarService
  {
    private readonly UserRepository _userRepository;
    private readonly AvatarRepository _avatarRepository;

    public AvatarService(UserRepository userRepository, AvatarRepository avatarRepository)
    {
      _userRepository = userRepository;
      _avatarRepository = avatarRepository;
    }

    public async Task<User> FindOrCreateUserAsync(string displayName)
    {
      var normalizedName = NameNormalizer.Normalize(displayName);
      var user = await _userRepository.FindByNormalizedNameAsync(normalizedName);

      if (user == null)
      {
        user = new User { DisplayName = displayName, NormalizedName = normalizedName };
        user = await _userRepository.CreateAsync(user);
      }

      return user;
    }

    public async Task<Avatar> SaveOriginalAvatarAsync(int userId, string imageData, string size)
    {
      var avatar = new Avatar
      {
        UserId = userId,
        ImageData = imageData,
        Size = size,
        ImageType = "original"
      };

      return await _avatarRepository.CreateAsync(avatar);
    }

    public async Task<Avatar> GenerateAndSaveAvatarAsync(int userId, string prompt, string size)
    {
      var generatedImage = ImageProcessor.GenerateImageFromPrompt(prompt);

      var avatar = new Avatar
      {
        UserId = userId,
        ImageData = generatedImage,
        Size = size,
        ImageType = "generated",
        Prompt = prompt
      };

      return await _avatarRepository.CreateAsync(avatar);
    }

    public async Task<Avatar?> GetAvatarAsync(int userId, string size, string type = "original")
    {
      return await _avatarRepository.GetByUserSizeAndTypeAsync(userId, size, type);
    }

    public async Task<List<Avatar>> GetGeneratedAvatarsAsync(int userId, string prompt = "")
    {
      return await _avatarRepository.GetGeneratedByUserAsync(userId, string.IsNullOrEmpty(prompt) ? null : prompt);
    }
  }
}
