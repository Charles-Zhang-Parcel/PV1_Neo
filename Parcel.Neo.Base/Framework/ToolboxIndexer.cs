using Parcel.Neo.Base.Toolboxes.Basic;
using Parcel.Neo.Base.Toolboxes.DataProcessing;
using Parcel.Neo.Base.Toolboxes.DataSource;
using Parcel.Neo.Base.Toolboxes.FileSystem;
using Parcel.Neo.Base.Toolboxes.Finance;
using Parcel.Neo.Base.Toolboxes.Logic;
using Parcel.Neo.Base.Toolboxes.Math;
using Parcel.Neo.Base.Toolboxes.Special;
using Parcel.Neo.Base.Toolboxes.String;
using Parcel.Toolbox.ControlFlow;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using System.IO;

namespace Parcel.Neo.Base.Framework
{
    public static class ToolboxIndexer
    {
        #region Cache
        private static Dictionary<string, ToolboxNodeExport[]>? _toolboxes;
        public static Dictionary<string, ToolboxNodeExport[]> Toolboxes
        {
            get
            {
                _toolboxes ??= IndexToolboxes();
                return _toolboxes;
            }
        }
        #endregion

        #region Method
        private static Dictionary<string, ToolboxNodeExport[]> IndexToolboxes()
        {
            Dictionary<string, Assembly> toolboxAssemblies = [];
            // Register Parcel packages (environment path)
            foreach ((string Name, string Path) package in GetPackages())
            {
                try
                {
                    RegisterToolbox(toolboxAssemblies, package.Name, Assembly.LoadFrom(package.Path));
                }
                catch (Exception) { continue; }
            }
            // Register entire (referenced) assemblies
            RegisterToolbox(toolboxAssemblies, "Plot", Assembly.Load("Parcel.Plots"));
            RegisterToolbox(toolboxAssemblies, "Generator", Assembly.Load("Parcel.Generators"));
            RegisterToolbox(toolboxAssemblies, "Vector", Assembly.Load("Parcel.Vector"));

            // Index nodes
            Dictionary<string, ToolboxNodeExport[]> toolboxes = IndexToolboxes(toolboxAssemblies);
            // Index new internal toolboxes
            AddToolbox(toolboxes, "Basic", new BasicToolbox());
            AddToolbox(toolboxes, "Control Flow", new ControlFlowToolbox());
            AddToolbox(toolboxes, "Data Processing", new DataProcessingToolbox());
            AddToolbox(toolboxes, "Data Source", new DataSourceToolbox());
            AddToolbox(toolboxes, "File System", new FileSystemToolbox());
            AddToolbox(toolboxes, "Finance", new FinanceToolbox());
            AddToolbox(toolboxes, "Logic", new LogicToolbox());
            AddToolbox(toolboxes, "Math", new MathToolbox());
            AddToolbox(toolboxes, "String", new StringToolbox());
            AddToolbox(toolboxes, "Special", new SpecialToolbox());
            // Register specific types
            RegisterType(toolboxes, "Data Grid", typeof(Types.DataGrid));
            return toolboxes;
        }
        #endregion

