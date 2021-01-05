using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AppSms.Serialize;
using Java.IO;

namespace AppSms.Export
{
    class TextExport : IExport
    {
        public byte ExportData(string filePath, ICursor cur, out string mesg)
        {
            mesg = string.Empty;
            SmsOperation smsOpera = new SmsOperation();
            var smsItems = smsOpera.GetSmsInfo(cur, int.MaxValue);
            StringBuilder smsText = new StringBuilder();
            foreach (var sms in smsItems)
            {
                smsText.Append("[ ");
                smsText.Append(sms.Address + ", ");
                smsText.Append(sms.Person + ", ");
                smsText.Append(sms.Body + ", ");
                smsText.Append(sms.Time);
                smsText.Append(" ]\n\n");
            }
            TextSerialize seria = new TextSerialize();
             byte code = seria.Serialize(filePath, smsText.ToString(), out string msg);
            mesg = msg;
            return code;
        }
    }
}