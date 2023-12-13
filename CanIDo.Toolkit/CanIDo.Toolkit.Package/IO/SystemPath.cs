#nullable enable
namespace CanIDo.Toolkit.Package.IO;

public abstract class SystemPath : IEquatable<SystemPath>, IComparable<SystemPath>
{
  public static string BakExtensionWithoutDot = "bak";
  public static string BakExtension = $".{BakExtensionWithoutDot}";

  private readonly string p_path;
  private volatile FileSystemInfo? p_info;

  protected SystemPath(FileInfo _info)
  {
    p_info = _info;
    p_path = DirectoryTools.NormalizeAbsolute(_info.FullName, false);
  }
  protected SystemPath(DirectoryInfo _info)
  {
    p_info = _info;
    p_path = DirectoryTools.NormalizeAbsolute(_info.FullName, true);
  }
  protected SystemPath(string _normalizedFileAndDirPath)
  {
    p_path = _normalizedFileAndDirPath;
  }

  public FileSystemInfo Info
  {
    get
    {
      if (p_info != null)
        return p_info;
      FileSystemInfo info;
      if (IsDirectory)
        info = new DirectoryInfo(p_path);
      else
        info = new FileInfo(p_path);
      p_info = info;
      return info;
    }
  }

  public bool IsDirectory => p_path.EndsWith(System.IO.Path.DirectorySeparatorChar);

  /// <summary>
  /// Normalized Path
  /// Ordinary separator
  /// Directories ends on separator
  /// </summary>
  public string Path => p_path;

  public string Name
  {
    get
    {
      if (IsDirectory)
      {
        var index = p_path.LastIndexOf(System.IO.Path.DirectorySeparatorChar, p_path.Length-2);
        if (index < 0)
          throw new InvalidOperationException("Incorrect path");
        return p_path.Substring(index + 1, p_path.Length - index - 2);
      }
      else
      {
        var index = p_path.LastIndexOf(System.IO.Path.DirectorySeparatorChar, p_path.Length-1);
        if (index < 0)
          throw new InvalidOperationException("Incorrect path");
        return p_path.Substring(index + 1, p_path.Length - index - 1);
      }
    }
  }

  public bool Exists
  {
    get
    {
      if (IsDirectory)
        return Directory.Exists(p_path);
      return File.Exists(p_path);
    }
  }

  public bool Equals(SystemPath? _other)
  {
    if (ReferenceEquals(null, _other))
      return false;
    if (ReferenceEquals(this, _other))
      return true;
    if (GetType() != _other.GetType())
      return false;
    return GetHashCode() == _other.GetHashCode() &&
           StringComparer.InvariantCultureIgnoreCase.Equals(Path, _other.Path);
  }

  public override bool Equals(object? _other) => Equals(_other as SystemPath);
  public override int GetHashCode() => StringComparer.InvariantCultureIgnoreCase.GetHashCode(Path);

  public int CompareTo(SystemPath? _other) =>
    _other == null 
      ? 1 
      : string.Compare(Path, _other.Path, StringComparison.Ordinal);

  public override string ToString() => Info.FullName;
  public static bool operator ==(SystemPath? _left, SystemPath? _right) => Equals(_left, _right);
  public static bool operator !=(SystemPath? _left, SystemPath? _right) => !Equals(_left, _right);

  public DirectoryPath? Parent
  {
    get
    {
      var normalizedAbsolutePath = DirectoryTools.NormalizeAbsolute(Info.FullName, false);
      var index = normalizedAbsolutePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar);

      if (index == -1)
        return null;

      if (IsNetworkPath())
      {
        if (index < 2)
          return null;
        if (normalizedAbsolutePath.IndexOf('\\', 2) == index) // UNC путь должен быть \\server\share
          return null;
      }

      return DirectoryPath.FromString(normalizedAbsolutePath.Substring(0, index));
    }
  }

  public bool IsNetworkPath()
  {
    var path = p_path;
    if (path.Length < 2)
      return false;
    return path[0] == '\\' && path[1] == '\\';
  }
  public static bool IsAbsolute(string _path)
  {
    if (_path == null)
      throw new ArgumentNullException(nameof(_path));

    if (_path.Length < 2)
      return false;

    if (char.IsLetter(_path[0]) && _path[1] == ':')
      return true;

    return _path[0] == '\\' && _path[1] == '\\';
  }

  internal static SystemPath New(FileSystemInfo _systemInfo)
  {
    switch (_systemInfo)
    {
      case DirectoryInfo directoryInfo:
        return new DirectoryPath(directoryInfo);
      case FileInfo fileInfo:
        return new FilePath(fileInfo);
    }

    throw new NotSupportedException();
  }

  public static bool PathEquals(string _path1, string _path2)
    => System.IO.Path.GetFullPath(_path1).Equals(System.IO.Path.GetFullPath(_path2),
      System.IO.Path.DirectorySeparatorChar == '/'
        ? StringComparison.CurrentCulture
        : StringComparison.CurrentCultureIgnoreCase);
}