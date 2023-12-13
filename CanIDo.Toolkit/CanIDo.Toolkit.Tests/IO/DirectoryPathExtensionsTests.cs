using System.Runtime.InteropServices;
using CanIDo.Toolkit.Package.IO;
using CanIDo.Toolkit.Package.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace CanIDo.Toolkit.Tests.IO;

public class DirectoryPathExtensionsTests : IOTest
{
  public DirectoryPathExtensionsTests(ITestOutputHelper _output) : base(_output)
  {
  }

  [Theory]
  [InlineData("test\\abc\\", true, "D:", "D:\\test\\abc")]
  [InlineData("abc\\", true, "D:\\test\\", "D:\\test\\abc")]
  [InlineData("..\\abc\\", false, "D:\\test\\1\\", "D:\\test\\abc")]
  [InlineData("..\\..\\", false, "D:\\test\\abc\\", "D:\\")]
  [InlineData(".\\", true, "D:\\test\\", "D:\\test\\")]
  [InlineData(".\\", true, "D:\\", "D:\\")]
  [InlineData("..\\..\\..\\", false, "\\baz\\a\\b\\c\\", "\\baz\\")]
  [InlineData("..\\..\\b1\\b2\\", false, "\\a1\\a2\\", "\\b1\\b2\\")]
  [InlineData("..\\", false, "D:\\test\\abc", "D:\\test")]
  public void DirectoryPath_GetRelative_Windows(string expectedPath, bool expectedParent, string fromPath, string toPath)
  {
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      return;

    var actualDir= DirectoryPath.FromString(toPath)
      .GetRelative(
        DirectoryPath.FromString(fromPath),
        out bool actualParent);

    Assert.Equal(expectedPath, actualDir.Path);
    Assert.Equal(expectedParent, actualParent);
  }
  
  [Theory]
  [InlineData("test/abc/", true, "/", "/test/abc")]
  [InlineData("abc/", true, "/test/", "/test/abc")]
  [InlineData("../abc/", false, "/test/1/", "/test/abc")]
  [InlineData("../../", false, "/test/abc/", "/")]
  [InlineData("./", true, "/test/", "/test/")]
  [InlineData("./", true, "/", "/")]
  [InlineData("../../../", false, "/baz/a/b/c/", "/baz/")]
  [InlineData("../../b1/b2/", false, "/a1/a2/", "/b1/b2/")]
  [InlineData("../", false, "/test/abc", "/test")]
  public void DirectoryPath_GetRelative_Linux(string expectedPath, bool expectedParent, string fromPath, string toPath)
  {
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      return;

    var actualDir= DirectoryPath.FromString(toPath)
      .GetRelative(
        DirectoryPath.FromString(fromPath),
        out bool actualParent);

    Assert.Equal(expectedPath, actualDir.Path);
    Assert.Equal(expectedParent, actualParent);
  }

  [Fact]
  public void DirectoryPath_File()
  {
    var abc = WorkDir.F("abc.txt");
    abc.TryCreate();
    Assert.Equal(abc.Path, abc.Parent.File(abc.Name).Path);
  }

  [Fact]
  public void DirectoryPath_Directory()
  {
    var dirPath = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/test/abc" : @"D:\test\abc";
    var abc = DirectoryPath.FromString(dirPath);
    Assert.Equal(abc.Parent.Directory(abc.Name).Path, abc.Path);
  }

  [Fact]
  public void DirectoryPath_Create()
  {
    var abc = WorkDir["abc"];
    abc.Create();
    Assert.True(abc.Exists);
  }

  [Fact]
  public void DirectoryPath_CreateInfo()
  {
    var abc = WorkDir["abc"];
    abc.Create();
    Assert.True(abc.Exists);
    Assert.True(abc.Info.LastWriteTime > DateTime.Now.Subtract(TimeSpan.FromDays(1)));
    Assert.True(abc.Info.Exists);
  }

  [Fact]
  public void DirectoryPath_Name()
  {
    Assert.Equal("abc", WorkDir["abc"].Name);
    Assert.Equal("ab", WorkDir["ab"].Name);
    Assert.Equal("a", WorkDir["a"].Name);
  }

