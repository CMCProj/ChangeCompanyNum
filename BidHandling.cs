using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ChangeCompanyNum
{
    class BidHandling
    {
        //String folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); 사용 안함 (23.02.02)
        public static string? filename;
        public static string? innerFileName;

        public static void BidToXml()
        {
            string copiedFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string[] bidFile = Directory.GetFiles(copiedFolder, "*_복사본.BID");
            string myfile = bidFile[0];
            filename = Path.GetFileNameWithoutExtension(bidFile[0]);
            if(File.Exists(filename + ".zip"))
            {
                File.Delete(filename + "zip");
            }

            File.Move(myfile, Path.ChangeExtension(myfile, ".zip"));

            ZipFile.ExtractToDirectory(Path.Combine(copiedFolder, filename + ".zip"), copiedFolder, true);
            string[] files = Directory.GetFiles(copiedFolder, "*.BID");
            innerFileName = Path.GetFileName(files[1]);
            string text = File.ReadAllText(files[1]); // 텍스트 읽기
            byte[] decodeValue = Convert.FromBase64String(text);  // base64 변환
            text = Encoding.UTF8.GetString(decodeValue);   // UTF-8로 디코딩

            if (File.Exists(copiedFolder + "\\OutputDataFromBID.xml"))    //기존 OutputDataFromBID.xml은 삭제한다. (23.02.02)
            {
                File.Delete(copiedFolder + "\\OutputDataFromBID.xml");
            }

            File.WriteAllText(Path.Combine(copiedFolder , "OutputDataFromBID.xml"), text, Encoding.UTF8); //폴더 경로 수정 (23.02.02)

            ChangeCompanyNum_Model.ChangeCompanyNumber();
        }

        public static void XmlToBid()
        {
            string myPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string myfile = Path.Combine(myPath, "OutputDataFromBID.xml");
            byte[] bytes = File.ReadAllBytes(myfile);
            string encodeValue = Convert.ToBase64String(bytes);

            if (File.Exists(myPath + innerFileName))    //기존 XmlToBID.BID은 삭제한다. (23.02.02)
            {
                File.Delete(myPath + innerFileName);
            }
            File.WriteAllText(Path.Combine(myPath, innerFileName), encodeValue);

            string resultFileName = filename + ".zip";

            if (File.Exists(myPath + "\\" + resultFileName))    //기존 공내역파일명.zip은 삭제한다. (23.02.02)
            {
                File.Delete(myPath + "\\" + resultFileName);
            }
            using (ZipArchive zip = ZipFile.Open(Path.Combine(myPath, resultFileName), ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(Path.Combine(myPath, innerFileName), innerFileName);
            }

            string resultBidPath = Path.ChangeExtension(Path.Combine(myPath, resultFileName), ".BID");

            if (File.Exists(resultBidPath))    //기존 공내역파일명.BID은 삭제한다. (23.02.02)
            {
                File.Delete(resultBidPath);
            }
            File.Move(Path.Combine(myPath, resultFileName), Path.ChangeExtension(Path.Combine(myPath, resultFileName), ".BID"));

            if (File.Exists(myPath + "\\OutputDataFromBID.xml"))
            {
                File.Delete(myPath + "\\OutputDataFromBID.xml");
            }

            if (File.Exists(myPath + "\\" + innerFileName))
            {
                File.Delete(myPath + "\\" + innerFileName);
            }
        }
    }   
}