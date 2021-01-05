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
    interface IExport
    {
        byte ExportData(string filePath, ICursor cur, out string mesg);
    }
}