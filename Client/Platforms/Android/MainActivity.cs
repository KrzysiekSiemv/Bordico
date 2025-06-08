using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Bordico.Client;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

// #if ANDROID
//         if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
//         {
//             var channel = new NotificationChannel("default_channel_id", "Default Channel", NotificationImportance.Default)
//             {
//                 Description = "Domyślny kanał powiadomień"
//             };

//             var notificationManager = (NotificationManager)GetSystemService(NotificationService);
//             notificationManager.CreateNotificationChannel(channel);
//         }
// #endif
    }

}
