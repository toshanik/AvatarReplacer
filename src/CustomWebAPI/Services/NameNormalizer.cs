using System.Text.RegularExpressions;

namespace CustomWebAPI.Services
{
  public static class NameNormalizer
  {
    public static string Normalize(string name)
    {
      // Убираем лишние пробелы
      name = Regex.Replace(name.Trim(), @"\s+", " ");

      var parts = name.Split(' ');

      if (parts.Length == 0) return name;

      var lastName = parts[0];
      var firstName = "";
      var middleName = "";

      for (int i = 1; i < parts.Length; i++)
      {
        var part = parts[i].Trim();

        if (i == 1) // имя
        {
          firstName = ExtractInitial(part);
        }
        else if (i == 2) // отчество
        {
          middleName = ExtractInitial(part);
        }
      }

      var result = lastName;
      if (!string.IsNullOrEmpty(firstName))
      {
        result += " " + firstName;
      }
      if (!string.IsNullOrEmpty(middleName))
      {
        result += " " + middleName;
      }

      return result;
    }

    private static string ExtractInitial(string namePart)
    {
      if (string.IsNullOrEmpty(namePart)) return "";

      // Если уже сокращено (например, "И." или "Иванович.")
      if (namePart.EndsWith("."))
      {
        // Убедимся, что это сокращение (1 буква + точка)
        if (namePart.Length == 2 && char.IsLetter(namePart[0]))
        {
          return namePart.ToUpper();
        }
      }

      // Извлекаем первую букву и добавляем точку
      return $"{namePart[0]}.".ToUpper();
    }
  }
}
