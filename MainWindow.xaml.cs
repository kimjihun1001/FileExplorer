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

                            subItem.Expanded += Click_TreeViewItem;
                            MakeDeeperTreeViewItem(subItem);
                            driveitem.Items.Add(subItem);
                        }
                    }
                    tvlist.Items.Add(driveitem);    // TreeView에 추가

                }
            }
        }

        private void Click_TreeViewItem(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem treeViewItem = (TreeViewItem)sender;
                DirectoryInfo directoryInfo = new DirectoryInfo(treeViewItem.Tag.ToString());
                DirectoryInfo[] childrenDI = directoryInfo.GetDirectories(); // 하위 디렉토리 불러오기

                lblist.Items.Clear();

                foreach (DirectoryInfo childDI in childrenDI)
                {
                    if ((childDI.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)  // 숨겨진 파일 아닌 것만 
                    {
                        TreeViewItem subItem = new TreeViewItem(); //자식들 추가
                        subItem.Header = childDI.Name;
                        subItem.Tag = childDI.FullName;
                        subItem.ToolTip = childDI.Name;
                        subItem.Expanded += Click_TreeViewItem;
                        MakeDeeperTreeViewItem(subItem);
                        treeViewItem.Items.Add(subItem);

                        Button button = new Button();
                        lblist.Items.Add(button);
                        button.Content = childDI.Name;
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

        private void Click_ListBoxItem(object sender, RoutedEventArgs e)
        {
            try
            {
                ListBoxItem listBoxItem = (ListBoxItem)sender;
                DirectoryInfo directoryInfo = new DirectoryInfo(listBoxItem.Tag.ToString());
                DirectoryInfo[] childrenDI = directoryInfo.GetDirectories(); // 하위 디렉토리 불러오기

                lblist.Items.Clear();

                foreach (DirectoryInfo childDI in childrenDI)
                {
                    if ((childDI.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)  // 숨겨진 파일 아닌 것만 
                    {
                        

                        Button button = new Button();
                        lblist.Items.Add(button);
                        button.Content = childDI.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private ImageSource GetFileImage(string filePath)
        {
            WrapPanel wrapPanel = new WrapPanel();
            wp.Children.Add(wrapPanel);

            

            // 얘 왜 그냥 Icon.Extract~하면 안뜨지??
            System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            
            TextBlock textBlock = new TextBlock();
            textBlock.Text = filePath;
            wrapPanel.Children.Add(textBlock);

            Image image = new Image();
            image.Source = imageSource;
            wrapPanel.Children.Add(image);

            return imageSource;
        }

        // 클릭 이벤트
        private void Directory(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(TreeViewItem))
                {
                    TreeViewItem directoryItem = (TreeViewItem)sender;
                    DirectoryInfo directoryInfo = new DirectoryInfo((directoryItem.Tag).ToString());   // 경로 받아오기

                    // treeview에 추가
                    directoryItem.Items.Clear();    // 기존에 있던 하위 아이템 제거
                    DirectoryInfo[] childrenDirectoryInfo = directoryInfo.GetDirectories(); //경로 하위 디렉토리 받아오기

                    foreach (DirectoryInfo childDirectoryInfo in childrenDirectoryInfo)
                    {
                        if ((childDirectoryInfo.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden) //숨겨진 파일 아닌것만
                        {
                            
                        }
                    }

                    //string[] files = Directory.GetFiles(baseDi.FullName, "*");

                    //foreach (string file in files)
                    //{
                    //    ListBoxItem listBoxItem;
                    //    GetFileImage(file);

                    //}

                }
                else if (sender.GetType() == typeof(ListBoxItem))
                {
                    ListBoxItem directoryItem = (ListBoxItem)sender;
                    DirectoryInfo directoryInfo = new DirectoryInfo((directoryItem.Tag).ToString());   // 경로 받아오기

                }
                else
                {
                    // 수정필요
                    ListBoxItem directoryItem = (ListBoxItem)sender;
                    DirectoryInfo directoryInfo = new DirectoryInfo((directoryItem.Tag).ToString());   // 경로 받아오기

                }


                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
