using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sungero.Logging;
using System.ComponentModel.DataAnnotations;

namespace CustomWebAPI.Models
{
  public class User
  {
    public int Id { get; set; }
    [Required]
    public string NormalizedName { get; set; } = string.Empty; // Иванов Иван Иванович
    public string DisplayName { get; set; } = string.Empty;    // Формат, как пришёл от клиента
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Avatar> Avatars { get; set; } = new List<Avatar>();
  }
}