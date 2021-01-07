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
        private string[] projection = new string[] { "display_name", "sort_key", "contact_id",
                        "data1"  };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_contact);

            var contact = GetContact();
            ListView lv = FindViewById<ListView>(Resource.Id.lvContact);

            IListAdapter contactAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, contact);
            lv.Adapter = contactAdapter;
        }

        /// <summary>
        /// 获取通讯录内容
        /// </summary>
        /// <returns></returns>
        private List<string> GetContact()
        {
            ICursor cur = ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, projection, null, null, null);
            List<string> items = new List<string>();
            while (cur.MoveToNext())
            {
                string name = cur.GetString(0);
                string number = cur.GetString(3);
                items.Add(name +"\r\n"+ number);
            }
            return items;
        }
    }
}