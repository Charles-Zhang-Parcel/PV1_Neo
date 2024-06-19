using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Toolboxes.Basic;

namespace Parcel.Neo
{
    public partial class PopupTab : BaseWindow
    {
        public PopupTab(Window owner)
        {
            Dictionary<string, Assembly> toolboxAssemblies = [];
            // Register (old-fashioned) assemblies
            RegisterToolbox(toolboxAssemblies, "Basic", Assembly.GetAssembly(typeof(ToolboxDefinition)));
            RegisterToolbox(toolboxAssemblies, "Control Flow", Assembly.GetAssembly(typeof(Toolbox.ControlFlow.ToolboxDefinition)));
            RegisterToolbox(toolboxAssemblies, "Data Processing", Assembly.GetAssembly(typeof(Toolbox.DataProcessing.ToolboxDefinition)));
            RegisterToolbox(toolboxAssemblies, "Data Source", Assembly.GetAssembly(typeof(Toolbox.DataSource.ToolboxDefinition)));
            RegisterToolbox(toolboxAssemblies, "File System", Assembly.GetAssembly(typeof(Toolbox.FileSystem.ToolboxDefinition)));
            RegisterToolbox(toolboxAssemblies, "Finance", Assembly.GetAssembly(typeof(Toolbox.Finance.ToolboxDefinition)));
            RegisterToolbox(toolboxAssemblies, "Generator", Assembly.GetAssembly(typeof(Toolbox.Generator.ToolboxDefinition)));
            RegisterToolbox(toolboxAssemblies, "Logic", Assembly.GetAssembly(typeof(Toolbox.Logic.ToolboxDefinition)));
            RegisterToolbox(toolboxAssemblies, "Math", Assembly.GetAssembly(typeof(Toolbox.Math.ToolboxDefinition)));
            RegisterToolbox(toolboxAssemblies, "String", Assembly.GetAssembly(typeof(Toolbox.String.ToolboxDefinition)));
            RegisterToolbox(toolboxAssemblies, "Special", Assembly.GetAssembly(typeof(Toolbox.Special.ToolboxDefinition)));
            // Register Parcel packages
            foreach (var package in GetPackages())
            {
                try
                {
                    RegisterToolbox(toolboxAssemblies, package.Name, Assembly.LoadFrom(package.Path));
                }
                catch (Exception) { continue; }
            }
            // Index nodes
            Dictionary<string, ToolboxNodeExport[]> toolboxes = IndexToolboxes(toolboxAssemblies);

            Owner = owner;
            InitializeComponent();

            // Additional setup
            PopulateToolboxItems(toolboxes);
            SearchTextBox.Focus();
        }

        #region States
        private List<ToolboxNodeExport> _availableNodes;
        private Dictionary<string, ToolboxNodeExport> _searchResultLookup;
        #endregion

