using System;
using Supabase.Realtime;

namespace eduChain.Models
{
    public class SupabaseAuthService
    {
        private static SupabaseAuthService _instance;
        private static Supabase.Client _supabaseClient;
        
        private SupabaseAuthService()
        {
            InitializeSupabaseClient();
        }

        private void InitializeSupabaseClient()
        {
            var url = "https://wcbvpqecetfhnfphtmae.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6IndjYnZwcWVjZXRmaG5mcGh0bWFlIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTIwNTUzNzMsImV4cCI6MjAyNzYzMTM3M30.c4_NP0Om5_Tbi4AswxxMAg22T9jWDFSg910fUn13cRo";

            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            _supabaseClient = new Supabase.Client(url, key, options);
        }

        public static SupabaseAuthService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SupabaseAuthService();
            }
            return _instance;
        }

        public Supabase.Client GetSupabaseClient()
        {
            return _supabaseClient;
        }
    }
}
