using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Relay;


namespace EventHubsAppConsole
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
            //executes the 
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            var cts = new CancellationTokenSource();
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(_KEYNAME, _KEY);
            var listener = new HybridConnectionListener(new Uri(string.Format("sb://{0}/{1}", _RELAYNAMESPACE, _CONNECTIONNAME)), tokenProvider);

            //Subscribe to the status event.
            listener.Connecting += (o, e) => { Console.WriteLine("Connecting"); };
            listener.Offline += (o, e) => { Console.Write("Offline"); };
            listener.Online += (o, e) => { Console.Write("Online"); };

            //Provide an HTTP request handler.
            listener.RequestHandler = (context) => 
            {
                //Do something with context.Request.Url, HttpMethod, Headers, InputStream ...
                context.Response.StatusCode = HttpStatusCode.OK;
                context.Response.StatusDescription = "... OK, This is working fine ...";
                using (var sw = new StreamWriter(context.Response.OutputStream))
                {
                    sw.WriteLine("Hello World!");
                }

                context.Response.Close();
            };

            //Opening async listener establishes the control channel to the Azure Relay Service.
            //The control channel is continously mantained, and is reestablished when connectivity is disrupted.
            await listener.OpenAsync();
            Console.WriteLine("Server Listening");

            //Start a new thread that will continously read the console.
            await Console.In.ReadLineAsync();

            //Close the listener after you exit the processing loop.
            await listener.CloseAsync();

        }

    }
}
