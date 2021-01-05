using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AppSms
{
    [Activity(Label = "ContentActivity")]
    public class ContactActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_contact);

            // Create your application here

            Button btnDisplay = FindViewById<Button>(Resource.Id.btnContact);
            btnDisplay.Click += BtnDisplay_Click;
        }

        private void BtnDisplay_Click(object sender, EventArgs e)
        {
            var contact = GetContact();
            TextView tv = FindViewById<TextView>(Resource.Id.tvSms);
            tv.SetText(contact, TextView.BufferType.Normal);
        }

        /// <summary>
        /// 获取通讯录内容
        /// </summary>
        /// <returns></returns>
        private string GetContact()
        {
            string[] projection = new string[] { "display_name", "sort_key", "contact_id",
                        "data1"  };

            StringBuilder contactStr = new StringBuilder();
            ICursor cur = ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, projection, null, null, null);

            while (cur.MoveToNext())
            {
                string name = cur.GetString(0);
                string number = cur.GetString(3);
                contactStr.Append($"姓名:{name},电话号码:{number}\n");
            }
            return contactStr.ToString();
        }
    }
}