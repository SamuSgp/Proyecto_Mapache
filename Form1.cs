using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;

namespace Mapache_web
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string connectionString = "Escritorio\\SQLEXPRESS;Initial Catalog=MapacheBD;Integrated Security=True;";

        }
        private string _connectionString =
        "Data Source=Escritorio\\SQLEXPRESS;" +
        "Initial Catalog=MapacheBD;" +
        "Integrated Security=True;";

        private async void Form1_Load(object sender, EventArgs e)
        {
            await webView21.EnsureCoreWebView2Async();

            // Carga el archivo HTML local (asegúrate de que "map.html" esté en el proyecto)
            string htmlPath = Path.Combine(Application.StartupPath, "map.html");
            webView21.Source = new Uri("file:///" + htmlPath);

            // Suscribe el evento para recibir mensajes de JavaScript
            webView21.WebMessageReceived += WebView21_WebMessageReceived;
        }

        private void WebView21_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            // Lee el mensaje como JSON
            string json = e.WebMessageAsJson;
            // Por ejemplo: {"action":"login","username":"admin","password":"1234"}

            // Deserializa con Newtonsoft.Json
            dynamic data = JsonConvert.DeserializeObject(json);

            // Verifica la "action" enviada desde el HTML
            if (data.action == "login")
            {
                // Obtén usuario y contraseña
                string user = data.username;
                string pass = data.password;

                // Llama al método que valida en la BD
                bool esValido = ValidarCredenciales(user, pass);

                if (esValido)
                {
                    // Respuesta exitosa al HTML
                    webView21.CoreWebView2.ExecuteScriptAsync("mostrarMensaje('¡Validación exitosa!')");
                }
                else
                {
                    // Respuesta de error al HTML
                    webView21.CoreWebView2.ExecuteScriptAsync("mostrarMensaje('Usuario o contraseña incorrectos')");
                }
            }
        }

        private bool ValidarCredenciales(string user, string pass)
        {
            string query = @"
          
                FROM Student
                WHERE name_user = @u
                  AND pass_user = @p
            ";

            // Aquí ya no tendrás error, porque _connectionString está declarado en la misma clase
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@u", user);
                    cmd.Parameters.AddWithValue("@p", pass);

                    int count = (int)cmd.ExecuteScalar();
                    return (count > 0);
                }
            }
        }
    


private void webView21_Click(object sender, EventArgs e)
        {

        }

    }

}