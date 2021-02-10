using System;
using System.Threading.Tasks;
//Post build references to use NuGet packages
/*
The following packages must be added before building the project 

dotnet add package Microsoft.Extensions.Configuration --version 5.0.0
dotnet add package Microsoft.Extensions.Configuration.FileExtensions --version 5.0.0
dotnet add package Microsoft.Extensions.Configuration.Json --version 5.0.0
dotnet add package Azure.Storage.Blobs --version 12.8.0
*/
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BlobConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Settings for protect the connection string used to manage storage account
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            //Set configuration for read access to appsettings.json where ConnectionString is storaged
            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();

            //Get the ConnectionString securely and prints in terminal
            string getConnectionString = config["ConnectionString"];
            string getStudentID = config["StudentID"];

           //Settings for new container creation
            BlobServiceClient myBlobServiceClient = new BlobServiceClient(getConnectionString);
            
            //Settings for new container name it takes StudentID from appsettings and joins with some other data
            string containerName = "asn-" + getStudentID + "-" + Guid.NewGuid().ToString();

            //Create the container and return an container client object
            BlobContainerClient myContainerClient = myBlobServiceClient.CreateBlobContainer(containerName);
            
            //Minimal validation for Blob container creation
            //DoubleCheck for Creation
            myContainerClient.CreateIfNotExists();
            
            //Set the container access Type to blobContainer
            myContainerClient.SetAccessPolicy(accessType: PublicAccessType.Blob);
            Console.WriteLine("New Blob Container Created at: " + myContainerClient.Uri + "for Account: " + myContainerClient.AccountName + "\n\n");
            Console.WriteLine("Wanna check the results? Go to your azure Account Service Dashboard");
            Console.WriteLine("Wannna Proced with File Upload to our Fresh New Blob Container: " + myContainerClient.Name + "? Press Y/N");
            char userResponse = Char.Parse(Console.ReadLine());

            //Conditional Statement for File Upload to a Blob Container
            if(userResponse=='y' || userResponse=='Y')
            {
                //Logic for Blob Upload
                string localFilePath = "Azure-Migrate.svg";
                BlobClient myBlobClient = myContainerClient.GetBlobClient(localFilePath);
                Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", myBlobClient.Uri);

                // Open the file and upload its data
                using FileStream uploadFileStream = File.OpenRead(localFilePath);
                myBlobClient.Upload(uploadFileStream, true);
                uploadFileStream.Close();

                //logic for Listing items into a container
                Console.WriteLine("Listing blobs container contents...\n\n");

                // List all blobs in the container
                foreach (BlobItem blobItem in myContainerClient.GetBlobs())
                {
                    Console.WriteLine("\t" + blobItem.Name);
                }
                Console.WriteLine("Wannna Proced to Delete to our Fresh New Blob Container: " + myContainerClient.Name + "? Press Y/N");
                userResponse = Char.Parse(Console.ReadLine());

                //Conditional Statement for File Upload to a Blob Container
                if(userResponse=='y' || userResponse=='Y')
                {

                    Console.Write("Press any key to begin clean up");
                    Console.ReadLine();
                    Console.WriteLine("Deleting blob container...");

                    //Sets deletion for this container
                    myContainerClient.Delete();
                    Console.WriteLine("Done");
                }
            }
            else
            {
                Console.WriteLine("fine, bye");
            }
        }
    }
}
