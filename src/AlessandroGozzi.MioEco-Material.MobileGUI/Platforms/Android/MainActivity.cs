using Android.App;
using Android.Content.PM;
using Android.OS;

namespace AlessandroGozzi.MioEco_Material.MobileGUI
{
    [Activity(
    Theme = "@style/Maui.SplashTheme", 
    MainLauncher = true, 
    Label = "MioEco",
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
}
}
