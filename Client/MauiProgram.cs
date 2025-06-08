using Microsoft.Extensions.Logging;
using DotNet.Meteor.HotReload.Plugin;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using System.Net.Http;
using Bordico.Client.Service;
// #if ANDROID
// using Plugin.Firebase.Auth;
// using Plugin.Firebase.Core.Platforms.Android;
// #endif
// #if IOS
// using Plugin.Firebase.Auth;
// using Plugin.Firebase.Core.Platforms.iOS;
// #endif
using Microsoft.Extensions.DependencyInjection;
using Bordico.Client.ViewModel;
using Bordico.Client.View;

namespace Bordico.Client;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
#if DEBUG
			.EnableHotReload()
#endif
// #if ANDROID || IOS
// 			.RegisterFirebaseServices()
// #endif
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
		builder.Services.AddScoped(sp => new HttpClient
		{
			BaseAddress = new Uri(Constants.Server)
		});
		builder.Services.AddSingleton<RestService>();
		builder.Services.AddSingleton<HomeViewModel>();
		builder.Services.AddSingleton<HomePage>();

		return builder.Build();
	}

// 	private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
//     {
//         builder.ConfigureLifecycleEvents(events => {
// #if IOS
//             events.AddiOS(iOS => iOS.WillFinishLaunching((_,__) => {
//                 CrossFirebase.Initialize();
//                 return false;
//             }));
// #elif ANDROID
//             events.AddAndroid(android => android.OnCreate((activity, _) =>
//                 CrossFirebase.Initialize(activity)));
// #endif
//         });
//         #if ANDROID || IOS
//         builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
// 		#endif
//         return builder;
//     }

}
