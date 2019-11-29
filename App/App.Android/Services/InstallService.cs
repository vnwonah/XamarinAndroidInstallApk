using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;
using Java.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App.Droid.Services
{
    public class InstallService
    {
        public async Task DownloadAndInstall(DownloadManager manager)
        {

            string url = @"https://app.cforce.live/com.payforce.apk";
            var source = Android.Net.Uri.Parse(url);
            var request = new DownloadManager.Request(source);
            request.AllowScanningByMediaScanner();
            request.SetAllowedOverRoaming(true);
            request.SetDescription("Downloading PayForce");
            request.SetAllowedOverMetered(true);
            request.SetVisibleInDownloadsUi(true);
            string filename = URLUtil.GuessFileName(url, null, MimeTypeMap.GetFileExtensionFromUrl(url));
            MainActivity.FileName = filename;
            request.SetDestinationInExternalPublicDir(Android.OS.Environment.DirectoryDownloads, filename);
            request.SetNotificationVisibility(DownloadManager.Request.VisibilityVisibleNotifyCompleted);
            request.SetAllowedNetworkTypes(DownloadNetwork.Wifi | DownloadNetwork.Mobile);
            MainActivity.DownloadId = manager.Enqueue(request);
        }
    }

    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { DownloadManager.ActionDownloadComplete })]
    public class DownloadReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // Do stuff here.
            long id = intent.GetLongExtra(DownloadManager.ExtraDownloadId, -1);
            if(id == MainActivity.DownloadId)
            {
                Intent installIntent = new Intent(Intent.ActionInstallPackage);
                installIntent.SetDataAndType(Android.Net.Uri.FromFile(new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads) +"/"+ MainActivity.FileName)), "application/vnd.android.package-archive");
                installIntent.SetFlags(ActivityFlags.NewTask);
                Forms.Context.StartActivity(installIntent);
            }
        }
    }
}