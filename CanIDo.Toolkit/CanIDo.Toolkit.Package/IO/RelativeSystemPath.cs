namespace CanIDo.Toolkit.Package.IO;

public abstract class RelativeSystemPath : IEquatable<RelativeSystemPath>
{
  public string Path { get; }
  protected RelativeSystemPath(string _path, bool _endSeparator)
  {
    Path = PathTools.Normalize(_path, false, _endSeparator);
  }
  public static bool IsRelative(string _path)
  {
    var invalids = System.IO.Path.GetInvalidPathChars();
    return _path.All(_x => !invalids.Contains(_x)) && !System.IO.Path.IsPathRooted(_path);
  }
  public override string ToString() => Path;
  public override bool Equals(object _obj)
  {
    if (ReferenceEquals(null, _obj)) return false;
    if (ReferenceEquals(this, _obj)) return true;
    if (_obj.GetType() != this.GetType()) return false;
    return Equals((RelativeSystemPath) _obj);
  }

  public bool Equals(RelativeSystemPath _other)
  {
    if (ReferenceEquals(null, _other)) return false;
    if (ReferenceEquals(this, _other)) return true;
    return string.Equals(Path, _other.Path, StringComparison.InvariantCultureIgnoreCase);
  }

  public override int GetHashCode()
  {
    return (Path != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Path) : 0);
  }

  public static bool operator ==(RelativeSystemPath _left, RelativeSystemPath _right) => Equals(_left, _right);
  public static bool operator !=(RelativeSystemPath _left, RelativeSystemPath _right) => !Equals(_left, _right);
}