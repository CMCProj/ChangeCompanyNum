using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

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
        }
    }
}
