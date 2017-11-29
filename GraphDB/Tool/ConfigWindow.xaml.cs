using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using GraphDB.Contract.Core;
using GraphDB.Contract.Enum;
using GraphDB.Core;
using GraphDB.Tool.Drawing;
using GraphDB.Tool.Layout;

using Microsoft.Win32;
using Microsoft.Windows.Controls.Ribbon;

//&lt; < 小于号 
//&gt; > 大于号 
//&amp; & 和 
//&apos; ' 单引号 
//&quot; " 双引号 
//(&#x0020;)  空格 
//(&#x0009;) Tab 
//(&#x000D;) 回车 
//(&#x000A;) 换行 

namespace GraphDB.Tool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWindow
    {
        Graph myGdb;
        private GraphRenderer myGraphRenderer;
        //MainDataSet DataSet;
        private readonly string myDataBasePath;

        bool myIsDbAvailable = false;
        bool myIsModified = false;
        DispatcherTimer myStatusUpadteTimer;
        int myIntNodeIndex = -1;
        int myIntPointNodeIndex = -1;
        INode myCurModifyNode;
        IEdge myCurModifyEdge;
        NodeInfo myCurSelectNode;
        
        public ConfigWindow(string dbPath)
        {
            InitializeComponent();
            myDataBasePath = dbPath;
        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowReset();
        }

        private void WindowReset()
        {
            AllReset();
            myGraphRenderer = new GraphRenderer();
            ChangeStyle("默认样式");
            StatusUpdateTimer_Init();
            myGdb = new Graph(myDataBasePath);
            FillNodeList();
            myIsModified = false;
            myIsDbAvailable = true;
        }

        private void FillNodeList()
        {
            NodeListBox.Items.Clear();
            int index = 0;
            foreach (var curItem in myGdb.Nodes)
            {
                NodeListBox.Items.Add( $"{index} Name:{curItem.Value.Name} Type:{curItem.Value.GetType().Name}" );
                index++;
            }
        }

        //完全重置
        private void AllReset()
        {
            myGdb = null;
            myCurModifyNode = null;
            myCurModifyEdge = null;
            myIsDbAvailable = false;
            myIntNodeIndex = -1;
            myIntPointNodeIndex = -1;
            SetCurrentNodeInfo(-1);
            NodeListBox.Items.Clear();
            ClearArrows(DrawingSurface);
            DrawingSurface.ClearVisuals();
            //Todo
            //ModifyEndName.Items.Clear();
            //ModifyEndType.Items.Clear();
            //RemoveEndName.Items.Clear();
            //RemoveEndType.Items.Clear();
        }
        //节点更新
        private void GraphNodeUpdate()
        {
            myCurModifyNode = null;
            myCurModifyEdge = null;
            myIntNodeIndex = -1;
            SetCurrentNodeInfo(-1);
            NodeListBox.Items.Clear();
            ClearArrows(DrawingSurface);
            DrawingSurface.ClearVisuals();
            //Todo
            //ModifyEndName.Items.Clear();
            //ModifyEndType.Items.Clear();
            //RemoveEndName.Items.Clear();
            //RemoveEndType.Items.Clear();
            FillNodeList();
        }
        //连边更新
        private void GraphEdgeUpdate()
        {
            myCurModifyEdge = null;
            ClearArrows(DrawingSurface);
            DrawingSurface.ClearVisuals();
            SelectNodes(myIntNodeIndex);
            FindCustomNode(myCurNodeName, myCurNodeType);
        }

        #region StatusTimer
        private void StatusUpdateTimer_Init()
        {
            myStatusUpadteTimer = new DispatcherTimer();
            myStatusUpadteTimer.Interval = new TimeSpan(0, 0, 3);
            myStatusUpadteTimer.Tick += StatusUpdateTimer_Tick;
            myStatusUpadteTimer.IsEnabled = false;
        }

        private void StatusUpdateTimer_Tick(object sender, EventArgs e)
        {
            StatusLabel.Content = "Ready";
            myStatusUpadteTimer.IsEnabled = false;
        }

        public void ShowStatus(string sStatus)
        {
            StatusLabel.Content = sStatus;
            myStatusUpadteTimer.Start();
        }
        #endregion
        
        #region FileCommand
        //新建命令执行函数
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ErrorCode err = ErrorCode.NoError;

            if (myIsDbAvailable == true)
            {
                var choice = MessageBox.Show("Save current graph database to file？", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (choice == MessageBoxResult.Yes)
                {
                    //保存网络
                    myGdb.SaveDataBase(out err); 
                    if (err != ErrorCode.NoError)
                    {
                        ShowStatus("Save Failed");
                        return;
                    }
                }
                else if (choice == MessageBoxResult.No)
                {
                }
                else if (choice == MessageBoxResult.Cancel)
                {
                    return;
                }
                AllReset();
            }
            //初始化对话框，文件类型，过滤器，初始路径等设置
            var savedialog = new SaveFileDialog();
            savedialog.Filter = "XML files (*.xml)|*.xml";
            savedialog.FilterIndex = 0;
            savedialog.RestoreDirectory = true;
            //成功选取文件后，根据文件类型执行读取函数
            if (savedialog.ShowDialog() != true)
            {
                return;
            }
            Cursor = Cursors.Wait;
            var strPath = savedialog.FileName;
            myGdb = new Graph(strPath);
            Cursor = Cursors.Arrow;
            if (err != ErrorCode.NoError)
            {
                MessageBox.Show("Can not open file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                ShowStatus("Create Failed.");
                return;
            }
            ShowStatus("Create Success.");
            myIsDbAvailable = true;
        }

        //打开文件命令执行函数
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ErrorCode err = ErrorCode.NoError;

            if (myIsDbAvailable == true)
            {
                var choice = MessageBox.Show("Save current graph database to file？", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (choice == MessageBoxResult.Yes)
                {
                    //保存网络
                    myGdb.SaveDataBase(out err);
                    if (err != ErrorCode.NoError)
                    {
                        ShowStatus("Save Failed.");
                        return;
                    }
                }
                else if (choice == MessageBoxResult.No)
                {
                }
                else if (choice == MessageBoxResult.Cancel)
                {
                    return;
                }
                AllReset();
            }
            //初始化对话框，文件类型，过滤器，初始路径等设置
            var opendialog = new OpenFileDialog();
            opendialog.Filter = "All files (*.*)|*.*|XML files (*.xml)|*.xml";
            opendialog.FilterIndex = 0;
            opendialog.RestoreDirectory = true;
            //成功选取文件后，根据文件类型执行读取函数
            if (opendialog.ShowDialog() != true)
            {
                return;
            }
            Cursor = Cursors.Wait;
            var strPath = opendialog.FileName;
            myGdb = new Graph(strPath);
            Cursor = Cursors.Arrow;
            if (err != ErrorCode.NoError)
            {
                MessageBox.Show("Can not open file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                ShowStatus("Open Failed.");
                return;
            }
            FillNodeList();
            ShowStatus("Open Success.");
            myIsDbAvailable = true;
        }

        //保存命令执行函数
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ErrorCode err;

            //保存网络
            myGdb.SaveDataBase(out err);
            if (err != ErrorCode.NoError)
            {
                ShowStatus("Save Failed.");
                return;
            }
            myIsModified = false;
            ShowStatus("Save Success.");
        }
        
        //另存为命令执行函数
        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ErrorCode err;

            //调出另存为对话框
            //初始化对话框，文件类型，过滤器，初始路径等设置
            var savedialog = new SaveFileDialog();
            savedialog.Filter = "XML files (*.xml)|*.xml";
            savedialog.FilterIndex = 0;
            savedialog.RestoreDirectory = true;
            //成功选取文件后，根据文件类型执行读取函数
            if (savedialog.ShowDialog() != true)
            {
                return;
            }
            Cursor = Cursors.Wait;
            var strPath = savedialog.FileName;
            //切换IO句柄中的目标地址,并保存
            myGdb.SaveAsDataBase(strPath, out err);
            Cursor = Cursors.Arrow;
            if (err != ErrorCode.NoError)
            {
                MessageBox.Show("Can not save file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                ShowStatus("Save As Failed.");
                return;
            }
            ShowStatus("Save As Success.");
        }
        
        //快速打印命令执行函数
        private void QuickPrintCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        
        //打印预览命令执行函数
        private void PrintPreviewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        
        //打印命令执行函数
        private void PrintCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        
        //关闭数据库执行函数
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (myIsDbAvailable == true)
            {
                var choice = MessageBox.Show("Save current graph database to file？", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (choice == MessageBoxResult.Yes)
                {
                    //保存网络
                    ErrorCode err;
                    myGdb.SaveDataBase(out err);
                    if (err != ErrorCode.NoError)
                    {
                        ShowStatus("Save Failed.");
                        return;
                    }
                }
                else if (choice == MessageBoxResult.No)
                {
                }
                else if (choice == MessageBoxResult.Cancel)
                {
                    return;
                }
                ShowStatus("Database Closed.");
                AllReset();
            }
        }
        
        //退出程序执行函数
        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
        
        //关闭窗体前检查
        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(myIsModified == false)
            {
                return;
            }
            if (myIsDbAvailable == true)
            {
                var choice = MessageBox.Show("Save current graph database to file？", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (choice == MessageBoxResult.Yes)
                {
                    //保存网络
                    ErrorCode err;
                    myGdb.SaveDataBase(out err);
                    if (err != ErrorCode.NoError)
                    {
                        ShowStatus("Save Failed.");
                        return;
                    }
                }
                else if (choice == MessageBoxResult.No)
                {
                }
                AllReset();
            }
        }
        
        //保存命令使能
        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }
        
        //另存为命令使能
        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }
        
        //快速打印命令使能
        private void QuickPrintCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }
        
        //打印预览命令使能
        private void PrintPreviewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }
        
        //打印命令使能
        private void PrintCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }
        
        //关闭数据库命令使能
        private void CloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }
        
        //刷新命令使能
        private void RefreshCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }
        
        //刷新命令执行
        private void RefreshCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (myIsDbAvailable == false)
            {
                return;
            }
            var choice = MessageBox.Show("Reload Database without Save?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (choice == MessageBoxResult.No)
            {
                return;
            }
            Refresh();
        }

        private void Refresh()
        {
            AllReset();
            myGdb = new Graph(myDataBasePath);
            FillNodeList();
            myIsDbAvailable = true;
        }

        #endregion

        #region Drawing
        private Graph mySubGraph;
        private bool myBolScrolltoCenter;

        //选择节点
        private void SelectNodes(int index)
        {
            if (index < 0)
            {
                return;
            }
            var curSelNode = myGdb.Nodes.ElementAt(index).Value;
            if (curSelNode == null)
            {
                return;
            }
            SetCurrentNodeInfo(index);
            BuildSubGraph(curSelNode);
            ClearArrows(DrawingSurface);
            myGraphRenderer.DrawNewGraph(DrawingSurface, curSelNode.OutBound.Count, mySubGraph);
        }

        private void BuildSubGraph(INode curSelNode)
        {
            ErrorCode err;
            LoadNodeInfo(curSelNode);
            var drawNodes = new List<INode>();
            var neibourNodes = new List<INode>();
            drawNodes.Add(curSelNode);
            mySubGraph = new Graph();
            mySubGraph.AddNode(new Node(curSelNode), out err);
            foreach (IEdge edge in curSelNode.OutBound)
            {
                neibourNodes.Add(edge.To);
                drawNodes.Add(edge.To);
                mySubGraph.AddNode(new Node(edge.To), out err);
                Edge newEdge = new Edge(edge.Attribute);
                mySubGraph.AddEdgeByGuid(curSelNode.Guid, edge.To.Guid, newEdge, out err);
            }
            foreach (INode node in neibourNodes)
            {
                foreach (IEdge edge in node.InBound)
                {
                    if (drawNodes.IndexOf(edge.From) < 0)
                    {
                        drawNodes.Add(edge.From);
                        mySubGraph.AddNode(new Node(edge.From), out err);
                    }
                    if ((edge.From.Name != curSelNode.Name)
                        && (neibourNodes.IndexOf(edge.From) < 0))
                    {
                        Edge newEdge = new Edge(edge.Attribute);
                        mySubGraph.AddEdgeByGuid(edge.From.Guid, edge.To.Guid, newEdge, out err);
                    }
                }
            }
        }

        //获取visual索引
        private string GetVisualIndex(DrawingVisual visual)
        {
            if (visual == null)
            {
                return "";
            }
            foreach (var curItem in myGraphRenderer.Visuals)
            {
                if (curItem.Value.Equals(visual) == true)
                {
                    return curItem.Key;
                }
            }
            return "";
        }
        
        //构造新的ToolTip
        private ToolTip BuildNewTip(string guid)
        {
            ToolTip nodeTip = new ToolTip();

            nodeTip.Content = mySubGraph.Nodes[guid].DataOutput();

            return nodeTip;
        }
        
        //显示节点细节信息
        private void LoadNodeInfo(INode curNode)
        {
            int intRow = 0;
            if(curNode == null)
            {
                return;
            }
            PropertyInfo[] pInfos = curNode.GetType().GetProperties();
            NodeInfoGrid.Children.Clear();
            NodeInfoGrid.RowDefinitions.Clear();
            foreach (PropertyInfo pInfo in pInfos)
            {
                //TitleLabel
                var newRow = new RowDefinition { Height = new GridLength(20, GridUnitType.Auto) }; 
                NodeInfoGrid.RowDefinitions.Add(newRow);
                var curTitle = new Label();
                curTitle.Content = pInfo.Name + ":";
                NodeInfoGrid.Children.Add(curTitle);
                curTitle.SetValue(Grid.RowProperty, intRow);
                curTitle.SetValue(Grid.ColumnProperty, 0);
                curTitle.HorizontalContentAlignment = HorizontalAlignment.Right;
                //Content
                var curContent = GetWidget(pInfo, curNode);
                if(curContent == null)
                {
                    curContent = new TextBox();
                    curContent.Margin = new Thickness(2);
                    curContent.Width = 200;
                }
                NodeInfoGrid.Children.Add(curContent);
                curContent.SetValue(Grid.RowProperty, intRow);
                curContent.SetValue(Grid.ColumnProperty, 1);
                intRow++;
            }
        }

        //生成控件
        private Control GetWidget(PropertyInfo pInfo, object curNode)
        {
            
            if(pInfo.PropertyType.Name == "List`1")
            {
                ListBox contentlistBox = new ListBox();
                contentlistBox.Margin = new Thickness(2);
                contentlistBox.Width = 200;
                dynamic x = pInfo.GetValue(curNode, null);
                foreach (var item in x)
                {
                    contentlistBox.Items.Add(item);
                }
                return contentlistBox;
            }
            var contentBox = new TextBox();
            var newLabel = new Label();
            contentBox.Margin = new Thickness(2);
            contentBox.Width = 200;
            contentBox.IsReadOnly = true;
            newLabel.Content = pInfo.GetValue(curNode, null);
            if(newLabel.Content == null)
            {
                newLabel.Content = "Null";
            }
            contentBox.Text = newLabel.Content.ToString();
            return contentBox;
        }

        //清除连边形状
        private void ClearArrows(UIElement element)
        {
            BackCanvas.Children.Clear();
            BackCanvas.Children.Add(element);
        }
        #endregion

        #region UICommand
        //节点列表框选中事件处理函数
        private void NodeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectNodes(NodeListBox.SelectedIndex);
            MainScroll.ScrollToBottom();
            MainScroll.ScrollToRightEnd();
            myBolScrolltoCenter = true;
        }
        //主滚动框自动居中
        private void MainScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (myBolScrolltoCenter == true)
            {
                var bottom = MainScroll.VerticalOffset;
                var right = MainScroll.HorizontalOffset;
                MainScroll.ScrollToVerticalOffset(bottom / 2);
                MainScroll.ScrollToHorizontalOffset(right / 2);
                myBolScrolltoCenter = false;
            }
        }
        //画布鼠标移动事件-节点标签显示
        private void DrawingSurfaceMouseMove(object sender, MouseEventArgs e)
        {
            string visualGuid = GetVisualIndex(DrawingSurface.GetVisual(e.GetPosition(DrawingSurface)));

            //PointLabel.Content = visualGuid;
            if (visualGuid == "")
            {
                return;
            }
            ToolTip nodeTip = BuildNewTip(visualGuid);
            //Todo
            //myIntPointNodeIndex = visualindex;
            nodeTip.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
            DrawingSurface.ToolTip = nodeTip;
        }
        //画布鼠标点击事件-切换选中节点并重新绘图
        private void DrawingSurfaceMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string visualGuid = GetVisualIndex(DrawingSurface.GetVisual(e.GetPosition(DrawingSurface)));
            if (visualGuid == "")
            {
                return;
            }
            int intNode = myGdb.IndexOf( visualGuid );
            if (intNode == -1)
            {
                return;
            }
            NodeListBox.SelectedIndex = intNode;
        }
        //清除命令框内容命令执行
        private void ClearCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
        }
        //清除命令框按钮使能
        private void ClearCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            
        }
        //清除结果框内容命令执行
        private void ClearResultCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
        }
        //清除结果框按钮使能
        private void ClearResultCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            
        }
        //样式选择框
        private void NodeStyleSelection_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var newItem = (RibbonGalleryItem)e.NewValue;
            ChangeStyle(newItem.ToolTipTitle);
            if (myIntNodeIndex == -1)
            {
                return;
            }
            myGraphRenderer.DrawGraph();
        }

        private void ChangeStyle(string style)
        {
            switch (style)
            {
                case "默认样式":
                    style = "DefaultNodeStyle";
                    break;
                case "深邃星空":
                    style = "PurpleNodeStyle";
                    break;
                case "底比斯之水":
                    style = "BlueNodeStyle";
                    break;
                case "千本樱":
                    style = "PinkNodeStyle";
                    break;
                default:
                    style = "DefaultNodeStyle";
                    break;
            }
            var curStyle = (Style)TryFindResource(style);
            myGraphRenderer.ChangeStyle( curStyle );
        }

        #endregion

        #region DATA
        string myCurNodeName = "";
        string myCurNodeType = "";
        //设置当前选中节点信息
        void SetCurrentNodeInfo(int index)
        {
            myIntNodeIndex = index;
            if (index < 0)
            {
                myCurSelectNode = new NodeInfo();
            }
            else
            {
                myCurSelectNode = new NodeInfo(myGdb.Nodes.ElementAt(myIntNodeIndex).Value);
            }
            StatusNameBox.Text = myCurSelectNode.Name;
            StatusTypeBox.Text = myCurSelectNode.Type;
            UpdateProperties();
        }
        //更新属性列表
        void UpdateProperties()
        {
            //Todo
            //ModifyPropertyComboBox.Items.Clear();
            //ModifyPropertyTextBox.Text = "";
            
        }
        //更新属性值
        private void ModifyPropertyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Todo
            //if (myIntNodeIndex < 0 || ModifyPropertyComboBox.SelectedIndex <0)
            //{
            //    return;
            //}
            //foreach (NodeProperty np in curSelectNode.Properties)
            //{
            //    if (np.Key == ModifyPropertyComboBox.SelectedItem.ToString())
            //    {
            //        ModifyPropertyTextBox.Text = np.Value;
            //    }
            //}
        }
        //名称文本框值改变
        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            myCurNodeName = ((TextBox)sender).Text;
            if (myCurSelectNode == null )
            {
                return;
            }
            FindCustomNode(myCurNodeName, myCurNodeType);
        }
        //类型文本框值改变
        private void TypeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            myCurNodeType = ((TextBox)sender).Text;
            if (myCurSelectNode == null)
            {
                return;
            }
            FindCustomNode(myCurNodeName, myCurNodeType);
        }
        //查找用户指定节点
        private void FindCustomNode(string sName, string sType)
        {
            //tODO
            //if (myGdb == null)
            //{
            //    return;
            //}
            
            //int index = myGdb.GetIndexByNameAndType(sName, sType);
            //if (index < 0)
            //{
            //    return;
            //}
            //NodeListBox.SelectedIndex = index;
            //myCurModifyNode = myGdb.GetNodeByName(sName, sType);
            //ModifyEndName.Items.Clear();
            //RemoveEndName.Items.Clear();
            //foreach (IEdge edge in myCurModifyNode.OutBound)
            //{
            //    if (ModifyEndName.Items.IndexOf(edge.End.Name) > 0)
            //    {
            //        continue;
            //    }
            //    ModifyEndName.Items.Add(edge.End.Name);
            //    RemoveEndName.Items.Add(edge.End.Name);
            //}
            //ModifyEndName.SelectedIndex = 0;
            //FillModifyEndType((string)ModifyEndName.SelectedItem);
            //RemoveEndName.SelectedIndex = 0;
            //FillRemoveEndType((string)RemoveEndName.SelectedItem);
            //FindCustomEdge();
            //return;
        }
        //修改节点名称改变
        private void ModifyNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).Text == "")
            {
                return;
            }
            if (e.AddedItems.Count <= 0)
            {
                return;
            }
            FillModifyEndType(e.AddedItems[0].ToString());
        }
        //填充修改类型列表内容
        private void FillModifyEndType(string sName)
        {
            //Todo
            //ModifyEndType.Items.Clear();
            //if (myCurModifyNode == null)
            //{
            //    return;
            //}
            //foreach (IEdge edge in myCurModifyNode.OutBound)
            //{
            //    if (ModifyEndType.Items.IndexOf(edge.To.Name) > 0)
            //    {
            //        continue;
            //    }
            //    if (sName != edge.To.Name)
            //    {
            //        continue;
            //    }
            //    ModifyEndType.Items.Add(edge.To.Type);
            //}
            //ModifyEndType.SelectedIndex = 0;
        }
        //修改节点类型改变
        private void ModifyTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).Text == "")
            {
                return;
            }
            FindCustomEdge();
        }
        //查找目标连边
        private void FindCustomEdge()
        {
            //Todo
            //if (myGdb == null)
            //{
            //    return;
            //}
            //ModifyStartName.Text = StatusNameBox.Text;
            //ModifyStartType.Text = StatusTypeBox.Text;
            //if (ModifyStartName.Text == ""
            //    || ModifyStartType.Text == ""
            //    || ModifyEndName.Text == ""
            //    || ModifyEndType.Text == "")
            //{
            //    return;
            //}
            //myCurModifyEdge = myGdb.GetEdgeByNameAndType(ModifyStartName.Text, ModifyStartType.Text, ModifyEndName.Text, ModifyEndType.Text);
            //if (myCurModifyEdge == null)
            //{
            //    return;
            //}
            //EdgeKeyBox.Text = myCurModifyEdge.Type;
            //EdgeValueBox.Text = myCurModifyEdge.Value;
        }
        //删除节点名称改变
        private void RemoveNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).Text == "")
            {
                return;
            }
            if (e.AddedItems.Count <= 0)
            {
                return;
            }
            FillRemoveEndType(e.AddedItems[0].ToString());
        }
        //填充删除节点两类型列表
        private void FillRemoveEndType(string sName)
        {
            //Todo
            //RemoveEndType.Items.Clear();
            //if (myCurModifyNode == null)
            //{
            //    return;
            //}
            //foreach (IEdge edge in myCurModifyNode.OutBound)
            //{
            //    if (RemoveEndType.Items.IndexOf(edge.End.Name) > 0)
            //    {
            //        continue;
            //    }
            //    if (sName != edge.End.Name)
            //    {
            //        continue;
            //    }
            //    RemoveEndType.Items.Add(edge.End.Type);
            //}
            //RemoveEndType.SelectedIndex = 0;
        }
        //加入节点命令执行函数
        private void AddNodeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Todo
            //string strName, strType, strProperty;
            //ErrorCode err = ErrorCode.NoError;
            //if (IsContentLegal(out strName,out strType,out strProperty) == false)
            //{
            //    ShowStatus("Name and Type are necessary.");
            //    return;
            //}
            ////gdb.AddNodeData(strName, strType, ref err, strProperty);
            //if (err != ErrorCode.NoError)
            //{
            //    switch (err)
            //    {
            //        case ErrorCode.NodeExists:
            //            ShowStatus("Add INode failed, INode already exists.");
            //            break;
            //        case ErrorCode.CreateNodeFailed:
            //            ShowStatus("Create INode failed.");
            //            break;
            //        default:
            //            ShowStatus("Add INode failed, error code:" + err.ToString());
            //            break;
            //    }
            //    return;
            //}
            //AddNodeProperties.Items.Clear();
            //AddNodeKey.SelectedIndex = 0;
            //AddNodeValue.Text = "";
            //FillNodeList();
            ShowStatus("Add INode Success.");
            return;
        }
        //校验节点创建入参
        //private bool IsContentLegal(out string sName, out string sType, out string sProperty)
        //{
            //Todo
            //string strName = null, strType = null, strProperty = "";

            //foreach (string strItem in AddNodeProperties.Items)
            //{
            //    switch (GetKeyFromItem(strItem))
            //    {
            //        case "Name":
            //            strName = GetValueFromItem(strItem);
            //            break;
            //        case "Type":
            //            strType = GetValueFromItem(strItem);
            //            break;
            //        default:
            //            strProperty += strItem + ",";
            //            break;
            //    }
            //}
            //if (strName == null || strType == null)
            //{
            //    sName = "";
            //    sType = "";
            //    sProperty = "";
            //    return false;
            //}
            //sName = strName;
            //sType = strType;
            //sProperty = strProperty;
            //return true;
        //}
        //加入属性命令执行函数
        private void AddPropertyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Todo
            //string strKey, strValue;

            //strKey = AddNodeKey.Text;
            //strValue = AddNodeValue.Text;
            //if (strKey == "" || strValue == "")
            //{
            //    ShowStatus("Key or Value can't be empty.");
            //    return;
            //}
            ////将键值对校验后加入列表
            //AddPropertyIntoList(strKey ,strValue);
            //SortList();
            //AddNodeKey.Text = "";
            //AddNodeValue.Text = "";
        }
        //在属性列表中加入新属性
        private void AddPropertyIntoList(string sKey, string sValue)
        {
            //Todo
            //int index = 0;
            //string strTar = null;

            //foreach (string strItem in AddNodeProperties.Items)
            //{
            //    if (GetKeyFromItem(strItem) == sKey)
            //    {
            //        strTar= strItem;
            //        break;
            //    }
            //    index++;
            //}
            //if (strTar != null)
            //{
            //    AddNodeProperties.Items.Insert(index, sKey + ":" + sValue);
            //    AddNodeProperties.Items.Remove(strTar);
            //}
            //else
            //{
            //    AddNodeProperties.Items.Add(sKey + ":" + sValue);
            //}
        }
        //获取当前列表项中的key字段
        private string GetKeyFromItem(string sItem)
        {
            int index = sItem.IndexOf(':');
            string strResult;

            if (index < 0)
            {
                return sItem;
            }
            strResult = sItem.Substring(0, index);
            return strResult;
        }
        //获取当前列表项中的Value字段
        private string GetValueFromItem(string sItem)
        {
            //Todo
            int index = sItem.IndexOf(':');
            string strResult;

            if (index < 0)
            {
                return "";
            }
            strResult = sItem.Substring(index+1);
            return strResult;
        }
        //对当前属性列表进行排序
        private void SortList()
        {
            //Todo
            //string strName = null, strType = null;
            //int index = -1;

            //foreach (string strItem in AddNodeProperties.Items)
            //{
            //    if (GetKeyFromItem(strItem) == "Name")
            //    {
            //        strName = strItem;
            //        index++;
            //        break;
            //    }
            //}
            //if (strName != null)
            //{
            //    AddNodeProperties.Items.Remove(strName);
            //    AddNodeProperties.Items.Insert(index, strName);
            //}
            //foreach (string strItem in AddNodeProperties.Items)
            //{
            //    if (GetKeyFromItem(strItem) == "Type")
            //    {
            //        strType = strItem;
            //        index++;
            //        break;
            //    }
            //}
            //if (strType != null)
            //{
            //    AddNodeProperties.Items.Remove(strType);
            //    AddNodeProperties.Items.Insert(index, strType);
            //}
        }
        //属性列表选择项改变响应函数
        private void AddNodeProperties_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Todo
            //string strItem;
            //if (AddNodeProperties.SelectedIndex < 0)
            //{
            //    return;
            //}
            //strItem = AddNodeProperties.SelectedItem.ToString();
            //AddNodeKey.Text = GetKeyFromItem(strItem);
            //AddNodeValue.Text = GetValueFromItem(strItem);
        }
        //移除属性命令执行函数
        private void RemovePropertyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Todo
            //if (AddNodeProperties.SelectedIndex < 0)
            //{
            //    return;
            //}
            //AddNodeProperties.Items.RemoveAt(AddNodeProperties.SelectedIndex);
        }
        //加入连边命令执行函数
        private void AddEdgeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Todo
            //ErrorCode err = ErrorCode.NoError;
            //string strStartName, strStartType, strEndName, strEndType, strEdgeKey, strEdgeValue;

            //if (AddStartName.Text == "" ||
            //    AddStartType.Text == "" ||
            //    AddEndName.Text == "" ||
            //    AddEndType.Text == "" ||
            //    AddEdgeKey.Text == "" ||
            //    AddEdgeValue.Text == "" )
            //{
            //    ShowStatus("All fields of IEdge can't be empty.");
            //    return;
            //}
            //strStartName = AddStartName.Text;
            //strStartType = AddStartType.Text;
            //strEndName = AddEndName.Text;
            //strEndType = AddEndType.Text;
            //strEdgeKey = AddEdgeKey.Text;
            //strEdgeValue = AddEdgeValue.Text;
            
            //myGdb.AddEdgeData(strStartName, strStartType, strEndName, strEndType, strEdgeKey, ref err, strEdgeValue);
            //if (err != ErrorCode.NoError)
            //{
            //    switch (err)
            //    {
            //        case ErrorCode.NodeNotExists:
            //            ShowStatus("Add IEdge failed, End INode not exists.");
            //            break;
            //        case ErrorCode.EdgeExists:
            //            ShowStatus("Add IEdge failed, IEdge already exists.");
            //            break;
            //        case ErrorCode.CreateEdgeFailed:
            //            ShowStatus("Create IEdge failed.");
            //            break;
            //        default:
            //            ShowStatus("Add IEdge failed, error code:" + err.ToString());
            //            break;
            //    }
            //    return;
            //}
            //GraphEdgeUpdate();
            //AddEndName.Text = "";
            //AddEndType.Text = "";
            //AddEdgeKey.Text = "";
            //AddEdgeValue.Text = "";
            //ShowStatus("Add IEdge Success.");
            //return;
        }
        //修改节点命令执行函数
        private void ModifyNodeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Todo
            //获取值
            //string strKey, strValue;
            ////NodeProperty npDel = null;

            //strKey = ModifyPropertyComboBox.Text;
            //strValue = ModifyPropertyTextBox.Text;
            //if (strKey == "")
            //{
            //    ShowStatus("Modify Property Failed, Key field can't be empty.");
            //    return;
            //}
            //if (myCurModifyNode == null)
            //{
            //    ShowStatus("Modify Property Failed, no INode be selected.");
            //    return;
            //}
            ////如果存在该key则修改
            //foreach (NodeProperty np in curModifyNode.Properties)
            //{
            //    if (strKey != np.Key)
            //    {
            //        continue;
            //    }
            //    if (strValue == "")
            //    {
            //        npDel = np;
            //        break;
            //    }
            //    np.Value = strValue;
            //    ShowStatus("Modify Property Success.");
            //    return;
            //}
            //如果value为空则删除该属性
            //if (npDel != null)
            //{
            //    curModifyNode.Properties.Remove(npDel);
            //    ShowStatus("Delete Property Success.");
            //    return;
            //}
            //if (strValue == "")
            //{
            //    ShowStatus("Add Property Failed, Value field can't be empty.");
            //    return;
            //}
            ////如果不存在则插入新属性
            ////curModifyNode.Properties.Add(new NodeProperty(strKey, strValue));
            //ShowStatus("Add Property Success.");
            //return;
        }
        //修改连边命令执行函数
        private void ModifyEdgeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Todo
            //string strKey, strValue;

            //strKey = EdgeKeyBox.Text;
            //strValue = EdgeValueBox.Text;
            //if (strKey == "" || strValue == "")
            //{
            //    ShowStatus("Modify IEdge Failed, Key or Value field can't be empty.");
            //    return;
            //}
            //if (myCurModifyEdge == null)
            //{
            //    ShowStatus("Modify IEdge Failed, no IEdge be selected.");
            //    return;
            //}
            //myCurModifyEdge.Type = strKey;
            //myCurModifyEdge.Value = strValue;
            ShowStatus("Modify IEdge Success.");
            return;
        }
        //移除节点命令执行函数
        private void RemoveNodeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Todo
            //ErrorCode err = ErrorCode.NoError;
            //string strName, strType;

            //strName = RemoveNodeName.Text;
            //strType = RemoveNodeType.Text;
            //if (strName == "" || strType == "")
            //{
            //    ShowStatus("Name or Type of INode can't be empty.");
            //    return;
            //}
            //myGdb.RemoveNodeData(strName, strType, ref err);
            //if (err != ErrorCode.NoError)
            //{
            //    ShowStatus("Remove INode failed, INode not exists.");
            //    return;
            //}
            //ShowStatus("Remove INode Success.");
            //GraphNodeUpdate();
            //return;
        }
        //移除连边命令执行函数
        private void RemoveEdgeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Todo
            //ErrorCode err = ErrorCode.NoError;
            //string strStartName, strStartType, strEndName, strEndType;

            //strStartName = RemoveStartName.Text;
            //strStartType = RemoveStartType.Text;
            //strEndName = RemoveEndName.Text;
            //strEndType = RemoveEndType.Text;
            //if (strStartName == "" ||
            //    strStartType == "" ||
            //    strEndName == "" ||
            //    strEndType == "")
            //{
            //    ShowStatus("Name or Type of Nodes can't be empty.");
            //    return;
            //}
            //myGdb.RemoveEdgeData(strStartName, strStartType, strEndName, strEndType, "", ref err);
            //if (err != ErrorCode.NoError)
            //{
            //    switch (err)
            //    {
            //        case ErrorCode.NodeNotExists:
            //            ShowStatus("Remove IEdge failed, Start INode or End INode not exists.");
            //            break;
            //        case ErrorCode.EdgeNotExists:
            //            ShowStatus("Remove IEdge failed, IEdge not exists.");
            //            break;
            //        default:
            //            ShowStatus("Remove IEdge failed, error code:" + err.ToString());
            //            break;
            //    }
            //    return;
            //}
            //ShowStatus("Remove IEdge Success.");
            //GraphEdgeUpdate();
            return;
        }

        private void AddNodeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }

        private void AddPropertyCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }

        private void RemovePropertyCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //Todo
            //if (AddNodeProperties.SelectedIndex < 0)
            //{
            //    e.CanExecute = false;
            //    return;
            //}
            //e.CanExecute = true;
            return;
        }

        private void AddEdgeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }

        private void ModifyNodeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }

        private void ModifyEdgeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }

        private void RemoveNodeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }

        private void RemoveEdgeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = myIsDbAvailable;
        }




        #endregion

        
    }
}
