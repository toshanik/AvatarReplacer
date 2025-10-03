using CustomWebAPI.Models;
using CustomWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
  private readonly AvatarService _avatarService;

  public UsersController(AvatarService avatarService)
  {
    _avatarService = avatarService;
  }

  // Массовое добавление сотрудников
  [HttpPost("batch")]
  public async Task<IActionResult> AddUsers([FromBody] List<UserDto> users)
  {
    var createdUsers = new List<User>();
    foreach (var dto in users)
    {
      var user = await _avatarService.FindOrCreateUserAsync(dto.DisplayName);
      createdUsers.Add(user);
    }
    return Ok(createdUsers.Select(u => new { u.Id, u.DisplayName }));
  }

  // Получить данные по пользователю по displayName
  [HttpGet("by-displayname/{displayName}")]
  public async Task<IActionResult> GetByDisplayName(string displayName)
  {
    var user = await _avatarService.FindOrCreateUserAsync(displayName);
    var avatar = await _avatarService.GetAvatarAsync(user.Id, "large", "generated");
    return Ok(new
    {
      user.Id,
      user.DisplayName,
      GeneratedImageUrl = avatar?.ImageData
    });
  }
}

public class UserDto
{
  public string DisplayName { get; set; } = string.Empty;
}
