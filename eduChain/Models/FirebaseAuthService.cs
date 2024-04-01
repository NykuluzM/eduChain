using Firebase.Auth.Providers;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduChain.Models
{

    public class FirebaseAuthService : IFirebaseAuthService
    {
        private static FirebaseAuthService _instance;

        private static FirebaseAuthClient _firebaseAuthClient;

        public FirebaseAuthService()
        {
            _firebaseAuthClient = new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = "AIzaSyBpW5HOgFZDZFjzDIHomFvPDNBdLhWNJdg",
                AuthDomain = "myacademe-70a3f.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                new EmailProvider()
                }
            });
        }

         public static FirebaseAuthService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FirebaseAuthService();
            }
            return _instance;
        }

        public FirebaseAuthClient GetFirebaseAuthClient()
        {
            return _firebaseAuthClient;
        }
    }

}
