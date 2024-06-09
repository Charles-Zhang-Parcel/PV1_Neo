using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Parcel.Neo.Shared;
using Parcel.Neo.Shared.Framework;
using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo
{
    public partial class PopupTab : BaseWindow
    {
        public PopupTab(Window owner)
        {
            _registry.RegisterToolbox("Basic", Assembly.GetAssembly(typeof(Parcel.Toolbox.Basic.ToolboxDefinition)));
            _registry.RegisterToolbox("Control Flow", Assembly.GetAssembly(typeof(Parcel.Toolbox.ControlFlow.ToolboxDefinition)));
            _registry.RegisterToolbox("Data Processing", Assembly.GetAssembly(typeof(Parcel.Toolbox.DataProcessing.ToolboxDefinition)));
            _registry.RegisterToolbox("Data Source", Assembly.GetAssembly(typeof(Parcel.Toolbox.DataSource.ToolboxDefinition)));
            _registry.RegisterToolbox("File System", Assembly.GetAssembly(typeof(Parcel.Toolbox.FileSystem.ToolboxDefinition)));
            _registry.RegisterToolbox("Finance", Assembly.GetAssembly(typeof(Parcel.Toolbox.Finance.ToolboxDefinition)));
            _registry.RegisterToolbox("Generator", Assembly.GetAssembly(typeof(Parcel.Toolbox.Generator.ToolboxDefinition)));
            _registry.RegisterToolbox("Graphing", Assembly.GetAssembly(typeof(Parcel.Toolbox.Graphing.ToolboxDefinition)));
            _registry.RegisterToolbox("Logic", Assembly.GetAssembly(typeof(Parcel.Toolbox.Logic.ToolboxDefinition)));
            _registry.RegisterToolbox("Math", Assembly.GetAssembly(typeof(Parcel.Toolbox.Math.ToolboxDefinition)));
            _registry.RegisterToolbox("Plotting", Assembly.GetAssembly(typeof(Parcel.Toolbox.Plotting.ToolboxDefinition)));
            _registry.RegisterToolbox("Present", Assembly.GetAssembly(typeof(Parcel.Toolbox.Present.ToolboxDefinition)));
            _registry.RegisterToolbox("Scripting", Assembly.GetAssembly(typeof(Parcel.Toolbox.Scripting.ToolboxDefinition)));
            _registry.RegisterToolbox("String", Assembly.GetAssembly(typeof(Parcel.Toolbox.String.ToolboxDefinition)));
            _registry.RegisterToolbox("Special", Assembly.GetAssembly(typeof(Parcel.Toolbox.Special.ToolboxDefinition)));

            Owner = owner;
            InitializeComponent();

            // Additional setup
            PopulateToolboxItems();
            SearchTextBox.Focus();
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
            _availableNodes = new List<ToolboxNodeExport>();
                
            foreach (string name in _registry.Toolboxes.Keys.OrderBy(k => k))
            {
                Menu menu = new Menu
                {
                    // Margin = new Thickness(1)
                };
                MenuItem topMenu = new MenuItem
                {
                    // Padding = new Thickness(4),
                    Header = name, 
                    Width = this.Width * 0.8,
                };
                menu.Items.Add(topMenu);
                
                string formalName = $"{name.Replace(" ", String.Empty)}"; 
                string toolboxHelperTypeName = $"Parcel.Toolbox.{formalName}.{formalName}Helper";
                foreach (Type type in _registry.Toolboxes[name]
                    .GetTypes().Where(p => typeof(IToolboxEntry).IsAssignableFrom(p)))
                {
                    IToolboxEntry toolbox = (IToolboxEntry)Activator.CreateInstance(type);
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

        #region View Properties
        #endregion

        #region GUI Events
        private void PopupTab_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void PopupTab_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                this.Close();
        }
        private void NodeMenuItemOnClick(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is MenuItem item) || item.Tag == null) return;
            
            ToolboxNodeExport toolSelection = item.Tag as ToolboxNodeExport;
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