        #region Helpers
        private static Dictionary<string, ToolboxNodeExport[]> IndexToolboxes(Dictionary<string, Assembly> assemblies)
        {
            Dictionary<string, ToolboxNodeExport[]> toolboxes = [];

            foreach (string toolboxName in assemblies.Keys.OrderBy(k => k))
            {
                Assembly assembly = assemblies[toolboxName];
                toolboxes[toolboxName] = [];

                // Load either old PV1 toolbox or new Parcel package
                IEnumerable<ToolboxNodeExport?>? exportedNodes = null;
                if (assembly
                    .GetTypes()
                    .Any(p => typeof(IToolboxDefinition).IsAssignableFrom(p)))
                {
                    // Loading per old PV1 convention
                    exportedNodes = GetExportNodesFromConvention(toolboxName, assembly);
                }
                else
                {
                    // Remark-cz: In the future we will utilize Parcel.CoreEngine.Service for this
                    // Load generic Parcel package
                    exportedNodes = GetExportNodesFromGenericAssembly(assembly);
                }
                toolboxes[toolboxName] = exportedNodes!.Select(n => n!).ToArray();
            }

            return toolboxes;
        }
        private static void RegisterToolbox(Dictionary<string, Assembly> toolboxAssemblies, string name, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentException($"Assembly is null.");

            if (toolboxAssemblies.ContainsKey(name))
                throw new InvalidOperationException($"Assembly `{assembly.FullName}` is already registered.");

            toolboxAssemblies.Add(name, assembly);
        }
        private static void AddToolbox(Dictionary<string, ToolboxNodeExport[]> toolboxes, string name, IToolboxDefinition toolbox)
        {
            List<ToolboxNodeExport> nodes = [];

            foreach (ToolboxNodeExport nodeExport in toolbox.ExportNodes)
                nodes.Add(nodeExport);

            toolboxes[name] = [.. nodes];
        }
        private static (string Name, string Path)[] GetPackages()
        {
            string packageImportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Parcel NExT", "Packages");

            string? environmentOverride = Environment.GetEnvironmentVariable("PARCEL_PACKAGES");
            if (environmentOverride != null && Directory.Exists(environmentOverride))
                packageImportPath = environmentOverride;

            if (Directory.Exists(packageImportPath))
                return Directory
                    .EnumerateFiles(packageImportPath)
                    .Where(file => Path.GetExtension(file).Equals(".dll", StringComparison.CurrentCultureIgnoreCase))
                    .Select(file => (Path.GetFileNameWithoutExtension(file), file))
                    .ToArray();
            return [];
        }
        private static void RegisterType(Dictionary<string, ToolboxNodeExport[]> toolboxes, string name, Type type)
        {
            List<ToolboxNodeExport> nodes = [.. GetInstanceMethods(type), .. GetStaticMethods(type)];

            toolboxes[name] = [.. nodes];
        }
        private static IEnumerable<ToolboxNodeExport> GetStaticMethods(Type type)
        {
            IEnumerable<MethodInfo> methods = type
                            .GetMethods(BindingFlags.Public | BindingFlags.Static)
                            .Where(m => m.DeclaringType != typeof(object))
                            .OrderBy(t => t.Name);
            foreach (MethodInfo method in methods)
                yield return new ToolboxNodeExport(method.Name, method);
        }
        private static IEnumerable<ToolboxNodeExport> GetInstanceMethods(Type type)
        {
            IEnumerable<MethodInfo> methods = type
                            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            .Where(m => m.DeclaringType != typeof(object))
                            .OrderBy(t => t.Name);
            foreach (MethodInfo method in methods)
                yield return new ToolboxNodeExport(method.Name, method);
        }
        private static IEnumerable<ToolboxNodeExport?> GetExportNodesFromConvention(string name, Assembly assembly)
        {
            string formalName = $"{name.Replace(" ", string.Empty)}";
            string toolboxHelperTypeName = $"Parcel.Toolbox.{formalName}.{formalName}Helper";
            foreach (Type type in assembly
                .GetTypes()
                .Where(p => typeof(IToolboxDefinition).IsAssignableFrom(p)))
            {
                IToolboxDefinition? toolbox = (IToolboxDefinition?)Activator.CreateInstance(type);
                if (toolbox == null) continue;

                foreach (ToolboxNodeExport nodeExport in toolbox.ExportNodes)
                    yield return nodeExport;
            }
        }
        private static IEnumerable<ToolboxNodeExport?> GetExportNodesFromGenericAssembly(Assembly assembly)
        {
            Type[] types = assembly.GetExportedTypes()
                .Where(t => t.IsAbstract)
                .Where(t => t.Name != "Object")
                .ToArray();

            foreach (Type type in types)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    // Every static class seems to export the methods exposed by System.Object, i.e. Object.Equal, Object.ReferenceEquals, etc. and we don't want that. // Remark-cz: Might because of BindingFlags.FlattenHierarchy, now we removed that, this shouldn't be an issue, pending verfication
                    .Where(m => m.DeclaringType != typeof(object))
                    .ToArray();

                foreach (MethodInfo method in methods)
                    yield return new ToolboxNodeExport(method.Name, method);

                // Add divider
                yield return null;
            }
        }
        #endregion
    }
}
