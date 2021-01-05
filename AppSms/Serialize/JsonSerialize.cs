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
using Newtonsoft.Json;

namespace AppSms.Json
{
    public class JsonSerialize
    {
        public string Serialize(object value,out string msg)
        {
            string text = string.Empty;
            msg = string.Empty;
            try
            {
                text = JsonConvert.SerializeObject(value, Formatting.Indented);
            }
            catch (Exception ex)
            {
                text = string.Empty;
                msg = ex.Message;
            }
            return text;
        }
    }
}