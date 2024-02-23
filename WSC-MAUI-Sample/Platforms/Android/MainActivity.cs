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
using Android.Runtime;
using Android.Views;
using Android.Content;
using System.Text;
using static Android.Provider.ContactsContract;
using System.Diagnostics;


namespace OEMInfo_MAUI_Sample
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private readonly string? TAG= "MainActivity";

        protected override void OnPostCreate(Bundle savedInstanceState)
        {

            base.OnPostCreate(savedInstanceState);

            WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(() => {
                    if (m.StartsWith("#"))
                        ApplyWSCconfig("Config_MANAGER.json").Wait();
                });
            });
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent? e)
        {
            WeakReferenceMessenger.Default.Send("keycode=" + keyCode);
            return true;
        }

        string jsonWSCconfig="n/a";

        public async void WriteTextToFile(string text, string targetFileName)
        {
            string targetFile = System.IO.Path.Combine("/enterprise/usr/", targetFileName); //you can also use different local folders
            using FileStream outputStream = System.IO.File.OpenWrite(targetFile);
            using StreamWriter streamWriter = new StreamWriter(outputStream);
            await streamWriter.WriteAsync(text);
        }


        async Task ApplyWSCconfig(string assetFile)
        {
            //READ JSON FROM ASSET FOLDER
            using var stream = await FileSystem.OpenAppPackageFileAsync(assetFile);
            using var reader = new StreamReader(stream);
            jsonWSCconfig = reader.ReadToEnd();
            WriteTextToFile(jsonWSCconfig, "wsc_config.txt");

            var path = "/enterprise/usr/wsc_config.txt";
            string[] cmd = { "chmod", "666", path };
            Java.Lang.Runtime? runtime = Java.Lang.Runtime.GetRuntime();
            var res = runtime?.Exec(cmd)!.WaitFor();

            String _target_package = "com.zebra.workstationconnect.release";
            String _target_sig = "";
            String targetPath = "com.zebra.workstationconnect.release/enterprise/workstation_connect_config.txt";
            FeedLocalFileToSSM(path, _target_package, _target_sig, targetPath);

        }

        private readonly string AUTHORITY_FILE = "content://com.zebra.securestoragemanager.securecontentprovider/files/";
        private readonly string RETRIEVE_AUTHORITY = "content://com.zebra.securestoragemanager.securecontentprovider/file/*";
        private readonly string COLUMN_DATA_NAME = "data_name";
        private readonly string COLUMN_DATA_VALUE = "data_value";
        private readonly string COLUMN_DATA_TYPE = "data_type";
        private readonly string COLUMN_DATA_PERSIST_REQUIRED = "data_persist_required";
        private readonly string COLUMN_TARGET_APP_PACKAGE = "target_app_package";
        private readonly string signature = "";
        private void FeedLocalFileToSSM(string srcFile, string targetPackage, string targetSig, string targetPath)
        {
            Android.Net.Uri cpUriQuery = Android.Net.Uri.Parse(AUTHORITY_FILE + AppInfo.Current.PackageName)!;
            //Log.Info(TAG, "FeedLocalFileToSSM authority  " + cpUriQuery.ToString());

            StringBuilder _sb = new StringBuilder();

            try
            {
                ContentValues values = new ContentValues();
                string packageSig = "{\"pkg\":\"" + targetPackage + "\",\"sig\":\"" + targetSig + "\"}";
                string allPackagesSigs = "{\"pkgs_sigs\":[" + packageSig + "]}";

                values.Put(COLUMN_TARGET_APP_PACKAGE, allPackagesSigs);
                values.Put(COLUMN_DATA_NAME, srcFile);
                values.Put(COLUMN_DATA_TYPE, "3");
                values.Put(COLUMN_DATA_VALUE, targetPath);
                values.Put(COLUMN_DATA_PERSIST_REQUIRED, "false");
                Android.Net.Uri createdRow = ContentResolver?.Insert(cpUriQuery, values)!;
                _sb.Append("Insert Result rows: " + createdRow + "\n");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SSM Insert File - error: " + e.Message + "\n\n");
                _sb.Append("SSM Insert File - error: " + e.Message + "\n\n");
            }
        }



    }
}
