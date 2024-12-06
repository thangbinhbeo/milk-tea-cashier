using Firebase.Storage;
using MilkTeaCashier.Data.Configuration;
using MilkTeaCashier.Service.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Services
{
    public class ImageUploadService
    {
        private readonly string _bucketName = "student-management-c2fb4.appspot.com";

        public async Task<string> UploadImageAsync(string filePath, string fileName)
        {
            try
            {
                FirebaseStorageConfig.InitializeFirebase();

                var stream = File.OpenRead(filePath);

                var task = new FirebaseStorage(_bucketName)
                    .Child("images")
                    .Child(fileName)
                    .PutAsync(stream);

                await task;

                return await new FirebaseStorage(_bucketName)
                    .Child("images")
                    .Child(fileName)
                    .GetDownloadUrlAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Upload failed: " + ex.Message);
            }
        }

    }
}
