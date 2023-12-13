using System.Reflection;
using System.Runtime.InteropServices;
using CanIDo.Toolkit.Package.IO;
using Xunit;

namespace CanIDo.Toolkit.Tests.IO;

public class FilePathTests
{
  [Fact]
  public void FilePath_FromString()
  {
    var pathStr = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/usr/tmp/1.tmp" : @"c:\temp\1.tmp";
    var dirStr = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/usr/tmp" : @"c:\temp";
    var filePath = FilePath.FromString(pathStr);
    Assert.Equal(pathStr, filePath.Path);
    Assert.Equal(pathStr, filePath.ToString());
    Assert.Equal("1.tmp", filePath.Name);
    Assert.Equal(".tmp", filePath.Extension);
    Assert.Equal("tmp", filePath.ExtensionWithoutDot);
    Assert.Equal(DirectoryPath.FromString(dirStr), filePath.Directory);
    Assert.Equal(DirectoryPath.FromString(dirStr), filePath.Parent);
  }
      
  [Fact]
  public void FilePath_FromRelativeOrAbsolute()
  {
    var pathStr = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/usr/tmp/1.tmp" : @"c:\temp\1.tmp";
    var dirStr = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/usr/tmp" : @"c:\temp";
    var filePath = FilePath.FromRelativeOrAbsolute(pathStr);
    Assert.Equal(pathStr, filePath.Path);
    Assert.Equal(".tmp", filePath.Extension);
    Assert.Equal("tmp", filePath.ExtensionWithoutDot);
    Assert.Equal(filePath.Directory, DirectoryPath.FromString(dirStr));
  }
  [Fact]
  public void FilePath_HashCode()
  {
    var expPathStr1 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/level0" : @"C:/level0";
    var actPathStr1 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/level0" : @"C:/level0";
    var expPathStr2 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/a/level0" : @"C:/a/level0";
    var actPathStr2 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/A/level0" : @"C:/A/level0";
    var expPathStr3 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/level0" : @"C:/level0";
    var actPathStr3 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/leVel0" : @"C:/leVel0";
    Assert.Equal(FilePath.FromString(expPathStr1).GetHashCode(), FilePath.FromString(actPathStr1).GetHashCode());
    Assert.Equal(FilePath.FromString(expPathStr2).GetHashCode(), FilePath.FromString(actPathStr2).GetHashCode());
    Assert.Equal(FilePath.FromString(expPathStr3).GetHashCode(), FilePath.FromString(actPathStr3).GetHashCode());
  }
  [Fact]
  public void FilePath_FromAssembly()
  {
    var filePath = FilePath.FromAssembly(Assembly.GetAssembly(typeof(FilePathTests)));
    var name = Assembly.GetAssembly(typeof(FilePathTests)).GetName().Name;
    Assert.Equal(name, filePath.GetFileNameWithoutExtensions());
    Assert.Equal(".DLL".ToLowerInvariant(), filePath.Extension.ToLowerInvariant());
    Assert.Equal("DLL".ToLowerInvariant(), filePath.ExtensionWithoutDot.ToLowerInvariant());
    filePath = FilePath.FromAssembly(typeof(FilePathTests));
    Assert.Equal(name, filePath.GetFileNameWithoutExtensions());

    filePath = FilePath.FromAssembly<FilePathTests>();
    Assert.Equal(name, filePath.GetFileNameWithoutExtensions());

  }
}