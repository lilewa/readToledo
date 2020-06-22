using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Report.Common
{
    public class UserInfo
    {
       public static string userid;
       public static string username;
       public static string ywbmbh;
       public static UserType type; //0:
    }

    public enum UserType
    {
       local,
       customer
    }

    public class CommonUse
    {
        public static XElement xmlAppSet;
        public static string connectionstring;
        public static Dictionary<string, DataTable> tables=new Dictionary<string, DataTable>();
    }
}
