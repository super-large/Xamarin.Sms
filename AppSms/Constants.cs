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
    class Constants
    {
        /// <summary>
        /// 短信收件箱
        /// </summary>
        public const string SMS_URI_ALL = "content://sms/";

        /// <summary>
        /// 联系人
        /// </summary>
        public const string CONTACT_URI_ALL = "content://com.android.contacts/contacts";
    }
}