using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InternetUser.Support
{
    class LogInReader
    {
        string path;
        string login;
        string password;

        public LogInReader(string nameOfLoginInput, string nameOfPasswordInput, WebBrowser Webby, TextBox Log, TextBox Pass)
        {
            path = AppDomain.CurrentDomain.BaseDirectory;    //Dopisz kod zamieniający istniejący plik na nowy i zrób funkcję zapisującą w oddzielnym pliku
            path = path.Replace(@"\", "/");
            path = path + "/Auctions";

            if (File.Exists(path + "/Password/Password.txt"))
            {
                StreamReader pass = new StreamReader(path + "/Password/Password.txt");
                password = pass.ReadLine();
                pass.Close();
            }
            else
            {
                password = Pass.Text;
                AuctionContentSaver pass = new AuctionContentSaver(password, "Password");
            }

            if (File.Exists(path + "/Login/Login.txt"))
            {
                StreamReader log = new StreamReader(path + "/Login/Login.txt");
                login = log.ReadLine();
                log.Close();
            }
            else
            {
                login = Log.Text;
                AuctionContentSaver log = new AuctionContentSaver(login, "Login");
            }

            LogIn start = new LogIn(login, password, Webby, nameOfLoginInput, nameOfPasswordInput);
        }

    }
}
