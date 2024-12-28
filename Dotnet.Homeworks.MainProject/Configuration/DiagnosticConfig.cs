using System.Diagnostics.Metrics;
using Dotnet.Homeworks.MainProject.Helpers;

namespace Dotnet.Homeworks.MainProject.Configuration;

public static class DiagnosticConfig
{
    public static readonly string ServiceName = AssemblyReference.Assembly.GetName().Name!;
    public static readonly Meter Meter = new(ServiceName);
    public static readonly Counter<int> PostProductCounter = Meter.CreateCounter<int>("product.post_count");
}