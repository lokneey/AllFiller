using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InternetUser.Support
{
    class GumAuctionFiller
    {
        string content;
        mshtml.HTMLDocument gumWork;

        public GumAuctionFiller() { }

        public void FillIt (TextBox TBinfozestronyObrobione, WebBrowser WebbyGum)
        {
            content = TBinfozestronyObrobione.Text;
            WebbyGum.Navigate("https://www.gumtree.pl/post.html");
            gumWork = (mshtml.HTMLDocument)WebbyGum.Document;
            //gumWork.getElementById("9286").click();     //znajduje, ale nie może kliknąć. Przy wybieraniu z listy na liniszycia może być ten sam problem
            var elements = gumWork.getElementsByTagName("li");
            foreach (mshtml.IHTMLElement el in elements)
            {
                if (el.innerHTML == "<a href=\"javascript:void(0);\"><span class=\"icon\"><span class=\"out icon-cat-elect\"></span><span class=\"over icon-cat-elect-white\"></span></span>Elektronika</a><ul class=\"size-m\"><li><a href=\"javascript:void(0);\">audio i hi-fi</a></li><li><a href=\"javascript:void(0);\">cesje</a></li><li><a href=\"javascript:void(0);\">fotografia i video</a></li><li><a href=\"javascript:void(0);\">gry video i konsole</a></li><li><a href=\"javascript:void(0);\">komputery i software</a></li><li><a href=\"javascript:void(0);\">radiokomunikacja</a></li><li><a href=\"javascript:void(0);\">tablety i bookreadery</a></li><li><a href=\"javascript:void(0);\">telefony i akcesoria</a></li><li><a href=\"javascript:void(0);\">telewizory i odtwarzacze</a></li><li><a href=\"javascript:void(0);\">elektronika inne</a></li><li><a href=\"javascript:void(0);\">kupię sprzęt elektroniczny</a></li></ul>")
                {

                    el.click();

                }
                if (el.innerHTML == "<a href=\"javascript:void(0);\">elektronika inne</a>")
                {
                    
                    el.click();
                    
                }
            }
            foreach (mshtml.IHTMLElement el in elements)
            {
                if (el.innerHTML == "<a href=\"javascript:void(0);\">Wielkopolskie</a><ul class=\"size-xl\"><li><a href=\"javascript:void(0);\">Buk</a></li><li><a href=\"javascript:void(0);\">Chodzież</a></li><li><a href=\"javascript:void(0);\">Czarnków</a></li><li><a href=\"javascript:void(0);\">Gniezno</a></li><li><a href=\"javascript:void(0);\">Gostyń</a></li><li><a href=\"javascript:void(0);\">Grodzisk Wielkopolski</a></li><li><a href=\"javascript:void(0);\">Jarocin</a></li><li><a href=\"javascript:void(0);\">Jastrowie</a></li><li><a href=\"javascript:void(0);\">Kalisz</a></li><li><a href=\"javascript:void(0);\">Kępno</a></li><li><a href=\"javascript:void(0);\">Koło</a></li><li><a href=\"javascript:void(0);\">Konin</a></li><li><a href=\"javascript:void(0);\">Kostrzyn</a></li><li><a href=\"javascript:void(0);\">Kościan</a></li><li><a href=\"javascript:void(0);\">Kórnik</a></li><li><a href=\"javascript:void(0);\">Krotoszyn</a></li><li><a href=\"javascript:void(0);\">Leszno</a></li><li><a href=\"javascript:void(0);\">Luboń</a></li><li><a href=\"javascript:void(0);\">Międzychód</a></li><li><a href=\"javascript:void(0);\">Mosina</a></li><li><a href=\"javascript:void(0);\">Murowana Goślina</a></li><li><a href=\"javascript:void(0);\">Nowy Tomyśl</a></li><li><a href=\"javascript:void(0);\">Oborniki</a></li><li><a href=\"javascript:void(0);\">Opalenica</a></li><li><a href=\"javascript:void(0);\">Ostrów Wielkopolski</a></li><li><a href=\"javascript:void(0);\">Ostrzeszów</a></li><li><a href=\"javascript:void(0);\">Piła</a></li><li><a href=\"javascript:void(0);\">Pleszew</a></li><li><a href=\"javascript:void(0);\">Pniewy</a></li><li><a href=\"javascript:void(0);\">Pobiedziska</a></li><li><a href=\"javascript:void(0);\">Poznań</a></li><li><a href=\"javascript:void(0);\">Puszczykowo</a></li><li><a href=\"javascript:void(0);\">Rawicz</a></li><li><a href=\"javascript:void(0);\">Rogoźno</a></li><li><a href=\"javascript:void(0);\">Słupca</a></li><li><a href=\"javascript:void(0);\">Swarzędz</a></li><li><a href=\"javascript:void(0);\">Szamotuły</a></li><li><a href=\"javascript:void(0);\">Śrem</a></li><li><a href=\"javascript:void(0);\">Środa Wielkopolska</a></li><li><a href=\"javascript:void(0);\">Trzcianka</a></li><li><a href=\"javascript:void(0);\">Trzemeszno</a></li><li><a href=\"javascript:void(0);\">Turek</a></li><li><a href=\"javascript:void(0);\">Wągrowiec</a></li><li><a href=\"javascript:void(0);\">Witkowo</a></li><li><a href=\"javascript:void(0);\">Wolsztyn</a></li><li><a href=\"javascript:void(0);\">Wronki</a></li><li><a href=\"javascript:void(0);\">Września</a></li><li><a href=\"javascript:void(0);\">Złotów</a></li></ul>")
                {

                    el.click();

                }
                if (el.innerHTML == "<a href=\"javascript:void(0);\">Poznań</a>")
                {

                    el.click();

                }
            }
            var texts = gumWork.getElementsByTagName("input");

            foreach (mshtml.IHTMLElement tx in texts)
            {
                //if (tx.getAttribute("classname") == "error user-success")
                if (tx.className == "Title")            //Wszystkie tagName i className są null
                    //if (tx.getAttribute("name") == "title")
                {
                    tx.setAttribute("value", "kupa");
                }
            }

        }

    }
}
