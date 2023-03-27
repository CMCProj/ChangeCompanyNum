using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
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

namespace ChangeCompanyNum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (Data.BidFile != null)
            {
                BidList.Text = Data.BidText;
            }
        }

        // 메세지 창
        static public void DisplayDialog(String dialog, String title)
        {
            MessageBox.Show(dialog, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void UproadBtnClick(object sender, RoutedEventArgs e)
        {
            // 파일 탐색기 열기
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "BID Files (*.BID)|*.BID|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog.ShowDialog() == true) // 파일을 정상적으로 업로드 한 경우
            {
                // 복사 파일 저장 폴더 -> 바탕화면
                string copiedFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); 

                //업로드한 공내역 파일을 [AutoBID\\EmptyBid] 폴더에 복사한다. (23.02.02)
                FileStream file;

                // 파일 복사
                using (FileStream SourceStream = File.Open(openFileDialog.FileName, FileMode.Open))
                {
                    using (FileStream DestinationStream = File.Create(copiedFilePath + "\\" + System.IO.Path.GetFileName(openFileDialog.FileName) + "_복사본"))
                    {
                        await SourceStream.CopyToAsync(DestinationStream);
                        file = DestinationStream;
                        DisplayDialog(DestinationStream.Name, "확인");
                    }
                }

                Data.BidText = System.IO.Path.GetFileName(openFileDialog.FileName) + "_복사본";
                BidList.Text = Data.BidText;
                Data.BidFile = file;
                Data.IsConvert = false;
            }
            else
            {
                DisplayDialog("파일을 업로드 해주세요.", "Error");
                Data.BidFile = null;
                Data.IsConvert = false;
            }
        }

        private void ChangeBtnClick(object sender, RoutedEventArgs e)
        {
            if (Data.BidFile == null)
            {
                DisplayDialog("BID파일을 업로드해주세요!", "Upload");
            }
            else
            {
                try
                {
                    BidHandling.BidToXml();
                }
                catch
                {
                    DisplayDialog("변경 도중 문제가 발생했습니다. BID파일을 다시 한 번 확인해주세요.", "Error");
                    return;
                }

                Data.IsConvert = true;
                DisplayDialog("회사 명 및 사업자 등록 번호를 변경했습니다.", "Success");
            }
        }
    }
}
