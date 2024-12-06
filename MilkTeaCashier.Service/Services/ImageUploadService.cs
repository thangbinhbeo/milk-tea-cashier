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
        private readonly FirebaseStorageHelper _firebaseHelper;

        public ImageUploadService()
        {
            string jsonPath = "firebase-adminsdk.json";
            string bucketName = "student-management-c2fb4.appspot.com";

            _firebaseHelper = new FirebaseStorageHelper(jsonPath, bucketName);
        }

        public string UploadImage(string localFilePath, string firebaseStoragePath)
        {
            return _firebaseHelper.UploadFile(localFilePath, firebaseStoragePath);
        }

    }
}
