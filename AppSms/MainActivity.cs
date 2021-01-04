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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //联系人
            Button btnContact = FindViewById<Button>(Resource.Id.btnContact);
            btnContact.Click += new System.EventHandler((sender,e) => {
                StartActivity(typeof(ContactActivity));
            });

            //短信
            Button btnSms = FindViewById<Button>(Resource.Id.btnSms);
            btnSms.Click += new System.EventHandler((sender, e) => {
                StartActivity(typeof(SmsActivity));
            });

            
            try
            {
                GetPermission(new string[] {
                    Manifest.Permission.ReadContacts,
                    Manifest.Permission.ReadSms });
            }
            catch (System.Exception ex)
            {
                Log.Info(nameof(MainActivity), "导出短信异常:" + ex.Message);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        
        private long mExitTime;

        /// <summary>
        /// 双击退出程序
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="e"></param>
        /// <returns></returns>
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
    }
}