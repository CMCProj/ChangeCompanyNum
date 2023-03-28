using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace ChangeCompanyNum
{
    internal class ChangeCompanyNum_Model
    {
        static XDocument? docBID;
        static IEnumerable<XElement>? eleBID;
        public static void ChangeCompanyNumber()
        {
            string copiedFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            docBID = XDocument.Load(Path.Combine(copiedFolder, "OutputDataFromBID.xml"));
            eleBID = docBID.Root.Elements();

            foreach (var bid in eleBID)
            {
                if (bid.Name == "T1")
                {
                    bid.Element("C17").Value = Data.CompanyRegistrationNum;
                    bid.Element("C18").Value = Data.CompanyRegistrationName;
                }
            }

            if(File.Exists(copiedFolder + "\\OutputDataFromBID.xml"))  //기존 OutputDataFromBID.xml 파일은 삭제한다.
            {
                File.Delete(copiedFolder + "\\OutputDataFromBID.xml");
            }

            //작업 후 xml 파일 저장
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xws = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true
            };
            using (XmlWriter xw = XmlWriter.Create(sb, xws))
            {
                docBID.WriteTo(xw);
            }
            File.WriteAllText(Path.Combine(copiedFolder, "OutputDataFromBID.xml"), sb.ToString());
        }
    }
}
