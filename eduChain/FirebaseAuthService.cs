using Firebase.Auth.Providers;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduChain
{

    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly FirebaseAuthClient _firebaseAuthClient;

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

        public FirebaseAuthClient GetFirebaseAuthClient()
        {
            return _firebaseAuthClient;
        }
    }

}
