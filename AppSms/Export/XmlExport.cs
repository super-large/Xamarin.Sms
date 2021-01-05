using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}