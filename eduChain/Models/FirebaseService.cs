using Firebase.Auth;

namespace eduChain.Models
{
    public class FirebaseService
    {
        private static FirebaseService _instance;
        private static FirebaseAuthService _authService;

        private FirebaseService()
        {
            _authService = FirebaseAuthService.GetInstance(); // Assuming GetInstance() is a static method to get the singleton instance
        }

        public static FirebaseService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FirebaseService();
            }
            return _instance;
        }

        public FirebaseAuthClient GetFirebaseAuthClient()
        {
            return _authService.GetFirebaseAuthClient();
        }
    }
}
