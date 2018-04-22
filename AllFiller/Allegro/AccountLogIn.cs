using AllFiller.pl.allegro.webapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AllFiller.Allegro
{
    public class AccountLogIn
    {
        public string login = "";
        public string pass = "";
        public int countryCode = 1;
        public string apiKey = "";
        

        public AccountLogIn() { }

        public long GetLocalVersionKey(AllegroWebApiService service)
        {
            long version = 0;
            try
            {
                var info = service.doQuerySysStatus(1, 1, apiKey, out version);
                //Shower.Items.Add("Kod wersji: " + version);

            }
            catch
            {
                MessageBoxResult wrongResult = MessageBox.Show("Zgubiłem klucz wersji. Zrestartuj mnie!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            return version;
        }

        public string LogIn(string sessionHandler, long version, AllegroWebApiService service)
        {
            long offset = 0;
            long serverTime = 0;
            try
            {
                sessionHandler = service.doLogin(login, pass, 1, apiKey, version, out offset, out serverTime);
            }
            catch
            {
                //Shower.Items.Add("Logowanie się nie powiodło");
                MessageBoxResult wrongResult = MessageBox.Show("Błąd w session Handlerze", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (wrongResult == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                }
            }
            return sessionHandler;
        }
    }
}
