using Android.App;
using Android.Content;
using Android.Database;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Java.Lang;
using Java.Text;
using Java.Util;
using Android.Util;
using Android.Provider;
using Android;
using Android.Content.PM;
using System.IO;
using Java.IO;
using Android.Preferences;
using Android.OS.Storage;
using Android.Views;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using System.Collections.Generic;

namespace AppSms
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        
        private Uri _smsUri;
        private string[] projection = new string[] { "_id", "address", "person", "body", "date", "type" };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _smsUri = Uri.Parse(Constants.SMS_URI_ALL);
            try
            {
                GetPermission(new string[] {
                    Manifest.Permission.ReadContacts,
                    Manifest.Permission.ReadSms });

                //显示短信
                Button btnDisplay = FindViewById<Button>(Resource.Id.btnDisplaySms);
                btnDisplay.Click += BtnDisplay_Click;

                //导出短信
                Button btExport = FindViewById<Button>(Resource.Id.btnExportSms);
                btExport.Click += btnExport_Click;

                //显示联系人
                Button btnDisplayContact = FindViewById<Button>(Resource.Id.btnDisplayContact);
                btnDisplayContact.Click += BtnDisplayContact_Click;
            }
            catch (System.Exception ex)
            {
                Log.Info(nameof(MainActivity), "导出短信异常:" + ex.Message);
            }
        }

        //显示联系人
        private void BtnDisplayContact_Click(object sender, System.EventArgs e)
        {
            var contact = GetContact();
            TextView tv = FindViewById<TextView>(Resource.Id.tvSms);
            tv.SetText(contact, TextView.BufferType.Normal);
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDisplay_Click(object sender, System.EventArgs e)
        {
            
            string text = GetSms(200);
            
            TextView tv = FindViewById<TextView>(Resource.Id.tvSms);
            tv.SetText(text, TextView.BufferType.Normal);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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

        private void btnExport_Click(object sender, System.EventArgs e)
        {
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
            

            //XmlHelper.Serialize(smsItems, fileSms.AbsolutePath);

            var task = System.Threading.Tasks.Task.Run(new System.Action(() =>
            {
                //ExportSms(fileSms);
                var smsItems = smsOpera.GetSmsInfo(cur);
                ExportSms(smsItems, path);
            }));

             

            //Android.App.AlertDialog.Builder dia = new Android.App.AlertDialog.Builder(this);
            //dia.SetPositiveButton("OK",
            //    new System.EventHandler<DialogClickEventArgs>((obj, args) =>
            //    {
            //        Toast.MakeText(this, "点击了OK", ToastLength.Short).Show();
            //    })
            //);

            //dia.Show();
        }

        private void ExportSms(List<SmsInfo>items,string path)
        {
           byte code =  XmlHelper.Serialize(items, path);
            if(code == 0)
            {
                Looper.Prepare();
                Toast.MakeText(this, $"{path}短信数据导出成功", ToastLength.Long).Show();
                Looper.Loop();
            }
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
                Log.Error(nameof(MainActivity), "写入文件异常:" + ex.Message);
            }
        }

        private long mExitTime;

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            //判断用户是否点击了“返回键”
            if (keyCode ==  Keycode.Back)
            {
                //与上次点击返回键时刻作差
                if ((JavaSystem.CurrentTimeMillis() - mExitTime) > 2000)
                {
                    //大于2000ms则认为是误操作，使用Toast进行提示
                    Toast.MakeText(this, "再按一次退出程序",  ToastLength.Short).Show();
                    //并记录下本次点击“返回键”的时刻，以便下次进行判断
                    mExitTime = JavaSystem.CurrentTimeMillis();
                }
                else
                {
                    //小于2000ms则认为是用户确实希望退出程序-调用System.exit()方法进行退出
                    JavaSystem.Exit(0);
                }
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }

        /// <summary>
        /// 获取系统权限
        /// </summary>
        /// <param name="permission"></param>
        private void GetPermission(string[] permission)
        {
            if (permission == null || permission.Length == 0)
                return;

            foreach (var item in permission)
            {
                if (ContextCompat.CheckSelfPermission(this,item) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { item }, 1);
                }
            }

            //Toast.MakeText(this, "已获得短信读取权限", ToastLength.Short).Show();
        }


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