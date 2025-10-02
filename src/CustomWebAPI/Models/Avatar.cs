using System;
using System.ComponentModel.DataAnnotations;

namespace CustomWebAPI.Models
{
  public class Avatar
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ImageData { get; set; } = string.Empty; // Base64 encoded image
    public string ImageType { get; set; } = "original"; // "original", "generated"
    public string Prompt { get; set; } = string.Empty; // Для ИИ-изображений
    public string Size { get; set; } = "large"; // "icon", "large"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
  }
}