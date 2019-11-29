using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using App.Droid.Services;
using System.Threading.Tasks;
using Android;
using Android.Support.V4.App;
using System.Linq;
using Android.Content;

namespace App.Droid
{
    [Activity(Label = "App", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static long DownloadId;
        public static string FileName;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            MessagingCenter.Subscribe<Xamarin.Forms.Button>(this, "LAUNCHINSTALLER", (sender) =>
            {
                if(haveStoragePermission() && hasRequestInstallPermisison())
                {
                    DownloadAndInstall();
                }
            });

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if(grantResults.Any(r => r == Permission.Granted))
            {
                MessagingCenter.Send<Xamarin.Forms.Button>(new Xamarin.Forms.Button(), "LAUNCHINSTALLER");
            }
        }

        public void DownloadAndInstall()
        {
            var manager = DownloadManager.FromContext(this); //(DownloadManager)this.GetSystemService(Android.Content.Context.DownloadService);
            var iservice = new InstallService();
            Task.Run(async () => await iservice.DownloadAndInstall(manager));
        }

        public bool hasRequestInstallPermisison()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if (CheckSelfPermission(Android.Manifest.Permission.RequestInstallPackages) == Permission.Granted)
                {
                    return true;
                }
                else
                {

                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.RequestInstallPackages }, 1);
                    return false;
                }
            }
            else
            { //you dont need to worry about these stuff below api level 23
                return true;
            }
        }
        
        public bool haveStoragePermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if (CheckSelfPermission(Android.Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                {
                    DownloadAndInstall();
                    return true;
                }
                else
                {

                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage }, 1);
                    return false;
                }
            }
            else
            { //you dont need to worry about these stuff below api level 23
                return true;
            }
        }

    }


}