using System.Runtime.InteropServices;
using CanIDo.Toolkit.Package.IO;
using Xunit;

namespace CanIDo.Toolkit.Tests.IO;

public class PathToolsTests
{
  [Fact]
  public void PathTools_Normalize()
  {
    var pathStr1 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"temp/1" : @"temp\1";
    var pathStr2 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"../temp/1" : @"..\temp\1";
    string normalize(string _path) => PathTools.Normalize(_path, false, false);
    Assert.Equal(pathStr1, normalize(pathStr1));
    Assert.Equal(pathStr1, normalize(pathStr1 + Path.DirectorySeparatorChar));
    Assert.Equal(pathStr2, normalize(pathStr2 + Path.DirectorySeparatorChar));
    Assert.Equal(@"", normalize(@""));
  }
  [Fact]
  public void PathTools_Normalize_BeginSeparator()
  {
    var pathStr1 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"temp/1" : @"temp\1";
    var pathStr2 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"../temp/1" : @"..\temp\1";
    string normalize(string _path) => PathTools.Normalize(_path, true, false);
    Assert.Equal(Path.DirectorySeparatorChar + pathStr1, normalize(pathStr1));
    Assert.Equal(Path.DirectorySeparatorChar + pathStr1, normalize(pathStr1 + Path.DirectorySeparatorChar));
    Assert.Equal(Path.DirectorySeparatorChar + pathStr2, normalize(pathStr2 + Path.DirectorySeparatorChar));
    Assert.Equal(Path.DirectorySeparatorChar.ToString(), normalize(@""));
  }
  [Fact]
  public void PathTools_Normalize_EndSeparator()
  {
    var pathStr1 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"temp/1" : @"temp\1";
    var pathStr2 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"../temp/1" : @"..\temp\1";
    string normalize(string _path) => PathTools.Normalize(_path, false, true);
    Assert.Equal(pathStr1 + Path.DirectorySeparatorChar, normalize(pathStr1));
    Assert.Equal(pathStr1 + Path.DirectorySeparatorChar, normalize(Path.DirectorySeparatorChar + pathStr1 + Path.DirectorySeparatorChar));
    Assert.Equal(pathStr2 + Path.DirectorySeparatorChar, normalize(pathStr2 + Path.DirectorySeparatorChar));
    Assert.Equal(Path.DirectorySeparatorChar.ToString(), normalize(@""));
  }
  [Fact]
  public void PathTools_Normalize_BeginSeparator_EndSeparator()
  {
    var pathStr1 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"temp/1" : @"temp\1";
    var pathStr2 = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"../temp/1" : @"..\temp\1";
    string normalize(string _path) => PathTools.Normalize(_path, true, true);
    Assert.Equal(Path.DirectorySeparatorChar + pathStr1 + Path.DirectorySeparatorChar, normalize(pathStr1));
    Assert.Equal(Path.DirectorySeparatorChar + pathStr1 + Path.DirectorySeparatorChar, normalize(@Path.DirectorySeparatorChar + pathStr1 + Path.DirectorySeparatorChar));
    Assert.Equal(Path.DirectorySeparatorChar + pathStr2 + Path.DirectorySeparatorChar, normalize(pathStr2 + Path.DirectorySeparatorChar));
    Assert.Equal(Path.DirectorySeparatorChar.ToString(), normalize(@""));
  }
}