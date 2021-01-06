using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AppSms
{
    /// <summary>
    /// 短信内容
    /// </summary>
    class SmsInfo:Java.Lang.Object
    {
        /// <summary>
        /// 号码
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 姓名id
        /// </summary>
        public int Person { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; }
    }
}