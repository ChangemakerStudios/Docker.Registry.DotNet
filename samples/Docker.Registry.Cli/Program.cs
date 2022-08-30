using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Authentication;
using Docker.Registry.DotNet.Models;
using Docker.Registry.DotNet.Registry;

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
            //string url = "https://registry-1.docker.io/";
            string url = "http://10.0.4.44:5000/";

            var configuration = new RegistryClientConfiguration(url);

            using (var client = configuration.CreateClient())
            {

                var bytes = File.ReadAllBytes(@"c:\layer.txt");

                string sha256Hash;

                using (var sha = SHA256.Create())
                {
                    var hash = sha.ComputeHash(bytes);

                    sha256Hash = $"sha256:{string.Join("", hash.Select(b => b.ToString("x")))}";
                }

                using (var source = File.OpenRead(@"c:\layer.txt"))
                {
                   // await client.BlobUploads.UploadBlobAsync("my-repo", (int)source.Length, source, sha256Hash);
                }

                ////Console.WriteLine("Ping...");

                ////var tags = await client.Tags.ListImageTagsAsync("resin", new ListImageTagsParameters());

                ////foreach (var tag in tags.Tags)
                ////{
                ////    Console.WriteLine();
                ////}

                //var catalog = await client.Catalog.GetCatalogAsync(new CatalogParameters()
                //{
                //    Number = 10
                //});

                //var repositories = catalog.Repositories;

                //foreach (var repository in repositories)
                //{
                //    Console.WriteLine("-----------------------------------------------");
                //    Console.WriteLine(repository);
                //    Console.WriteLine("-----------------------------------------------");

                //    var tags = await client.Tags.ListImageTagsAsync(repository, new ListImageTagsParameters());

                //    foreach (var tag in tags.Tags)
                //    {
                //        Console.WriteLine($"  {tag}");
                //    }
                //}

                //if (string.IsNullOrWhiteSpace(repository))
                //{
                //    Console.WriteLine("No repository found.");
                //}
                //else
                //{
                //    var tagsReponse =  await client.Tags.ListImageTagsAsync(repository, new ListImageTagsParameters());

                //    var tag = tagsReponse.Tags.FirstOrDefault();

                //    if (string.IsNullOrWhiteSpace(tag))
                //    {
                //        Console.WriteLine("No tags found.");
                //    }
                //    else
                //    {
                //        var manifestResult = await client.Manifest.GetManifestAsync(repository, tag);

                //        Console.WriteLine(manifestResult.Manifest.GetType().Name);

                //        var imageManifest = manifestResult.Manifest as ImageManifest2_1;

                //        if (imageManifest != null)
                //        {
                //            var layer = imageManifest.FsLayers.First();

                //            var getBlobResponse = await client.Blobs.GetBlobAsync(repository, layer.BlobSum);

                //            Console.WriteLine($"\t\tDigetst: {getBlobResponse.DockerContentDigest}");

                //            using (getBlobResponse.Stream)
                //            using (Stream targetStream = File.OpenWrite(@"c:\test.layer"))
                //            {
                //                await getBlobResponse.Stream.CopyToAsync(targetStream);
                //            }   
                //        }
                //    }

                //}

            }
        }
    }
}
