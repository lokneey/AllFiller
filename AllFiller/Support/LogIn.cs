using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace InternetUser.Support
{
    class LogIn
    {
        public LogIn (string login, string password, WebBrowser Webby, string nameOfLoginInput, string nameOfPasswordInput)
        {
                     
            //Logowanie za pomocą WebBrowser
            mshtml.HTMLDocument worksheeet;
            //Thread.Sleep(5000);
            worksheeet = (mshtml.HTMLDocument)Webby.Document;
            //Thread.Sleep(5000);

            //Dodaj kod uruchamaiający dalszą część kodu dopuki strony sie nie załadują

            var webInput = worksheeet.getElementsByTagName("input");

            //Wpisywanie loginu
            foreach (mshtml.IHTMLElement element in webInput)
            {
                if (element.getAttribute("name") == nameOfLoginInput)
                {
                    element.setAttribute("value", login);
                    break;
                }
            }

            //Wpisywanie hasła
            foreach (mshtml.IHTMLElement element in webInput)
            {
                if (element.getAttribute("name") == nameOfPasswordInput)
                {
                    element.setAttribute("value", password);
                    break;
                }
            }
            //Naciśnięcie przycisku
            worksheeet.getElementById("login-button").click();
            //Chyba warto orzucić 1/2 sekundowe opóźnienia, bo rzy pierwszy debuggowaniu ma problemy
            
        }
    }
}
