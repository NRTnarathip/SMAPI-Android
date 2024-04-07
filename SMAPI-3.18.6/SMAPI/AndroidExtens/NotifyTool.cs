using Android.App;
using StardewValley;
using System;

namespace StardewModdingAPI.AndroidExtens
{
    public class NotifyTool
    {
        public static Activity Activity => MainActivity.instance;
        public static void Confirm(string title, string msg, System.Action<bool> callback, string btnOk = "Yes", string btnCancel = "No")
        {
            AlertDialog.Builder builder = new(Activity);
            builder.SetTitle(title);
            builder.SetMessage(msg);
            builder.SetPositiveButton(btnOk, delegate
            {
                callback(true);
            });
            builder.SetNegativeButton(btnCancel, delegate
            {
                callback(false);
            });
            builder.SetCancelable(cancelable: false);
            AlertDialog alertDialog = builder.Create();
            alertDialog.Show();
        }
        public static void ConfirmOnly(string title, string msg, Action confirmCallback)
        {
            Confirm(title, msg, (isConfirm) =>
            {
                if (isConfirm)
                    confirmCallback();
            });
        }
        public static void Notify(string title, string msg)
        {
            AlertDialog.Builder builder = new(Activity);
            builder.SetTitle(title);
            builder.SetMessage(msg);
            builder.SetCancelable(cancelable: true);
            builder.SetPositiveButton("OK", delegate
            {

            });
            AlertDialog alertDialog = builder.Create();
            alertDialog.Show();
        }
    }
}