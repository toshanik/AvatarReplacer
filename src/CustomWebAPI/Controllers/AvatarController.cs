using CustomWebAPI.Models;
using CustomWebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sungero.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomWebAPI.Host.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AvatarController : ControllerBase
  {
    internal static ILog Logger => Logs.GetLogger<AvatarController>();
    private readonly AvatarService _avatarService;

    public AvatarController(AvatarService avatarService)
    {
      _avatarService = avatarService;
    }

    [HttpPost("GetAvatar")]
    public async Task<IActionResult> GetAvatar([FromBody] AvatarRequestDto request)
    {
      try
      {
        var user = await _avatarService.FindOrCreateUserAsync(request.Author);

        Avatar? avatar = null;

        if (request.ImageType == "original")
        {
          if (!string.IsNullOrEmpty(request.ImageData))
            await _avatarService.SaveOriginalAvatarAsync(user.Id, request.ImageData, request.Size);

          avatar = await _avatarService.GenerateAndSaveAvatarAsync(user.Id, request.Prompt, request.Size);
        }
        else if (request.ImageType == "generated")
        {
          // Тут нужно получить сгенерированную картинку.
          avatar = await _avatarService.GenerateAndSaveAvatarAsync(user.Id, request.Prompt, request.Size);
        }

        if (avatar == null)
        {
          return NotFound("Avatar not found.");
        }

        return Ok(new { ImagePath = avatar.ImagePath });
      }
      catch (Exception ex)
      {
        Logger.Error(ex, "Error in ReplaceAvatar");
        return StatusCode(500, "Internal server error");
      }
    }

    [HttpGet("user/{author}/avatars")]
    public async Task<IActionResult> GetUserAvatars(string author, [FromQuery] string prompt = "")
    {
      var user = await _avatarService.FindOrCreateUserAsync(author);
      var avatars = await _avatarService.GetGeneratedAvatarsAsync(user.Id, prompt);

      return Ok(avatars);
    }
  }

  public class AvatarRequestDto
  {
    public string Author { get; set; } = string.Empty;
    public string? ImageData { get; set; } // Base64
    public string Size { get; set; } = "large"; // "icon", "large"
    public string ImageType { get; set; } = "original"; // "original", "generated"
    public string? Prompt { get; set; }
  }

}
