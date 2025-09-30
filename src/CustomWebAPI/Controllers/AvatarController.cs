using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sungero.Logging;
using CustomWebAPI.Models;

namespace CustomWebAPI.Host.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AvatarController : ControllerBase
  {
    internal static ILog logger => Logs.GetLogger<CustomController>();
    private readonly UserService _userService;

    public AvatarController(UserService userService)
    {
      _userService = userService;
    }

    [HttpGet("{id}", Name = "GetAvatar")]
    public string Get(int id)
    {
      return id.ToString();
    }

    [HttpPost("{id},{name}")]
    public string Test(int id, string name)
    {
      logger.Debug("Âûחמג Test ס ןאנאלוענאלט {@id}, {@name}", id, name);
      return id + name;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CustomWebAPI.Models.User model)
    {
      var isUserCreated = await _userService.CreateUserAsync(model);
      return Ok("User created successfully");
    }
  }
}
