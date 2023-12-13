using Xunit.Abstractions;

namespace CanIDo.Toolkit.Package.xUnit
{
  public static class ITestOutputHelperExtensions
  {
    public static XUnitWriter AsWriter(this ITestOutputHelper _this, string _scope = "root") => new (_this, _scope);
  }
}
