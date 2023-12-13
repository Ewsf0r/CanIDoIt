using System.Runtime.InteropServices;
using System.Text;
using CanIDo.Toolkit.Package.IO;
using CanIDo.Toolkit.Package.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace CanIDo.Toolkit.Tests.IO;

public class FilePathExtensionsTests : IOTest
{
  public FilePathExtensionsTests(ITestOutputHelper _output) : base(_output)
  {
  }

  [Fact]
  public void FilePath_SetExtensionIfNot()
  {
    var hasExtPath = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "/abc.txt" : "C:\\abc.txt";
    var noExtPath = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "/abc" : "C:\\abc";
    Assert.Equal(hasExtPath, FilePath.FromString(noExtPath).SetExtensionIfNot("txt").Path);
    Assert.Equal(hasExtPath, FilePath.FromString(noExtPath).SetExtensionIfNot(".txt").Path);
    Assert.Equal(hasExtPath, FilePath.FromString(hasExtPath).SetExtensionIfNot("txt").Path);
    Assert.Equal(hasExtPath, FilePath.FromString(hasExtPath).SetExtensionIfNot(".txt").Path);
  }

  [Fact]
  public void FilePath_TryReplace()
  {
    var abc = WorkDir.F("abc.txt");
    var def = WorkDir.F("def.txt");
    var ghi = WorkDir.F("ghi.txt");
    abc.TryCreate();
    def.TryCreate();
    Assert.True(abc.TryReplace(def, ghi));
    abc.TryCreate();
    Assert.True(abc.TryReplace(def));
    abc.TryCreate();
  }
  [Fact]
  public void FilePath_TryCopy()
  {
    var abc = WorkDir.F("abc.txt");
    abc.TryCreate();
    abc.WriteAllText("abc");
    var def = WorkDir.F("def.txt");
    def.TryDelete();

    Assert.True(abc.TryCopy(def, false));
    Assert.True(abc.TryCopy(def, true));
    Assert.False(abc.TryCopy(def, false));

    Assert.Equal(abc.ReadAllBytes(), def.ReadAllBytes());
  }
  [Fact]
  public void FilePath_Create()
  {
    var abc = WorkDir.F($"abc.txt");

    using (abc.Create())
    {
    }

    Assert.True(abc.Exists);
  }
  [Fact]
  public void FilePath_CreateInfo()
  {
    var abc = WorkDir.F($"abc.txt");

    using (abc.Create())
    {
    }

    Assert.True(abc.Exists);
    Assert.True(abc.Info.LastWriteTime > DateTime.Now.Subtract(TimeSpan.FromDays(1)));
    Assert.True(abc.Info.Exists);
  }
  [Fact]
  public void FilePath_Name()
  {
    Assert.Equal("abc", WorkDir.F("abc").Name);
    Assert.Equal("ab", WorkDir.F("ab").Name);
    Assert.Equal("a", WorkDir.F("a").Name);
  }
  [Fact]
  public void FilePath_TryRestore()
  {
    var abc = WorkDir.F("abc.txt");
    abc.TryCreate();
    Assert.True(abc.TryRestore());
  }

  [Fact]
  public void FilePath_TryMoveOrReplace()
  {
    var abc = WorkDir.F("abc.txt");
    var def = WorkDir.F("def.txt");
    var ghi = WorkDir.F("ghi.txt");
    abc.TryCreate();
    Assert.True(abc.TryMoveOrReplace(def, ghi));
    abc.TryCreate();
  }

  [Fact]
  public void FilePath_TryBackupAndReplace()
  {
    var abc = WorkDir.F("abc.txt");
    var def = WorkDir.F("def.txt");
    abc.TryCreate();
    Assert.True(abc.TryBackupAndReplace(def));
  }

  [Fact]
  public void FilePath_TryMove()
  {
    var abc = WorkDir.F("abc.txt");
    var def = WorkDir.F("def.txt");
    abc.TryCreate();
    def.TryDelete();
    Assert.True(abc.TryMove(def));
  }

  [Fact]
  public void FilePath_TryCreate()
  {
    var abc = WorkDir.F("abc.txt");

    Assert.True(abc.TryCreate(out var stream));
    stream.Dispose();
    Assert.True(abc.TryCreate());
  }

  [Fact]
  public void FilePath_Prefix()
  {
    var strPath = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "/test/abc.txt" : "D:\\test\\abc.txt";
    var prefPath = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "/test/defabc.txt" : "D:\\test\\defabc.txt";
    var abc = FilePath.FromString(strPath);
    Assert.Equal(abc.Prefix("def").Path, prefPath);
  }

