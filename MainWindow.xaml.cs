using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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

        // 업로드 버튼 헨들러
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

                //업로드한 BID 파일을 바탕화면에 복사한다. (23.02.02)
                FileStream file;

                // 파일의 복사본 복사
                using (FileStream SourceStream = File.Open(openFileDialog.FileName, FileMode.Open))
                {
                    using (FileStream DestinationStream = File.Create(copiedFilePath + "\\" + System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName) + "_복사본.BID"))
                    {
                        await SourceStream.CopyToAsync(DestinationStream);
                        file = DestinationStream;
                        DisplayDialog(DestinationStream.Name, "확인");
                    }
                }

                // 복사본의 이름 저장
                Data.BidText = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName) + "_복사본.BID";
                BidList.Text = Data.BidText;
                Data.BidFile = file;
                Data.IsConvert = false;
            }
            else // 업로드되지 않았다면
            {
                DisplayDialog("파일을 업로드 해주세요.", "Error");
                Data.BidFile = null;
                Data.IsConvert = false;
            }
        }

        // 변경 버튼 헨들러
        private void ChangeBtnClick(object sender, RoutedEventArgs e)
        {
            if (Data.BidFile == null)   // 업로드된 BID파일이 없다면
            {
                DisplayDialog("BID파일을 업로드해주세요!", "Upload");
            }
            else
            {
                if (CompanyNum.Text == string.Empty)    // 사업자등록번호가 입력되지 않았다면
                {
                    DisplayDialog("사업자등록번호를 입력해주세요.", "Fail");
                    return;
                }
                else if (CompanyNum.GetLineLength(0) != 10) // 입력한 사용자등록번호가 10자리가 아닐 때
                {
                    DisplayDialog("올바른 사용자등록번호를 입력해주세요.", "Fail");
                    return;
                }
                else if (CompanyName.Text == string.Empty)  // 회사 명이 입력되지 않았다면
                {
                    DisplayDialog("회사명을 입력해주세요.", "Fail");
                    return;
                }
                else
                {
                    Data.CompanyRegistrationName = CompanyName.Text;
                    Data.CompanyRegistrationNum = CompanyNum.Text;
                }

                try    // BID 파일 회사 명, 사업자등록번호 변경
                {
                    BidHandling.BidToXml();
                    BidHandling.XmlToBid();
                }
                catch  // 도중 문제가 생겼을 경우
                {
                    DisplayDialog("변경 도중 문제가 발생했습니다. BID파일을 다시 한 번 확인해주세요.", "Error");
                    return;
                }

                // 변경 완료
                Data.IsConvert = true;
                DisplayDialog("회사 명 및 사업자 등록 번호를 변경했습니다.", "Success");
            }
        }

        // 회사 명 텍스트박스 헨들러
        private void CompanyName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox CompanyName = sender as TextBox;
            int selectionStart = CompanyName.SelectionStart;
            string result = string.Empty;
            Data.CompanyRegistrationName = CompanyName.GetLineText(0);
            foreach (char character in CompanyName.Text.ToCharArray())
            {
                result += character;
            }
            CompanyName.Text = result;
            CompanyName.SelectionStart = selectionStart <= CompanyName.Text.Length ? selectionStart : CompanyName.Text.Length;
        }

        // 사업자등록번호 텍스트박스 헨들러
        private void CompanyNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox CompanyNum = sender as TextBox;
            CompanyNum.MaxLength = 10;
            int selectionStart = CompanyNum.SelectionStart;
            string result = string.Empty;
            foreach (char character in CompanyNum.Text.ToCharArray())
            {
                if (char.IsDigit(character) || char.IsControl(character))
                {
                    result += character;

                }
            }
            CompanyNum.Text = result;
            CompanyNum.SelectionStart = selectionStart <= CompanyNum.Text.Length ? selectionStart : CompanyNum.Text.Length;
        }
    }
}
