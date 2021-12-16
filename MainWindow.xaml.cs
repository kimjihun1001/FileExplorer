using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace FileExplorer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<TreeViewItem> presentNav = new List<TreeViewItem>();
        public static List<WrapPanel> presentScreen = new List<WrapPanel>();
        public static List<Button> presentPath = new List<Button>();

        public static Stack<List<TreeViewItem>> backwardStack_Nav = new Stack<List<TreeViewItem>>();
        public static Stack<List<WrapPanel>> backwardStack_Screen = new Stack<List<WrapPanel>>();
        public static Stack<List<Button>> backWardStack_Path = new Stack<List<Button>>();

        public static Stack<List<TreeViewItem>> forwardStack_Nav = new Stack<List<TreeViewItem>>();
        public static Stack<List<WrapPanel>> forwardStack_Screen = new Stack<List<WrapPanel>>();
        public static Stack<List<Button>> forWardStack_Path = new Stack<List<Button>>();


        public MainWindow()
        {
            InitializeComponent();
            GetDrives();
                        
        }

        private void Click_Backward(object sender, RoutedEventArgs e)
        {
            try
            {
                if (presentNav != null)
                {
                    forwardStack_Nav.Push(presentNav);
                }

                if (presentScreen != null)
                {
                    forwardStack_Screen.Push(presentScreen);
                }

                if (presentPath != null)
                {
                    forWardStack_Path.Push(presentPath);
                }

                presentNav = backwardStack_Nav.Pop();
                presentScreen = backwardStack_Screen.Pop();
                presentPath = backWardStack_Path.Pop();

                nav.Items.Clear();
                foreach(TreeViewItem treeViewItem in presentNav)
                {
                    nav.Items.Add(treeViewItem);
                }

                screen.Children.Clear();
                foreach(WrapPanel panel in presentScreen)
                {
                    screen.Children.Add(panel);
                }

                path.Children.Clear();
                foreach(Button button in presentPath)
                {
                    path.Children.Add(button);
                }
                
                CheckStack();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Click_Forward(object sender, RoutedEventArgs e)
        {
            try
            {
                if (presentNav != null)
                {
                    backwardStack_Nav.Push(presentNav);
                }

                if (presentScreen != null)
                {
                    backwardStack_Screen.Push(presentScreen);
                }

                if (presentPath != null)
                {
                    backWardStack_Path.Push(presentPath);
                }

                presentNav = forwardStack_Nav.Pop();
                presentScreen = forwardStack_Screen.Pop();
                presentPath = forWardStack_Path.Pop();

                nav.Items.Clear();
                nav.Items.Add(presentNav);

                screen.Children.Clear();
                foreach (WrapPanel panel in presentScreen)
                {
                    screen.Children.Add(panel);
                }

                path.Children.Clear();
                foreach (Button button in presentPath)
                {
                    path.Children.Add(button);
                }

                CheckStack();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void GetDrives() //드라이브 검색
        {
            DriveInfo[] drives = DriveInfo.GetDrives(); // 내컴퓨터 모든 드라이브 정보 얻기
            DirectoryInfo directoryInfo;

            foreach (DriveInfo drive in drives)
            {
                TreeViewItem driveitem = new TreeViewItem();    // 자식들을 추가해주려고
                if (drive.IsReady == true) // 드라이브가 준비되었으면
                {
                    directoryInfo = new DirectoryInfo(drive.Name); // 경로 정보 갖고오기
                    driveitem.Header = drive.Name;  // 이름
                    driveitem.Tag = directoryInfo.FullName; //전체경로를 태그에 저장 
                    driveitem.ToolTip = drive.Name; // 마우스 위에 올려놓았을때 보이는거

                    DirectoryInfo[] childrenDI = directoryInfo.GetDirectories(); // 하위 디렉토리 불러오기

                    foreach (DirectoryInfo childDI in childrenDI)
                    {
                        if ((childDI.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)  // 숨겨진 파일 아닌 것만 
                        {
                            TreeViewItem subItem = new TreeViewItem(); //자식들 추가
                            subItem.Header = childDI.Name;
                            subItem.Tag = childDI.FullName;
                            subItem.ToolTip = childDI.Name;
                            subItem.Expanded += Expend_TreeViewItem;
                            MakeDeeperTreeViewItem(subItem);
                            driveitem.Items.Add(subItem);
                        }
                    }
                    nav.Items.Add(driveitem);    // nav에 추가

                }
            }

        }

        // nav에 추가
        private void Expend_TreeViewItem(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem treeViewItem = (TreeViewItem)e.Source;
                DirectoryInfo directoryInfo = new DirectoryInfo(treeViewItem.Tag.ToString());
                DirectoryInfo[] childrenDI = directoryInfo.GetDirectories(); // 하위 디렉토리 불러오기
                
                treeViewItem.Items.Clear();

                foreach (DirectoryInfo childDI in childrenDI)
                {
                    if ((childDI.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)  // 숨겨진 파일 아닌 것만 
                    {
                        TreeViewItem subItem = new TreeViewItem(); //자식들 추가
                        subItem.Header = childDI.Name;
                        subItem.Tag = childDI.FullName;
                        subItem.ToolTip = childDI.Name;
                        subItem.Expanded += Expend_TreeViewItem;
                        MakeDeeperTreeViewItem(subItem);
                        treeViewItem.Items.Add(subItem);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void MakeDeeperTreeViewItem(TreeViewItem treeViewItem)
        {
            try
            {  
                DirectoryInfo directoryInfo = new DirectoryInfo(treeViewItem.Tag.ToString());
                DirectoryInfo[] childrenDI = directoryInfo.GetDirectories(); // 하위 디렉토리 불러오기

                foreach (DirectoryInfo childDI in childrenDI)
                {
                    if ((childDI.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)  // 숨겨진 파일 아닌 것만 
                    {
                        TreeViewItem subItem = new TreeViewItem(); //자식들 추가
                        subItem.Header = childDI.Name;
                        subItem.Tag = childDI.FullName;
                        subItem.ToolTip = childDI.Name;

                        treeViewItem.Items.Add(subItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // scren에 추가
        private void Click_TreeViewItem(object sender, RoutedEventArgs e)
        {
            try
            {                
                SavePresentState();

                TreeViewItem treeViewItem = (TreeViewItem)e.Source;
                DirectoryInfo directoryInfo = new DirectoryInfo(treeViewItem.Tag.ToString());
                DirectoryInfo[] childrenDI = directoryInfo.GetDirectories(); // 하위 디렉토리 불러오기

                screen.Children.Clear();
                int NumOfContents = 0;

                // 폴더
                foreach (DirectoryInfo childDI in childrenDI)
                {
                    if ((childDI.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)  // 숨겨진 파일 아닌 것만 
                    {                        
                        WrapPanel wrapPanel = new WrapPanel();
                        wrapPanel.Width = 100;
                        wrapPanel.Height = 100;

                        Button button = new Button();
                        button.Background = new ImageBrush(new BitmapImage(new Uri("C:\\Users\\kimji\\source\\repos\\FileExplorer\\Asset\\folder.png")));
                        button.BorderThickness = new Thickness(0);
                        button.Height = 60;
                        button.Width = 60;                        
                        button.Tag = childDI.FullName;
                        button.MouseDoubleClick += DoubleClick_ScreenImage;
                        wrapPanel.Children.Add(button);

                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = childDI.Name;
                        textBlock.TextWrapping = TextWrapping.Wrap;
                        textBlock.Width = 90;
                        wrapPanel.Children.Add(textBlock);

                        screen.Children.Add(wrapPanel);

                    }

                }

                // 파일
                string[] files = Directory.GetFiles(directoryInfo.FullName, "*");
                foreach (string file in files)
                {
                    WrapPanel wrapPanel = new WrapPanel();
                    wrapPanel.Width = 100;
                    wrapPanel.Height = 100;

                    Button button = new Button();
                    button.Background = new ImageBrush(GetFileImage(file));
                    button.BorderThickness = new Thickness(0);                    
                    button.Height = 60;
                    button.Width = 60;
                    button.Tag = file;
                    button.MouseDoubleClick += DoubleClick_ScreenImage_File;
                    wrapPanel.Children.Add(button);

                    TextBlock textBlock = new TextBlock();
                    string fileName = (file.Replace(directoryInfo.FullName, "")).Replace("\\", "");  // 파일 전체 경로에서 파일 이름만 따오기
                    textBlock.Text = fileName;
                    textBlock.TextWrapping = TextWrapping.Wrap;
                    textBlock.Width = 90;
                    wrapPanel.Children.Add(textBlock);
                    screen.Children.Add(wrapPanel);
                }

                NumOfContents = childrenDI.Length + files.Length;
                showNum.Text = NumOfContents.ToString() + "개 항목";

                // 경로 표시하기
                path.Children.Clear();

                List<DirectoryInfo> directoryInfoList = new List<DirectoryInfo>();
                DirectoryInfo directoryInfo1 = new DirectoryInfo(directoryInfo.FullName);
                directoryInfoList.Add(directoryInfo1);

                while (true)
                {
                    if (directoryInfo1.Parent == null)
                    {
                        break;
                    }
                    else if (directoryInfo1.Parent.Exists == true)
                    {
                        directoryInfo1 = directoryInfo1.Parent;
                        directoryInfoList.Add(directoryInfo1);
                    }
                    else
                        break;
                }

                directoryInfoList.Reverse();

                foreach (DirectoryInfo directoryInfo2 in directoryInfoList)
                {
                    Button button1 = new Button();
                    button1.BorderThickness = new Thickness(0);
                    button1.Padding = new Thickness(5);
                    button1.Content = directoryInfo2.Name + " >";
                    button1.Tag = directoryInfo2.FullName;
                    button1.MouseDoubleClick += DoubleClick_ScreenImage;
                    path.Children.Add(button1);
                }

                UpdatePresentState();

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Screen에서 파일 클릭 이벤트
        private void DoubleClick_ScreenImage(object sender, RoutedEventArgs e)
        {
            try
            {
                Button screenImage = (Button)e.Source;
                DirectoryInfo directoryInfo = new DirectoryInfo(screenImage.Tag.ToString());
                DirectoryInfo[] childrenDI = directoryInfo.GetDirectories(); // 하위 디렉토리 불러오기

                screen.Children.Clear();
                int NumOfContents = 0;
                // 폴더
                foreach (DirectoryInfo childDI in childrenDI)
                {
                    if ((childDI.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)  // 숨겨진 파일 아닌 것만 
                    {
                        WrapPanel wrapPanel = new WrapPanel();
                        wrapPanel.Width = 100;
                        wrapPanel.Height = 100;

                        Button button = new Button();
                        button.Background = new ImageBrush(new BitmapImage(new Uri("C:\\Users\\kimji\\source\\repos\\FileExplorer\\Asset\\folder.png")));
                        button.BorderThickness = new Thickness(0);                    
                        button.Height = 60;
                        button.Width = 60;
                        button.Tag = childDI.FullName;
                        button.MouseDoubleClick += DoubleClick_ScreenImage;
                        wrapPanel.Children.Add(button);

                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = childDI.Name;
                        textBlock.TextWrapping = TextWrapping.Wrap;
                        textBlock.Width = 90;
                        wrapPanel.Children.Add(textBlock);

                        screen.Children.Add(wrapPanel);

                    }
                }

                // 파일
                string[] files = Directory.GetFiles(directoryInfo.FullName, "*");
                foreach (string file in files)
                {
                    WrapPanel wrapPanel = new WrapPanel();
                    wrapPanel.Width = 100;
                    wrapPanel.Height = 100;

                    Button button = new Button();
                    button.Background = new ImageBrush(GetFileImage(file));
                    button.BorderThickness = new Thickness(0);
                    button.Height = 60;
                    button.Width = 60;
                    button.Tag = file;
                    button.MouseDoubleClick += DoubleClick_ScreenImage_File;
                    wrapPanel.Children.Add(button);

                    TextBlock textBlock = new TextBlock();
                    string fileName = (file.Replace(directoryInfo.FullName, "")).Replace("\\", "");  // 파일 전체 경로에서 파일 이름만 따오기
                    textBlock.Text = fileName;
                    textBlock.TextWrapping = TextWrapping.Wrap;
                    textBlock.Width = 90;
                    wrapPanel.Children.Add(textBlock);
                    screen.Children.Add(wrapPanel);
                }

                NumOfContents = childrenDI.Length + files.Length;
                showNum.Text = NumOfContents.ToString() + "개 항목";

                // 경로 표시하기
                path.Children.Clear();

                List<DirectoryInfo> directoryInfoList = new List<DirectoryInfo>();
                DirectoryInfo directoryInfo1 = new DirectoryInfo(directoryInfo.FullName);
                directoryInfoList.Add(directoryInfo1);

                while (true)
                {
                    if (directoryInfo1.Parent == null)
                    {
                        break;
                    }
                    else if (directoryInfo1.Parent.Exists == true)
                    {
                        directoryInfo1 = directoryInfo1.Parent;
                        directoryInfoList.Add(directoryInfo1);
                    }
                    else
                        break;
                }

                directoryInfoList.Reverse();

                foreach (DirectoryInfo directoryInfo2 in directoryInfoList)
                {
                    Button button1 = new Button();
                    button1.BorderThickness = new Thickness(0);
                    button1.Padding = new Thickness(5);
                    button1.Content = directoryInfo2.Name;
                    button1.Tag = directoryInfo2.FullName;
                    button1.MouseDoubleClick += DoubleClick_ScreenImage;
                    path.Children.Add(button1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // 파일 실행
        private void DoubleClick_ScreenImage_File(object sender, RoutedEventArgs e)
        {
            try
            {
                Button screenImage = (Button)e.Source;
                System.Diagnostics.Process run = new System.Diagnostics.Process();
                run.StartInfo.FileName = screenImage.Tag.ToString();
                run.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Click_SearchButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (searchForName.IsChecked == true)
                {

                }
                else if (searchForPath.IsChecked == true)
                {
                    SavePresentState();

                    string searchWord = searchBox.Text;
                    DirectoryInfo directoryInfo = new DirectoryInfo(searchWord);
                    DirectoryInfo[] childrenDI = directoryInfo.GetDirectories(); // 하위 디렉토리 불러오기

                    screen.Children.Clear();
                    int NumOfContents = 0;
                    // 폴더
                    foreach (DirectoryInfo childDI in childrenDI)
                    {
                        if ((childDI.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)  // 숨겨진 파일 아닌 것만 
                        {
                            WrapPanel wrapPanel = new WrapPanel();
                            wrapPanel.Width = 100;
                            wrapPanel.Height = 100;

                            Button button = new Button();
                            button.Background = new ImageBrush(new BitmapImage(new Uri("C:\\Users\\kimji\\source\\repos\\FileExplorer\\Asset\\folder.png")));
                            button.BorderThickness = new Thickness(0);
                            button.Height = 60;
                            button.Width = 60;
                            button.Tag = childDI.FullName;
                            button.MouseDoubleClick += DoubleClick_ScreenImage;
                            wrapPanel.Children.Add(button);

                            TextBlock textBlock = new TextBlock();
                            textBlock.Text = childDI.Name;
                            textBlock.TextWrapping = TextWrapping.Wrap;
                            textBlock.Width = 90;
                            wrapPanel.Children.Add(textBlock);

                            screen.Children.Add(wrapPanel);

                        }
                    }

                    // 파일
                    string[] files = Directory.GetFiles(directoryInfo.FullName, "*");
                    foreach (string file in files)
                    {
                        WrapPanel wrapPanel = new WrapPanel();
                        wrapPanel.Width = 100;
                        wrapPanel.Height = 100;

                        Button button = new Button();
                        button.Background = new ImageBrush(GetFileImage(file));
                        button.BorderThickness = new Thickness(0);
                        button.Height = 60;
                        button.Width = 60;
                        button.Tag = file;
                        button.MouseDoubleClick += DoubleClick_ScreenImage_File;
                        wrapPanel.Children.Add(button);

                        TextBlock textBlock = new TextBlock();
                        string fileName = (file.Replace(directoryInfo.FullName, "")).Replace("\\", "");  // 파일 전체 경로에서 파일 이름만 따오기
                        textBlock.Text = fileName;
                        textBlock.TextWrapping = TextWrapping.Wrap;
                        textBlock.Width = 90;
                        wrapPanel.Children.Add(textBlock);
                        screen.Children.Add(wrapPanel);
                    }

                    NumOfContents = childrenDI.Length + files.Length;
                    showNum.Text = NumOfContents.ToString() + "개 항목";

                    // 경로 표시하기
                    path.Children.Clear();

                    List<DirectoryInfo> directoryInfoList = new List<DirectoryInfo>();
                    DirectoryInfo directoryInfo1 = new DirectoryInfo(directoryInfo.FullName);
                    directoryInfoList.Add(directoryInfo1);

                    while (true)
                    {
                        if (directoryInfo1.Parent == null)
                        {
                            break;
                        }
                        else if (directoryInfo1.Parent.Exists == true)
                        {
                            directoryInfo1 = directoryInfo1.Parent;
                            directoryInfoList.Add(directoryInfo1);
                        }
                        else
                            break;
                    }

                    directoryInfoList.Reverse();

                    foreach (DirectoryInfo directoryInfo2 in directoryInfoList)
                    {
                        Button button1 = new Button();
                        button1.BorderThickness = new Thickness(0);
                        button1.Padding = new Thickness(5);
                        button1.Content = directoryInfo2.Name;
                        button1.Tag = directoryInfo2.FullName;
                        button1.MouseDoubleClick += DoubleClick_ScreenImage;
                        path.Children.Add(button1);
                    }

                    UpdatePresentState();
                }
                else
                { }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("검색 결과가 없습니다.");
                Console.WriteLine(ex.Message);
            }
        }

        private ImageSource GetFileImage(string filePath)
        {
            // 얘 왜 그냥 Icon.Extract~하면 안뜨지??
            System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        // 스택 체크하고 뒤로, 앞으로 가기 버튼 활성화
        private void CheckStack()
        {
            if (backwardStack_Nav.Count > 0)
            {
                backward.IsEnabled = true;
            }
            else
            {
                backward.IsEnabled = false;
            }

            if (forwardStack_Nav.Count > 0)
            {
                forward.IsEnabled = true;
            }
            else
            {
                forward.IsEnabled = false;
            }
        }
        
        // 현재 상태를 스택에 업데이트하고 초기화
        private void SavePresentState()
        {
            if (presentNav != null)
            {
                backwardStack_Nav.Push(presentNav);
                presentNav.Clear();
            }

            if (presentScreen != null)
            {
                backwardStack_Screen.Push(presentScreen);
                presentScreen.Clear();
            }

            if (presentPath != null)
            {
                backWardStack_Path.Push(presentPath);
                presentPath.Clear();
            }
        }

        // 현재 상태를 변수에 저장
        private void UpdatePresentState()
        {
            foreach (TreeViewItem treeViewItem in nav.Items)
            {
                if (treeViewItem != null)
                {
                    presentNav.Add(treeViewItem);
                }
            }

            foreach (WrapPanel wrapPanel in screen.Children)
            {
                if (wrapPanel != null)
                {
                    presentScreen.Add(wrapPanel);
                }
            }

            foreach (Button button in path.Children)
            {
                if (button != null)
                {
                    presentPath.Add(button);
                }
            }

            CheckStack();
        }
    }
}
