using System;
using System.Windows;
using AllFiller.pl.allegro.webapi;
using HtmlAgilityPack;
using System.Linq;
using AllFiller.Support;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace AllFiller
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Zrób trzy tryby, full auto, semi auto (domyślnie), ręczny
        //Jeżeli brak ilosci w magazynie lub SKU(lepsze) to pobiera dane z jakiegoś dokumentu
        //Muszisz wybierać typ przesyłki: list polecony i priorytetowy razem, kurier za pobraniem i opłata z góry razem, paleta, opłata z góry i pobranie razem, dopisz jeszcze opcję inne gdzie możesz dopisać wpłasne opcje dla kuriera
        //Zrób zapisywanie do pliku wszystkich numerów aukcji i ich tytułów, które wystawisz
        //Dorób pokazywanie zaciągniętych obrazów i przypisywanie ich wtedy do globalnych zmiennych

        AllegroWebApiService service;   // globalny obiekt AllegroWebApiService
        string sessionHandler;
        long offset = 0;
        long serverTime = 0;
        UInt32 imageCounter;
        
        //string currentPhotoTitle;

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
        FieldsValue[] formFiller = new FieldsValue[410];
        ItemTemplateCreateStruct itemStruct;
        VariantStruct[] variants;
        TagNameStruct[] auctionTags;
        AfterSalesServiceConditionsStruct afterSale;
        string addicionalServicesGroup;
        string itemCost;
        int itemPromStatus;
        DateTime currentDate;
        DateTime currentTime;
        UInt32 imageSelector;
        bool isThereATable = false;
        string descriptionWorker;
        DateTime timeWorker;
        DateTime[] dateWorker = new DateTime[1000];


        public MainWindow()
        {
            InitializeComponent();

            this.Title = "AllFiller - Łukasz Granat";

            service = new AllegroWebApiService();// inicjalizacja obiektu service
            GetLocalVersionKey();

            currentDate = DateTime.Now;
            Kurier.IsChecked = true;

        }

        public long GetLocalVersionKey() //returns localVersion key of Api and stores it
        {
            try
            {
                var info = service.doQuerySysStatus(1, 1, apiKey, out version);//Country code PL=1 (second parameter)
                Shower.Items.Add("Kod wersji: " + version);
                
            }
            catch
            {
                MessageBoxResult wrongResult = MessageBox.Show("Zgubiłem klucz wersji. Zrestartuj mnie!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
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

            sessionHandlerTB.Text = sessionHandler;
            sessionHandlerTB.Visibility = Visibility.Visible;
            sessionIDLabel.Visibility = Visibility.Visible;
            Shower.Items.Add(sessionHandler);
            Shower.Items.Add(offset.ToString());
            Shower.Items.Add(serverTime.ToString());
            LogBut.Visibility = Visibility.Hidden;

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
            DescriptionTB.Text = currentDescription;

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
            imageCounter = 0;
            HtmlNode[] photo = document.DocumentNode.SelectNodes(".//*[contains(@class,'woocommerce-product-gallery__image')]").ToArray();
            PhotoDown down = new PhotoDown(currentOrginalName, photo);
            imageCounter = (UInt32)photo.Length;
            
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

        private void Continue_Click(object sender, RoutedEventArgs e)       //Zawartość tego clicka przenieś do jednej, lub kilku funkcji
        {                                                                   //Dopisz zabezpieczenie sprawdzające czy zostały już pobrane info ze strony



            /*
            if (Kurier.IsChecked == true) { }
            else if (Paleta.IsChecked == true) { }
            else if (List.IsChecked == true) { }
            else if (TylkoOsobisty.IsChecked == true) { }   //Nie widzę tej opcji
            else if (InnaDostawa.IsChecked == true) { }
            else
            {
                MessageBoxResult wrongResult = MessageBox.Show("Musisz zaznaczyć opcję wysyłki!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }*/

            if (OfferFrequencyCB.SelectedIndex>-1)
            { }
            else
            {
                MessageBoxResult wrongResult = MessageBox.Show("Musisz ustalić częstotliwość wystawiania!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
           // try
            //{
                CatInfoType[] categories = service.doGetCatsData(1, 0x0, apiKey, true, out verkey,  out verstr);
                //Automatyka przemysłowa: 121338
                //Zgrzewarki 252416
                //StreamWriter catSave = new StreamWriter("C:/Users/Lokney/Desktop/catSave.txt");
                SellFormFieldsForCategoryStruct auctionForm = service.doGetSellFormFieldsForCategory(apiKey, 1, 252416);
                //StreamWriter auctionFormSave = new StreamWriter("C:/Users/Lokney/Desktop/auctionFormSave.txt");
            
            formFiller[0] = new FieldsValue();
            formFiller[0].fid = 0;

            
            formFiller[400] = new FieldsValue();      //Waga z opakowaniem - dopasuj zależnie od wybranych opcji odstawy
            formFiller[400].fid = 22164;
            if (formFiller[400].fid == 22164)
            {
                formFiller[400].fvaluefloat = 20;

            }
            
            formFiller[401] = new FieldsValue();       //Stan
            formFiller[401].fid = 32611;
            if(formFiller[401].fid == 32611)
            {
                formFiller[401].fvalueint = 1;

            }
            /*
             for (UInt32 i = 1; i < 350; i++)  
              {
                  formFiller[i] = new FieldsValue();
                  formFiller[i].fid = (int)i;
                  Shower.Items.Add(formFiller[i].fid);
              }*/

            /* for (UInt32 i = 1; i < 350; i++)  
         {


             switch (formFiller[i].fid)
             {
                 case 1:     //Nazwa
                     formFiller[i].fvaluestring = "Zgrzewarka";

                     break;
                 case 2:     //Kategoria
                     formFiller[i].fvalueint = 252416;
                     break;
                 case 3:     //Data wystawienia - string Wartość pola dla dat(dd-mm-yyyy)
                     //formFiller[i].fvaluedate = 
                     break;
                 case 4:     //Okres trwania
                     formFiller[i].fvalueint = 99;
                     break;
                 case 5:     //Liczba sztuk
                     formFiller[i].fvalueint = 5;
                     break;
                 case 6:     //Cena wywoławacza
                     break;
                 case 7:     //Cema minimalna
                     break;
                 case 8:     //Cena kup teraz
                     formFiller[i].fvaluefloat = 999;
                     break;
                 case 9:     //Kraj 
                     formFiller[i].fvalueint = 1;
                     break;
                 case 10:    //Województwo
                     formFiller[i].fvalueint = 15;
                     break;
                 case 11:    //Miasto
                     formFiller[i].fvaluestring = "Poznań";
                     break;
                 case 12:    //Kto pokrywa koszty przesyłki
                     formFiller[i].fvalueint = 1;
                     break;
                 case 13:    //Formy dostawy
                     break;
                 case 14:    //Formy płatności
                     formFiller[i].fvalueint = 32;
                     break;
                 case 15:    //Opcej dodatkowe
                     break;
                 case 16:    //Zdjęcie 1 - fotmat base64Binary 
                     //form Filler[i].fvalueimage = 
                     break;
                 case 17:    //Zdjęcie 2
                     break;
                 case 18:    //Zdjęcie 3
                     break;
                 case 19:    //Zdjęcie 4
                     break;
                 case 20:    //Zdjęcie 5
                     break;
                 case 21:    //Zdjęcie 6
                     break;
                 case 22:    //Zdjęcie 7
                     break;
                 case 23:    //Zdjęcie 8
                     break;
                 case 24:    //Opis oferty
                     formFiller[i].fvaluestring = "Najlepsz opis oferty";
                     break;
                 case 25:    //Nieaktywne
                     break;
                 case 26:    //Nieaktywne
                     break;
                 case 27:    //Dodatkowe info o dostawie i płatnościach
                     formFiller[i].fvaluestring = "Numer konta bankowego: 97 1140 2004 0000 3102 7532 3271";
                     break;
                 case 28:    //Ilość sztuk
                     formFiller[i].fvalueint = 0;
                     break;
                 case 29:    //Kup teraz lub Licytacja||Sklep||Ogłoszenie
                     formFiller[i].fvalueint = 1;
                     break;
                 case 30:    //Automatyczne wznowienie
                     formFiller[i].fvalueint = 1;
                     break;
                 case 32:    //Kod pocztowy
                     formFiller[i].fvaluestring = "60-715";
                     break;
                 case 33:    //Pierwsze konto bankowe
                     formFiller[i].fvaluestring = "97 1140 2004 0000 3102 7532 3271";
                     break;
                 case 34:    //Drugie konto bankowe
                     break;
                 case 35:    //Darmowe opcje przesyłki
                     formFiller[i].fvalueint = 1;    //może zmienić na 5
                     break;
                 case 36:    //Paczka pocztowa ekonomiczna (Pierwsza sztuka)
                     break;
                 case 37:    //List ekonomiczny (Pierwsza sztuka)
                     break;
                 case 38:    //Paczka pocztowa priorytetowa (pierwsza sztuka)
                     break;
                 case 39:    //List priorytetowy (pierwsza sztuka)
                     break;
                 case 40:    //Przesyłka pobraniowa / Paczka48 pobranie (pierwsza sztuka)
                     break;
                 case 41:    //List polecony ekonomiczny (pierwsza sztuka)
                     /*if (List.IsChecked == true)
                     {
                         formFiller[i].fvaluefloat = (float)4.20;
                     }
                     break;
                 case 42:    //Przesyłka pobraniowa priorytetowa / Paczka24 pobranie (pierwsza sztuka)
                     break;
                 case 43:    //List polecony priorytetowy (pierwsza sztuka)
                     /*if (List.IsChecked == true)
                     {
                         formFiller[i].fvaluefloat = 7;
                     }
                     break;
                 case 44:    //Przesyłka kurierska (pierwsza sztuka)
                             /*if (Kurier.IsChecked == true)
                             {
                                 formFiller[i].fvaluefloat = 21;
                             }
                             else if (Paleta.IsChecked == true)
                             {
                                 formFiller[i].fvaluefloat = 160;
                             }
                             else if (InnaDostawa.IsChecked == true)
                             {

                             }

                     formFiller[i].fvaluefloat = 21;
                     break;
                 case 45:    //Przesyłka kurierska pobraniowa (pierwsza sztuka)
                     /*if (Kurier.IsChecked == true)
                     {
                         formFiller[i].fvaluefloat = 30;
                     }
                     else if (Paleta.IsChecked == true)
                     {
                         formFiller[i].fvaluefloat = 160;
                     }
                     break;
                 case 46:    //Odbiór w punkcie po przedpłacie - PACZKA W RUCHu (pierwsza sztuka)
                     break;
                 case 47:    //Odbiór w punkcie po przedpłacie - Paczkomaty 24/7 (pierwsza sztuka)
                     break;
                 case 48:    //Odbiór w punkcie - PACZKA W RUCHu (pierwsza sztuka)
                     break;
                 case 49:    //Odbiór w punkcie - Paczkomaty 24/7 (pierwsza sztuka)
                     break;
                 case 50:    //Allegro MiniPaczka InPost (pierwsza sztuka)
                     break;
                 case 51:    //Odbiór w punkcie po przedpłacie - E-PRZESYŁKA / Paczka48 Odbiór w Punkcie (pierwsza sztuka)
                     break;
                 case 52:    //Odbiór w punkcie - E-PRZESYŁKA / Paczka48 Odbiór w Punkcie (pierwsza sztuka)
                     break;
                 case 53:    //Pocztex Kurier48 (pierwsza sztuka)
                     break;
                 case 54:    //Pocztex Kurier48 pobraniowy (pierwsza sztuka)
                     break;
                 case 55:    //Paczka24 (pierwsza sztuka)
                     break;
                 case 56:    //Paczka48 (pierwsza sztuka)
                     break;
                 case 57:    //Odbiór w punkcie po przedpłacie - Paczka24 Odbiór w Punkcie (pierwsza sztuka)
                     break;
                 case 58:    //Odbiór w punkcie - Paczka24 Odbiór w Punkcie (pierwsza sztuka)
                     break;
                 case 59:    //Odbiór w punkcie po przedpłacie - Allegro Paczkomaty InPost (pierwsza sztuka)
                     break;
                 case 60:    //Odbiór w punkcie - Allegro Paczkomaty InPost (pierwsza sztuka)
                     break;
                 case 61:    //Allegro Kurier InPost (pierwsza sztuka)
                     break;
                 case 62:    //Allegro Kurier InPost (za pobraniem) (pierwsza sztuka)
                     break;
                 case 63:    //Niemcy (pierwsza sztuka)
                     break;
                 case 64:    //Wielka Brytania (pierwsza sztuka)
                     break;
                 case 65:    //Czechy (pierwsza sztuka)
                     break;
                 case 66:    //Irlandia (pierwsza sztuka)
                     break;
                 case 67:    //Rosja (pierwsza sztuka)
                     break;
                 case 68:    //Słowacja (pierwsza sztuka)
                     break;
                 case 69:    //Szwecja (pierwsza sztuka)
                     break;
                 case 70:    //Holandia (pierwsza sztuka)
                     break;
                 case 71:    //Litwa (pierwsza sztuka)
                     break;
                 case 72:    //Białoruś (pierwsza sztuka)
                     break;
                 case 73:    //Norwegia (pierwsza sztuka)
                     break;
                 case 74:    //Ukraina (pierwsza sztuka)
                     break;
                 case 75:    //Włochy (pierwsza sztuka)
                     break;
                 case 76:    //Francja (pierwsza sztuka
                     break;
                 case 77:    //Belgia (pierwsza sztuka)
                     break;
                 case 78:    //Dania (pierwsza sztuka)
                     break;
                 case 79:    //Hiszpania (pierwsza sztuka)
                     break;
                 case 80:    //Austria (pierwsza sztuka)
                     break;
                 case 81:    //Pocztex Kurier24 (pierwsza sztuka)
                     break;
                 case 82:    //Pocztex Kurier24 pobraniowy (pierwsza sztuka)
                     break;
                 case 98:    //Kurier wieczór (pierwsza sztuka)
                     break;
                 case 99:    //Kurier wieczór pobraniowy (pierwsza sztuka)
                     break;
                 case 136:    //Paczka pocztowa ekonomiczna (kolejna sztuka)
                     break;
                 case 137:    //List ekonomiczny (kolejna sztuka)
                     break;
                 case 138:    //Paczka pocztowa priorytetowa (kolejna sztuka)
                     break;
                 case 139:    //List priorytetowy (kolejna sztuka)
                     break;
                 case 140:    //Przesyłka pobraniowa / Paczka48 pobranie (kolejna sztuka)
                     break;
                 case 141:    //List polecony ekonomiczny (kolejna sztuka)
                     /*if (List.IsChecked == true)
                     {
                         formFiller[i].fvaluefloat = (float)4.20;
                     }
                     break;
                 case 142:    //Przesyłka pobraniowa priorytetowa / Paczka24 pobranie (kolejna sztuka)
                     break;
                 case 143:    //List polecony priorytetowy (kolejna sztuka)
                     /*if (List.IsChecked == true)
                     {
                         formFiller[i].fvaluefloat = 7;
                     }
                     break;
                 case 144:    //Przesyłka kurierska (kolejna sztuka)
                              /*if (Kurier.IsChecked == true)
                                {
                                    formFiller[i].fvaluefloat = 21;
                                }
                                else if (Paleta.IsChecked == true)
                                {
                                    formFiller[i].fvaluefloat = 160;
                                }

                     formFiller[i].fvaluefloat = 21;
                     break;
                 case 145:    //Przesyłka kurierska pobraniowa (kolejna sztuka)
                     /*if (Kurier.IsChecked == true)
                     {
                         formFiller[i].fvaluefloat = 30;
                     }
                     else if (Paleta.IsChecked == true)
                     {
                         formFiller[i].fvaluefloat = 160;
                     }
                     break;
                 case 146:    //Odbiór w punkcie po przedpłacie - PACZKA W RUCHu (kolejna sztuka)
                     break;
                 case 147:    //Odbiór w punkcie po przedpłacie - Paczkomaty 24/7 (kolejna sztuka)
                     break;
                 case 148:    //Odbiór w punkcie - PACZKA W RUCHu (kolejna sztuka)
                     break;
                 case 149:    //Odbiór w punkcie - Paczkomaty 24/7 (kolejna sztuka)
                     break;
                 case 150:    //Allegro MiniPaczka InPost (kolejna sztuka)
                     break;
                 case 151:    //Odbiór w punkcie po przedpłacie - E-PRZESYŁKA / Paczka48 Odbiór w Punkcie (kolejna sztuka)
                     break;
                 case 152:    //Odbiór w punkcie - E-PRZESYŁKA / Paczka48 Odbiór w Punkcie (kolejna sztuka)
                     break;
                 case 153:    //Pocztex Kurier48 (kolejna sztuka)
                     break;
                 case 154:    //Pocztex Kurier48 pobraniowy (kolejna sztuka)
                     break;
                 case 155:    //Paczka24 (kolejna sztuka)
                     break;
                 case 156:    //Paczka48 (kolejna sztuka)
                     break;
                 case 157:    //Odbiór w punkcie po przedpłacie - Paczka24 Odbiór w Punkcie (kolejna sztuka)
                     break;
                 case 158:    //Odbiór w punkcie - Paczka24 Odbiór w Punkcie (kolejna sztuka)
                     break;
                 case 159:    //Odbiór w punkcie po przedpłacie - Allegro Paczkomaty InPost (kolejna sztuka)
                     break;
                 case 160:    //Odbiór w punkcie - Allegro Paczkomaty InPost (kolejna sztuka)
                     break;
                 case 161:    //Allegro Kurier InPost (kolejna sztuka)
                     break;
                 case 162:    //Allegro Kurier InPost (za pobraniem) (kolejna sztuka)
                     break;
                 case 163:    //Niemcy (kolejna sztuka)
                     break;
                 case 164:    //Wielka Brytania (kolejna sztuka)
                     break;
                 case 165:    //Czechy (kolejna sztuka)
                     break;
                 case 166:    //Irlandia (kolejna sztuka
                     break;
                 case 167:    //Rosja (kolejna sztuka)
                     break;
                 case 168:    //Słowacja (kolejna sztuka)
                     break;
                 case 169:    //Szwecja (kolejna sztuka)
                     break;
                 case 170:    //Holandia (kolejna sztuka)
                     break;
                 case 171:    //Litwa (kolejna sztuka)
                     break;
                 case 172:    //Białoruś (kolejna sztuka)
                     break;
                 case 173:    //Norwegia (kolejna sztuka)
                     break;
                 case 174:    //Ukraina (kolejna sztuka)
                     break;
                 case 175:    //Włochy (kolejna sztuka)
                     break;
                 case 176:    //Francja (kolejna sztuka)
                     break;
                 case 177:    //Belgia (kolejna sztuka)
                     break;
                 case 178:    //Dania (kolejna sztuka)
                     break;
                 case 179:    //Hiszpania (kolejna sztuka)
                     break;
                 case 180:    //Austria (kolejna sztuka)
                     break;
                 case 181:    //Pocztex Kurier24 (kolejna sztuka)
                     break;
                 case 182:    //Pocztex Kurier24 pobraniowy (kolejna sztuka)
                     break;
                 case 198:    //Kurier wieczór (kolejna sztuka)
                     break;
                 case 199:    //Kurier wieczór pobraniowy (kolejna sztuka)
                     break;
                 case 236:    //Paczka pocztowa ekonomiczna (ilość w paczce)
                     break;
                 case 237:    //List ekonomiczny (ilość w paczce)
                     break;
                 case 238:    //Paczka pocztowa priorytetowa (ilość w paczce)
                     break;
                 case 239:    //List priorytetowy (ilość w paczce)
                     break;
                 case 240:    //Przesyłka pobraniowa / Paczka48 pobranie (ilość w paczce)
                     break;
                 case 241:    //List polecony ekonomiczny (ilość w paczce)
                     /*if (List.IsChecked == true)
                     {
                         formFiller[i].fvalueint = 1;
                     }
                     break;
                 case 242:    //Przesyłka pobraniowa priorytetowa / Paczka24 pobranie (ilość w paczce)
                     break;
                 case 243:    //List polecony priorytetowy (ilość w paczce)
                     /*if (List.IsChecked == true)
                     {
                         formFiller[i].fvalueint = 1;
                     }
                     break;
                 case 244:    //Przesyłka kurierska (ilość w paczce)
                     /*if (Kurier.IsChecked == true)
                     {
                         formFiller[i].fvalueint = 1;
                     }
                     else if (Paleta.IsChecked == true)
                     {
                         formFiller[i].fvalueint = 1;
                     }

                     formFiller[i].fvalueint = 1;
                     break;
                 case 245:    //Przesyłka kurierska pobraniowa (ilość w paczce)
                     /*if (Kurier.IsChecked == true)
                     {
                         formFiller[i].fvalueint = 1;
                     }
                     else if (Paleta.IsChecked == true)
                     {
                         formFiller[i].fvalueint = 1;
                     }
                     break;
                 case 246:    //Odbiór w punkcie po przedpłacie - PACZKA W RUCHu (ilość w paczce)
                     break;
                 case 247:    //Odbiór w punkcie po przedpłacie - Paczkomaty 24/7 (ilość w paczce)
                     break;
                 case 248:    //Odbiór w punkcie - PACZKA W RUCHu (ilość w paczce)
                     break;
                 case 249:    //Odbiór w punkcie - Paczkomaty 24/7 (ilość w paczce)
                     break;
                 case 250:    //Allegro MiniPaczka InPost (ilość w paczce)
                     break;
                 case 251:    //Odbiór w punkcie po przedpłacie - E-PRZESYŁKA / Paczka48 Odbiór w Punkcie (ilość w paczce)
                     break;
                 case 252:    //Odbiór w punkcie - E-PRZESYŁKA / Paczka48 Odbiór w Punkcie (ilość w paczce)
                     break;
                 case 253:    //Pocztex Kurier48 (ilość w paczce)
                     break;
                 case 254:    //Pocztex Kurier48 pobraniowy (ilość w paczce)
                     break;
                 case 255:    //Paczka24 (ilość w paczce)
                     break;
                 case 256:    //Paczka48 (ilość w paczce)
                     break;
                 case 257:    //Odbiór w punkcie po przedpłacie - Paczka24 Odbiór w Punkcie (ilość w paczce)
                     break;
                 case 258:    //Odbiór w punkcie - Paczka24 Odbiór w Punkcie (ilość w paczce)
                     break;
                 case 259:    //Odbiór w punkcie po przedpłacie - Allegro Paczkomaty InPost (ilość w paczce)
                     break;
                 case 260:    //Odbiór w punkcie - Allegro Paczkomaty InPost (ilość w paczce)
                     break;
                 case 261:    //Allegro Kurier InPost (ilość w paczce)
                     break;
                 case 262:    //Allegro Kurier InPost (za pobraniem) (ilość w paczce)
                     break;
                 case 263:    //Niemcy (ilość w paczce)
                     break;
                 case 264:    //Wielka Brytania (ilość w paczce)
                     break;
                 case 265:    //Czechy (ilość w paczce)
                     break;
                 case 266:    //Irlandia (ilość w paczce)
                     break;
                 case 267:    //Rosja (ilość w paczce)
                     break;
                 case 268:    //Słowacja (ilość w paczce)
                     break;
                 case 269:    //Szwecja (ilość w paczce)
                     break;
                 case 270:    //Holandia (ilość w paczce)
                     break;
                 case 271:    //Litwa (ilość w paczce)
                     break;
                 case 272:    //Białoruś (ilość w paczce)
                     break;
                 case 273:    //Norwegia (ilość w paczce)
                     break;
                 case 274:    //Ukraina (ilość w paczce)
                     break;
                 case 275:    //Włochy (ilość w paczce)
                     break;
                 case 276:    //Francja (ilość w paczce)
                     break;
                 case 277:    //Belgia (ilość w paczce)
                     break;
                 case 278:    //Dania (ilość w paczce)
                     break;
                 case 279:    //Hiszpania (ilość w paczce)
                     break;
                 case 280:    //Austria (ilość w paczce)
                     break;
                 case 281:    //Pocztex Kurier24 (ilość w paczce)
                     break;
                 case 282:    //Pocztex Kurier24 pobraniowy (ilość w paczce)
                     break;
                 case 298:    //Kurier wieczór (ilość w paczce)
                     break;
                 case 299:    //Kurier wieczór pobraniowy (ilość w paczce)
                     break;
                 case 337:    //Europejski Kod Towarowy
                     break;
                 case 340:    //Wysyłka w ciągu
                     formFiller[i].fvalueint = 1;
                     break;
                 case 341:    //Nowy opis oferty
                     //Zrób tutaj wywołanie funkcji, lub obiektu w którym tworzysz plik z opisem (na tym etapie masz już informację o ilosci zdjęć, oraz przetworzony tekst ze strony lini)
                     break;
                 case 342:    //Zdjęcie 9
                     break;
                 case 343:    //Zdjęcie 10
                     break;
                 case 344:    //Zdjęcie 11
                     break;
                 case 345:    //Zdjęcie 12
                     break;
                 case 346:    //Zdjęcie 13
                     break;
                 case 347:    //Zdjęcie 14
                     break;
                 case 348:    //Zdjęcie 15
                     break;
                 case 349:    //Zdjęcie 16
                     break;

             }


         }*/

            //Żeby zrobić afterSale to musisz mieć zrobiony programik do wyciągania kodów poprzez rest api albo użyć doGetItemsFields z wystawionej oferty
            afterSale = new AfterSalesServiceConditionsStruct();
            afterSale.warranty = "de4e9e97-f3fa-445e-ba87-a9d70f3e670e";
            afterSale.returnpolicy = "e711157a-609b-4845-a916-37eae22a94e5";
            afterSale.impliedwarranty = "8e2c1aca-5237-4b36-9853-1783a8d4bd97";

            //Określanie częstotliwości wystawiania
            formFiller[1] = new FieldsValue();       
            formFiller[1].fid = 1;
            formFiller[1].fvaluestring = NameOfProdTB.Text;
            formFiller[2] = new FieldsValue();       
            formFiller[2].fid = 2;
            formFiller[2].fvalueint = 252416;               //zrób jakiś inteligętny sposób na kategorie
            formFiller[4] = new FieldsValue();       
            formFiller[4].fid = 4;
            formFiller[4].fvalueint = 99;
            formFiller[5] = new FieldsValue();       
            formFiller[5].fid = 5;
            formFiller[5].fvalueint = Int32.Parse(AmountTB.Text);       //Zabezpieczenie gdyby było 0
            formFiller[8] = new FieldsValue();       
            formFiller[8].fid = 8;
            formFiller[8].fvaluefloat = float.Parse(PriceTB.Text);      
            formFiller[9] = new FieldsValue();       
            formFiller[9].fid = 9;
            formFiller[9].fvalueint = 1;
            formFiller[10] = new FieldsValue();       
            formFiller[10].fid = 10;
            formFiller[10].fvalueint = 15;
            formFiller[11] = new FieldsValue();       
            formFiller[11].fid = 11;
            formFiller[11].fvaluestring = "Poznań";
            formFiller[12] = new FieldsValue();       
            formFiller[12].fid = 12;
            formFiller[12].fvalueint = 1;
            formFiller[14] = new FieldsValue();       
            formFiller[14].fid = 14;
            formFiller[14].fvalueint = 32;


            string path = AppDomain.CurrentDomain.BaseDirectory;    //Dopisz kod zamieniający istniejący plik na nowy i zrób funkcję zapisującą w oddzielnym pliku
            path = path.Replace(@"\", "/");
            string normalPath = path;
            path = path + "/Auctions/" + currentOrginalName + "/Photos/"+ currentOrginalName;

            BitmapImage photoBitmapBeggining = new BitmapImage(new Uri(normalPath + "head.png"));     //W .Net robi się to inaczej           
            byte[] photoBeggining = getJPGFromImageControl(photoBitmapBeggining);
            formFiller[342] = new FieldsValue();
            formFiller[342].fid = 342;
            formFiller[342].fvalueimage = photoBeggining;
            
            if(imageCounter>7)
            {
                imageCounter = 7;
            }
            imageSelector = 0;
            for (UInt32 i = 16; i < 16 + imageCounter; i++)
            {
                BitmapImage photoBitmap = new BitmapImage(new Uri(path + imageSelector + ".jpg"));     //Zrób sprawdzanie dla różnych typów obrazów jpg png gif może przez exists
                byte[] photo = getJPGFromImageControl(photoBitmap);
                formFiller[i] = new FieldsValue();
                formFiller[i].fid = (int)i;
                formFiller[i].fvalueimage = photo;
                imageSelector++;
            }
            if (File.Exists(normalPath + "Table/" + SKUTB.Text + ".png"))
            {
                BitmapImage photoBitmapTable = new BitmapImage(new Uri(normalPath + "Table/" + SKUTB.Text + ".png"));    //Sprawdzaj, czy tabela dla danego kodu istnieje
                byte[] photoTable = getJPGFromImageControl(photoBitmapTable);
                formFiller[343] = new FieldsValue();
                formFiller[343].fid = 343;
                formFiller[343].fvalueimage = photoTable;
                isThereATable = true;
            }

            //formFiller[24] = new FieldsValue();       
            //formFiller[24].fid = 24;
            //formFiller[24].fvaluestring = "Najlepsz opis oferty";
            formFiller[27] = new FieldsValue();       
            formFiller[27].fid = 27;
            formFiller[27].fvaluestring = "Numer konta bankowego: 97 1140 2004 0000 3102 7532 3271";
            formFiller[28] = new FieldsValue();       
            formFiller[28].fid = 28;
            formFiller[28].fvalueint = 0;
            formFiller[29] = new FieldsValue();       
            formFiller[29].fid = 29;
            formFiller[29].fvalueint = 1;
            formFiller[30] = new FieldsValue();       
            formFiller[30].fid = 30;
            formFiller[30].fvalueint = 1;
            formFiller[32] = new FieldsValue();       
            formFiller[32].fid = 32;
            formFiller[32].fvaluestring = "60-715";
            formFiller[33] = new FieldsValue();       
            formFiller[33].fid = 33;
            formFiller[33].fvaluestring = "97 1140 2004 0000 3102 7532 3271";
            formFiller[34] = new FieldsValue();
            formFiller[34].fid = 34;
            formFiller[34].fvaluestring = "97 1140 2004 0000 3102 7532 3271";
            formFiller[35] = new FieldsValue();
            formFiller[35].fid = 35;
            formFiller[35].fvalueint = 1;

            if (TylkoOsobisty.IsChecked == true)
            {
                Paleta.IsChecked = false;
                Kurier.IsChecked = false;
                List.IsChecked = false;
                InnaDostawa.IsChecked = false;
            }

            if (Paleta.IsChecked == false)
            {
                if (Kurier.IsChecked == true)
                {
                    formFiller[44] = new FieldsValue();                   //Kurier opłacony z góry
                    formFiller[44].fid = 44;
                    formFiller[44].fvaluefloat = 21;
                    formFiller[144] = new FieldsValue();
                    formFiller[144].fid = 144;
                    formFiller[144].fvaluefloat = 21;
                    formFiller[244] = new FieldsValue();
                    formFiller[244].fid = 244;
                    formFiller[244].fvalueint = 1;

                    formFiller[45] = new FieldsValue();                   //Kurier za pobraniem
                    formFiller[45].fid = 45;
                    formFiller[45].fvaluefloat = 30;
                    formFiller[145] = new FieldsValue();
                    formFiller[145].fid = 145;
                    formFiller[145].fvaluefloat = 30;
                    formFiller[245] = new FieldsValue();
                    formFiller[245].fid = 245;
                    formFiller[245].fvalueint = 1;
                }
            }

            if (List.IsChecked == true)
            {
                formFiller[41] = new FieldsValue();                   //list polecony ekonomiczny
                formFiller[41].fid = 41;
                formFiller[41].fvaluefloat = (float)4.20;
                formFiller[141] = new FieldsValue();
                formFiller[141].fid = 141;
                formFiller[141].fvaluefloat = (float)4.20;
                formFiller[241] = new FieldsValue();
                formFiller[241].fid = 241;
                formFiller[241].fvalueint = 1;

                formFiller[43] = new FieldsValue();                   //list polecony priorytetowy
                formFiller[43].fid = 43;
                formFiller[43].fvaluefloat = 7;
                formFiller[143] = new FieldsValue();
                formFiller[143].fid = 143;
                formFiller[143].fvaluefloat = 7;
                formFiller[243] = new FieldsValue();
                formFiller[243].fid = 243;
                formFiller[243].fvalueint = 1;
            }

            if (Kurier.IsChecked == false)
            {
                if (Paleta.IsChecked == true)
                {
                    formFiller[44] = new FieldsValue();                   //Paleta opłacona z góry
                    formFiller[44].fid = 44;
                    formFiller[44].fvaluefloat = 160;
                    formFiller[144] = new FieldsValue();
                    formFiller[144].fid = 144;
                    formFiller[144].fvaluefloat = 160;
                    formFiller[244] = new FieldsValue();
                    formFiller[244].fid = 244;
                    formFiller[244].fvalueint = 1;

                    formFiller[45] = new FieldsValue();                   //Paleta za pobraniem
                    formFiller[45].fid = 45;
                    formFiller[45].fvaluefloat = 160;
                    formFiller[145] = new FieldsValue();
                    formFiller[145].fid = 145;
                    formFiller[145].fvaluefloat = 160;
                    formFiller[245] = new FieldsValue();
                    formFiller[245].fid = 245;
                    formFiller[245].fvalueint = 1;

                }
            }

            if (InnaDostawa.IsChecked == true)
            {
                formFiller[44] = new FieldsValue();                   //Kurier opłacony z góry
                formFiller[44].fid = 44;
                formFiller[44].fvaluefloat = float.Parse(CustomKurierZwykłyTB.Text);
                formFiller[144] = new FieldsValue();
                formFiller[144].fid = 144;
                formFiller[144].fvaluefloat = float.Parse(CustomKurierZwykłyTB.Text);
                formFiller[244] = new FieldsValue();
                formFiller[244].fid = 244;
                formFiller[244].fvalueint = 1;

                formFiller[45] = new FieldsValue();                   //Kurier za pobraniem
                formFiller[45].fid = 45;
                formFiller[45].fvaluefloat = float.Parse(CustomKurierPobranieTB.Text);
                formFiller[145] = new FieldsValue();
                formFiller[145].fid = 145;
                formFiller[145].fvaluefloat = float.Parse(CustomKurierPobranieTB.Text);
                formFiller[245] = new FieldsValue();
                formFiller[245].fid = 245;
                formFiller[245].fvalueint = 1;
            }

            formFiller[340] = new FieldsValue();       
            formFiller[340].fid = 340;
            formFiller[340].fvalueint = 1;
            formFiller[341] = new FieldsValue();
            formFiller[341].fid = 341;
            switch (imageCounter)       //Upewnij się że na pewno jest tyle możliwości zdjęć
            {
                case 0:     //Z jakiegoś powodu może nie przepuścić zbyt długiego opisu
                    if (isThereATable == true)
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis0phototable.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;                        
                        desc.Close();
                    }
                    else
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis0photo.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;
                        desc.Close();
                    }
                    break;
                case 1:
                    if (isThereATable == true)
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis1phototable.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;                        
                        desc.Close();
                    }
                    else
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis1photo.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;
                        desc.Close();
                    }
                    break;
                case 2:
                    if (isThereATable == true)
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis2phototable.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;                       
                        desc.Close();
                    }
                    else
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis2photo.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;
                        desc.Close();
                    }
                    break;
                case 3:
                    if (isThereATable == true)
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis3phototable.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;                        
                        desc.Close();
                    }
                    else
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis3photo.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;
                        desc.Close();
                    }
                    break;
                case 4:
                    if (isThereATable == true)
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis4phototable.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;                        
                        desc.Close();
                    }
                    else
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis4photo.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;
                        desc.Close();
                    }
                    break;
                case 5:
                    if (isThereATable == true)
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis5phototable.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;                        
                        desc.Close();
                    }
                    else
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis5photo.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;
                        desc.Close();
                    }
                    break;
                case 6:
                    if (isThereATable == true)
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis6phototable.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;                        
                        desc.Close();
                    }
                    else
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis6photo.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;
                        desc.Close();
                    }
                    break;
                case 7:
                    if (isThereATable == true)
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis7phototable.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;                        
                        desc.Close();
                    }
                    else
                    {
                        StreamReader desc = new StreamReader(normalPath + "Desc/opis7photo.json");
                        descriptionWorker = desc.ReadToEnd();
                        descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;
                        desc.Close();
                    }
                    break;
            }
            isThereATable = false;

           
            service.doNewAuctionExt(sessionHandler, formFiller, 1, 1, itemStruct, variants, auctionTags, afterSale, 
                addicionalServicesGroup, out itemCost, out itemPromStatus);

            Shower.Items.Add("Koszt: " + itemCost);
            Shower.Items.Add("NR:" + itemPromStatus);

            for (int i =0;i<10;i++)
                {
                
                /*
                try
                {
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformid);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformtitle);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformcat);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformtype);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformrestype);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformdefvalue);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformopt);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformpos);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformlength);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellminvalue);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellmaxvalue);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformdesc);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformoptsvalues);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformfielddesc);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformparamid);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformparamvalues);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformparentid);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformparamvalues);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformunit);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldslist[i].sellformoptions);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldsversionkey);
                    auctionFormSave.WriteLine(auctionForm.sellformfieldscomponentvalue);
                }
                catch { }
                */
                
            }

            /* 
          }
          catch
          {
              Shower.Items.Add("Wystawianie oferty nie powiodło");
              MessageBoxResult wrongResult = MessageBox.Show("Błąd przy wystawianiu oferty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
              if (wrongResult == MessageBoxResult.OK)
              {
                  Application.Current.Shutdown();
              }
          }*/
            for (UInt32 i = 0; i <= 5; i++)     //Przemyśl, czy to jest odpowiednie miejsce na wybór ilosci wystawień
            {
                timeWorker = DateTime.Now.AddMinutes(20);      // sprawdzaj, czy date worker is null

                switch (i)
                {
                    case 0:
                        if (OfferFrequencyCB.SelectedIndex == i)        //Jednorazowo
                        {

                        }
                        break;
                    case 1:
                        if (OfferFrequencyCB.SelectedIndex == i)        //Co dwa tygodnie
                        {
                            timeWorker = DateTime.Now.AddMinutes(20);
                        }
                        break;
                    case 2:
                        if (OfferFrequencyCB.SelectedIndex == i)        //Co tydzień
                        {
                            timeWorker = DateTime.Now.AddMinutes(20);
                        }
                        break;
                    case 3:
                        if (OfferFrequencyCB.SelectedIndex == i)        //Co 3 dni
                        {
                            timeWorker = DateTime.Now.AddMinutes(20);
                        }
                        break;
                    case 4:
                        if (OfferFrequencyCB.SelectedIndex == i)        //Codziennie
                        {
                            timeWorker = DateTime.Now.AddMinutes(20);
                        }
                        break;
                    case 5:
                        if (OfferFrequencyCB.SelectedIndex == i)        //Custom
                        {
                            CustomDates();
                            if (OfferCallendar.SelectedDate != null)
                            {
                            }
                        }
                        break;
                }
            }
        }

        public void CustomDates()
        {
            DatePick.Visibility = Visibility.Visible;
            OfferCallendar.Visibility = Visibility.Visible;
            DateConfirm.Visibility = Visibility.Visible;
        }

        private void DateConfirm_Click(object sender, RoutedEventArgs e) //Wybiera tylko daty po dniu obecnym a obecny dzień pomija - wtedy po prostu przy wystawianiu można dopisać wystawiania w dniu dzisiejszy i reselling tej samej aukcji, zmień jeszcze godziny na taką w jakiej wystawiasz
        {
            if (OfferFrequencyCB.SelectedIndex == 5)
            {
                try
                {

                    for (int i = 0; i < OfferCallendar.SelectedDates.Count; i++)
                    {
                        if (DateTime.Compare(currentDate.Date, OfferCallendar.SelectedDates[i].Date) <= 0)
                        {                            
                            if (OfferCallendar.SelectedDates[i] != null)
                            {
                                dateWorker[i] = new DateTime();
                                dateWorker[i] = OfferCallendar.SelectedDates[i].Date;
                                Shower.Items.Add(dateWorker[i].ToString());
                            }
                        }
                    }
                }
                catch
                {
                    
                }

                if (OfferCallendar.SelectedDate == null)
                {
                    MessageBoxResult wrongResult = MessageBox.Show("Musisz wybrać przynajmniej jedną datę!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (wrongResult == MessageBoxResult.OK)
                    {
                        return;
                    }
                }
            }
            DatePick.Visibility = Visibility.Hidden;
            OfferCallendar.Visibility = Visibility.Hidden;
            DateConfirm.Visibility = Visibility.Hidden;
        }

        
        public byte[] getJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }

        private void NameOfProdTB_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            signsNumberLabel.Content = 50 - NameOfProdTB.Text.Length;
        }

        private void PriceTB_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            OnlyNumberTexbox(PriceTB);

        }
        
        public void OnlyNumberTexbox(TextBox tex)
        {
            tex.Text = tex.Text.Replace("p", "");
            tex.Text = tex.Text.Replace("P", "");
            tex.Text = tex.Text.Replace("o", "");
            tex.Text = tex.Text.Replace("O", "");
            tex.Text = tex.Text.Replace("i", "");
            tex.Text = tex.Text.Replace("I", "");
            tex.Text = tex.Text.Replace("u", "");
            tex.Text = tex.Text.Replace("U", "");
            tex.Text = tex.Text.Replace("y", "");
            tex.Text = tex.Text.Replace("Y", "");
            tex.Text = tex.Text.Replace("t", "");
            tex.Text = tex.Text.Replace("T", "");
            tex.Text = tex.Text.Replace("r", "");
            tex.Text = tex.Text.Replace("R", "");
            tex.Text = tex.Text.Replace("e", "");
            tex.Text = tex.Text.Replace("E", "");
            tex.Text = tex.Text.Replace("w", "");
            tex.Text = tex.Text.Replace("W", "");
            tex.Text = tex.Text.Replace("q", "");
            tex.Text = tex.Text.Replace("Q", "");
            tex.Text = tex.Text.Replace("a", "");
            tex.Text = tex.Text.Replace("A", "");
            tex.Text = tex.Text.Replace("s", "");
            tex.Text = tex.Text.Replace("S", "");
            tex.Text = tex.Text.Replace("d", "");
            tex.Text = tex.Text.Replace("D", "");
            tex.Text = tex.Text.Replace("f", "");
            tex.Text = tex.Text.Replace("F", "");
            tex.Text = tex.Text.Replace("g", "");
            tex.Text = tex.Text.Replace("G", "");
            tex.Text = tex.Text.Replace("h", "");
            tex.Text = tex.Text.Replace("H", "");
            tex.Text = tex.Text.Replace("j", "");
            tex.Text = tex.Text.Replace("J", "");
            tex.Text = tex.Text.Replace("k", "");
            tex.Text = tex.Text.Replace("K", "");
            tex.Text = tex.Text.Replace("l", "");
            tex.Text = tex.Text.Replace("L", "");
            tex.Text = tex.Text.Replace("m", "");
            tex.Text = tex.Text.Replace("M", "");
            tex.Text = tex.Text.Replace("n", "");
            tex.Text = tex.Text.Replace("N", "");
            tex.Text = tex.Text.Replace("b", "");
            tex.Text = tex.Text.Replace("B", "");
            tex.Text = tex.Text.Replace("v", "");
            tex.Text = tex.Text.Replace("V", "");
            tex.Text = tex.Text.Replace("c", "");
            tex.Text = tex.Text.Replace("C", "");
            tex.Text = tex.Text.Replace("x", "");
            tex.Text = tex.Text.Replace("X", "");
            tex.Text = tex.Text.Replace("z", "");
            tex.Text = tex.Text.Replace("Z", "");
            tex.Text = tex.Text.Replace("!", "");
            tex.Text = tex.Text.Replace("@", "");
            tex.Text = tex.Text.Replace("#", "");
            tex.Text = tex.Text.Replace("$", "");
            tex.Text = tex.Text.Replace("%", "");
            tex.Text = tex.Text.Replace("^", "");
            tex.Text = tex.Text.Replace("&", "");
            tex.Text = tex.Text.Replace("*", "");
            tex.Text = tex.Text.Replace("(", "");
            tex.Text = tex.Text.Replace(")", "");
            tex.Text = tex.Text.Replace("-", "");
            tex.Text = tex.Text.Replace("_", "");
            tex.Text = tex.Text.Replace("=", "");
            tex.Text = tex.Text.Replace("+", "");
            tex.Text = tex.Text.Replace("[", "");
            tex.Text = tex.Text.Replace("{", "");
            tex.Text = tex.Text.Replace("]", "");
            tex.Text = tex.Text.Replace("}", "");
            tex.Text = tex.Text.Replace("\\", "");
            tex.Text = tex.Text.Replace("|", "");
            tex.Text = tex.Text.Replace(";", "");
            tex.Text = tex.Text.Replace(":", "");
            tex.Text = tex.Text.Replace("'", "");
            tex.Text = tex.Text.Replace("\"", "");
            tex.Text = tex.Text.Replace(",", "");
            tex.Text = tex.Text.Replace("<", "");
            tex.Text = tex.Text.Replace(".", "");
            tex.Text = tex.Text.Replace(">", "");
            tex.Text = tex.Text.Replace("/", "");
            tex.Text = tex.Text.Replace("?", "");
            tex.Text = tex.Text.Replace("`", "");
            tex.Text = tex.Text.Replace("~", "");
            tex.Text = tex.Text.Replace("ł", "");
            tex.Text = tex.Text.Replace("Ł", "");
            tex.Text = tex.Text.Replace("ó", "");
            tex.Text = tex.Text.Replace("ń", "");
            tex.Text = tex.Text.Replace("Ń", "");
            tex.Text = tex.Text.Replace("ś", "");
            tex.Text = tex.Text.Replace("Ś", "");
            tex.Text = tex.Text.Replace("Ó", "");
            tex.Text = tex.Text.Replace("ź", "");
            tex.Text = tex.Text.Replace("Ź", "");
            tex.Text = tex.Text.Replace("ę", "");
            tex.Text = tex.Text.Replace("Ę", "");
            tex.Text = tex.Text.Replace("ą", "");
            tex.Text = tex.Text.Replace("Ą", "");
            tex.Text = tex.Text.Replace("ć", "");
            tex.Text = tex.Text.Replace("Ć", "");
            tex.Text = tex.Text.Replace(" ", "");
            tex.Text = tex.Text.Replace("ż", "");
            tex.Text = tex.Text.Replace("Ż", "");

        }

        private void SKUTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnlyNumberTexbox(SKUTB);
        }

        private void CustomKurierZwykłyTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnlyNumberTexbox(CustomKurierZwykłyTB);
        }

        private void CustomKurierPobranieTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnlyNumberTexbox(CustomKurierPobranieTB);
        }        

        private void OfferFrequencyCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(OfferFrequencyCB.SelectedIndex == 5)
            {
                CustomDates();
            }
        }

        private void ClearAllBut_Click(object sender, RoutedEventArgs e)            //funkcja czyszcząca wszystkie tablice, texboxy itp.
        {                                                                           //Przede wszystkim czyszczenie formFillera

        }
    }
}
