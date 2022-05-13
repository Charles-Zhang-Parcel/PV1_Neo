﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Nodify;
using Parcel.Shared;
using Parcel.Shared.Algorithms;
using Parcel.Shared.Framework;
using Parcel.Shared.Framework.ViewModels;
using Parcel.Shared.Framework.ViewModels.BaseNodes;
using Parcel.Shared.Framework.ViewModels.Primitives;
using Parcel.Toolbox.Basic;
using Parcel.Toolbox.Basic.Nodes;
using Parcel.Toolbox.ControlFlow;
using Parcel.Toolbox.DataProcessing;
using Parcel.Toolbox.DataProcessing.Nodes;
using Parcel.Toolbox.FileSystem;
using Parcel.Toolbox.Finance;
using BaseConnection = Parcel.Shared.Framework.ViewModels.BaseConnection;
using PendingConnection = Parcel.Shared.Framework.ViewModels.PendingConnection;

namespace Parcel.FrontEnd.NodifyWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : BaseWindow
    {
        private const string _parcelWorkflowFileNameFilter = "Parcel workflow file (*.parcel)|*.parcel|YAML file (*.yaml)|*.yaml";

        #region Constructor
        public MainWindow()
        {
            RepeatLastCommand = new DelegateCommand(() => SpawnNode(LastTool, Editor.MouseLocation), 
                () => LastTool != null && !(FocusManager.GetFocusedElement(this) is TextBox) && !(Keyboard.FocusedElement is TextBox));
            SaveCanvasCommand = new DelegateCommand(() => SaveCanvas(false), () => true);
            NewCanvasCommand = new DelegateCommand(() => SaveCanvas(true), () => true);
            OpenCanvasCommand = new DelegateCommand(OpenCanvas, () => true);
            OpenWebHostCommand = new DelegateCommand(() => WebHostRuntime.Singleton.Open(), () => true);

            // WebAccessPointUrl = WebHostRuntime.Singleton?.BaseUrl;
            
            InitializeComponent();
            
            EventManager.RegisterClassHandler(typeof(Nodify.BaseConnection), MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnConnectionInteraction));
            EventManager.RegisterClassHandler(typeof(Nodify.Node), MouseLeftButtonDownEvent, new MouseButtonEventHandler(NodeDoubleclick_OpenProperties));
            EventManager.RegisterClassHandler(typeof(Nodify.GroupingNode), MouseLeftButtonDownEvent, new MouseButtonEventHandler(NodeDoubleclick_OpenProperties));
        }
        public NodesCanvas Canvas { get; set; } = new NodesCanvas();
        #endregion

        #region Commands
        private ToolboxNodeExport LastTool { get; set; }
        
        public ICommand RepeatLastCommand { get; }
        
        public ICommand CreateCalculatorNodeCommand { get; }
        public ICommand SaveCanvasCommand { get; }
        public ICommand NewCanvasCommand { get; }
        public ICommand OpenCanvasCommand { get; }
        
        public ICommand ShowHelpCommand { get; }
        public ICommand OpenWebHostCommand { get; }
        #endregion

        #region View Properties
        private const string TitlePrefix = "Parcel - Workflow Engine";
        private string _dynamicTitle = TitlePrefix;
        public string DynamicTitle { get => _dynamicTitle; set => SetField(ref _dynamicTitle, value); }
        private string _webAccessPointUrl;
        public string WebAccessPointUrl { get => _webAccessPointUrl; set => SetField(ref _webAccessPointUrl, value); }
        private string _currentFilePath;
        public string CurrentFilePath
        {
            get => _currentFilePath;
            set
            {
                SetField(ref _currentFilePath, value);
                DynamicTitle = $"{(Owner != null ? "<Reference>" : "<Main>")} {System.IO.Path.GetFileNameWithoutExtension(value)}";
            }
        }

        #endregion

        #region Advanced Node Graph Behaviors
        private void OnConnectionInteraction(object sender, MouseButtonEventArgs e)
        {
            if (sender is Nodify.BaseConnection ctrl && ctrl.DataContext is BaseConnection connection)
            {
                if (Keyboard.Modifiers == ModifierKeys.Alt)
                {
                    connection.Remove();
                }
                else if (e.ClickCount > 1)
                {
                    connection.Split(e.GetPosition(ctrl) - new Vector(30, 15));
                }
            }
        }
        #endregion

        #region Events
        private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                ShowSearchNodePopup();
                e.Handled = true;
            }
        }
        private void Editor_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Use MouseLeftButtonDown instead of MouseDoubleClick event to deal with WPF's e.handled not effective issue
            if (e.ClickCount != 2) return;
            
            OpenCanvas();
            e.Handled = true;

        }
        private void Editor_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowSearchNodePopup();
            e.Handled = true;
        }
        private void NodeDoubleclick_OpenProperties(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2) return;
            if (!(e.Source is Nodify.Node {DataContext: ProcessorNode processorNode})
                && !(e.Source is Nodify.GroupingNode {DataContext: CommentNode commentNode})) return;

            if (e.Source is Nodify.Node node)
                SpawnPropertyWindow(node.DataContext as BaseNode);
            else if (e.Source is Nodify.GroupingNode groupingNode)
                SpawnPropertyWindow(groupingNode.DataContext as CommentNode);
            
            e.Handled = true;
        }
        private void OpenFileNode_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is Button {DataContext: OpenFileNode node})) return;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select File to Open",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                node.Path = openFileDialog.FileName;
            }
            e.Handled = true;
        }
        private void ProcessorNodeTogglePreviewButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Border {Tag: ProcessorNode node} border)) return;

            node.IsPreview = !node.IsPreview;
            e.Handled = true;
        }
        private void ProcessorNodePreviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button {Tag: ProcessorNode node} button)) return;

            // Auto-Generate
            if ((node is CSV || node is Excel) && node.ShouldHaveConnection)
            {
                OpenFileNode filePathNode = SpawnNode(new ToolboxNodeExport("File Input", typeof(OpenFileNode)),
                    node.Location + new Vector(-200, -60)) as OpenFileNode;
                Canvas.Schema.TryAddConnection(filePathNode!.MainOutput, node.Input.First());

                OpenFileDialog openFileDialog = new OpenFileDialog() { Title = "Select File to Open" };
                openFileDialog.Filter = node switch
                {
                    CSV _ => "CSV file (*.csv)|*.csv|All types (*.*)|*.*",
                    Excel _ => "Excel file (*.xlsx)|*.xlsx|All types (*.*)|*.*",
                    _ => openFileDialog.Filter
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    filePathNode.Path = openFileDialog.FileName;
                }
            }
            else if (node is IAutoConnect autoConnect && autoConnect.ShouldHaveConnection && node.AutoGenerateNodes != null)
            {
                foreach (Tuple<ToolboxNodeExport,Vector,InputConnector> generateNode in autoConnect.AutoGenerateNodes)
                {
                    BaseNode temp = SpawnNode(generateNode.Item1, node.Location + generateNode.Item2);
                    if(temp is IMainOutputNode outputNode)
                        Canvas.Schema.TryAddConnection(outputNode.MainOutput, generateNode.Item3);
                }
                
                e.Handled = true;
                return;
            }
            
            // Connection check
            if (node.ShouldHaveConnection && node.AutoGenerateNodes == null)
            {
                node.Message.Content = "Require Connection.";
                node.Message.Type = NodeMessageType.Error;
                
                e.Handled = true;
                return;
            }
            
            // This is a good chance to auto-save, before anything can crash
            if (CurrentFilePath != null || System.IO.File.Exists(CurrentFilePath))
                Canvas.Save(GetAutoSavePath(CurrentFilePath));
            
            node.IsPreview = true;
            if (!(node is IWebPreviewProcessorNode)) // IWebPreviewProcessorNode opens web preview during execution 
                SpawnPreviewWindow(node);
            if (!(node is GraphReference reference) || _graphPreviewWindows.ContainsKey(reference) || _previewWindows.ContainsKey(reference))  // For graph reference we really don't want to execute it during preview the first time
                ExecuteAll();

            e.Handled = true;
        }
        private void WebAccessUrlDisplayElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            WebHostRuntime.Singleton.Open();
            e.Handled = true;
        }
        private void HostAddressNodeUIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            WebHostRuntime.Singleton.Open((sender as Label).Content as string);
            e.Handled = true;
        }
        #endregion

        #region Routine
        private BaseNode SpawnNode(ToolboxNodeExport tool, Point spawnLocation)
        {
            if (tool.Descriptor != null && tool.Type != typeof(AutomaticProcessorNode))
                throw new ArgumentException("Wrong type.");
            
            BaseNode node = tool.Descriptor != null 
                ? new AutomaticProcessorNode(tool.Descriptor, tool.Toolbox)
                : (BaseNode) Activator.CreateInstance(tool.Type);

            node!.Location = spawnLocation;
            Canvas.Nodes.Add(node);
            return node;
        }
        private void SpawnPreviewWindow(ProcessorNode node)
        {
            if (_previewWindows.ContainsKey(node))
            {
                _previewWindows[node].Activate();
            }
            else
            {
                if (node is GraphReference graph)
                {
                    if (graph.GraphPath == null)
                        InitializeGraphReferenceNode(graph);
                    if (graph.GraphPath == null) return;
                
                    MainWindow graphPreview = new MainWindow()
                    {
                        Owner = this,
                        CurrentFilePath = graph.GraphPath,
                    };
                    graphPreview.Canvas.Open(graph.GraphPath);
                    _graphPreviewWindows[graph] = graphPreview;
                    graphPreview.Closed += (sender, args) => _graphPreviewWindows.Remove(graph);
                    graphPreview.Show();
                }
                
                PreviewWindow preview = new PreviewWindow(this, node);
                _previewWindows.Add(node, preview);
                preview.Closed += (sender, args) => _previewWindows.Remove((sender as PreviewWindow)!.Node); 
                preview.Show();   
            }
        }
        private void SpawnPropertyWindow(BaseNode node)
        {
            if (node is GraphReference graphReference)
            {
                InitializeGraphReferenceNode(graphReference);
                return;
            }
            
            Point cursor = GetCurosrWindowPosition();
            if(node is ProcessorNode processorNode)
                new PropertyWindow(this, processorNode)
                {
                    Left = cursor.X,
                    Top = cursor.Y
                }.Show();
            else if(node is CommentNode commentNode)
                new CommentWindow(this, commentNode)
                {
                    Left = cursor.X,
                    Top = cursor.Y
                }.Show();
        }
        private static void InitializeGraphReferenceNode(GraphReference graphReference)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Existing Workflow",
                Filter = _parcelWorkflowFileNameFilter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                graphReference.GraphPath = openFileDialog.FileName;
                graphReference.Title = System.IO.Path.GetFileNameWithoutExtension(graphReference.GraphPath);
            }
        }
        private void ExecuteAll()
        {
            AlgorithmHelper.ExecuteGraph(Canvas);
            foreach (PreviewWindow p in _previewWindows.Values)
                p.Update();
        }
        private void ShowSearchNodePopup()
        {
            Point cursor = GetCurosrWindowPosition();
            Point spawnLocation = Editor.MouseLocation;

            PopupTab popupTab = new PopupTab(this)
            {
                Left = cursor.X,
                Top = cursor.Y,
                Topmost = true
            };
            void action(ToolboxNodeExport toolboxNodeExport)
            {
                if (toolboxNodeExport != null 
                    && toolboxNodeExport.Type != typeof(object)) // Don't do anything for placeholder nodes
                {
                    LastTool = toolboxNodeExport;
                    SpawnNode(LastTool, spawnLocation);
                }
            }
            popupTab.ItemSelected += action;
            popupTab.MouseLeave += delegate { popupTab.Close(); };  // ISSUE: This will cause Closed event being called before the OnClick event
            popupTab.Show();
        }
        private void OpenCanvas()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Parcel Workflow File",
                Filter = _parcelWorkflowFileNameFilter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentFilePath = openFileDialog.FileName;
                Canvas.Open(CurrentFilePath);
            }
        }
        private void SaveCanvas(bool createNewFile = false)
        {
            if (createNewFile || CurrentFilePath == null || !System.IO.File.Exists(CurrentFilePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Choose Where to Save Current Workflow",
                    Filter = _parcelWorkflowFileNameFilter
                };
                if (saveFileDialog.ShowDialog() == true)
                    CurrentFilePath = saveFileDialog.FileName;
                else return;
            }

            Canvas.Save(CurrentFilePath);
        }
        private Point GetCurosrWindowPosition()
        {
            Point cursor = Mouse.GetPosition(this);
            RECT rect = GetWindowRectangle(this);
            return new Point((this.WindowState == WindowState.Maximized ? rect.Left : this.Left) + cursor.X,
                (this.WindowState == WindowState.Maximized ? rect.Top : this.Top) + cursor.Y);
        }
        private string GetAutoSavePath(string currentFilePath)
        {
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(currentFilePath);
            string fileFolder = System.IO.Path.GetDirectoryName(currentFilePath);
            string extension = System.IO.Path.GetExtension(currentFilePath);

            string dateString = DateTime.Now.ToString("yyyyMMdd HH-mm-ss");
            string newFilePath = System.IO.Path.Combine(fileFolder!, $"{fileNameWithoutExtension}_{dateString}{extension}");
            return newFilePath;
        }
        #endregion

        #region State
        private readonly Dictionary<ProcessorNode, PreviewWindow> _previewWindows =
            new Dictionary<ProcessorNode, PreviewWindow>();
        private readonly Dictionary<GraphReference, MainWindow> _graphPreviewWindows =
            new Dictionary<GraphReference, MainWindow>();
        #endregion

        #region Interop
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        // Make sure RECT is actually OUR defined struct, not the windows rect.
        public static RECT GetWindowRectangle(Window window)
        {
            RECT rect;
            GetWindowRect((new WindowInteropHelper(window)).Handle, out rect);

            return rect;
        }
        #endregion
    }
}