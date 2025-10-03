using CustomWebAPI.Data;
using CustomWebAPI.Models;
using CustomWebAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
      // 🔹 Декодируем Base64 строку в байты
      var imageBytes = Convert.FromBase64String(imageData);

      // 🔹 Генерируем уникальное имя файла
      var fileName = $"{Guid.NewGuid()}.png"; // или .jpg, в зависимости от типа
      var uploadPath = Path.Combine("wwwroot", "uploads", "avatars"); // Папка для хранения

      // 🔹 Создаём папку, если её нет
      Directory.CreateDirectory(uploadPath);

      // 🔹 Полный путь к файлу
      var filePath = Path.Combine(uploadPath, fileName);

      // 🔹 Сохраняем файл
      await File.WriteAllBytesAsync(filePath, imageBytes);

      // 🔹 Создаём сущность с путём к файлу
      var avatar = new Avatar
      {
        UserId = userId,
        ImagePath = $"/uploads/avatars/{fileName}", // Сохраняем относительный путь
        Size = size,
        ImageType = "original"
      };

      return await _avatarRepository.CreateAsync(avatar);
    }

    public async Task<Avatar> GenerateAndSaveAvatarAsync(int userId, string prompt, string size)
    {
      var originalAvatar = await this.GetAvatarAsync(userId, size, "original");
      var generatedImage = ImageProcessor.GenerateImageFromPrompt(prompt, originalAvatar?.ImageData);

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
      var avatar = await _avatarRepository.GetByUserSizeAndTypeAsync(userId, null, size);
      if (avatar == null)
        avatar =  await this.GenerateAndSaveAvatarAsync(userId, null, size);
      return avatar;
    }

    public async Task<List<Avatar>> GetGeneratedAvatarsAsync(int userId, string prompt = "")
    {
      return await _avatarRepository.GetGeneratedByUserAsync(userId, string.IsNullOrEmpty(prompt) ? null : prompt);
    }
  }
}
