using CanIDo.Toolkit.Package.IO;
using Xunit;

namespace CanIDo.Toolkit.Tests.IO;

public class RelativeDirectoryPathTests
{
  [Fact]
  public void RelativeDirectoryPath_HashCode()
  {
    Assert.Equal(RelativeDirectoryPath.FromString("/level0").GetHashCode(), RelativeDirectoryPath.FromString("level0").GetHashCode());
    Assert.Equal(RelativeDirectoryPath.FromString("/level0/").GetHashCode(), RelativeDirectoryPath.FromString("level0").GetHashCode());
    Assert.Equal(RelativeDirectoryPath.FromString("level0/").GetHashCode(), RelativeDirectoryPath.FromString("level0").GetHashCode());
    Assert.Equal(RelativeDirectoryPath.FromString("level0/").GetHashCode(), RelativeDirectoryPath.FromString("level0/").GetHashCode());
    Assert.Equal(RelativeDirectoryPath.FromString("level0").GetHashCode(), RelativeDirectoryPath.FromString("level0").GetHashCode());
    Assert.Equal(RelativeDirectoryPath.FromString("a/level0").GetHashCode(), RelativeDirectoryPath.FromString("A/level0").GetHashCode());
    Assert.Equal(RelativeDirectoryPath.FromString("level0").GetHashCode(), RelativeDirectoryPath.FromString("leVel0").GetHashCode());
  }
  [Fact]
  public void RelativeDirectoryPath_Equals()
  {
    Assert.Equal(RelativeDirectoryPath.FromString("/level0"), RelativeDirectoryPath.FromString("level0"));
    Assert.Equal(RelativeDirectoryPath.FromString("/level0/"), RelativeDirectoryPath.FromString("level0"));
    Assert.Equal(RelativeDirectoryPath.FromString("level0/"), RelativeDirectoryPath.FromString("level0"));
    Assert.Equal(RelativeDirectoryPath.FromString("level0/"), RelativeDirectoryPath.FromString("level0/"));
    Assert.Equal(RelativeDirectoryPath.FromString("level0"), RelativeDirectoryPath.FromString("level0"));
    Assert.Equal(RelativeDirectoryPath.FromString("a/level0"), RelativeDirectoryPath.FromString("A/level0"));
    Assert.Equal(RelativeDirectoryPath.FromString("level0"), RelativeDirectoryPath.FromString("leVel0"));
  }
}