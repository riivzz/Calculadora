using Microsoft.Extensions.DependencyInjection;

namespace Calculadora
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            //Fijamos el tamaño exacto para Windows
            const int anchoFijo = 400;
            const int altoFijo = 650;

            window.Width = anchoFijo;
            window.Height = altoFijo;

            //Al poner el mínimo y el máximo igual que el tamaño actual, bloqueamos el redimensionamiento
            window.MinimumWidth = anchoFijo;
            window.MinimumHeight = altoFijo;
            window.MaximumWidth = anchoFijo;
            window.MinimumHeight = altoFijo;

            return window;
        }
    }
}