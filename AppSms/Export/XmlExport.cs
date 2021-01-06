using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AppSms.Export
{
    class XmlExport : IExport
    {
        public byte ExportData(string filePath, ICursor cur,out string mesg)
        {
            SmsOperation smsOpera = new SmsOperation();
            var smsItems = smsOpera.GetSmsInfo(cur, int.MaxValue);
            XmlSerialize xml = new XmlSerialize(filePath);
            byte code = xml.Serialize(smsItems, out string msg);
            mesg = msg;
            return code;
        }

        private string FormatXml(string sUnformattedXml)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(sUnformattedXml);
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter xtw = null;
            try
            {
                xtw = new XmlTextWriter(sw);
                xtw.Formatting = Formatting.Indented;
                xtw.Indentation = 1;
                xtw.IndentChar = '\t';
                xd.WriteTo(xtw);
            }
            finally
            {
                if (xtw != null)
                    xtw.Close();
            }
            return sb.ToString();
        }
    }
}