namespace CanIDo.Toolkit.Package.IO;

static class PathTools
{
  /// <summary>
  /// Нормализовать путь
  /// </summary>
  /// <param name="_path">Путь</param>
  /// <param name="_beginSeparator">Начинать с разделителя ("\dir1\dir2\...")</param>
  /// <param name="_endSeparator">Завершать разделителем ("...dir1\dir2\")</param>
  /// <returns></returns>
  public static string Normalize(string _path, bool _beginSeparator, bool _endSeparator)
  {
    if (_path == string.Empty)
    {
      if (_beginSeparator || _endSeparator)
        return Path.DirectorySeparatorChar.ToString();
      return string.Empty;
    }

    var path = _path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

    if (_beginSeparator)
    {
      if (path[0] != Path.DirectorySeparatorChar)
        path = Path.DirectorySeparatorChar + path;
    }
    else
    {
      path = path.TrimStart(Path.DirectorySeparatorChar);
    }

    if (_endSeparator)
    {
      if (path.Length > 0 && path[path.Length-1] != Path.DirectorySeparatorChar)
        return path + Path.DirectorySeparatorChar;
    }
    else
    {
      path = path.TrimEnd(Path.DirectorySeparatorChar);
    }

    return path;
  }
}