  [Fact]
  public void DirectoryPath_TryCreate()
  {
    var abc = WorkDir["abc"];
    Assert.True(abc.TryCreate());
    Assert.True(abc.Exists);
  }

  [Fact]
  public void DirectoryPath_RelativePathExt()
  {
    var dirPath = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? @"/test/abc1" : "D:\\test\\abc1";
    var abc = DirectoryPath.FromString(dirPath);
    var childDir1 = abc.Directory($"1");
    var childFile1 = abc.F($"1.txt");
    var relDir = RelativeDirectoryPath.FromString(dirPath).Directory("1").Path;
    var relFile = RelativeDirectoryPath.FromString(dirPath).File("1.txt").Path;
    var expDir1 = RelativeDirectoryPath.FromString(childDir1.Path).Path;
    var expFile1 = RelativeFilePath.FromString(childFile1.Path).Path;
    Assert.Equal(expDir1, relDir);
    Assert.Equal(expFile1, relFile);
  }

  [Fact]
  public void DirectoryPath_Delete()
  {
    var abc = WorkDir["abc"];
    var childDir1 = abc.Directory("1");
    var childDir2 = childDir1.Directory("1");
    var childFile1 = abc.F("1.txt");
    var childFile2 = childDir1.F("1.txt");
    var childFile3 = childDir2.F("1.txt");
    abc.TryCreate();
    childDir1.TryCreate();
    childDir2.TryCreate();
    childFile1.TryCreate();
    childFile2.TryCreate();
    childFile3.TryCreate();

    Assert.Throws<IOException>(() => abc.Delete());
    Assert.Throws<IOException>(() => abc.Delete(false));
    Assert.False(abc.TryDelete(false));
    Assert.False(abc.TryDelete());
    Assert.True(abc.TryDelete(true));
    Assert.False(abc.Exists);

    abc.TryCreate();
    childFile1.TryCreate();
    childDir1.TryCreate();
    childFile2.TryCreate();
    childDir2.TryCreate();
    childFile2.TryCreate();
    abc.Delete(true);
    Assert.False(abc.Exists);

    abc.TryCreate();
    abc.Delete(false);
    Assert.False(abc.Exists);
    abc.TryCreate();
    abc.Delete();
    Assert.False(abc.Exists);
    abc.TryCreate();
    Assert.True(abc.TryDelete(false));
    Assert.False(abc.Exists);
    abc.TryCreate();
    Assert.True(abc.TryDelete());
    Assert.False(abc.Exists);
  }

  [Fact]
  public void DirectoryPath_EnumerateDirectories()
  {
    var abc = WorkDir["abc"];
    abc.TryDelete(true);
    var childDir1 = abc.Directory($"A1");
    var childDir2 = abc.Directory($"B1");
    var childDir3 = childDir1.Directory($"B1");
    abc.TryCreate();
    childDir1.TryCreate();
    childDir2.TryCreate();
    childDir3.TryCreate();

    var expectedList = new List<DirectoryPath>()
    {
      childDir1,
      childDir2
    };
    var actualList = abc.EnumerateDirectories().ToList();
    Assert.Equal(expectedList.OrderBy(x=>x), actualList.OrderBy(x=>x));

    expectedList = new List<DirectoryPath>() {childDir2};
    actualList = abc.EnumerateDirectories(childDir2.Name).ToList();
    Assert.Equal(expectedList, actualList);
    actualList = abc.EnumerateDirectories(childDir2.Name, SearchOption.TopDirectoryOnly).ToList();
    Assert.Equal(expectedList, actualList);

    expectedList = new List<DirectoryPath>()
    {
      childDir2,
      childDir3
    };
    actualList = abc.EnumerateDirectories(childDir2.Name, SearchOption.AllDirectories).ToList();
    Assert.Equal(expectedList, actualList);
  }

