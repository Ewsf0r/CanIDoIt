using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using CanIDo.Toolkit.Package.IO;
using Xunit;

namespace CanIDo.Toolkit.Tests.IO;

public class DirectoryPathTests
{

  private static int p_instance = 0;
  [Fact]
  public void DirectoryPath_FromAssembly()
  {
    var dirPath = DirectoryPath.FromAssembly(Assembly.GetAssembly(typeof(DirectoryPathTests)));
    var location = Assembly.GetAssembly(typeof(DirectoryPathTests)).Location;
    var expected = FilePath.FromString(location).Directory;
    Assert.Equal(dirPath,expected);

    dirPath = DirectoryPath.FromAssembly(typeof(DirectoryPathTests));
    Assert.Equal(dirPath,expected);

    dirPath = DirectoryPath.FromAssembly<DirectoryPathTests>();
    Assert.Equal(dirPath, expected);

  }
  [Fact]
  public void DirectoryPath_Children()
  {
    var id = Interlocked.Increment(ref p_instance);
    var sub = $"{DateTime.Now::O}{Stopwatch.GetTimestamp():X}{id}".Replace(":", "_");
    var root = DirectoryPath.FromCurrentDirectory()[sub];
    root.TryCreate();

    var WorkDir = root;
    var abc = WorkDir["abc"];
    var childDir1 = abc.Directory($"A");
    abc.TryCreate();
    childDir1.TryCreate();
    var childFile1 = abc.F(RelativeFilePath.FromString($"A.txt"));
    childFile1.TryCreate();
    Assert.Equal(childDir1, abc[$"A"]);
    Assert.Equal(childDir1, abc[RelativeDirectoryPath.FromString($"A")]);
    Assert.Equal(childFile1, abc[RelativeFilePath.FromString($"A.txt")]);
  }
      
  [Fact]
  public void DirectoryPath_FromRelativeOrAbsolute()
  {
    var pathStr = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/usr/tmp/1/" : @"D:\test\1\";
    var filePath = DirectoryPath.FromRelativeOrAbsolute(pathStr);
    Assert.Equal(filePath.Path, pathStr);
  }

  [Fact]
  public void DirectoryPath_FromBaseDirectory()
  {
    var location = Assembly.GetAssembly(typeof(DirectoryPathTests)).Location;
    var expected = FilePath.FromString(location).Directory;
    var dirPath = DirectoryPath.FromBaseDirectory();
    Assert.Equal(expected, dirPath);

    dirPath = DirectoryPath.FromAppDomain();
    Assert.Equal(expected, dirPath);

    dirPath = DirectoryPath.FromAssembly<DirectoryPathTests>();
    Assert.Equal(expected, dirPath);
  }
  [Fact]
  public void DirectoryPath_HashCode()
  {
    Assert.Equal(DirectoryPath.FromString("D:/level0").GetHashCode(), DirectoryPath.FromString("D:/level0/").GetHashCode());
    Assert.Equal(DirectoryPath.FromString("D:/level0/").GetHashCode(), DirectoryPath.FromString("D:/level0/").GetHashCode());
    Assert.Equal(DirectoryPath.FromString("D:/level0").GetHashCode(), DirectoryPath.FromString("D:/level0").GetHashCode());
    Assert.Equal(DirectoryPath.FromString("D:/A/level0").GetHashCode(), DirectoryPath.FromString("D:/a/level0").GetHashCode());
    Assert.Equal(DirectoryPath.FromString("D:/leVel0").GetHashCode(), DirectoryPath.FromString("D:/level0").GetHashCode());
  }
  [Fact]
  public void DirectoryPath_GetTempPath()
  {
    var expected = System.IO.Path.GetTempPath();
    var dirPath = DirectoryPath.GetTempPath().Path;
    Assert.Equal(expected, dirPath);
  }
}