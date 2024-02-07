using Android.App;
using Android.Content.PM;
using Android.Database;
using Android.Nfc;
using Android.OS;
using Android.Util;
using Android.Widget;

using System;
using System.Data;
using Android.Database;
using Android.Net;
using Android.Util;
using Android.Widget;
using CommunityToolkit.Mvvm.Messaging;


namespace OEMInfo_MAUI_Sample
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private readonly string? TAG= "MainActivity";

        private readonly string URI_SERIAL = "content://oem_info/oem.zebra.secure/build_serial";
        private readonly string URI_IMEI = "content://oem_info/wan/imei";
        private readonly string URI_BT_MAC = "content://oem_info/oem.zebra.secure/bt_mac";
        private readonly string URI_WIFI_MAC = "content://oem_info/oem.zebra.secure/wifi_mac";

        protected override void OnPostCreate(Bundle savedInstanceState)
        {

            base.OnPostCreate(savedInstanceState);
            RetrieveOEMInfo(Android.Net.Uri.Parse(URI_SERIAL));
        }


        private void RetrieveOEMInfo(Android.Net.Uri? uri)
        {
            ICursor? cursor = ContentResolver?.Query(uri!, null, null, null, null);
            if (cursor == null || cursor.Count < 1)
            {
                string errorMsg = "Null cursor";
                WeakReferenceMessenger.Default.Send("#1-"+errorMsg);

                return;
            }
            while (cursor.MoveToNext())
            {
                if (cursor.ColumnCount == 0)
                {
                    string errorMsg = "Error: " + uri + " no data";

                    WeakReferenceMessenger.Default.Send("#2-" + errorMsg);
                }
                else
                {
                    for (int i = 0; i < cursor.ColumnCount; i++)
                    {
                        Log.Verbose(TAG, "column " + i + "=" + cursor.GetColumnName(i));
                        try
                        {
                            string? data = cursor.GetString(cursor.GetColumnIndex(cursor.GetColumnName(i)));

                            WeakReferenceMessenger.Default.Send("Serial: " + data);
                        }
                        catch (Exception e)
                        {
                            Log.Info(TAG, "Exception column " + cursor.GetColumnName(i));
                        }
                    }
                }
            }
            cursor.Close();
        }

    }
}
