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
using AppSms.Json;
using AppSms.Serialize;

namespace AppSms.Export
{
    class JsonExport : IExport
    {
        public byte ExportData(string filePath, ICursor cur, out string mesg)
        {
            mesg = string.Empty;
            SmsOperation opera = new SmsOperation();
            var smsItems = opera.GetSmsInfo(cur, int.MaxValue);
            JsonSerialize jsonSera = new JsonSerialize();
            string jsonText = jsonSera.Serialize(smsItems, out string msg);
            mesg = msg;

            TextSerialize seria = new TextSerialize();
            byte code = seria.Serialize(filePath, jsonText, out msg);
            mesg = msg;
            return code;
        }
    }
}