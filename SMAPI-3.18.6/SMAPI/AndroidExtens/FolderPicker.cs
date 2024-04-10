using Android.Content;
using StardewValley;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Uri = Android.Net.Uri;

namespace StardewModdingAPI.AndroidExtens
{
    public class FolderPicker
    {
        public static string ExternalFilesDir => MainActivity.instance.ApplicationContext.GetExternalFilesDir(null).AbsolutePath;
        public static string ExternalPublicDir => Android.OS.Environment.GetExternalStoragePublicDirectory("").AbsolutePath;
        public static string DownloadDir => Path.Combine(ExternalPublicDir, "Download");
        public static string StardewFarmMigratePath => Path.Combine(ExternalPublicDir, "StardewValley");
        public static string ExternalSMAPIDir => Path.Combine(StardewFarmMigratePath, "SMAPI-Game");
        public static string ExternalModsDir => Path.Combine(ExternalSMAPIDir, "Mods");

        public static int RequestCode_Pick = "FolderPicer_Pick".GetHashCode();
        static TaskCompletionSource<Uri> taskFolderPickerResult;
        public static Assembly MonoAndroidExtens = null;
        public static void Init()
        {
            MainActivityPatcher.OnActivityResult += OnActivityResult;
        }
        public static async Task<Uri> Pick()
        {
            var intent = new Intent("android.intent.action.OPEN_DOCUMENT_TREE");
            intent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
            taskFolderPickerResult = new TaskCompletionSource<Uri>();
            MainActivity.instance.StartActivityForResult(intent, RequestCode_Pick);

            await taskFolderPickerResult.Task;
            return taskFolderPickerResult.Task.Result;
        }

        private static void OnActivityResult(int reqCode, Android.App.Result result, Intent data)
        {
            AndroidLog.Log("On FolderPicker req: " + reqCode + ", reulst: " + result + ", data " + data);
            if (reqCode == RequestCode_Pick)
            {
                taskFolderPickerResult.SetResult(data?.Data);
                taskFolderPickerResult = null;
            }
        }
    }
}