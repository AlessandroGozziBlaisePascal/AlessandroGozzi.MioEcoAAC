namespace AlessandroGozzi.AppAAC.MobileGUI
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        private readonly IntroPage _introPage;

        public App(IntroPage introPage)
        {
            InitializeComponent();
            _introPage = introPage;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_introPage);
        }
    }
}