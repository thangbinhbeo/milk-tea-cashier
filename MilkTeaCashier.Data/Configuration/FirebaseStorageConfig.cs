using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Data.Configuration
{
    public static class FirebaseStorageConfig
    {
        private static bool _isInitialized = false;

        public static void InitializeFirebase()
        {
            string jsonPath = Environment.GetEnvironmentVariable("FIREBASE_JSON_PATH");

            if (string.IsNullOrEmpty(jsonPath))
            {
                throw new Exception("Environment variable 'FIREBASE_JSON_PATH' is not set.");
            }

            if (!_isInitialized)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(jsonPath)
                });
                _isInitialized = true;
            }
        }
    }
}
