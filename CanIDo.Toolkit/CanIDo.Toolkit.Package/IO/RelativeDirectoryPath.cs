namespace CanIDo.Toolkit.Package.IO;

/// <summary>
/// Относительный путь директории.
/// Всегда начинается с одной "\".
/// </summary>
public sealed class RelativeDirectoryPath : RelativeSystemPath
{
  public static readonly RelativeDirectoryPath Root = new(string.Empty);
  private RelativeDirectoryPath(string _path)
    : base(_path, true) {}
  /// <summary>
  /// Получить относительную папку, относительно этой
  /// </summary>
  /// <param name="_relativePath"></param>
  /// <returns></returns>
  public RelativeDirectoryPath Directory(string _relativePath)
  {
    if (_relativePath == null)
      throw new ArgumentNullException(nameof(_relativePath));

    var relative = PathTools.Normalize(_relativePath, false, true);
    var path = Path + relative;
    return FromString(path);
  }
  /// <summary>
  /// Получить относительный файл, относительно этой
  /// </summary>
  /// <param name="_relativePath"></param>
  /// <returns></returns>
  public RelativeFilePath File(string _relativePath)
  {
    if (_relativePath == null)
      throw new ArgumentNullException(nameof(_relativePath));

    var relative = PathTools.Normalize(_relativePath, false, false);
    var path = Path + relative;
    return RelativeFilePath.FromString(path);
  }
  /// <summary>
  /// Создать относительную папку из строки
  /// </summary>
  /// <param name="_path"></param>
  /// <returns></returns>
  public static RelativeDirectoryPath FromString(string _path)
  {
    if (_path == null)
      throw new ArgumentNullException(nameof(_path));

    return new RelativeDirectoryPath(_path);
  }
}