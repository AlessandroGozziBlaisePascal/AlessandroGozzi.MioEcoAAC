using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlessandroGozzi.MioEco_Material.MobileGUI.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [RelayCommand]
        public async Task InviaSegnalazioneAsync()
        {
            string emailDestinazione = "alessandrogozzi14@gmail.com";
            string oggetto = "[MioEco] Segnalazione / Supporto";
            string corpo = "Ciao Alessandro,\n\n" +
                           "Scrivo in merito all'applicazione MioEco per segnalare quanto segue:\n\n" +
                           "[Descrivi qui il problema riscontrato o il suggerimento per l'app]\n\n" +
                           "Grazie e a presto!";

#if ANDROID
            try
            {
                string uriCompleto = $"mailto:{emailDestinazione}?subject={Android.Net.Uri.Encode(oggetto)}&body={Android.Net.Uri.Encode(corpo)}";
                var uri = Android.Net.Uri.Parse(uriCompleto);

                var intent = new Android.Content.Intent(Android.Content.Intent.ActionSendto, uri);
                intent.PutExtra(Android.Content.Intent.ExtraEmail, new string[] { emailDestinazione });
                intent.PutExtra(Android.Content.Intent.ExtraSubject, oggetto);
                intent.PutExtra(Android.Content.Intent.ExtraText, corpo);
                intent.SetPackage("com.google.android.gm");
                intent.AddFlags(Android.Content.ActivityFlags.NewTask);

                Android.App.Application.Context.StartActivity(intent);
                return;
            }
            catch (Android.Content.ActivityNotFoundException)
            {
                try
                {
                    string uriCompleto = $"mailto:{emailDestinazione}?subject={Android.Net.Uri.Encode(oggetto)}&body={Android.Net.Uri.Encode(corpo)}";
                    var fallbackIntent = new Android.Content.Intent(Android.Content.Intent.ActionSendto, Android.Net.Uri.Parse(uriCompleto));
                    fallbackIntent.AddFlags(Android.Content.ActivityFlags.NewTask);

                    Android.App.Application.Context.StartActivity(fallbackIntent);
                    return;
                }
                catch (Exception)
                {
                    await Shell.Current.DisplayAlert("Avviso", "Nessuna applicazione email configurata sul dispositivo.", "OK");
                    return;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Errore", $"Impossibile avviare l'email: {ex.Message}", "OK");
                return;
            }
#endif

            try
            {
                var uriString = $"mailto:{emailDestinazione}?subject={Uri.EscapeDataString(oggetto)}&body={Uri.EscapeDataString(corpo)}";
                await Launcher.Default.OpenAsync(new Uri(uriString));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Errore", ex.Message, "OK");
            }
        }
    }
}
