using System.Security;

namespace CanIDo.Toolkit.Package.IO;

public static class DirectoryPathExtensions
{
  /// <summary>
  /// Получить относительный путь директории
  /// </summary>
  /// <param name="_this"></param>
  /// <param name="_path">Папка, относительно которой нужно получить путь</param>
  /// <returns></returns>
  public static RelativeDirectoryPath GetRelative(this DirectoryPath _this, DirectoryPath _path) 
    => _this.GetRelative(_path, out bool _);

  /// <summary>
  /// Получить относительный путь директории
  /// </summary>
  /// <param name="_this"></param>
  /// <param name="_path">Папка, относительно которой нужно получить путь</param>
  /// <param name="_isParentDirectory">Является ли заданная папка родителем</param>
  /// <returns></returns>
  public static RelativeDirectoryPath GetRelative(this DirectoryPath _this, DirectoryPath _path, out bool _isParentDirectory)
  {
    var rootPath = DirectoryTools.NormalizeAbsolute(_path.Path, true);
    var filePath = _this.Info.FullName;
    
    var resPath = Path.GetRelativePath(rootPath, filePath);
    _isParentDirectory = resPath == "." || filePath.Contains(rootPath);
    return RelativeDirectoryPath.FromString(resPath);
  }
  public static FilePath File(this DirectoryPath _this, RelativeFilePath _relativePath)
  {
    var normalizedAbsolutePath = DirectoryTools.NormalizeAbsolute(_this.Info.FullName, true);
    return FilePath.FromString($"{normalizedAbsolutePath}{_relativePath.Path}");
  }
  public static FilePath File(this DirectoryPath _this, string _relativePath)
    => _this.File(RelativeFilePath.FromString(_relativePath));
  public static DirectoryPath Directory(this DirectoryPath _this, RelativeDirectoryPath _relativePath)
  {
    if (_relativePath == RelativeDirectoryPath.Root)
      return _this;
    var normalizedAbsolutePath = _this.Path;
    return DirectoryPath.FromString($"{normalizedAbsolutePath}{_relativePath.Path}");
  }
  public static DirectoryPath Directory(this DirectoryPath _this, string _relativePath)
    => _this.Directory(RelativeDirectoryPath.FromString(_relativePath));
  public static void Create(this DirectoryPath _this) => System.IO.Directory.CreateDirectory(_this.Path);
  public static bool TryCreate(this DirectoryPath _this)
  {
    try
    {
      System.IO.Directory.CreateDirectory(_this.Path);
    }
    catch (Exception e)
    {
#if DEBUG
      Console.WriteLine($"Can't create folder: {e}");
#endif
      if (e is IOException)
        return false;
      throw;
    }
    return true;
  }
  public static void Delete(this DirectoryPath _this) => _this.Info.Delete();
  public static void Delete(this DirectoryPath _this, bool _recursive) => _this.Info.Delete(_recursive);
  public static bool TryDelete(this DirectoryPath _this)
  {
    try
    {
      _this.Info.Delete();
    }
    catch (Exception e)
    {
#if DEBUG
      Console.WriteLine($"Can't delete folder: {e}");
#endif
      if (e is UnauthorizedAccessException ||
          e is DirectoryNotFoundException ||
          e is IOException ||
          e is SecurityException)
        return false;
      throw;
    }
    return true;
  }
  public static bool TryDelete(this DirectoryPath _this, bool _recursive)
  {
    try
    {
      _this.Info.Delete(_recursive);
    }
    catch (Exception e)
    {
#if DEBUG
      Console.WriteLine($"Can't delete folder: {e}");
#endif
      if (e is UnauthorizedAccessException ||
          e is DirectoryNotFoundException ||
          e is IOException ||
          e is SecurityException)
        return false;
      throw;
    }
    return true;
  }

  public static IEnumerable<DirectoryPath> EnumerateDirectories(this DirectoryPath _this)
    => _this.Info.EnumerateDirectories().Select(_x => new DirectoryPath(_x));
  public static IEnumerable<DirectoryPath> EnumerateDirectories(this DirectoryPath _this, string _searchPattern)
    => _this.Info.EnumerateDirectories(_searchPattern).Select(_x => new DirectoryPath(_x));
  public static IEnumerable<DirectoryPath> EnumerateDirectories(this DirectoryPath _this, string _searchPattern, SearchOption _searchOptions)
    => _this.Info.EnumerateDirectories(_searchPattern, _searchOptions).Select(_x => new DirectoryPath(_x));
  public static IEnumerable<FilePath> EnumerateFiles(this DirectoryPath _this)
    => _this.Info.EnumerateFiles().Select(_x => new FilePath(_x));
  public static IEnumerable<FilePath> EnumerateFiles(this DirectoryPath _this, string _searchPattern)
    => _this.Info.EnumerateFiles(_searchPattern).Select(_x => new FilePath(_x));
  public static IEnumerable<FilePath> EnumerateFiles(this DirectoryPath _this, string _searchPattern, SearchOption _searchOptions)
    => _this.Info.EnumerateFiles(_searchPattern, _searchOptions).Select(_x => new FilePath(_x));
  public static IEnumerable<SystemPath> EnumerateSystemPath(this DirectoryPath _this)
    => _this.Info.EnumerateFileSystemInfos().Select(SystemPath.New);
  public static IEnumerable<SystemPath> EnumerateSystemPath(this DirectoryPath _this, string _searchPattern)
    => _this.Info.EnumerateFileSystemInfos(_searchPattern).Select(SystemPath.New);
  public static IEnumerable<SystemPath> EnumerateSystemPath(this DirectoryPath _this, string _searchPattern, SearchOption _searchOptions)
    => _this.Info.EnumerateFileSystemInfos(_searchPattern, _searchOptions).Select(SystemPath.New);
  public static FilePath GetTempFile(this DirectoryPath _this)
  {
    for(var i = 0; i < 100;i++)
    {
      var candidate = _this.File(RelativeFilePath.Temp());
      if (!candidate.Info.Exists)
        return candidate;
    }

    return null;
  }
  public static void MoveTo(this DirectoryPath _path, DirectoryPath _newPath) => System.IO.Directory.Move(_path.Path, _newPath.Path);
}