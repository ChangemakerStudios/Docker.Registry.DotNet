using System;
using System.Threading.Tasks;
using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Authentication;
using Docker.Registry.DotNet.Models;

namespace Docker.Registry.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TestAsync().GetAwaiter().GetResult();
            }
            catch (UnauthorizedApiException ex)
            {
                Console.WriteLine();
                Console.WriteLine(nameof(UnauthorizedApiException));
                Console.WriteLine("=================================================================");

                foreach (var header in ex.Headers)
                {
                    Console.WriteLine($"{header.Key}: {string.Join(" ,", header.Value)}");
                }
            }
        }

        private static async Task TestAsync()
        {
            string url = "https://registry-1.docker.io/";
            //string url = "http://10.0.4.44:5000/";

            var configuration = new RegistryClientConfiguration(new Uri(url));

            using (var client = configuration.CreateClient(new AnonymousOAuthAuthenticationProvider()))
            {
                //Console.WriteLine("Ping...");

                //var tags = await client.Tags.ListImageTagsAsync("resin", new ListImageTagsParameters());

                //foreach (var tag in tags.Tags)
                //{
                //    Console.WriteLine();
                //}

                var catalog = await client.Catalog.GetCatalogAsync(new CatalogParameters()
                {
                    Number = 10
                });
            }
        }
    }
}
