using System.Diagnostics;
using CanIDo.Toolkit.Package.IO;
using Xunit.Abstractions;

namespace CanIDo.Toolkit.Package.xUnit;

public abstract class IOTest : IDisposable
{
  private static long InstanceId = Environment.TickCount64;

  protected IOTest(ITestOutputHelper _output)
  {
    var id = Interlocked.Increment(ref InstanceId);
    var sub = $"{DateTime.Now:O}{Stopwatch.GetTimestamp():X}{id}".Replace(":", "_");
    var root = DirectoryPath.FromCurrentDirectory()[sub];
    WorkDir = root;
    WorkDir.TryCreate();

    Output = _output.AsWriter();
  }
  public XUnitWriter Output { get; }
  public DirectoryPath WorkDir { get; }

  public void Dispose()
  {
    Console.WriteLine("TearDown...");
    if(!WorkDir.Exists)
      return;
    foreach (var file in WorkDir.EnumerateFiles("*", SearchOption.AllDirectories))
      file.TryDelete();
    foreach (var dir in WorkDir.EnumerateDirectories("*", SearchOption.AllDirectories).OrderByDescending(_ => _))
      dir.TryDelete(true);
    WorkDir.TryDelete(true);
    Console.WriteLine("TearDown ok");
  }
}