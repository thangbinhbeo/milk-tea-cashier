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
            if (!_isInitialized)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("firebase-adminsdk.json")
                });
                _isInitialized = true;
            }
        }
    }
}
