﻿using Parcel.Neo.Base.Toolboxes.Basic;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using System.IO;
using Humanizer;
using Parcel.CoreEngine.Helpers;

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
            RegisterToolbox(toolboxAssemblies, "Large Language Model", Assembly.Load("Parcel.LLM"));
            RegisterToolbox(toolboxAssemblies, "In-Memory Database", Assembly.Load("Parcel.InMemoryDB"));
            RegisterToolbox(toolboxAssemblies, "Database Service", Assembly.Load("Parcel.InMemoryDB.Integration"));
            RegisterToolbox(toolboxAssemblies, "Database Application", Assembly.Load("Parcel.InMemoryDB.WebSurveys"));
            RegisterToolbox(toolboxAssemblies, "File System", Assembly.Load("Parcel.FileSystem"));
            RegisterToolbox(toolboxAssemblies, "Yahoo Finance", Assembly.Load("Parcel.YahooFinance"));
            
            // Index specific nodes
            Dictionary<string, ToolboxNodeExport?[]> toolboxes = IndexToolboxes(toolboxAssemblies);
            // Register front-end specific toolboxes (In general we try to eliminate those, or to say the least standardization effort is needed to make sure those are understood across implementations
            AddToolbox(toolboxes, "Basic", new BasicToolbox());
            // Register specific types - Parcel Standard
            RegisterType(toolboxes, "Data Grid", typeof(Types.DataGrid));
            RegisterType(toolboxes, "Data Grid", typeof(Types.DataGridOperationsHelper));
            RegisterType(toolboxes, "Data Grid", typeof(Integration.DataGridIntegration));
            RegisterType(toolboxes, "Data Grid", typeof(Integration.DataProcessingHelper));
            RegisterType(toolboxes, "Math", typeof(Processing.Utilities.Calculator));
            RegisterType(toolboxes, "String Processing", typeof(Standard.Types.StringRoutines));
            RegisterType(toolboxes, "Boolean Logic", typeof(Standard.Types.BooleanRoutines));
            RegisterType(toolboxes, "Boolean Logic", typeof(Standard.Types.LogicRoutines));
            // Register specific types - directly borrow from libraries
            RegisterType(toolboxes, "Collections", typeof(Enumerable));
            RegisterType(toolboxes, "Statistics", typeof(MathNet.Numerics.Statistics.Statistics)); // TODO: Might provide selective set of functions instead of everything; Alternative, figure out how to do in-app documentation
            RegisterType(toolboxes, "Statistics", typeof(MathNet.Numerics.Statistics.Correlation));
            RegisterType(toolboxes, "String Processing", typeof(InflectorExtensions));
            RegisterType(toolboxes, "Console", typeof(System.Console));
            // Remark: Notice that boolean algebra and String are available in PSL - Pending deciding whether we need dedicated exposure

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
        private static void AddToolbox(Dictionary<string, ToolboxNodeExport?[]> toolboxes, string name, IToolboxDefinition toolbox)
        {
            List<ToolboxNodeExport?> nodes = [];

            foreach (ToolboxNodeExport? nodeExport in toolbox?.ExportNodes ?? [])
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
        private static void RegisterType(Dictionary<string, ToolboxNodeExport?[]> toolboxes, string name, Type type)
        {
            List<ToolboxNodeExport> nodes = [.. GetInstanceMethods(type), .. GetStaticMethods(type)];

            if (toolboxes.ContainsKey(name))
                // Add divider
                toolboxes[name] = [.. toolboxes[name], null, .. nodes];
            else
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
            // Try get enhanced annotation
            string xmlDocumentationPath = GetDefaultXMLDocumentationPath(assembly.Location);
            Dictionary<string, string>? nodeSummary = null;
            if (File.Exists(assembly.Location) && File.Exists(xmlDocumentationPath))
            {
                DocumentationHelper.Documentation documentation = DocumentationHelper.ParseXML(xmlDocumentationPath);
                nodeSummary = documentation.Members
                    .Where(m => m.MemberType == DocumentationHelper.MemberType.Member)
                    .ToDictionary(m => m.Signature, m => m.Summary);
            }

            // Export nodes from types
            Type[] types = assembly.GetExportedTypes()
                .Where(t => t.IsAbstract) // TODO: Support instance methods
                .Where(t => t.Name != "Object")
                .ToArray();
            foreach (Type type in types)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    // Every static class seems to export the methods exposed by System.Object, i.e. Object.Equal, Object.ReferenceEquals, etc. and we don't want that. // Remark-cz: Might because of BindingFlags.FlattenHierarchy, now we removed that, this shouldn't be an issue, pending verfication
                    .Where(m => m.DeclaringType != typeof(object))
                    .ToArray();

                foreach (MethodInfo method in methods)
                {
                    string? tooltip = null;
                    string signature = GenerateMethodSignature(method);
                    nodeSummary?.TryGetValue(signature, out tooltip);
                    yield return new ToolboxNodeExport(method.Name, method)
                    {
                        Tooltip = tooltip
                    };
                }

                // Add divider
                yield return null;
            }

            static string GetDefaultXMLDocumentationPath(string assemblyLocation)
            {
                string filename = Path.GetFileNameWithoutExtension(assemblyLocation);
                string extension = ".xml";
                string folder = Path.GetDirectoryName(assemblyLocation);
                return Path.Combine(folder, filename + extension);
            }
            static string GenerateMethodSignature(MethodInfo methodInfo)
            {
                return $"M:{methodInfo.DeclaringType.FullName}.{methodInfo.Name}({string.Join(",", methodInfo.GetParameters().Select(p => p.ParameterType.FullName))})";
            }
        }
        #endregion
    }
}
