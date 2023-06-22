using Microsoft.Maui.Controls;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ponto.Pages
{
    public partial class Login : ContentPage
    {
        private const string apiUrl = "https://ida.ceub.br/api/v1.0/pp/Credencial";
        private const string username = "PP.CEUB.BR";
        private const string password = "JoBonizm9uId/uM1ED3Q7WKtr9umltxBUl+UepBHF2A=";

        public Login()
        {
            InitializeComponent();
        }

        private async void btnLogin_Clicked(object sender, EventArgs e)
        {
            string matricula = txtMatricula.Text;
            string senha = txtSenha.Text;

            // Autenticar o usuário
            bool isValid = await Authenticate(matricula, senha);

            if (isValid)
            {
                // Obter token de autenticação
                string token = await GetAuthToken(matricula, senha);

                if (!string.IsNullOrEmpty(token))
                {
                    // Extrair os dados do token JWT
                    var tokenData = ExtractTokenData(token);

                    if (tokenData != null)
                    {
                        // Usar os dados do token conforme necessário
                        string nome = tokenData.Nome;
                        string cargo = tokenData.Cargo;

                        // Redirecionar para a página Home com o token
                        Application.Current.MainPage = new Home(token);
                    }
                    else
                    {
                        await DisplayAlert("Erro", "Falha ao extrair dados do token", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Erro", "Falha ao obter token de autenticação", "OK");
                }
            }
            else
            {
                await DisplayAlert("Erro", "Credenciais inválidas", "OK");
            }
        }

        private async Task<bool> Authenticate(string matricula, string senha)
        {
            var httpClient = new HttpClient();

            // Configurar o cabeçalho de autenticação básica
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));

            var requestData = new
            {
                drt = matricula,
                senha
            };

            var dataJson = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            // Enviar a solicitação de autenticação
            var response = await httpClient.PostAsync(apiUrl, dataJson);

            return response.IsSuccessStatusCode;
        }

        private async Task<string> GetAuthToken(string matricula, string senha)
        {
            var httpClient = new HttpClient();

            // Configurar o cabeçalho de autenticação básica
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));

            var requestData = new
            {
                drt = matricula,
                senha
            };

            var dataJson = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            // Enviar a solicitação para obter o token de autenticação
            var response = await httpClient.PostAsync(apiUrl, dataJson);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent.Trim('"');
            }

            return null;
        }

        private TokenData ExtractTokenData(string token)
        {
            try
            {
                // Dividir o token em partes
                var tokenParts = token.Split('.');
                if (tokenParts.Length < 2)
                {
                    return null;
                }

                // Decodificar a carga útil do token JWT
                var base64Payload = tokenParts[1];
                var payloadBytes = Convert.FromBase64String(base64Payload);
                var payloadJson = Encoding.UTF8.GetString(payloadBytes);
                var tokenData = JsonSerializer.Deserialize<TokenData>(payloadJson);

                return tokenData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Falha ao decodificar o token JWT: " + ex.Message);
                return null;
            }
        }
    }

    public class TokenData
    {
        public string Nome { get; set; }
        public string Cargo { get; set; }
    }
}
