using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Parcel.Neo.Base;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo
{
    public partial class PopupTab : BaseWindow
    {
        public PopupTab(Window owner)
        {
            _registry.RegisterToolbox("Basic", Assembly.GetAssembly(typeof(Toolbox.Basic.ToolboxDefinition)));
            _registry.RegisterToolbox("Control Flow", Assembly.GetAssembly(typeof(Toolbox.ControlFlow.ToolboxDefinition)));
            _registry.RegisterToolbox("Data Processing", Assembly.GetAssembly(typeof(Toolbox.DataProcessing.ToolboxDefinition)));
            _registry.RegisterToolbox("Data Source", Assembly.GetAssembly(typeof(Toolbox.DataSource.ToolboxDefinition)));
            _registry.RegisterToolbox("File System", Assembly.GetAssembly(typeof(Toolbox.FileSystem.ToolboxDefinition)));
            _registry.RegisterToolbox("Finance", Assembly.GetAssembly(typeof(Toolbox.Finance.ToolboxDefinition)));
            _registry.RegisterToolbox("Generator", Assembly.GetAssembly(typeof(Toolbox.Generator.ToolboxDefinition)));
            _registry.RegisterToolbox("Logic", Assembly.GetAssembly(typeof(Toolbox.Logic.ToolboxDefinition)));
            _registry.RegisterToolbox("Math", Assembly.GetAssembly(typeof(Toolbox.Math.ToolboxDefinition)));
            _registry.RegisterToolbox("String", Assembly.GetAssembly(typeof(Toolbox.String.ToolboxDefinition)));
            _registry.RegisterToolbox("Special", Assembly.GetAssembly(typeof(Toolbox.Special.ToolboxDefinition)));

            // Register packages
            foreach (var package in GetPackages())
                _registry.RegisterToolbox(package.Name, Assembly.LoadFrom(package.Path));

            Owner = owner;
            InitializeComponent();

            // Additional setup
            PopulateToolboxItems();
            SearchTextBox.Focus();

            static (string Name, string Path)[] GetPackages()
            {
                string packageImportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Parcel NExT", "Packages");

                string? environmentOverride = Environment.GetEnvironmentVariable("PARCEL_PACKAGES");
                if (environmentOverride != null && Directory.Exists(environmentOverride))
                    packageImportPath = environmentOverride;

                if (Directory.Exists(packageImportPath))
                    return Directory
                        .EnumerateFiles(packageImportPath)
                        .Where(file => Path.GetExtension(file).ToLower() == ".dll")
                        .Select(file => (Path.GetFileNameWithoutExtension(file), file))
                        .ToArray();
                return [];
            }
        }

        #region States
        private readonly ToolboxRegistry _registry = new ToolboxRegistry();
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
        private void AddMenuItem(ToolboxNodeExport node, MenuItem topMenu)
        {
            if (node == null)
                topMenu.Items.Add(new Separator());
            else
            {
                MenuItem item = new MenuItem {Header = node.Name, Tag = node};
                item.Click += NodeMenuItemOnClick;
                topMenu.Items.Add(item);
                
                _availableNodes.Add(node);
            }
        }
        private void UpdateSearch(string searchText)
        {
            _searchResultLookup = new Dictionary<string, ToolboxNodeExport>();
            SearchResults = new ObservableCollection<string>(_availableNodes
                .Where(n => n.Name.ToLower().Contains(searchText.ToLower()))
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
        private void PopulateToolboxItems()
        {
            // TODO: The setup here need complete changes to conform to POS assembly formats
            _availableNodes = [];
                
            foreach (string name in _registry.Toolboxes.Keys.OrderBy(k => k))
            {
                Menu menu = new();
                MenuItem topMenu = new()
                {
                    Header = name, 
                    Width = Width * 0.8,
                };
                menu.Items.Add(topMenu);
                
                string formalName = $"{name.Replace(" ", string.Empty)}"; 
                string toolboxHelperTypeName = $"Parcel.Toolbox.{formalName}.{formalName}Helper";
                foreach (Type type in _registry.Toolboxes[name]
                    .GetTypes().Where(p => typeof(IToolboxEntry).IsAssignableFrom(p)))
                {
                    IToolboxEntry? toolbox = (IToolboxEntry?)Activator.CreateInstance(type);
                    if (toolbox == null) continue;

                    foreach (ToolboxNodeExport nodeExport in toolbox.ExportNodes)
                    {
                        if (nodeExport != null) nodeExport.Toolbox = toolbox;
                        AddMenuItem(nodeExport, topMenu);
                    }
                    foreach (AutomaticNodeDescriptor definition in toolbox.AutomaticNodes)
                        AddMenuItem(definition == null ? null : new ToolboxNodeExport(definition.NodeName, typeof(AutomaticProcessorNode))
                        {
                            Descriptor = definition,
                            Toolbox = toolbox,
                        }, topMenu);
                }

                ModulesListView.Items.Add(menu);
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