#nullable enable
using System.Reflection;

namespace CanIDo.Toolkit.Package.IO;

public sealed class FilePath : SystemPath, IEquatable<FilePath>, IComparable<FilePath>
{
  public static string BakPostfix { get; } = ".bak";
  public static string TmpPostfix {get;} = ".~~tmp";
  internal FilePath(FileInfo _info)
    : base(_info) {}
  internal FilePath(string _normalizedFilePath)
    : base(_normalizedFilePath) {}

  public new FileInfo Info => (FileInfo)base.Info;
  public DirectoryPath Directory => new(Info.Directory);
  public string Extension => Info.Extension;
  public string ExtensionWithoutDot => Extension.TrimStart('.');
  public static explicit operator FilePath(string _path)
    => FromString(_path);
  public static FilePath FromString(string _path)
  {
    if (!System.IO.Path.IsPathRooted(_path))
      throw new ArgumentException($"{nameof(_path)} is not absolute");

    var normalizedPath = DirectoryTools.NormalizeAbsolute(_path, false);
    return new FilePath(normalizedPath);
  }

  public static FilePath FromRelativeOrAbsolute(string _path)
    => FromRelativeOrAbsolute(_path, DirectoryPath.FromCurrentDirectory());
  public static FilePath FromRelativeOrAbsolute(string _path, DirectoryPath _root)
    => RelativeSystemPath.IsRelative(_path) ? _root.File(_path) : FromString(_path);

  public static FilePath FromAssembly(Assembly _assembly) => FromString(new Uri(_assembly.Location).LocalPath);
  public static FilePath FromAssembly<TAnyAssemblyType>() => FromAssembly(typeof(TAnyAssemblyType));
  public static FilePath FromAssembly(Type _type) => FromAssembly(_type.Assembly);
  public static FilePath? FromEntryAssembly()
  {
    var entry = Assembly.GetEntryAssembly();
    if (entry == null)
      return null;
    return FromAssembly(entry);
  }

  public static FilePath FromExecutingAssembly() => FromAssembly(Assembly.GetExecutingAssembly());
  public bool Equals(FilePath? _other) => base.Equals(_other);
  public int CompareTo(FilePath? _other) => base.CompareTo(_other);
}