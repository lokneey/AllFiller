using System;
using System.Windows;
using AllFiller.pl.allegro.webapi;
using HtmlAgilityPack;
using System.Linq;
using AllFiller.Support;
using System.Collections.ObjectModel;

namespace AllFiller
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Zrób trzy tryby, full auto, semi auto (domyślnie), ręczny
        //Jeżeli brak ilosci w magazynie lub SKU(lepsze) to pobiera dane z jakiegoś dokumentu

        AllegroWebApiService service;   // globalny obiekt AllegroWebApiService
        string sessionHandler;
        long offset = 0;
        long serverTime = 0;

        //Logowanie
        string login = "maszyny2@op.pl";
        string pass = "1963Alla";
        int countryCode = 1;
        string apiKey = "a1a09772";
        long version;

        //Aukcja
        UInt32 numOfProd;
        string price;
        string nameOfProd;
        string description;

        //Pobieranie aukcji
        long[] auctionsID;
        MyAccountStruct2[] accountStructTable;

        //Pobieranie info ze strony
        string currentURL;
        string currentPrice;
        string currentOrginalName;
        string currentChangedName;
        string currentNumberOfProducts;
        string currentSKU;
        string currentDescription;
        int spaceSearcher = 49;
        public ObservableCollection<string> currrentModel = new ObservableCollection<string>();
        //string[] currentModels;

        //Wystawianie
        string verstr;
        long verkey;


        public MainWindow()
        {
            InitializeComponent();

            service = new AllegroWebApiService();// inicjalizacja obiektu service
            GetLocalVersionKey();
        }

        public long GetLocalVersionKey() //returns localVersion key of Api and stores it
        {
            var info = service.doQuerySysStatus(1, 1, apiKey, out version);//Country code PL=1 (second parameter)
            Shower.Items.Add("Kod wersji: "+version);
            return version;
        }

        
        private void LogBut_Click(object sender, RoutedEventArgs e)
        {
            long offset = 0;
            long serverTime = 0;
            try
            {
                sessionHandler = service.doLogin(login, pass, 1, apiKey, version, out offset, out serverTime);
            }
            catch
            {
                Shower.Items.Add("Logowanie się nie powiodło");
                MessageBoxResult wrongResult = MessageBox.Show("Błąd w session Handlerze", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (wrongResult == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                }
            }

            Shower.Items.Add(sessionHandler);
            Shower.Items.Add(offset.ToString());
            Shower.Items.Add(serverTime.ToString());

        }

        private void StartBut_Click(object sender, RoutedEventArgs e)
        {
            long offset = 0;
            
            try
            {
                accountStructTable = service.doMyAccount2(sessionHandler, "sell", (int)offset, auctionsID, 100);
            }
            catch
            {
                Shower.Items.Add("Nie można sprawdzić obecnych aukcji");
                MessageBoxResult wrongResult = MessageBox.Show("Błąd podczas pobierania aukcji", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (wrongResult == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                }
            }

            foreach (MyAccountStruct2 accountStruct in accountStructTable)
            {
                Shower.Items.Add(
                   accountStruct.myaccountarray[0] + " " +
                   accountStruct.myaccountarray[9] + " " +
                   accountStruct.myaccountarray[8]
                );
            }

        }

        private void StartAuction_Click(object sender, RoutedEventArgs e)
        {

            currentURL = URLTB.Text;
            if (currentURL=="")
            {
                goto escape;
            }

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(currentURL);
            CodeArtist changer = new CodeArtist();
            ModelsDownloader currentModels = new ModelsDownloader();
            /*
            PolishSigns sign = new PolishSigns();       //Allegro przyjmuje polskie znaki
            ModelsDownloader currentModels = new ModelsDownloader();
            */

            //Opis
            HtmlNode des = document.DocumentNode.SelectNodes("//div[@id='tab-description']").First();
            currentDescription = changer.CodeArtists(des.InnerHtml, "node");
            ProductDescriptionTextBlock.Text = currentDescription;

            //Nazwa
            HtmlNode firstName = document.DocumentNode.SelectNodes(".//*[contains(@class,'product_title entry-title')]").First();
            currentOrginalName = firstName.InnerHtml;
            NameOfProdTB.Text = currentOrginalName + " ";

            //Krótki opis
            HtmlNode secondName = document.DocumentNode.SelectNodes(".//*[contains(@class,'post-content woocommerce-product-details__short-description')]").First();
            currentChangedName = changer.CodeArtists(secondName.InnerHtml, "node");
            NameOfProdTB.Text += currentChangedName;

            if (NameOfProdTB.Text.Length > 50)      //Poprawić, żeby były również modele
            {
                spaceSearcher = 49;
                NameOfProdTB.Text = NameOfProdTB.Text.Remove(50, NameOfProdTB.Text.Length-50);
                while (NameOfProdTB.Text[spaceSearcher].ToString() != " ")
                {
                    spaceSearcher--;
                }
                NameOfProdTB.Text = NameOfProdTB.Text.Remove(spaceSearcher, NameOfProdTB.Text.Length - spaceSearcher);
            }
            

            //Cena - bez wariantów (podaje domyślną wtedy
            HtmlNode piripiri = document.DocumentNode.SelectNodes(".//*[contains(@class,'price')]").First();
            currentPrice = changer.CodeArtists(piripiri.InnerHtml, "prince");
            PriceTB.Text = currentPrice;

            //Ilość - bez wariantów
            HtmlNode countto10 = document.DocumentNode.SelectNodes(".//*[contains(@class,'avada-availability')]").First();
            currentNumberOfProducts = changer.CodeArtists(countto10.InnerText, "tite");
            AmountTB.Text = currentNumberOfProducts;

            //Zdjęcia
            HtmlNode[] photo = document.DocumentNode.SelectNodes(".//*[contains(@class,'woocommerce-product-gallery__image')]").ToArray();
            PhotoDown down = new PhotoDown(currentOrginalName, photo);
            
            //SKU - bez wariantów
            HtmlNode SKU = document.DocumentNode.SelectNodes(".//*[contains(@class,'sku_wrapper')]").First();
            currentSKU = changer.CodeArtists(SKU.InnerText, "SKU");
            SKUTB.Text = currentSKU;

            //Model - odczyt zawsze zaczynać od 1 nie 0
            try
            {
                HtmlNode[] Model = document.DocumentNode.SelectNodes("//option").ToArray();
                currrentModel = currentModels.ModelDownloader(Model, Shower);
                Shower.Items.Add(currrentModel);
            }
            catch
            { }

            escape:
            URLTB.Text = "";
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            //long offset = 0;
            //long serverTime = 0;
            
            try
            {
                CatInfoType[] categories = service.doGetCatsData(1, 0x0, apiKey, true, out verkey,  out verstr);

                for(int i =0;i<categories.Length;i++)
                {
                    Shower.Items.Add(categories[i].catname + ": " + categories[i].catid);
                }
            }
            catch
            {
                Shower.Items.Add("Wystawianie oferty nie powiodło");
                MessageBoxResult wrongResult = MessageBox.Show("Błąd przy wystawianiu oferty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (wrongResult == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
