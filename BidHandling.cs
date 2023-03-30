using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ChangeCompanyNum
{
    class BidHandling
    {
        public static string? filename;         //BID파일 복사본 이름
        public static string? innerFileName;    //BID파일 복사본 압축 해제 후 내부 파일의 이름

        // BID파일을 xml 파일로 변경 후 회사 명 및 사업자등록번호 변경
        public static void BidToXml()
        {
            string copiedFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); //바탕화면 경로
            string[] bidFile = Directory.GetFiles(copiedFolder, Data.BidText);                  //BID파일 복사본
            string myfile = bidFile[0];                                 //BID파일 복사본
            filename = Path.GetFileNameWithoutExtension(bidFile[0]);    //BID파일 복사본 이름

            if(File.Exists(filename + ".zip"))  //같은 이름을 가진 기존의 압축 파일이 이미 존재하면 삭제
            {
                File.Delete(filename + "zip");
            }

            File.Move(myfile, Path.ChangeExtension(myfile, ".zip"));    //BID파일 복사본을 압축파일로 변경

            ZipFile.ExtractToDirectory(Path.Combine(copiedFolder, filename + ".zip"), copiedFolder + "\\" + filename, true);    //압축 해제
            string[] files = Directory.GetFiles(copiedFolder + "\\" + filename, "*.BID");   //압축 해제 후 내부 파일을 가져옴
            innerFileName = Path.GetFileName(files[0]); //내부 파일 이름
            string text = File.ReadAllText(files[0]); // 텍스트 읽기
            byte[] decodeValue = Convert.FromBase64String(text);  // base64 변환
            text = Encoding.UTF8.GetString(decodeValue);   // UTF-8로 디코딩

            if (File.Exists(copiedFolder + "\\OutputDataFromBID.xml"))    //기존 OutputDataFromBID.xml은 삭제
            {
                File.Delete(copiedFolder + "\\OutputDataFromBID.xml");
            }

            File.WriteAllText(Path.Combine(copiedFolder , "OutputDataFromBID.xml"), text, Encoding.UTF8);   //디코딩된 내용을 OutputDataFromBID.xml에 기록

            ChangeCompanyNum_Model.ChangeCompanyNumber();   //회사 명 및 사업자등록번호 변경
        }

        //xml파일을 BID파일로 변경
        public static void XmlToBid()
        {
            string myPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); //바탕화면 경로
            string myfile = Path.Combine(myPath, "OutputDataFromBID.xml");                //작업을 진행한 OutputDataFromBID.xml 파일
            byte[] bytes = File.ReadAllBytes(myfile);               //OutputDataFromBID.xml 파일 내용
            string encodeValue = Convert.ToBase64String(bytes);     //Base64로 Encoding
                
            if (File.Exists(myPath + innerFileName))    //기존에 이미 내부 BID파일이 있다면 삭제
            {
                File.Delete(myPath + innerFileName);
            }
            File.WriteAllText(Path.Combine(myPath, innerFileName), encodeValue);    //내부 파일 이름으로 OutputDataFromBID.xml 파일 내용 기록

            string resultFileName = filename + ".zip";  //BID파일 복사본 압축 파일 이름

            if (File.Exists(myPath + "\\" + resultFileName))    //같은 이름을 가진 기존의 압축 파일이 이미 존재하면 삭제
            {
                File.Delete(myPath + "\\" + resultFileName);
            }

            using (ZipArchive zip = ZipFile.Open(Path.Combine(myPath, resultFileName), ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(Path.Combine(myPath, innerFileName), innerFileName);    //내부 파일의 내용을 압축 파일로 만듬
            }

            string resultBidPath = Path.ChangeExtension(Path.Combine(myPath, resultFileName), ".BID");  //사업자등록번호가 변경된 최종 BID파일 복사본 이름

            if (File.Exists(resultBidPath))    //기존에 이미 BID파일 복사본이 있다면 삭제
            {
                File.Delete(resultBidPath);
            }

            File.Move(Path.Combine(myPath, resultFileName), Path.ChangeExtension(Path.Combine(myPath, resultFileName), ".BID"));    //사업자등록번호가 변경된 최종 BID파일 복사본 생성

            //---------------작업 진행 중 생겨난 파일 및 폴더 삭제---------------
            if (File.Exists(myPath + "\\OutputDataFromBID.xml"))
            {
                File.Delete(myPath + "\\OutputDataFromBID.xml");
            }

            if (Directory.Exists(myPath + "\\" + filename))
            {
                Directory.Delete(myPath + "\\" + filename, true);
            }

            if (File.Exists(myPath + "\\" + innerFileName))
            {
                File.Delete(myPath + "\\" + innerFileName);
            }
        }
    }   
}