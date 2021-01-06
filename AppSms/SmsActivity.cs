
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

            SetContentView(Resource.Layout.activity_sms);
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

            string path = Path.Combine(GetExternalFilesDir(Environment.DirectoryDcim).AbsolutePath,
                $"Sms_QQPhoneManager({System.DateTime.Now.ToLongDateString()})");
            
            IExport ex = null;
            switch (radioGrp.CheckedRadioButtonId)
            {
                case Resource.Id.rdoJson:
                    ex = new JsonExport();
                    path = string.Concat(path, ".json");
                    break;
                case Resource.Id.rdoText:
                    ex = new TextExport();
                    path = string.Concat(path, ".txt");
                    break;
                case Resource.Id.rdoXml:
                    ex = new XmlExport();
                    path = string.Concat(path, ".xml");
                    break;
            }

            if (ex == null)
            {
                Toast.MakeText(this, $"请选择导出短信文本类型", ToastLength.Long).Show();
                return;
            }

            Java.IO.File fileSms = new Java.IO.File(path);
            if (fileSms.Exists())
                fileSms.Delete();

            bool suc = fileSms.CreateNewFile();
            if (!suc)
                return;

            ICursor cur = ContentResolver.Query(_smsUri, projection, null, null, null);
          

            var task = Task.Run(new System.Action(() =>
            {
                Looper.Prepare();
                byte code = ex.ExportData(path, cur, out string msg);
                if (code == 0)
                    Toast.MakeText(this, $"导出短信导出成功:{path}", ToastLength.Long).Show();
                else
                    Toast.MakeText(this, $"导出短信导出异常:{msg}", ToastLength.Long).Show();
                Looper.Loop();
            }));
        }

        private void BtnDisplay_Click(object sender, System.EventArgs e)
        {
            //string text = GetSms(200);
            //TextView tv = FindViewById<TextView>(Resource.Id.tvSms);
            //tv.SetText(text, TextView.BufferType.Normal);
        }
    }
}