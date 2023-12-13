using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace CanIDo.Toolkit.Package.xUnit
{
  public class XUnitWriter
  {
    private readonly ITestOutputHelper p_output;
    private readonly string p_scope;
    private readonly Stopwatch p_sw;

    public XUnitWriter(ITestOutputHelper _output, string _scope = "root")
    {
      p_output = _output;
      p_scope = _scope;
      p_sw = Stopwatch.StartNew();
    }

    public XUnitWriter this[string _subscope] => new(p_output, p_scope + "\\" + _subscope);

    public void WriteLine(string _line)
    {
      ITestOutputHelper pOutput = p_output;
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 3);
      interpolatedStringHandler.AppendFormatted(p_sw.Elapsed.TotalMilliseconds, "F2");
      interpolatedStringHandler.AppendLiteral(" | ");
      interpolatedStringHandler.AppendFormatted(p_scope);
      interpolatedStringHandler.AppendLiteral("| ");
      interpolatedStringHandler.AppendFormatted(_line);
      var stringAndClear = interpolatedStringHandler.ToStringAndClear();
      pOutput.WriteLine(stringAndClear);
    }
  }
}
