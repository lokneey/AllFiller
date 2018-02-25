using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;
using System.Windows;

namespace InternetUser.Support
{
    class AuctionContentSaver
    {
        
        public AuctionContentSaver(string file, string title)
        {
           
            string path = AppDomain.CurrentDomain.BaseDirectory;    //Dopisz kod zamieniający istniejący plik na nowy i zrób funkcję zapisującą w oddzielnym pliku
            path = path.Replace(@"\", "/");
            path = path + "/Auctions/" +title +"/";
            DirectoryInfo directoryForAuctions = new DirectoryInfo(path);
            if (!directoryForAuctions.Exists)
            {
                directoryForAuctions.Create();
            }
            path = path + title + ".txt";

            if (!File.Exists(path))
            {
                StreamWriter htmlCodeForAllAuction = File.CreateText(path);
                htmlCodeForAllAuction.Write(file);
                htmlCodeForAllAuction.Close();
            }

        }

        public AuctionContentSaver(string file, string title, string obrobione)
        {
                        string path = AppDomain.CurrentDomain.BaseDirectory;    //Dopisz kod zamieniający istniejący plik na nowy i zrób funkcję zapisującą w oddzielnym pliku
            path = path.Replace(@"\", "/");
            path = path + "/Auctions/" + title + "/";
            DirectoryInfo directoryForAuctions = new DirectoryInfo(path);
            if (!directoryForAuctions.Exists)
            {
                directoryForAuctions.Create();
            }
            path = path + title + obrobione + ".txt";

            if (!File.Exists(path))
            {
                StreamWriter htmlCodeForAllAuction = File.CreateText(path);
                htmlCodeForAllAuction.Write(file);
                htmlCodeForAllAuction.Close();
            }

        }



        //Dobrze chyba było napisać też dekonstruktory zwalniające pamięć


    }
}
