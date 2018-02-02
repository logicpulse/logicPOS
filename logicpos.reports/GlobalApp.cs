using System;
using DevExpress.Xpo;
using System.Collections;
using System.Collections.Specialized;
using System.Resources;
using System.Globalization;

namespace logicpos.reports
{
    public static class GlobalApp
    { //Settings
        public static NameValueCollection Settings;
        //Localization
        public static CultureInfo CurrentCulture;
        public static CultureInfo CurrentCultureNumberFormat;
        //System Paths
        public static Hashtable Path;
        //Session
        public static Session SessionXpo;
        public static Session SessionXpoBackoffice;
        //Windows
        public static StartupWindow WindowStartup;
        public static System.Windows.Forms.Form WindowReportsWinForm;
        public static System.Windows.Forms.Form PreviewReport;
        //Database
        public static String DatabaseType;
        //Other
        public static Boolean MultiUserEnvironment;
        public static Boolean UseVirtualKeyBoard;
        //Licence
        public static String licenceVersion;
        public static String licenceDate;
        public static String licenceName;
        public static String licenceCompany;
        public static String licenceAddress;
        public static String licenceEmail;
        public static String licenceTelephone;
    }
}
