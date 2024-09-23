using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using InterAPI.Utils;
using InterAPI.Model;

namespace TEDIE.Service
{
    public class SOAuth
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly BlobServiceClient _blobServiceClient;

        public static AccessToken GetToken(X509Certificate2 certSign)
        {
            string urlOAuth = "https://cdpj.partners.bancointer.com.br/oauth/v2/token";
            string clientId = "20e2637f-9063-462f-9c07-156794ea1432";
            string clientSecret = "dd1ea4b7-e4fd-4b74-a6c0-ab14285cfff2";
            string scope = "extrato.read boleto-cobranca.read boleto-cobranca.write pagamento-boleto.write pagamento-boleto.read pagamento-darf.write barrascob.write cob.read cob.write cobv.write cobv.read pix.write pix.read webhook.read webhook.write payloadlocation.write payloadlocation.read pagamento-pix.write pagamento-pix.read webhook-banking.write webhook-banking.read";
            string grantType = "client_credentials";

            try
            {
                try
                {
                    //Caminho onde o certificado será salvo localmente no container
                    //Console.WriteLine($"obtendo certificado");
                    string userProfileDirectory = @"C:\Users\Workstation W7\Downloads\Desenvolvimento\xmljson\xmljson\Certificado";
                    string localCertPath = Path.Combine(userProfileDirectory, "Certificate.pfx");

                    var certPassword = "0525";
                    X509Certificate2 cert = new X509Certificate2(localCertPath, certPassword);

                    ////////////////////////////////////////////////////////////////////////////////////////////
                    var myModel = new Dictionary<string, string> {
                        { "client_id",clientId },
                        { "client_secret", clientSecret },
                        { "scope", scope },
                        { "grant_type", grantType }
                    };

                    //System.IO.File.AppendAllText(@"c:\home\site\wwwroot\log.txt", $"certificado {cert}");

                    var content = new FormUrlEncodedContent(myModel);

                    var clientHandler = new HttpClientHandler
                    {
                        ClientCertificates = { cert },
                        ClientCertificateOptions = ClientCertificateOption.Manual
                        ,
                        ServerCertificateCustomValidationCallback = (httpRequestMessage, cert2, certChain, policyErrors) => true,
                        SslProtocols = (SslProtocols)(SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.SystemDefault)
                    };

                    using (var httpClient = new HttpClient(clientHandler))
                    {
                        httpClient.DefaultRequestHeaders.ExpectContinue = true;
                        //System.IO.File.AppendAllText(@"c:\home\site\wwwroot\log.txt", $"hnd --");
                        var response = httpClient.PostAsync(urlOAuth, content).Result;

                        response.EnsureSuccessStatusCode();

                        var jsonString = response.Content.ReadAsStringAsync().Result;
                        var token = JsonConvert.DeserializeObject<AccessToken>(jsonString);

                        token.Expire = DateTime.Now.AddSeconds(token.Expires_in);
                        //System.IO.File.AppendAllText(@"c:\home\site\wwwroot\log.txt", $"TK{token}");
                        return token;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro Certificado: {ex.Message}");
                    //System.IO.File.AppendAllText(@"c:\home\site\wwwroot\log.txt", $"API {ex.InnerException.ToString()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                //System.IO.File.AppendAllText(@"c:\home\site\wwwroot\log.txt", $"GERAL {ex.InnerException.ToString()}");
            }

            return null;
        }

        public SOAuth(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            var connectionString = _configuration["BlobStorageConnectionString"];
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        private string PathCert => _configuration["PathCert"];
        private string PassCert => _configuration["PassCert"];

        public async Task<AccessToken> GetTokenAsync()
        {
            string urlOAuth = "https://cdpj.partners.bancointer.com.br/oauth/v2/token";
            string clientId = _configuration["Pix:ClientId"];
            string clientSecret = _configuration["Pix:ClientSecret"];
            string scope = _configuration["Pix:Scope"];
            string grantType = "client_credentials";

            try
            {
                var myModel = new Dictionary<string, string>
                {
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "scope", scope },
                    { "grant_type", grantType }
                };

                var content = new FormUrlEncodedContent(myModel);

                var clientHandler = new HttpClientHandler
                {
                    ClientCertificates = { ConstApi.certificate2 },
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert2, certChain, policyErrors) => true,
                    SslProtocols = SslProtocols.Tls12
                };

                using (var httpClient = new HttpClient(clientHandler))
                {
                    var response = await httpClient.PostAsync(urlOAuth, content);
                    response.EnsureSuccessStatusCode();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var token = JsonConvert.DeserializeObject<AccessToken>(jsonString);

                    token.Expire = DateTime.Now.AddSeconds(token.Expires_in);
                    ConstApi.Token = token;
                    return token;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                return null;
            }
        }

        public static bool SetCertificate(byte[] certFile, string password)
        {
            var x = new X509Certificate2(certFile, password);
            if (x != null)
            {
                ConstApi.certificate2 = x;
                return true;
            }
            return false;
        }

        public async Task<string> DownloadToTextAsync(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var memoryStream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(memoryStream);
                return System.Text.Encoding.Default.GetString(memoryStream.ToArray());
            }
        }
    }
}
