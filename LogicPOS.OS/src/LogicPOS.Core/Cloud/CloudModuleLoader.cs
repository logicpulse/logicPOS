using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core;

/// <summary>
/// Loads the private cloud adapter when UseCloud is true.
/// The public application does not reference that project.
/// </summary>
public static class CloudModuleLoader
{
    private static AssemblyDependencyResolver? _dependencies;
    private static string? _cloudDirectory;
    private static bool _resolving;

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        var path = ResolvePath(configuration);
        _cloudDirectory = Path.GetDirectoryName(path);
        _dependencies = new AssemblyDependencyResolver(path);
        if (_resolving == false)
        {
            _resolving = true;
            AssemblyLoadContext.Default.Resolving += ResolveDependency;
        }

        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        var type = assembly.GetType("LogicPOS.Cloud.CloudComposition")
            ?? throw new InvalidOperationException("O módulo cloud não expõe CloudComposition.");
        var method = type.GetMethod("AddCloud", BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException("O módulo cloud não expõe AddCloud.");

        try
        {
            method.Invoke(null, [services, configuration]);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            throw exception.InnerException;
        }
    }

    private static string ResolvePath(IConfiguration configuration)
    {
        var configured = configuration["LogicPOS:Cloud:AssemblyPath"];
        if (string.IsNullOrWhiteSpace(configured) == false)
        {
            var full = Path.GetFullPath(configured);
            if (File.Exists(full))
            {
                return full;
            }

            throw new FileNotFoundException("Não encontrei LogicPOS.Cloud.dll em LogicPOS:Cloud:AssemblyPath.", full);
        }

        var beside = Path.Combine(AppContext.BaseDirectory, "LogicPOS.Cloud.dll");
        if (File.Exists(beside))
        {
            return beside;
        }

        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            foreach (var build in new[] { "Debug", "Release" })
            {
                var candidate = Path.Combine(
                    directory.FullName,
                    "logicpos-apiclient",
                    "src",
                    "LogicPOS.Cloud",
                    "bin",
                    build,
                    "net10.0",
                    "LogicPOS.Cloud.dll");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(
            "Não encontrei LogicPOS.Cloud.dll. Compila o projeto no repositório logicpos-apiclient ou indica LogicPOS:Cloud:AssemblyPath.");
    }

    private static Assembly? ResolveDependency(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        var path = _dependencies?.ResolveAssemblyToPath(assemblyName);
        if (string.IsNullOrWhiteSpace(path) || File.Exists(path) == false)
        {
            var name = assemblyName.Name;
            path = string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(_cloudDirectory)
                ? null
                : Path.Combine(_cloudDirectory, name + ".dll");
        }

        if (string.IsNullOrWhiteSpace(path) || File.Exists(path) == false)
        {
            return null;
        }

        return context.LoadFromAssemblyPath(path);
    }
}
