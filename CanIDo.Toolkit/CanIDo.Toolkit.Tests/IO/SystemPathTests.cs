using System.Runtime.InteropServices;
using CanIDo.Toolkit.Package.IO;
using Xunit;

namespace CanIDo.Toolkit.Tests.IO;

public class SystemPathTests
{
  [Fact]
  public void SystemPath_IsAbsolute()
  {
    Assert.Throws<ArgumentNullException>(() => SystemPath.IsAbsolute(null));
    Assert.False(SystemPath.IsAbsolute("1"));
    Assert.True(SystemPath.IsAbsolute("c:"));
    Assert.True(SystemPath.IsAbsolute(@"c:\temp\"));
    Assert.True(SystemPath.IsAbsolute(@"c:\temp.tmp"));
    Assert.True(SystemPath.IsAbsolute(@"\\"));
    Assert.True(SystemPath.IsAbsolute(@"\\server"));
    Assert.True(SystemPath.IsAbsolute(@"\\server\video"));
  }
  [Fact]
  public void SystempPath_Equals()
  {
    var pathStr1 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/temp/tmp" : @"C:\temp\tmp";
    var pathStr2 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/temp/tmp2" : @"C:\temp\tmp2";
    var pathStr3 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/temp/tmp1" : @"C:\temp\tmp1";
    var file1 = FilePath.FromString(pathStr1);
    var file11 = FilePath.FromString(pathStr1);
    object file1Obj = FilePath.FromString(pathStr1);
    var file2 = FilePath.FromString(pathStr2);
    var dir1 = DirectoryPath.FromString(pathStr3);
    var dir11 = DirectoryPath.FromString(pathStr3);
    object dir1Obj = DirectoryPath.FromString(pathStr3);
    var dir2 = DirectoryPath.FromString(pathStr2);
         
    Assert.True(file1 == file11);
    Assert.True(file1 != file2);
    Assert.Equal(file1, file1Obj);
    Assert.False(file1.Equals(null));
    Assert.True(file1.Equals(file1));
    Assert.False(file1.Equals(dir1));
         
    Assert.True(dir1 == dir11);
    Assert.True(dir1 != dir2);
    Assert.Equal(dir1, dir1Obj);
    Assert.False(dir1.Equals(null));
    Assert.True(dir1.Equals(dir1));
    Assert.False(dir1.Equals(file1));
  }
  [Fact]
  public void SystemPath_PathEquals_Windows()
  {
    if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      return;
    Assert.True(SystemPath.PathEquals(@"c:\temp", @"C:\Temp"));
  }
  [Fact]
  public void SystemPath_PathEquals_Linux()
  {
    if(!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      return;
    Assert.False(SystemPath.PathEquals(@"/temp", @"/Temp"));
  }
  [Fact]
  public void SystemPath_New_FilePath()
  {
    var path = Path.GetTempFileName();
    var fInfo = new FileInfo(path);
    var file = SystemPath.New(fInfo);
    Assert.True(file is FilePath);
    Assert.Equal(file, FilePath.FromString(path));
  }
  [Fact]
  public void SystemPath_New_DirectopryPath()
  {
    var path = Path.GetTempFileName();
    var dInfo = new DirectoryInfo(path);
    var dir = SystemPath.New(dInfo);
    Assert.True(dir is DirectoryPath);
    Assert.Equal(dir, DirectoryPath.FromString(path));
  }
  [Fact]
  public void SystemPath_Exists()
  {
    var path = Path.GetTempFileName();
    var file = FilePath.FromString(path);
    file.TryCreate();
    Assert.True(file.Exists);
    Assert.True(file.TryCreate());
    Assert.True(file.Exists);
    Assert.True(file.TryDelete());
    Assert.False(file.Exists);
  }
}