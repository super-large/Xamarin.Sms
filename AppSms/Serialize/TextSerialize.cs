using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace AppSms.Serialize
{
    class TextSerialize
    {
        public byte Serialize(string filePath,string smsText,out string mesg)
        {
            byte code = 0xff;
            mesg = string.Empty;
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(smsText);
            try
            {
                File fileSms = new File(filePath);
                OutputStream outp = new FileOutputStream(fileSms);
                outp.Write(buf, 0, buf.Length);
                outp.Close();
                outp.Dispose();
                code = 0;
                mesg = "短信数据导出成功";
            }
            catch (System.Exception ex)
            {
                Log.Error(nameof(SmsActivity), "写入文件异常:" + ex.Message);
                mesg = "导出text文件异常" + ex.Message;
            }
            return code;
        }
    }
}