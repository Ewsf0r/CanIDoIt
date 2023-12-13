#nullable enable
using System.Reflection;

namespace CanIDo.Toolkit.Package.IO;

public sealed class DirectoryPath : SystemPath, IEquatable<DirectoryPath>, IComparable<DirectoryPath>
{
  internal DirectoryPath(DirectoryInfo _info) : base(_info)
  {
  }
  internal DirectoryPath(string _normalizedDirectoryPath) : base(_normalizedDirectoryPath)
  {
  }

  public new DirectoryInfo Info => (DirectoryInfo) base.Info;
  public static DirectoryPath? FromAssembly(Assembly _assembly)
  {
    var path = new Uri(_assembly.Location).LocalPath;
    var dir = System.IO.Path.GetDirectoryName(path);
       
    if (dir == null)
      return null;

    return FromString(dir);
  }

  public DirectoryPath this[string _dir] => this.Directory(_dir);
  public DirectoryPath this[RelativeDirectoryPath _dir] => this.Directory(_dir);
  public FilePath this[RelativeFilePath _file] => this.File(_file);
  public FilePath F(RelativeFilePath _file) => this.File(_file);
  public FilePath F(string _file) => this.File(_file);

  public static explicit operator DirectoryPath(string _path)
    => FromString(_path);

  public static DirectoryPath? FromAssembly<TAnyAssemblyType>() => FromAssembly(typeof(TAnyAssemblyType));
  public static DirectoryPath? FromAssembly(Type _type) => FromAssembly(_type.Assembly);
  public static DirectoryPath? FromExecutingAssembly() => FromAssembly(Assembly.GetExecutingAssembly());
  public static DirectoryPath? FromEntryAssembly()
  {
    var entry = Assembly.GetEntryAssembly();
    if (entry == null)
      return null;
    return FromAssembly(entry);
  }

  public static DirectoryPath FromString(string _path) => new(DirectoryTools.NormalizeAbsolute(_path, true));
  public static DirectoryPath FromRelativeOrAbsolute(string _path)
    => FromRelativeOrAbsolute(_path, FromCurrentDirectory());
  public static DirectoryPath FromRelativeOrAbsolute(string _path, DirectoryPath _root)
    => RelativeSystemPath.IsRelative(_path) 
      ? _root.Directory(_path) 
      : FromString(_path);

  public static DirectoryPath FromCurrentDirectory()
    => FromString(Directory.GetCurrentDirectory());

  public static DirectoryPath FromBaseDirectory() => FromString(AppContext.BaseDirectory);
  public static DirectoryPath FromAppDomain(AppDomain _domain) => FromString(_domain.BaseDirectory);
  public static DirectoryPath FromAppDomain() => FromAppDomain(AppDomain.CurrentDomain);

  public static DirectoryPath GetTempPath() =>
    FromString(System.IO.Path.GetTempPath());

  public static DirectoryPath GetSpecialFolderPath(Environment.SpecialFolder _folder) =>
    FromString(Environment.GetFolderPath(_folder));

  public bool Equals(DirectoryPath? _other) =>((Object)this).Equals(_other);
  public int CompareTo(DirectoryPath? _other) => base.CompareTo(_other);
}