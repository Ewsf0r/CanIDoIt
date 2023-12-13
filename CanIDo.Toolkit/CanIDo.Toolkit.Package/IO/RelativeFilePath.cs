using System.Diagnostics;

namespace CanIDo.Toolkit.Package.IO;

/// <summary>
/// Относительный путь к файлу.
/// Всегда начинается с одной "\".
/// </summary>
public sealed class RelativeFilePath : RelativeSystemPath
{
  /// <summary>
  /// Конструктор
  /// </summary>
  /// <param name="_path"></param>
  private RelativeFilePath(string _path)
    : base(_path, false) {}
  /// <summary>
  /// Из строки
  /// </summary>
  /// <param name="_path"></param>
  /// <returns></returns>
  public static RelativeFilePath FromString(string _path)
  {
    if (string.IsNullOrEmpty(_path))
      throw new ArgumentNullException(nameof(_path));

    return new RelativeFilePath(_path);
  }
  public static RelativeFilePath Temp()
  {
    var now = DateTime.Now;
    var tempName = $"{now.Year}{now.Month}{now.Day}{now.Hour}{now.Minute}{now.Second}{now.Millisecond}{Stopwatch.GetTimestamp():X}";
    return new RelativeFilePath(tempName);
  }
}