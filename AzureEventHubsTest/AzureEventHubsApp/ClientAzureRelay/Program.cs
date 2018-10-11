using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Relay;

namespace ClientAzureRelay
{
    class Program
    {
        #region "CONSTANTES DEFINICION"

        //Name of the relay namespace to be used
        private const string _RELAYNAMESPACE = "cgcfacturarelay.servicebus.windows.net";
        //Name of the hybrid connection to be used
        private const string _CONNECTIONNAME = "hybridcgcfactura";
        //Name of the Shared Access Policy Key to be used
        private const string _KEYNAME = "RootManageSharedAccessKey";
        //Primary Key Used for the Shared Access Policy Key
        private const string _KEY = "v6DO4CoWYyc3WTYJCnNa9q8+6+S5VQckF9uqpDbXQ38=";

        #endregion

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(_KEYNAME, _KEY);

            var uri = new Uri(String.Format("https://{0}/{1}", _RELAYNAMESPACE, _CONNECTIONNAME));

            var token = (await tokenProvider.GetTokenAsync(uri.AbsoluteUri, TimeSpan.FromHours(1))).TokenString;

            var client = new HttpClient();

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get,
            };
            request.Headers.Add("ServiceBusAuthorization", token);

            var response = await client.SendAsync(request);

            Console.WriteLine(await response.Content.ReadAsStringAsync());
            Console.ReadLine();
        }
    }
}
