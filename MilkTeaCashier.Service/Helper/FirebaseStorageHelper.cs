using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Helper
{
    public class FirebaseStorageHelper
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public FirebaseStorageHelper(string jsonPath, string bucketName)
        {
            _storageClient = StorageClient.Create(Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(jsonPath));
            _bucketName = bucketName;
        }

        public string UploadFile(string localFilePath, string firebaseStoragePath)
        {
            try
            {
                using var fileStream = File.OpenRead(localFilePath);
                var obj = _storageClient.UploadObject(_bucketName, firebaseStoragePath, null, fileStream);

                string publicUrl = $"https://storage.googleapis.com/{_bucketName}/{firebaseStoragePath}";
                return publicUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi upload file: {ex.Message}", ex);
            }
        }

    }
}
