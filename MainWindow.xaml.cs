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
        public static Stack<object> PreStack_Tag = new Stack<object>();
        public MainWindow()
        {
            InitializeComponent();
            GetDrives();
            GetFileImage("C:\\Users\\kimji\\Pictures\\아이유.jpg");
            GetFileImage("C:\\Users\\kimji\\Pictures\\아이유.jpg");

        }

        private void Update_PreStack(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(TreeViewItem))
                {
                    TreeViewItem treeViewItem = (TreeViewItem)sender;
                    PreStack_Tag.Push(treeViewItem.Tag);
                    Console.WriteLine(treeViewItem.Tag.ToString() + "뒤로 가기에 추가됨");
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

        private ImageSource GetFileImage(string filePath)
        {
            // 얘 왜 그냥 Icon.Extract~하면 안뜨지??
            System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        
    }
}