        #region View Properties
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetField(ref _searchText, value);
                UpdateSearch(_searchText);
            }
        }
        private ObservableCollection<string> _searchResults;
        public ObservableCollection<string> SearchResults
        {
            get => _searchResults;
            set => SetField(ref _searchResults, value);
        }
        private Visibility _defaultCategoriesVisibility = Visibility.Visible;
        public Visibility DefaultCategoriesVisibility
        {
            get => _defaultCategoriesVisibility;
            set => SetField(ref _defaultCategoriesVisibility, value);
        }
        private Visibility _searchResultsVisibility = Visibility.Collapsed;
        public Visibility SearchResultsVisibility
        {
            get => _searchResultsVisibility;
            set => SetField(ref _searchResultsVisibility, value);
        }
        #endregion

        #region Routines
        private void AddMenuItem(ToolboxNodeExport? node, MenuItem topMenu)
        {
            if (node == null)
                topMenu.Items.Add(new Separator());
            else
            {
                MenuItem item = new() { Header = node.Name, Tag = node };
                item.Click += NodeMenuItemOnClick;
                topMenu.Items.Add(item);
                
                _availableNodes.Add(node);
            }
        }
        private void UpdateSearch(string searchText)
        {
            _searchResultLookup = [];
            SearchResults = new ObservableCollection<string>(_availableNodes
                .Where(n => n.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                .Select(n =>
                {
                    string key = $"{n.Toolbox.ToolboxName} -> {n.Name}";
                    _searchResultLookup[key] = n;
                    return key;
                }));

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                DefaultCategoriesVisibility = Visibility.Collapsed;
                SearchResultsVisibility = Visibility.Visible;    
            }
            else
            {
                DefaultCategoriesVisibility = Visibility.Visible;
                SearchResultsVisibility = Visibility.Collapsed;
            }
        }
        private void PopulateToolboxItems(Dictionary<string, ToolboxNodeExport[]> toolboxes)
        {
            // TODO: The setup here need complete changes to conform to POS assembly formats
            _availableNodes = [];
                
            foreach ((string Name, ToolboxNodeExport[] Nodes) in toolboxes)
            {
                // Create menu instance
                Menu menu = new();
                MenuItem topMenu = new()
                {
                    Header = Name, 
                    Width = Width * 0.8,
                };
                menu.Items.Add(topMenu);

                // Add to menu
                foreach (ToolboxNodeExport export in Nodes)
                    AddMenuItem(export, topMenu);

                // Add menu to GUI
                ModulesListView.Items.Add(menu);
            }
        }
        private static Dictionary<string, ToolboxNodeExport[]> IndexToolboxes(Dictionary<string, Assembly> assemblies)
        {
            Dictionary<string, ToolboxNodeExport[]> toolboxes = [];

            foreach (string name in assemblies.Keys.OrderBy(k => k))
            {
                Assembly assembly = assemblies[name];
                toolboxes[name] = [];

                // Load either old PV1 toolbox or new Parcel package
                IEnumerable<ToolboxNodeExport?>? exportedNodes = null;
                if (assembly
                    .GetTypes()
                    .Any(p => typeof(IToolboxDefinition).IsAssignableFrom(p)))
                {
                    // Loading per old PV1 convention
                    exportedNodes = GetExportNodesFromConvention(name, assembly);
                }
                else
                {
                    // Remark-cz: In the future we will utilize Parcel.CoreEngine.Service for this
                    // Load generic Parcel package
                    exportedNodes = GetExportNodesFromGenericAssembly(name, assembly);
                }
                toolboxes[name] = exportedNodes!.Select(n => n!).ToArray();
            }

            return toolboxes;
        }
        #endregion

        #region Helpers
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
        private void RegisterToolbox(Dictionary<string, Assembly> toolboxAssemblies, string name, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentException($"Assembly is null.");

            if (toolboxAssemblies.ContainsKey(name))
                throw new InvalidOperationException($"Assembly `{assembly.FullName}` is already registered.");

            toolboxAssemblies.Add(name, assembly);
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
                {
                    if (nodeExport != null) 
                        nodeExport.Toolbox = toolbox;
                    yield return nodeExport;
                }
                foreach (AutomaticNodeDescriptor definition in toolbox.AutomaticNodes)
                    yield return definition == null
                        ? null
                        : new ToolboxNodeExport(definition.NodeName, typeof(AutomaticProcessorNode))
                        {
                            Descriptor = definition,
                            Toolbox = toolbox,
                        };
            }
        }
        private static IEnumerable<ToolboxNodeExport?> GetExportNodesFromGenericAssembly(string name, Assembly assembly)
        {
            Type[] types = assembly.GetExportedTypes()
                .Where(t => t.IsAbstract)
                .Where(t => t.Name != "Object")
                .ToArray();
            GenericToolbox toolbox = new()
            {
                ToolboxName = name,
                ToolboxAssemblyFullName = assembly.FullName,
            };

            foreach (Type type in types)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    // Every static class seems to export the methods exposed by System.Object, i.e. Object.Equal, Object.ReferenceEquals, etc. and we don't want that. // Remark-cz: Might because of BindingFlags.FlattenHierarchy, now we removed that, this shouldn't be an issue, pending verfication
                    .Where(m => m.DeclaringType != typeof(object))
                    .ToArray();

                foreach (MethodInfo method in methods)
                {
                    Type[] parameterTypes = method.GetParameters().Select(p => p.GetType()).ToArray();
                    Type returnType = method.ReturnType;
                    ToolboxNodeExport export;
                    try
                    {
                        export = new ToolboxNodeExport(method.Name, typeof(AutomaticProcessorNode))
                        {
                            Descriptor = new AutomaticNodeDescriptor(method.Name,
                            parameterTypes.Select(CacheTypeHelper.ConvertToCacheDataType).ToArray(),
                            CacheTypeHelper.ConvertToCacheDataType(returnType),
                            objects => method.Invoke(null, objects)
                        ),
                            Toolbox = toolbox,
                        };
                    }
                    catch (Exception) { continue; }

                    yield return export;
                }
            }
        }
        #endregion

        #region Interface
        public Action<ToolboxNodeExport> ItemSelectedAdditionalCallback { get; set; }
        #endregion

        #region GUI Events
        private void PopupTab_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void PopupTab_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                Close();
        }
        private void NodeMenuItemOnClick(object sender, RoutedEventArgs e)
        {
            if (e.Source is not MenuItem item || item.Tag == null) return;
            
            ToolboxNodeExport? toolSelection = item.Tag as ToolboxNodeExport;
            ItemSelectedAdditionalCallback(toolSelection);
            Close();
        }
        private void SearchResultsListViewLabel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ItemSelectedAdditionalCallback(_searchResultLookup[(((Label) sender).Content as string)!]);
            Close();
        }
        private void SearchResultsListView_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SearchResults.Count != 0)
            {
                ItemSelectedAdditionalCallback(_searchResultLookup[(string)((ListBox) sender).SelectedItem ?? SearchResults.First()]);
                Close();
                e.Handled = true;
            }
        }
        private void SearchTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SearchResults.Count >= 1)
            {
                ToolboxNodeExport export = _searchResultLookup[SearchResults.First()];
                ItemSelectedAdditionalCallback(export);
                e.Handled = true;
                Close();
            }
            else if (e.Key == Key.Up || e.Key == Key.Down)
            {
                SearchResultsListView.Focus();
                e.Handled = true;
            }
        }
        #endregion
    }
}