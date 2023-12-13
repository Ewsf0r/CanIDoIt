using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace CanIDo.Toolkit.Package.IO;

public static class FilePathExtensions
{
  public static void Replace(this FilePath _this, FilePath _destination, FilePath _destinationBackup)
    => File.Replace(_this.Info.FullName, _destination.Info.FullName, _destinationBackup.Info.FullName);
  public static bool TryReplace(this FilePath _this, FilePath _destination, FilePath _destinationBackup)
  {
    var success = true;
    try
    {
      File.Replace(_this.Info.FullName, _destination.Info.FullName, _destinationBackup.Info.FullName);
    }
    catch (UnauthorizedAccessException ae)
    {
      success = false;
    }
    catch (PathTooLongException ptle)
    {
      success = false;
    }
    catch (IOException ioe)
    {
      success = false;
    }
    return success;
  }
  public static bool TryMoveOrReplace(this FilePath _this, FilePath _destination, FilePath _destinationBackup)
  {
    if (!_destination.Exists)
      return TryMove(_this, _destination);
    return TryReplace(_this, _destination, _destinationBackup);
  }

  public static FilePath PostfixBak(this FilePath _this) => _this.Postfix(FilePath.BakPostfix);
  public static FilePath PostfixTmp(this FilePath _this) => _this.Postfix(FilePath.TmpPostfix);
  public static FilePath Postfix(this FilePath _this, string _postfix)
    => FilePath.FromString(_this.Path + _postfix);
  public static FilePath Prefix(this FilePath _this, string _prefix)
    => _this.Directory.File(_prefix + _this.Name);

  public static FilePath SetExtensionIfNot(this FilePath _this, string _extension)
  {
    if (_extension[0] != '.')
    {
      if (!string.Equals(_this.ExtensionWithoutDot, _extension, StringComparison.InvariantCultureIgnoreCase))
        return _this.Postfix($".{_extension}");
      return _this;
    }
    if (!string.Equals(_this.Extension, _extension, StringComparison.InvariantCultureIgnoreCase))
      return _this.Postfix(_extension);
    return _this;
  }

  /// <summary>
  /// Изменить расширение файла
  /// </summary>
  public static FilePath ChangeExtension(this FilePath _this, string _newExtension)
  {
    if (_newExtension == null)
      throw new ArgumentNullException();

    return FilePath.FromString(Path.ChangeExtension(_this.Info.FullName, _newExtension));
  }
  /// <summary>
  /// Получить относительный путь относительно папки.
  /// </summary>
  /// <param name="_this"></param>
  /// <param name="_path"></param>
  /// <returns></returns>
  public static RelativeFilePath GetRelative(this FilePath _this, DirectoryPath _path)
    => _this.GetRelative(_path, out bool _);

  /// <summary>
  /// Получить относительный путь
  /// </summary>
  /// <param name="_this"></param>
  /// <param name="_path">Директория, относительно которой нужно получить путь</param>
  /// <param name="_isParentDirectory">Является ли заданная директория предком</param>
  /// <returns></returns>
  public static RelativeFilePath GetRelative(this FilePath _this, DirectoryPath _path, out bool _isParentDirectory)
  {
    if (_path == null)
      throw new ArgumentNullException();

    var beginSeparator = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    var rootPath = PathTools.Normalize(_path.Info.FullName, beginSeparator, true);
    var filePath = _this.Info.FullName;
    
    var resPath = Path.GetRelativePath(rootPath, filePath);
    _isParentDirectory = resPath == "." || filePath.Contains(rootPath);
    return RelativeFilePath.FromString(resPath);
  }
  public static void Delete(this FilePath _this) => _this.Info.Delete();
  public static bool TryDelete(this FilePath _this)
  {
    try
    {
      _this.Info.Delete();
    }
    catch (Exception e)
    {
      if (e is UnauthorizedAccessException ||
          e is IOException ||
          e is SecurityException)
        return false;
      throw;
    }
    return true;
  }

  public static void Move(this FilePath _this, FilePath _dst)
    => File.Move(_this.Info.FullName, _dst.Info.FullName);
  public static bool TryMove(this FilePath _this, FilePath _dst)
  {
    try
    {
      File.Move(_this.Info.FullName, _dst.Info.FullName);
    }
    catch (Exception e)
    {
      if (e is UnauthorizedAccessException ||
          e is IOException ||
          e is SecurityException)
        return false;
      throw;
    }
    return true;
  }
  public static bool TryRestore(this FilePath _this)
    => _this.TryRestore(_this.Postfix(SystemPath.BakExtension));
  public static bool TryRestore(this FilePath _this, FilePath _bak)
  {
    if (_this.Exists)
      return true;
    if (_bak.Exists)
      return _bak.TryMove(_this);
    return false;
  }
  public static bool TryBackupAndReplace(this FilePath _this, FilePath _dst)
    => _this.TryBackupAndReplace(_dst, _dst.Postfix(SystemPath.BakExtension));
  public static bool TryBackupAndReplace(this FilePath _this, FilePath _dst, FilePath _bak)
  {
    if (_dst.Exists)
    {
      if (_bak.Exists)
      {
        if (!_bak.TryDelete())
          return false;
      }
      if (!_dst.TryMove(_bak))
        return false;
      return _this.TryMove(_dst);
    }
    return _this.TryMove(_dst);
  }
  public static bool TryReplace(this FilePath _this, FilePath _dst)
  {
    if (!_dst.Exists)
      return _this.TryMove(_dst);
    if (!_dst.TryDelete())
      return false;
    return _this.TryMove(_dst);
  }
  public static FileStream OpenWrite(this FilePath _this) => _this.Info.OpenWrite();
  public static FileStream OpenRead(this FilePath _this) => _this.Info.OpenRead();
  public static FileStream Open(this FilePath _this, FileMode _mode) => _this.Info.Open(_mode);
  public static FileStream Open(this FilePath _this, FileMode _mode, FileAccess _access) => _this.Info.Open(_mode, _access);
  public static FileStream Create(this FilePath _this) => _this.Info.Create();
  public static bool TryCreate(this FilePath _this, out FileStream _stream)
  {
    try
    {
      _stream = _this.Info.Create();
    }
    catch (Exception e)
    {
      if (e is IOException)
      {
        _stream = null;
        return false;
      }
      throw;
    }
    return true;
  }
  public static bool TryCreate(this FilePath _this)
  {
    try
    {
      using (_this.Info.Create()) { }
    }
    catch (Exception e)
    {
      if (e is IOException)
        return false;
      throw;
    }
    return true;
  }

