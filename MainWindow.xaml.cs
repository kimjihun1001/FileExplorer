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
using System.Drawing;

namespace FileExplorer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Stack<DirectoryInfo> directoryInfo = new Stack<DirectoryInfo>();
        public MainWindow()
        {
            InitializeComponent();
            GetDrives();
        }

        private void GetDrives() //드라이브 검색
        {
            DriveInfo[] drives = DriveInfo.GetDrives(); // 내컴퓨터 모든 드라이브 정보 얻기
            DirectoryInfo Di;

            foreach (DriveInfo drive in drives)
            {
                TreeViewItem driveitem = new TreeViewItem();//자식들을 추가해주려고
                if (drive.IsReady == true) // 드라이브가 준비되었으면
                {
                    Di = new DirectoryInfo(drive.Name); //경로정보 갖고오기
                    driveitem.Header = drive.Name;//제일처음에 보이는거
                    driveitem.Tag = Di.FullName;//전체경로를 테그에 넣고 
                    driveitem.ToolTip = drive.Name;//마우스 위에 올려놓았을때 보이는거

                    // 이게 위의 Di랑 다른 건가??
                    // A. 같은거임
                    DirectoryInfo baseDi = new DirectoryInfo(Di.FullName); //경로 받아오기
                    DirectoryInfo[] childrenDI = baseDi.GetDirectories(); //경로안에 디렉토리 모두 알려주기
                    Console.WriteLine("Hello");

                    // 이건 왜 foreach 아니고 for??
                    // A. 상관없음
                    for (int i = 0; i < childrenDI.Length; i++)
                    {  //폴더
                        if ((childrenDI[i].Attributes & FileAttributes.Hidden) != FileAttributes.Hidden) //숨겨진 파일 아닌것만
                        {
                            TreeViewItem subItem = new TreeViewItem(); //자식들 추가
                            subItem.Header = childrenDI[i].Name;
                            subItem.Tag = childrenDI[i].FullName;
                            subItem.ToolTip = childrenDI[i].Name;
                            subItem.Expanded += ShowDirectory;
                            // subItem.MouseLeftButtonUp += ShowDirectory;
                            driveitem.Items.Add(subItem);
                        }
                    }

                    tvlist.Items.Add(driveitem);//전체를 추가

                }
            }
        }

        private ImageSource GetFileImage(string filePath)
        {
            // 얘 왜 그냥 Icon.Extract~하면 안뜨지??
            Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }

        // 클릭 이벤트
        private void ShowDirectory(object sender, RoutedEventArgs e)
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
                            directoryItem.Items.Add(MakeChildDirectory_treeViewItem(childDirectoryInfo));
                            MakeChildDirectory_ListBoxItem(childDirectoryInfo);
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

        // TreeViewItem으로 하위 디렉토리 만들기
        private TreeViewItem MakeChildDirectory_treeViewItem(DirectoryInfo childDirectoryInfo)
        {
            TreeViewItem childDirectoryItem_treeViewItem = new TreeViewItem();   // 자식들 추가
            childDirectoryItem_treeViewItem.Header = childDirectoryInfo.Name;    // 폴더 이름
            childDirectoryItem_treeViewItem.Tag = childDirectoryInfo.FullName;   // 폴더 경로
            childDirectoryItem_treeViewItem.ToolTip = childDirectoryInfo.Name;
            childDirectoryItem_treeViewItem.Expanded += ShowDirectory;
            // childDirectoryItem_treeViewItem.MouseLeftButtonUp += ShowDirectory;

            return childDirectoryItem_treeViewItem;
        }
        // ListBoxItem으로 하위 디렉토리 만들기
        private ListBoxItem MakeChildDirectory_ListBoxItem(DirectoryInfo childDirectoryInfo)
        {
            fileName.Text = childDirectoryInfo.FullName;
            ListBoxItem childDirectoryItem_ListBoxItem = new ListBoxItem();   // 자식들 추가
            //Span span = new Span();
            //System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            //TextBlock textBlock_FileName = new TextBlock();
            //textBlock_FileName.Text = childDirectoryInfo.Name;  // 폴더 이름
            //childDirectoryItem_ListBoxItem.Tag = childDirectoryInfo.FullName;   // 폴더 경로
            //childDirectoryItem_ListBoxItem.ToolTip = childDirectoryInfo.Name;
            //childDirectoryItem_ListBoxItem.MouseDoubleClick += ShowDirectory;


            //lblist.Items.Add(childDirectoryItem_ListBoxItem);

            return childDirectoryItem_ListBoxItem;
        }

        private void UpdateState

    }
}
