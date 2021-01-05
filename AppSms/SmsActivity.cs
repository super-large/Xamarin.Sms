
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AppSms.Export;
using Java.IO;
using Java.Lang;
using Java.Text;
using Java.Util;

namespace AppSms
{
    [Activity(Label = "SmsActivity")]
    public class SmsActivity : Activity
    {
        private Uri _smsUri = Uri.Parse(Constants.SMS_URI_ALL);
        private string[] projection = new string[] { "_id", "address", "person", "body", "date", "type" };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //显示短信
            Button btnDisplay = FindViewById<Button>(Resource.Id.btnDisplaySms);
            btnDisplay.Click += BtnDisplay_Click;


            //导出短信
            Button btnExport = FindViewById<Button>(Resource.Id.btnExportSms);
            btnExport.Click += BtnExport_Click;
        }

        private void BtnExport_Click(object sender, System.EventArgs e)
        {
            RadioGroup radioGrp = FindViewById<RadioGroup>(Resource.Id.rdoGroupSms);
            if (radioGrp == null)
                return;

            IExport ex = null;
            switch (radioGrp.CheckedRadioButtonId)
            {
                case Resource.Id.rdoJson:
                    ex = new JsonExport();
                    break;
                case Resource.Id.rdoText:
                    ex = new TextExport();
                    break;
                case Resource.Id.rdoXml:
                    ex = new XmlExport();
                    break;
                default:
                    return;
            }

            string path = Path.Combine(GetExternalFilesDir(Environment.DirectoryDcim).AbsolutePath,
                $"Sms_QQPhoneManager({System.DateTime.Now.ToString("yyyy-HH-dd")}).xml");
            Java.IO.File fileSms = new Java.IO.File(path);
            if (fileSms.Exists())
                fileSms.Delete();

            bool suc = fileSms.CreateNewFile();
            if (!suc)
                return;
            ICursor cur = ContentResolver.Query(_smsUri, projection, null, null, null);
            SmsOperation smsOpera = new SmsOperation();

            var task = Task.Run(new System.Action(() =>
            {
                var smsItems = smsOpera.GetSmsInfo(cur);
                ExportSms(smsItems, path);
            }));
        }

        private void BtnDisplay_Click(object sender, System.EventArgs e)
        {
            string text = GetSms(200);
            TextView tv = FindViewById<TextView>(Resource.Id.tvSms);
            tv.SetText(text, TextView.BufferType.Normal);
        }

        private string GetSms(int count)
        {
            var cur = ContentResolver.Query(_smsUri, projection, null, null, null);
            StringBuilder smsBuilder = new StringBuilder();
            int i = 0;
            while (cur.MoveToNext())
            {
                if (i > count)
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

                smsBuilder.Append("[ ");
                smsBuilder.Append(strAddress + ", ");
                smsBuilder.Append(intPerson + ", ");
                smsBuilder.Append(strbody + ", ");
                smsBuilder.Append(date);
                smsBuilder.Append(" ]\n\n");
                i++;
            }
            return smsBuilder.ToString();
        }

        private void ExportSms(List<SmsInfo> items, string path)
        {
            XmlSerialize xml = new XmlSerialize(path);
            byte code = xml.Serialize(items, out string msg);
            Looper.Prepare();
            if (code == 0)
            {
                Toast.MakeText(this, $"{path}短信数据导出成功", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, $"导出短信数据异常:{msg}", ToastLength.Long).Show();
            }
            Looper.Loop();
        }

        private void ExportSms(Java.IO.File fileSms)
        {
            string text = GetSms(10000);
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(text);
            try
            {
                OutputStream outp = new FileOutputStream(fileSms);
                outp.Write(buf, 0, buf.Length);
                outp.Close();
                outp.Dispose();
                Looper.Prepare();
                Toast.MakeText(this, $"{fileSms.AbsoluteFile}短信数据导出成功", ToastLength.Long).Show();
                Looper.Loop();
            }
            catch (System.Exception ex)
            {
                Log.Error(nameof(SmsActivity), "写入文件异常:" + ex.Message);
            }
        }
    }
}