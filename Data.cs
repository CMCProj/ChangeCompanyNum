using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeCompanyNum
{
    class Data
    {
        public static FileStream? BidFile;
        public static string? BidText;
        public static bool IsConvert = false; // 변환을 했는지 안했는지
        public static string CompanyRegistrationNum = ""; //1.31 사업자등록번호 추가
        public static string CompanyRegistrationName = ""; // 2.02 회사명 추가
    }
}
