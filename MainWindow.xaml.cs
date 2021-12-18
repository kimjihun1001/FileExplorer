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
        public static DirectoryInfo presentDirectoryInfo = null;

        public static Stack<DirectoryInfo> backwardStack = new Stack<DirectoryInfo>();
        
        public static Stack<DirectoryInfo> forwardStack = new Stack<DirectoryInfo>();


        public MainWindow()
        {
            InitializeComponent();
            GetDrives();
                        
        }

        private void Click_Backward(object sender, RoutedEventArgs e)
        {
            try
            {
                if (presentDirectoryInfo != null)
                {
                    // 현재 상태를 forwardStack에 저장
                    forwardStack.Push(presentDirectoryInfo);

                    // backwardStack에서 현재 상태 꺼내오기
                    UpdatePresent(backwardStack.Pop());

                    // screen에 폴더, 파일 표시하기
                    ShowScreen(presentDirectoryInfo);

                    // 경로 표시하기
                    ShowPath(presentDirectoryInfo);

                }
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
                if (presentDirectoryInfo != null)
                {
                    // 현재 상태를 backwardStack에 저장
                    backwardStack.Push(presentDirectoryInfo);

                    // forwardStack에서 현재 상태 꺼내오기
                    UpdatePresent(forwardStack.Pop());

                    // screen에 폴더, 파일 표시하기
                    ShowScreen(presentDirectoryInfo);

                    // 경로 표시하기
                    ShowPath(presentDirectoryInfo);

                }
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

                    driveitem.PreviewMouseLeftButtonUp += Click_TreeViewItem;

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
                            subItem.PreviewMouseLeftButtonUp += Click_TreeViewItem;

                            MakeDeeperTreeViewItem(subItem);
                            driveitem.Items.Add(subItem);
                        }
                    }
                    nav.Items.Add(driveitem);    // nav에 추가

                }
            }

        }

        // nav에 하위 폴더 추가 이벤트
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
                        subItem.PreviewMouseLeftButtonUp += Click_TreeViewItem;

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

        // nav에서 폴더 클릭 이벤트
        private void Click_TreeViewItem(object sender, RoutedEventArgs e)
        {
            try
            {                
                SavePresent();

                TreeViewItem treeViewItem = (TreeViewItem)e.Source;
                DirectoryInfo directoryInfo = new DirectoryInfo(treeViewItem.Tag.ToString());

                // screen에 폴더, 파일 표시하기
                ShowScreen(directoryInfo);

                // 경로 표시하기
                ShowPath(directoryInfo);

                UpdatePresent(directoryInfo);

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Screen에서 폴더 클릭 이벤트
        private void DoubleClick_ScreenImage(object sender, RoutedEventArgs e)
        {
            try
            {
                SavePresent();

                Button screenImage = (Button)e.Source;
                DirectoryInfo directoryInfo = new DirectoryInfo(screenImage.Tag.ToString());

                // screen에 폴더, 파일 표시하기
                ShowScreen(directoryInfo);

                // 경로 표시하기
                ShowPath(directoryInfo);

                UpdatePresent(directoryInfo);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // 파일 실행 이벤트
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


                    // 이름으로 검색
                    
                }
                else if (searchForPath.IsChecked == true)
                {
                    SavePresent();

                    // 경로로 검색
                    string searchWord = searchBox.Text;
                    DirectoryInfo directoryInfo = new DirectoryInfo(searchWord);

                    // screen에 폴더, 파일 표시하기
                    ShowScreen(directoryInfo);

                    // 경로 표시하기
                    ShowPath(directoryInfo);

                    UpdatePresent(directoryInfo);

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

        // screen에 폴더, 파일 표시하기
        private void ShowScreen(DirectoryInfo directoryInfo)
        {
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

            // 항목 개수 표시하기
            NumOfContents = childrenDI.Length + files.Length;
            showNum.Text = NumOfContents.ToString() + "개 항목";
        }

        // path에 경로 버튼 표시하기
        private void ShowPath(DirectoryInfo directoryInfo)
        {
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
        }

        // 스택 체크하고 뒤로, 앞으로 가기 버튼 활성화
        private void CheckStack()
        {
            if (backwardStack.Count > 0)
            {
                backward.IsEnabled = true;
            }
            else
            {
                backward.IsEnabled = false;
            }

            if (forwardStack.Count > 0)
            {
                forward.IsEnabled = true;
            }
            else
            {
                forward.IsEnabled = false;
            }
        }
        
        // nav, screen, path 클릭할 때 backwardStack에 저장
        private void SavePresent()
        {
            // 현재상태였던 걸 backwardStack에 저장
            if (presentDirectoryInfo != null)
            {
                // 뒤로 가기에 이미 저장되어 있으면 저장하지 않음
                if (backwardStack.Count > 0)
                {
                    DirectoryInfo directoryInfo = backwardStack.Peek();
                    if (presentDirectoryInfo.FullName != directoryInfo.FullName)
                    {
                        backwardStack.Push(presentDirectoryInfo);
                        presentDirectoryInfo = null;
                    }
                    else
                    {

                    }
                }
                else
                {
                    backwardStack.Push(presentDirectoryInfo);
                    presentDirectoryInfo = null;
                }
                
            }

            // forwardStack 초기화
            forwardStack.Clear();

            // 뒤로 가기, 앞으로 가기 버튼 활성화, 비활성화
            CheckStack();
        }

        // 현재 상태를 변수에 저장
        private void UpdatePresent(DirectoryInfo directoryInfo)
        {
            presentDirectoryInfo = directoryInfo;

            // 뒤로 가기, 앞으로 가기 버튼 활성화, 비활성화
            CheckStack();
        }
    }
}
