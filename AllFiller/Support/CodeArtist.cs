using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AllFiller.Support
{
    class CodeArtist
    {
       
        int indexer=0;
        int[] tenTable = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
        public CodeArtist() {  }
        public string CodeArtists(string done, string what)
        {
            if (what == "node")
            {
                try
                {
                    indexer = done.IndexOf("Specyfikacja:");
                    done = done.Remove(indexer, done.Length - indexer);
                }
                catch
                { }
            }
            else if (what == "tite")
            {
                try
                {
                    indexer = done.IndexOf(" w magazynie");
                    done = done.Remove(indexer, done.Length - indexer);
                }
                catch { }
            }
            else if (what == "SKU")
            {
                try
                {
                    done = done.Replace("SKU: ", "");
                }
                catch { }
            }
            else if (what == "prince")
            {
                try
                {
                    indexer = done.IndexOf(" amount");
                    done = done.Remove(0, indexer);
                    indexer = done.IndexOf(",");
                    done = done.Remove(indexer, done.Length - indexer);
                    done = done.Replace("amount", "");
                    done = done.Replace(".", "");

                }
                catch { }
            }
            /*else if (what == "film")      //Może to dlatego, że zawsze znajduje specyfikację wcześniej i pewnie zdąży usunąć ten kod
            {
                indexer = done.IndexOf("ytp-title-subtext", 2);
                done = done.Remove(indexer, done.Length - indexer);
            }*/

            if (done.Contains("fusion-text"))
            {
                indexer = done.IndexOf("fusion-text");
                done = done.Remove(11, indexer);
            }

            if (done.Contains("Opis"))
            {
                indexer = done.IndexOf("Opis");
                done = done.Remove(0, indexer);
            }


            done = done.Replace("url", "");
            done = done.Replace("normal ", "");
            done = done.Replace("font-weight: ", "");
            done = done.Replace("left ", "");
            done = done.Replace("CENTER ", "");
            done = done.Replace("normal", "");
            done = done.Replace("font-weight:", "");
            done = done.Replace("left", "");
            done = done.Replace("CENTER", "");
            done = done.Replace("font-style:", "");
            done = done.Replace("class", "");
            done = done.Replace("\n", "");
            done = done.Replace("\t", "");
            done = done.Replace("<p>", "");
            done = done.Replace("<em>", ""); 
            done = done.Replace("<div ", "");
            done = done.Replace("</p>" , "");
            done = done.Replace(" <ul> ", "");
            done = done.Replace("<ul> ", "");
            done = done.Replace(" <ul>", "");
            done = done.Replace("<ul>", "");
            done = done.Replace(" <li> ", "");
            done = done.Replace("<li> ", "");
            done = done.Replace(" <li>", "");
            done = done.Replace("<li>", "");
            done = done.Replace(" <h3> ", "");
            done = done.Replace(" <h3>", "");
            done = done.Replace("<h3> ", "");
            done = done.Replace("<h3>", "");
            done = done.Replace("info_desc", ""); 
            done = done.Replace(" </ul> ", "");
            done = done.Replace("</ul> ", "");
            done = done.Replace(" </ul>", "");
            done = done.Replace("</ul>", "");
            done = done.Replace(" </li> ", "");
            done = done.Replace("</li> ", "");
            done = done.Replace(" </li>", "");
            done = done.Replace("</li>", "");
            done = done.Replace(" </h3> ", "");
            done = done.Replace(" </h3>", "");
            done = done.Replace("</h3> ", "");
            done = done.Replace("</h3>", "");
            done = done.Replace("<p>", "");
            done = done.Replace("<p class=", "");
            done = done.Replace("<p class= ", "");
            done = done.Replace("line-height: 1.5em; ", "");
            done = done.Replace("line-height: 1.5em;", "");
            done = done.Replace("line-height: 1.5em", "");
            done = done.Replace("line-height: ", "");
            done = done.Replace("line-height:", "");
            done = done.Replace("line-height", "");
            done = done.Replace("MsoNormal", "");
            done = done.Replace("background-color:", "");
            done = done.Replace("background-color: ", "");
            done = done.Replace("white", "");
            done = done.Replace("font-size: 8pt; ", "");
            done = done.Replace("font-size: 8pt;", "");
            done = done.Replace("font-size: 8pt", "");
            done = done.Replace("font-size: 10pt; ", "");
            done = done.Replace("font-size: 10pt;", "");
            done = done.Replace("font-size: 10pt", "");
            done = done.Replace(" 10pt", "");
            done = done.Replace("10pt", "");
            done = done.Replace(" 8pt", "");
            done = done.Replace("8pt", "");
            done = done.Replace("font-size: ", "");
            done = done.Replace("font-size:", "");
            done = done.Replace("</em>", "");
            done = done.Replace("text-align:", "");
            done = done.Replace("text-align: ", "");
            done = done.Replace("text-align", "");
            done = done.Replace("p style", "");
            done = done.Replace(" center ", "");
            done = done.Replace(" center", "");
            done = done.Replace("center ", "");
            done = done.Replace("center", "");
            done = done.Replace("margin-", "");
            done = done.Replace("bottom: ", "");
            done = done.Replace("0cm", "");
            done = done.Replace("align", "");
            done = done.Replace(" align", "");
            done = done.Replace("LEFT", "");
            done = done.Replace("<hr>", "");
            done = done.Replace(" #ffffff  ", "");
            done = done.Replace(" #ffffff ", "");
            done = done.Replace("#ffffff  ", "");
            done = done.Replace("#ffffff ", "");
            done = done.Replace("#ffffff", "");
            done = done.Replace("#ff0000", "");
            done = done.Replace("<strong>", "");
            done = done.Replace("<span style = ", "");
            done = done.Replace("<span style= ", "");
            done = done.Replace("<span style=", "");
            done = done.Replace("<span ", "");
            done = done.Replace("id=", "");
            done = done.Replace("our_price_display", "");
            done = done.Replace("<div id=", "");
            done = done.Replace("idTab1", "");
            done = done.Replace("<id", "");
            done = done.Replace("rte", "");
            done = done.Replace("Verdana", "");
            done = done.Replace(" class=", "");
            done = done.Replace("span style=", "");
            done = done.Replace("/strong", "");
            done = done.Replace("/span", "");
            done = done.Replace("/p", "");
            done = done.Replace("><p ", "");
            done = done.Replace("<p ", "");
            done = done.Replace("<br>", "\n");
            done = done.Replace("<h1>", "");
            done = done.Replace("</div>", "");
            done = done.Replace("</h1>", "");
            done = done.Replace("verdana,geneva", "");
            done = done.Replace("arial,helvetica", "");
            done = done.Replace(",sans-serif", ""); 
            done = done.Replace("color:", "");
            done = done.Replace("color: ", "");
            done = done.Replace("#000000", "");
            done = done.Replace("<p>", "");
            done = done.Replace("font-family: ", "");
            done = done.Replace("font-family:", "");
            done = done.Replace("=:", "");
            done = done.Replace(";", "");
            done = done.Replace(":  ", "");
            done = done.Replace(":   ", "");
            done = done.Replace("=", "");
            done = done.Replace("</p>", "\n");
            done = done.Replace("\"", "");
            done = done.Replace(" < ", "");
            done = done.Replace(" <", "");
            done = done.Replace("< ", "");
            done = done.Replace("<", "");
            done = done.Replace(" > ", "");
            done = done.Replace(" >", "");
            done = done.Replace("> ", "");
            done = done.Replace(">", "");
            done = done.Replace("  ", " ");
            done = done.Replace("  ", " ");


            return done;
        }
    }
}
