using System;
using System.Windows;
using AllFiller.pl.allegro.webapi;
using HtmlAgilityPack;
using System.Linq;
using AllFiller.Support;
using System.Collections.ObjectModel;
using System.IO;

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
        FieldsValue[] formFiller = new FieldsValue[40000];
        ItemTemplateCreateStruct itemStruct;
        VariantStruct[] variants;
        TagNameStruct[] auctionTags;
        AfterSalesServiceConditionsStruct afterSale;
        string addicionalServicesGroup;
        string itemCost;
        int itemPromStatus;


        public MainWindow()
        {
            InitializeComponent();

            service = new AllegroWebApiService();// inicjalizacja obiektu service
            GetLocalVersionKey();
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

            
            formFiller[22164] = new FieldsValue();      //Waga z opakowaniem
            formFiller[22164].fid = 22164;
            if (formFiller[22164].fid == 22164)
            {
                formFiller[22164].fvaluefloat = 20;

            }
            
            formFiller[32611]= new FieldsValue();       //Stan
            formFiller[32611].fid = 32611;
            if(formFiller[32611].fid == 32611)
            {
                formFiller[32611].fvalueint = 1;

            }
          
            for (UInt32 i = 1; i < 350; i++)  
            {
                formFiller[i] = new FieldsValue();
                formFiller[i].fid = (int)i;
                Shower.Items.Add(formFiller[i].fid);
            }

                for (UInt32 i = 1; i < 350; i++)  //Sprawdź czy ten length ma sens
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
                        formFiller[i].fvalueint = 1;
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
                        }*/
                        break;
                    case 42:    //Przesyłka pobraniowa priorytetowa / Paczka24 pobranie (pierwsza sztuka)
                        break;
                    case 43:    //List polecony priorytetowy (pierwsza sztuka)
                        /*if (List.IsChecked == true)
                        {
                            formFiller[i].fvaluefloat = 7;
                        }*/
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
                            
                        }*/
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
                        }*/
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
                        }*/
                        break;
                    case 142:    //Przesyłka pobraniowa priorytetowa / Paczka24 pobranie (kolejna sztuka)
                        break;
                    case 143:    //List polecony priorytetowy (kolejna sztuka)
                        /*if (List.IsChecked == true)
                        {
                            formFiller[i].fvaluefloat = 7;
                        }*/
                        break;
                    case 144:    //Przesyłka kurierska (kolejna sztuka)
                                 /*if (Kurier.IsChecked == true)
                                   {
                                       formFiller[i].fvaluefloat = 21;
                                   }
                                   else if (Paleta.IsChecked == true)
                                   {
                                       formFiller[i].fvaluefloat = 160;
                                   }*/
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
                        }*/
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
                        }*/
                        break;
                    case 242:    //Przesyłka pobraniowa priorytetowa / Paczka24 pobranie (ilość w paczce)
                        break;
                    case 243:    //List polecony priorytetowy (ilość w paczce)
                        /*if (List.IsChecked == true)
                        {
                            formFiller[i].fvalueint = 1;
                        }*/
                        break;
                    case 244:    //Przesyłka kurierska (ilość w paczce)
                        /*if (Kurier.IsChecked == true)
                        {
                            formFiller[i].fvalueint = 1;
                        }
                        else if (Paleta.IsChecked == true)
                        {
                            formFiller[i].fvalueint = 1;
                        }*/
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
                        }*/
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


            }

            //Żeby zrobić afterSale to musisz mieć zrobiony programik do wyciągania kodów poprzez rest api
            //afterSale.warranty = "";
            //afterSale.returnpolicy = "";
            //afterSale.impliedwarranty = "";


            service.doNewAuctionExt(sessionHandler, formFiller, 1, 1, itemStruct, variants, auctionTags, afterSale, 
                addicionalServicesGroup, out itemCost, out itemPromStatus);
                
            for (int i =0;i<10;i++)
                {
                Shower.Items.Add("Koszt: "+itemCost);
                Shower.Items.Add("NR:"+ itemPromStatus);
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
        }
    }
}
