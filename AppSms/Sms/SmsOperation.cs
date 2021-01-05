using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;

namespace AppSms
{
    class SmsOperation
    {
        public List<SmsInfo> GetSmsInfo(ICursor cur,int count)
        {
            List<SmsInfo> smsItems = new List<SmsInfo>();
            while (cur.MoveToNext())
            {
                if (cur.Count == count)
                    break;

                int index_Address = cur.GetColumnIndex("address");
                int index_Person = cur.GetColumnIndex("person");
                int index_Body = cur.GetColumnIndex("body");
                int index_Date = cur.GetColumnIndex("date");

                string strAddress = cur.GetString(index_Address);
                int intPerson = cur.GetInt(index_Person);
                string strbody = cur.GetString(index_Body);
                long longDate = cur.GetLong(index_Date);
                DateFormat format = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
                Date d = new Date(longDate);
                string date = format.Format(d);

                SmsInfo sms = new SmsInfo()
                {
                    Address = strAddress,
                    Person = intPerson,
                    Body = strbody,
                    Time = date
                };
                smsItems.Add(sms);
            }

            return smsItems;
        }
    }
}