  public static FileStream Open(this FilePath _this, FileMode _mode, FileAccess _access, FileShare _share)
    => _this.Info.Open(_mode, _access, _share);
  public static void Copy(this FilePath _this, FilePath _destination, bool _overwrite)
    => File.Copy(_this.Info.FullName, _destination.Info.FullName, _overwrite);
  public static bool TryCopy(this FilePath _this, FilePath _destination, bool _overwrite)
  {
    var success = true;
    try
    {
      File.Copy(_this.Info.FullName, _destination.Info.FullName, _overwrite);
    }
    catch (UnauthorizedAccessException)
    {
      success = false;
    }
    catch (PathTooLongException)
    {
      success = false;
    }
    catch (IOException)
    {
      success = false;
    }
    return success;
  }
  public static string ReadAllText(this FilePath _this) => File.ReadAllText(_this.Info.FullName);
  public static string ReadAllText(this FilePath _this, Encoding _encoding) => File.ReadAllText(_this.Info.FullName, _encoding);
  public static byte[] ReadAllBytes(this FilePath _this) => File.ReadAllBytes(_this.Info.FullName);
  public static IEnumerable<string> ReadLines(this FilePath _this) => File.ReadLines(_this.Info.FullName);
  public static IEnumerable<string> ReadLines(this FilePath _this, Encoding _encoding) => File.ReadLines(_this.Info.FullName, _encoding);
  public static string[] ReadAllLines(this FilePath _this) => File.ReadAllLines(_this.Info.FullName);
  public static string ReadLine(this FilePath _this, int _line)
  {
    var i = 0;
    var lines = ReadLines(_this);
    foreach (var line in lines)
    {
      if (i == _line)
        return line;
      i++;
    }
    return null;
  }
  public static string ReadLine(this FilePath _this, int _line, Encoding _encoding)
  {
    var i = 0;
    var lines = ReadLines(_this, _encoding);
    foreach (var line in lines)
    {
      if (i == _line)
        return line;
      i++;
    }
    return null;
  }
  public static string[] ReadAllLines(this FilePath _this, Encoding _encoding) => File.ReadAllLines(_this.Info.FullName, _encoding);
  public static void WriteAllText(this FilePath _this, string _content, Encoding _encoding) => File.WriteAllText(_this.Info.FullName, _content, _encoding);
  public static void WriteAllText(this FilePath _this, string _content) => File.WriteAllText(_this.Info.FullName, _content);
  public static void WriteAllBytes(this FilePath _this, byte[] _bytes) => File.WriteAllBytes(_this.Info.FullName, _bytes);
  public static void WriteAllBytes(this FilePath _this, byte[] _bytes, int _offset, int _length)
  {
    using (var stream = new FileStream(_this.Info.FullName, FileMode.Create, FileAccess.Write, FileShare.Read))
      stream.Write(_bytes, _offset, _length);
  }
  public static void WriteAllLines(this FilePath _this, IEnumerable<string> _contents)
    => File.WriteAllLines(_this.Info.FullName, _contents);
  public static void WriteAllLines(this FilePath _this, IEnumerable<string> _contents, Encoding _encoding)
    => File.WriteAllLines(_this.Info.FullName, _contents, _encoding);
  public static void AppendAllText(this FilePath _this, string _content, Encoding _encoding)
    => File.AppendAllText(_this.Info.FullName, _content, _encoding);
  public static void AppendAllText(this FilePath _this, string _content)
    => File.AppendAllText(_this.Info.FullName, _content);
  public static void AppendAllBytes(this FilePath _this, byte[] _bytes)
  {
    using (var stream = File.Open(_this.Info.FullName, FileMode.Append, FileAccess.Write, FileShare.Read))
      stream.Write(_bytes, 0, _bytes.Length);
  }
  public static void AppendAllLines(this FilePath _this, IEnumerable<string> _contents)
    => File.AppendAllLines(_this.Info.FullName, _contents);
  public static void AppendAllLines(this FilePath _this, IEnumerable<string> _contents, Encoding _encoding)
    => File.AppendAllLines(_this.Info.FullName, _contents, _encoding);
  public static string GetFileNameWithoutExtensions(this FilePath _filePath)
    => Path.GetFileNameWithoutExtension(_filePath.Path);
  public static FilePath GetFilePathWithoutExtensions(this FilePath _filePath)
    => _filePath.Directory.File(_filePath.GetFileNameWithoutExtensions());
  public static void MoveTo(this FilePath _path, FilePath _newPath) => File.Move(_path.Path, _newPath.Path);
}