  [Fact]
  public void FilePath_ChangeExtension()
  {
    var abcPath = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "/test/abc.txt" : "D:\\test\\abc.txt";
    var zipPath = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "/test/abc.zip" : "D:\\test\\abc.zip";
    var abc = FilePath.FromString(abcPath);
    var zip = FilePath.FromString(zipPath);
    Assert.Equal(zipPath, abc.ChangeExtension("zip").Path);
    Assert.Equal(abcPath, zip.ChangeExtension(".txt").Path);
    Assert.Equal(abcPath, abc.ChangeExtension("txt").Path);
    Assert.Equal(abcPath, abc.ChangeExtension(".txt").Path);
    Assert.Throws<ArgumentNullException>(() => abc.ChangeExtension(null));
  }
  
  [Theory]
  [InlineData("test\\abc\\abc.txt", true, "D:\\", "D:\\test\\abc\\abc.txt")]
  [InlineData("..\\abc.txt", false, "D:\\test\\1\\", "D:\\test\\abc.txt")]
  [InlineData("..\\abc\\abc.txt", false, "D:\\test\\1\\", "D:\\test\\abc\\abc.txt")]
  [InlineData("..\\..\\abc.txt", false, "D:\\test\\abc\\", "D:\\abc.txt")]
  [InlineData(".", true, "D:\\test\\abc.txt", "D:\\test\\abc.txt")]
  [InlineData(".", true, "D:\\abc.txt", "D:\\abc.txt")]
  [InlineData("..\\..\\..\\abc.txt", false, "\\baz\\a\\b\\c\\", "\\baz\\abc.txt")]
  [InlineData("..\\..\\b1\\b2\\abc.txt", false, "\\a1\\a2\\", "\\b1\\b2\\abc.txt")]
  [InlineData("abc.txt", true, "D:\\test\\abc\\", "D:\\test\\abc\\abc.txt")]
  public void FilePath_GetRelative_Windows(string expectedPath, bool expectedParent, string fromPath, string toPath)
  {
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      return;

    var actualDir = FilePath.FromString(toPath)
      .GetRelative(
        DirectoryPath.FromString(fromPath),
        out bool actualParent);

    Assert.Equal(expectedPath, actualDir.Path);
    Assert.Equal(expectedParent, actualParent);
  }
  
  [Theory]
  [InlineData("test/abc/abc.txt", true, "/", "/test/abc/abc.txt")]
  [InlineData("../abc.txt", false, "/test/1/", "/test/abc.txt")]
  [InlineData("../abc/abc.txt", false,"/test/1/", "/test/abc/abc.txt")]
  [InlineData("../../abc.txt", false,"/test/abc/", "/abc.txt")]
  [InlineData(".", true,"/test/abc.txt", "/test/abc.txt")]
  [InlineData(".", true,"/abc.txt", "/abc.txt")]
  [InlineData("../../../abc.txt", false, "/baz/a/b/c/", "/baz/abc.txt")]
  [InlineData("../../b1/b2/abc.txt", false, "/a1/a2/", "/b1/b2/abc.txt")]
  [InlineData("abc.txt", true, "/test/abc/", "/test/abc/abc.txt")]
  public void FilePath_GetRelative_Linux(string expectedPath, bool expectedParent, string fromPath, string toPath)
  {
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      return;
    
    var actualDir= FilePath.FromString(toPath)
      .GetRelative(
        DirectoryPath.FromString(fromPath),
        out bool actualParent);

    Assert.Equal(expectedPath, actualDir.Path);
    Assert.Equal(expectedParent, actualParent);
  }

  [Fact]
  public void FilePath_ReadAllText_WriteAllText()
  {
    var abc = WorkDir.F("abc.txt");
    abc.TryCreate();
    var expectedText = "abc";
    abc.WriteAllText(expectedText);
    string actualText;

    void check() => Assert.Equal(expectedText, actualText);

    actualText = abc.ReadAllText();
    check();

    abc.WriteAllText(expectedText, Encoding.Unicode);
    actualText = abc.ReadAllText(Encoding.Unicode);
    check();

    actualText = abc.ReadLine(0);
    check();

    actualText = abc.ReadLine(0, Encoding.Unicode);
    check();
  }

  [Fact]
  public void FilePath_ReadAllBytes_WriteAllBytes()
  {
    var abc = WorkDir.F("abc.txt");
    abc.TryCreate();
    var expectedBytes = new byte[] { 1, 2, 3 };
    var actualBytes = default(byte[]);
    abc.WriteAllBytes(expectedBytes);
    abc.WriteAllBytes(expectedBytes, 0, 3);
    actualBytes = abc.ReadAllBytes();
    Assert.Equal(expectedBytes, actualBytes);
  }

  [Fact]
  public void FilePath_ReadAllLines_WriteAllLines()
  {
    var abc = WorkDir.F("abc.txt");
    abc.TryCreate();
    var expectedLines = new[] { "1", "2", "3" };
    abc.WriteAllLines(expectedLines);

    var actualLines = abc.ReadAllLines();
    Assert.Equal(expectedLines, actualLines);

    abc.WriteAllLines(expectedLines, Encoding.Unicode);
    actualLines = abc.ReadAllLines(Encoding.Unicode);
    Assert.Equal(expectedLines, actualLines);
  }
}