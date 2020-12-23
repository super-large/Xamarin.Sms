﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace AppSms
{
    class XmlHelper
    {
        internal static byte Serialize(List<SmsInfo> smsItems, string fileName)
        {
            byte code = 0;
            var s = Android.Util.Xml.NewSerializer();
            Java.IO.Writer fos = new Java.IO.FileWriter(fileName);
            string text = string.Empty;
            try
            {
                s.SetOutput(fos);
                s.StartDocument("utf-8", Java.Lang.Boolean.True);
                s.StartTag(null, "SmsInfo");
                foreach (var item in smsItems)
                {
                    text = item.Body;
                    s.StartTag(null, "Address");    // <person>
                    s.Attribute(null, "Number", item.Address);

                    s.StartTag(null, "Person");
                    s.Text(item.Person.ToString());
                    s.EndTag(null, "Person");

                    s.StartTag(null, "Body");
                    string body = RemoveSpecialCharacter(item.Body);
                    s.Text(body);
                    s.EndTag(null, "Body");

                    s.StartTag(null, "Time");
                    s.Text(item.Time);
                    s.EndTag(null, "Time");

                    s.EndTag(null, "Address");

                }

                code = 0;

                s.EndTag(null, "SmsInfo");
                s.EndDocument();
                fos.Close();
                s.Flush();
                s.Dispose();
            }
            catch (System.Exception ex)
            {
                code = 1;
            }
            return code;
        }

        public static string RemoveSpecialCharacter(string hexData)
        {
            return Regex.Replace(hexData, "[ \\[ \\] \\^ \\-_*×――(^)$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]", "").ToUpper();
        }
    }
}