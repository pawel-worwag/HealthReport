using System.Reflection;
using HealthReport.Infrastructure.TempFileStorage;
using Microsoft.Extensions.Options;

namespace HealthReport.Infrastructure.Tests;

public class FileSystemTempFileStorageTests
{
    private static (object instance, MethodInfo method) CreateResolver()
    {
        var options = Options.Create(new FileSystemTempFileStorageOptions { BasePath = "/tmp" });
        var storage = new FileSystemTempFileStorage(options);
        var method = storage.GetType().GetMethod("ResolvePath", BindingFlags.Instance | BindingFlags.NonPublic);
        return method is null ? throw new InvalidOperationException() : (storage, method);
    }

    private static string InvokeResolve(object instance, MethodInfo mi, string? id)
    {
        var result = mi.Invoke(instance, [id]);
        return result is null ? throw new InvalidOperationException() : (string)result;
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyOrWhitespace(string id)
    {
        var (storage, method) = CreateResolver();
        var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(storage, [id]));
        Assert.IsType<ArgumentException>(ex.InnerException);
    }

    [Fact]
    public void Null()
    {
        var (storage, method) = CreateResolver();
        var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(storage, [null]));
        Assert.IsType<ArgumentException>(ex.InnerException);
    }

    [Theory]
    [InlineData("~/ala.txt")]
    [InlineData("ala/../ola.txt")]
    [InlineData("/ola.sh")]
    [InlineData("/etc/passwd")]
    [InlineData("../passwd")]
    public void PathTraversalOrSeparators(string id)
    {
        var (storage, method) = CreateResolver();
        var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(storage, [id]));
        Assert.IsType<ArgumentException>(ex.InnerException);
    }
    
    
    [Theory]
    [InlineData("\0" )]
    [InlineData("name\0bad")]
    [InlineData("%2e%2e/%2e%2e")]
    public void BadNames(string id)
    {
        var (storage, method) = CreateResolver();
        var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(storage, [id]));
        Assert.IsType<ArgumentException>(ex.InnerException);
    }

    [Theory]
    [InlineData("ala")]
    [InlineData("absc-1231-dsce-232e3-sdw")]
    [InlineData("00fdb775-70ce-47d3-8ffe-5060aa53f78d")]
    [InlineData("ala..txt")]
    [InlineData(".ala")]
    [InlineData("..ala")]
    public void ValidNames(string id)
    {
        var (instance, mi) = CreateResolver();
        var full = InvokeResolve(instance, mi, id);
        Assert.Equal(Path.GetFullPath(Path.Combine("/tmp", id)), full);
    }
}