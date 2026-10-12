using System.Globalization;

namespace Calculadora
{
    public partial class MainPage : ContentPage
    {
        private double subtotal = 0;
        private double memoria = 0;
        private string operadorActual = "";
        private bool nuevaEntrada = true;

        public MainPage()
        {
            InitializeComponent();
        }

        // Método auxiliar para evitar que Android crashee por el separador decimal (coma vs punto)
        private double ObtenerNumeroDePantalla()
        {
            string textoLimpio = PantallaLabel.Text.Replace(',', '.');
            if (double.TryParse(textoLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out double resultado))
            {
                return resultado;
            }
            return 0;
        }

        // Único evento para todos lo números (0..9 y ,)
        private void OnNumeroClicked(object sender, EventArgs e)
        {
            Vibrar();
            Button boton = (Button)sender;
            string numeroPulsado = boton.Text;

            if (nuevaEntrada)
            {
                PantallaLabel.Text = numeroPulsado == "," ? "0," : numeroPulsado;
                nuevaEntrada = false;
            }
            else
            {
                // Evitar múltiples comas
                if (numeroPulsado == "," && PantallaLabel.Text.Contains(","))
                    return;

                PantallaLabel.Text += numeroPulsado;
            }
        }

        // Único evento para operadores (+, -, *, /)
        private void OnOperadorClicked(object sender, EventArgs e)
        {
            Vibrar();
            Button boton = (Button)sender;
            string operadorPulsado = boton.Text;

            // Si ya había un operador pendiente, calculamos el subtotal
            if (!nuevaEntrada && !string.IsNullOrEmpty(operadorActual))
            {
                CalcularSubtotal();
            }
            else
            {
                subtotal = ObtenerNumeroDePantalla();
            }

            operadorActual = operadorPulsado;
            nuevaEntrada = true;
        }

        private void OnIgualClicked(object sender, EventArgs e)
        {
            Vibrar();
            if (string.IsNullOrEmpty(operadorActual)) return;

            double valorAnterior = subtotal;
            double valorActual = ObtenerNumeroDePantalla();
            string operadorUsado = operadorActual;

            CalcularSubtotal();

            // Agregamos la entrada completa al historial deslizable
            AgregarHistorial($"{valorAnterior} {operadorUsado} {valorActual} = {PantallaLabel.Text}");

            operadorActual = "";
            nuevaEntrada = true;
        }

        // Método auxiliar para el cálculo
        private void CalcularSubtotal()
        {
            double valorActual = ObtenerNumeroDePantalla();
            double resultado = 0;

            switch (operadorActual)
            {
                case "+": resultado = subtotal + valorActual; break;
                case "-": resultado = subtotal - valorActual; break;
                case "*": resultado = subtotal * valorActual; break;
                case "/":
                    if (valorActual != 0) resultado = subtotal / valorActual;
                    else { PantallaLabel.Text = "Error"; return; }
                    break;
            }

            subtotal = resultado;
            PantallaLabel.Text = Convert.ToString(subtotal);
        }

        private async void AgregarHistorial(string textoOperacion)
        {
            Label nuevaLinea = new Label
            {
                Text = textoOperacion,
                TextColor = Color.FromArgb("#8E8E93"),
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Center
            };

            HistorialContainer.Children.Add(nuevaLinea);

            await Task.Delay(50);
            await HistorialScroll.ScrollToAsync(0, HistorialScroll.ContentSize.Height, true);
        }

        // Evento para las funciones de memoria
        private void OnMemoriaClicked(object sender, EventArgs e)
        {
            Vibrar();
            Button boton = (Button)sender;
            string accion = boton.Text;

            // Protegemos contra posibles errores de pantalla
            if (PantallaLabel.Text == "Error") return;

            double valorActual = ObtenerNumeroDePantalla();

            switch (accion)
            {
                case "M+":
                    memoria += valorActual;
                    break;
                case "M-":
                    memoria -= valorActual;
                    break;
                case "MC":
                    memoria = 0;
                    break;
                case "M":
                    PantallaLabel.Text = Convert.ToString(memoria);
                    nuevaEntrada = true;
                    break;
            }

            MemoriaLabel.Text = memoria != 0 ? $"Mem: {memoria}" : "";
        }

        // Evento para borrar
        private void OnClearClicked(object sender, EventArgs e)
        {
            Vibrar();
            PantallaLabel.Text = "0";
            subtotal = 0;
            operadorActual = "";
            nuevaEntrada = true;
        }

        private void Vibrar()
        {
            try
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
            catch (Exception)
            {
                // Captura cualquier excepción si se ejecuta en plataformas o móviles sin soporte háptico
            }
        }

        private void OnBorrarHistorialClicked(object sender, EventArgs e)
        {
            Vibrar();
            // Limpiamos todos los labels generados dentro del historial
            HistorialContainer.Children.Clear();
        }
    }
}