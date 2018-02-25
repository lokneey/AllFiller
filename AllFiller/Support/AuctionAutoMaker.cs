using HtmlAgilityPack;
using mshtml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace InternetUser.Support
{
    class AuctionAutoMaker
    {
        private
        string baseHtmlCode;
        string beforeBaseHtmlCode;
        string afterBaseHtmlCode;
        //Vector photos;
        string done;
        string mainTitleForInternetAuction;
        string mainTitle;
        string price;
        string numOfThings = "1";
        UInt32 numOfAuctions;
        string url;
        int indexer;
        //int modelIndexer =0;
        string filmCode;
        string modelMaker;
        public ObservableCollection<string> modelParts = new ObservableCollection<string>();
        public ObservableCollection<string> priceParts = new ObservableCollection<string>();
        mshtml.HTMLDocument worksheeet;



        public AuctionAutoMaker(TextBox TBinfozestrony, TextBox TBinfozestronyObrobione, ListView Roboczy, ListView Roboczy1, string url, WebBrowser PriceWeb)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(url);
            //CodeArtist changer = new CodeArtist();
            PolishSigns sign = new PolishSigns();
            ModelsDownloader currentModels = new ModelsDownloader();


            //Tytuł
            HtmlNode tite = document.DocumentNode.SelectNodes("//div[@id='primary_block']").First();
            //mainTitleForInternetAuction = changer.CodeArtists(tite.InnerHtml, "tite");
            mainTitle = sign.PolishSignsRemover(mainTitleForInternetAuction);






            /*
            //Wyszukiwanie filmów na aukcji
            HtmlNode film = document.DocumentNode.SelectNodes("//div[@id='idTab1']").First();   //Z jakiegoś powodu wywala, błąd, chyba nie znajduje zadanego teksu
            filmCode = changer.CodeArtists(film.OuterHtml, "film");
            */

            //Pobieranie kodu opisu aukcji
            HtmlNode node = document.DocumentNode.SelectNodes("//div[@id='idTab1']").First();

            baseHtmlCode = node.OuterHtml;
            TBinfozestrony.Text = baseHtmlCode;

            AuctionContentSaver contentSaver = new AuctionContentSaver(baseHtmlCode, mainTitle);

            //Obrabianie kodu aukcji

//            done = changer.CodeArtists(baseHtmlCode, "node");
            TBinfozestronyObrobione.Text = done;

            AuctionContentSaver contentSaverOb = new AuctionContentSaver(done, mainTitle, "Obrobione");

            //Pobieranie zdjęć

            HtmlNode[] photo = document.DocumentNode.SelectNodes("//div[@id='views_block']//a").ToArray();
            //PhotoDown down = new PhotoDown(mainTitle, photo);

            //Model

            HtmlNode[] Model = document.DocumentNode.SelectNodes("//option").ToArray();
            modelParts = currentModels.ModelDownloader(Model);
            Roboczy.ItemsSource = modelParts;


            //Cena
            // Daj ifa, że cena na wystawienie nie może być zero i takie linki zapisuj do odzielnego pliku
            //Dodatkowow to pobiera tylko i wyłącznie cenę domyślnego modelu


            //Selector(PriceWeb);
            //Roboczy1.ItemsSource = priceParts;


            /*
            HtmlNode prince = document.DocumentNode.SelectNodes("//span[@class='our_price_display']").First();
            price = prince.InnerHtml;
            price = changer.CodeArtists(prince.InnerHtml, "prince");
            */

            /*

            ChagePage(PriceWeb);
            Thread.Sleep(5000);
            Selector(PriceWeb);
            Roboczy1.ItemsSource = priceParts;
            */

        }

        public WebBrowser ChagePage(WebBrowser PriceWeb)
        {
            PriceWeb.Navigate(new Uri("http://www.liniaszycia.pl/pl/maszyny-do-pakowania/83-fkr.html"), string.Empty, null, string.Format("User-Agent: {0}", "Your user agent string here"));
            //PriceWeb.LoadCompleted += webb_LoadCompleted;
            return PriceWeb;
        }
        /*
        void webb_LoadCompleted(object sender, NavigationEventArgs e)
        {
            System.Windows.MessageBox.Show("Completed loading the page");

            mshtml.HTMLDocument doc = PriceWeb.Document as mshtml.HTMLDocument;         //Dlaczego do jasnej ciasnej ProceWeb nie znajduje?
            mshtml.HTMLInputElement obj = doc.getElementById("gs_taif0") as mshtml.HTMLInputElement;
            mshtml.HTMLFormElement form = doc.forms.item(Type.Missing, 0) as mshtml.HTMLFormElement;

            PriceWeb.LoadCompleted -= webb_LoadCompleted; //REMOVE THE OLD EVENT METHOD BINDING
            PriceWeb.LoadCompleted += webb_LoadCompleted2; //BIND TO A NEW METHOD FOR THE EVENT
            obj.value = "test search";
            form.submit(); //PERFORM THE POST ON THE FORM OR SEARCH
        }

        //SECOND EVENT TO FIRE AFTER YOU POST INFORMATION
        void webb_LoadCompleted2(object sender, NavigationEventArgs e)
        {
            System.Windows.MessageBox.Show("Completed loading the page second time after post");
        }
        */

        /*
        void LoadEnder(object sender,
                                 WebBrowserDocumentCompletedEventArgs e)
        {
            myDoc = (IHTMLDocument2)((WebBrowser)sender).Document.DomDocument;
            myDoc.designMode = "On";
            HtmlEditor.Refresh(WebBrowserRefreshOption.Completely);
            myContentsChanged = false;
        }
        */
        public ObservableCollection<string> Selector(WebBrowser PriceWeb)
        {
            
            var htmlDocument = PriceWeb.Document as IHTMLDocument2;
            if (htmlDocument != null)
            {
                var dropdown = ((IHTMLElement)htmlDocument.all.item("group_13"));
                var dropdownItems = (IHTMLElementCollection)dropdown.children;

                foreach (IHTMLElement option in dropdownItems)
                {
                    var value = option.getAttribute("value").ToString();
                    priceParts.Add(value);
                }

            }
            return priceParts;
        }

        


            /*
            public ObservableCollection<string> Selector(WebBrowser PriceWeb)
                {
                //PriceWeb.Navigate("http://www.liniaszycia.pl/pl/maszyny-do-pakowania/83-fkr.html");
                //Thread.Sleep(5000);
                worksheeet = (mshtml.HTMLDocument)PriceWeb.Document; //Zjakiegoś powodu ducument jest null dlatego worksheet też jest null i wywywala wyjątek

                for (int i = 0; i < modelParts.Count - 1; i++)
                {

                    worksheeet.getElementById("group_12").children[i].SetAttribute("title", modelParts[i]);
                    //worksheeet.getElementById("group_12").setAttribute("title", modelParts[i]);

                    var currentPrice = worksheeet.getElementById("our_price_display");

                    price = currentPrice.innerHTML;
                    priceParts.Add(price);
                }
                return priceParts;
            }
            */
        }
}
