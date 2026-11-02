using AlessandroGozzi.MioEco_Material.MobileGUI.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace AlessandroGozzi.MioEco_Material.MobileGUI
{
    public partial class App : Application
    {
        public App(IntroPage introPage)
        {
            InitializeComponent();

            // Imposta la pagina di avvio con l'animazione e il suono
            MainPage = introPage;
        }
    }
}