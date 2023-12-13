namespace CanIDo.Toolkit.Package.IO;

static class DirectoryTools
{
  public static string NormalizeAbsolute(string _path, bool _appendSeparator) =>
    PathTools.Normalize(_path, false, _appendSeparator);
}