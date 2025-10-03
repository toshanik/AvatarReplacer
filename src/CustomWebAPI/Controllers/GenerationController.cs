using CustomWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class GenerationController : ControllerBase
{
  private readonly AvatarService _avatarService;

  public GenerationController(AvatarService avatarService)
  {
    _avatarService = avatarService;
  }

  // Генерация для одного пользователя по ID или DisplayName
  [HttpPost("{userId}")]
  public async Task<IActionResult> GenerateForUser(int userId, [FromBody] GenerationRequest req)
  {
    var avatar = await _avatarService.GenerateAndSaveAvatarAsync(userId, req.Prompt, req.Size);
    return Ok(new { avatar.Id, avatar.ImageData });
  }

  [HttpPost("by-displayname")]
  public async Task<IActionResult> GenerateForUserByName([FromBody] GenerationRequestByName req)
  {
    var user = await _avatarService.FindOrCreateUserAsync(req.DisplayName);
    var avatar = await _avatarService.GenerateAndSaveAvatarAsync(user.Id, req.Prompt, req.Size);
    return Ok(new { avatar.Id, avatar.ImageData });
  }

  // Массовая генерация
  [HttpPost("batch")]
  public async Task<IActionResult> GenerateForAll([FromBody] BatchGenerationRequest req)
  {
    // тут можно параллельно пройтись по всем пользователям
    // для примера синхронно
    var results = new List<object>();
    foreach (var user in req.UserIds ?? new List<int>())
    {
      var avatar = await _avatarService.GenerateAndSaveAvatarAsync(user, req.Prompt, req.Size);
      results.Add(new { avatar.UserId, avatar.Id });
    }
    return Ok(results);
  }

  // История
  [HttpGet("history/{displayName}")]
  public async Task<IActionResult> GetHistory(string displayName)
  {
    var userId = await _avatarService.FindOrCreateUserAsync(displayName);
    var avatars = await _avatarService.GetGeneratedAvatarsAsync(userId.Id);
    return Ok(avatars.Select(a => new {
      a.Id,
      a.ImageType,
      a.Size,
      a.Prompt,
      a.ImageData,
      a.CreatedAt
    }));
  }
}

public class GenerationRequest
{
  public string Prompt { get; set; } = string.Empty;
  public string Size { get; set; } = "large";
}

public class GenerationRequestByName : GenerationRequest
{
  public string DisplayName { get; set; } = string.Empty;
}

public class BatchGenerationRequest : GenerationRequest
{
  public List<int>? UserIds { get; set; }
}