  [Fact]
  public void DirectoryPath_EnumerateFiles()
  {
    var abc = WorkDir["abc"];
    var childDir1 = abc.Directory($"A1");
    abc.TryCreate();
    childDir1.TryCreate();
    var childFile1 = abc.F($"A1.txt");
    var childFile2 = abc.F($"B1.txt");
    var childFile3 = childDir1.F($"B1.txt");
    childFile1.TryCreate();
    childFile2.TryCreate();
    childFile3.TryCreate();

    var expectedList = new List<FilePath>()
    {
      childFile1,
      childFile2
    };
    var actualList = abc.EnumerateFiles().ToList();
    Assert.Equal(expectedList, actualList);

    expectedList = new List<FilePath>() {childFile2};
    actualList = abc.EnumerateFiles(childFile2.Name).ToList();
    Assert.Equal(expectedList, actualList);
    actualList = abc.EnumerateFiles(childFile2.Name, SearchOption.TopDirectoryOnly).ToList();
    Assert.Equal(expectedList, actualList);

    expectedList = new List<FilePath>()
    {
      childFile2,
      childFile3
    };
    actualList = abc.EnumerateFiles(childFile2.Name, SearchOption.AllDirectories).ToList();
    Assert.Equal(expectedList, actualList);
  }

  [Fact]
  public void DirectoryPath_EnumerateSystemPath()
  {
    var abc = WorkDir["abc"];
    var childDir1 = abc.Directory($"A1");
    var childDir2 = abc.Directory($"B1");
    ;
    var childDir3 = childDir1.Directory($"B1");
    abc.TryCreate();
    childDir1.TryCreate();
    childDir2.TryCreate();
    ;
    childDir3.TryCreate();
    var childFile1 = abc.F($"A1.txt");
    var childFile2 = abc.F($"B1.txt");
    var childFile3 = childDir1.F($"B1.txt");
    childFile1.TryCreate();
    childFile2.TryCreate();
    childFile3.TryCreate();

    var expectedList = new List<SystemPath>()
    {
      childDir1,
      childFile1,
      childDir2,
      childFile2
    };
    var actualList = abc.EnumerateSystemPath().ToList();
    Assert.Equal(expectedList.OrderBy(x => x), actualList.OrderBy(x => x));

    expectedList = new List<SystemPath>()
    {
      childDir2
    };
    actualList = abc.EnumerateSystemPath(childDir2.Name).ToList();
    Assert.Equal(expectedList, actualList);
    actualList = abc.EnumerateSystemPath(childDir2.Name, SearchOption.TopDirectoryOnly).ToList();
    Assert.Equal(expectedList, actualList);

    expectedList = new List<SystemPath>()
    {
      childFile2,
      childFile3
    };
    actualList = abc.EnumerateSystemPath(childFile2.Name, SearchOption.AllDirectories).ToList();
    Assert.Equal(expectedList, actualList);
  }

  [Fact]
  public void DirectoryPath_GetTempFile()
  {
    var dir = WorkDir;
    var tmp = dir.GetTempFile();
    Assert.True(tmp.TryCreate());
    Assert.Equal(1, dir.EnumerateFiles().Count());
  }

  [Fact]
  public void DirectoryPath_MoveTo()
  {
    var root = WorkDir["abc"];
    var childDir1 = root.Directory($"A1");
    root.TryCreate();
    childDir1.TryCreate();
    var childFile1 = root.F($"A1.txt");
    var childFile2 = root.F($"B1.txt");
    var childFile3 = childDir1.F($"B1.txt");
    childFile1.TryCreate();
    childFile2.TryCreate();
    childFile3.TryCreate();

    var dst = WorkDir["def1"];
    var expectedChildDir1 = dst.Directory($"A1");
    var expectedChildFile1 = dst.F($"A1.txt");
    var expectedChildFile2 = dst.F($"B1.txt");
    var expectedChildFile3 = expectedChildDir1.F($"B1.txt");

    Assert.False(dst.Exists);

    root.MoveTo(dst);
    Assert.False(root.Exists);
    Assert.True(dst.Exists);

    var expectedList = new List<SystemPath>()
    {
      expectedChildDir1,
      expectedChildFile1,
      expectedChildFile2,
      expectedChildFile3
    };
    var actualList = dst.EnumerateSystemPath().ToList();
    actualList.AddRange(expectedChildDir1.EnumerateSystemPath());
    Assert.Equal(expectedList.OrderBy(x => x), actualList.OrderBy(x => x));
  }
}