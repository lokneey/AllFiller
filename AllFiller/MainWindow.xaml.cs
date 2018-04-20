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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace AllFiller
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Zrób trzy tryby, full auto, semi auto (domyślnie), ręczny
        //Jeżeli brak ilosci w magazynie lub SKU(lepsze) to pobiera dane z jakiegoś dokumentu
        //Zrób zapisywanie do pliku wszystkich numerów aukcji i ich tytułów, które wystawisz
        //Dorób pokazywanie zaciągniętych obrazów i przypisywanie ich wtedy do globalnych zmiennych

        //Ogólne
        AllegroWebApiService service;   
        string sessionHandler;
        long offset = 0;
        long serverTime = 0;
        UInt32 imageCounter;               

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
        string itemCost = null;
        int itemPromStatus;
        DateTime currentDate;
        DateTime currentTime;
        UInt32 imageSelector;
        bool isThereATable = false;
        string descriptionWorker;
        DateTime timeWorker;
        DateTime[] dateWorker = new DateTime[1000];
        UInt32 numberOfDataWorkers = 0;
        List<int> categoriesList = new List<int>();
        bool toMuchWeight = false;
        bool toMuchWeightNoticed = false;
        

        public MainWindow()
        {
            InitializeComponent();

            this.Title = "AllFiller - Łukasz Granat";

            service = new AllegroWebApiService();
            GetLocalVersionKey();

            currentDate = DateTime.Now;
            Kurier.IsChecked = true;
            DatePick.Visibility = Visibility.Hidden;
            OfferCallendar.Visibility = Visibility.Hidden;
            DateConfirm.Visibility = Visibility.Hidden;

            DataGridGenerator();
            
            LoadingKiller();

        }

        public void DataGridGenerator()
        {
            DataGridTextColumn Column1 = new DataGridTextColumn();
            Column1.Header = "Nazwa aukcji";
            Column1.Binding = new Binding("Column1");
            DownloadedAuctionData.Columns.Add(Column1);
            DataGridTextColumn Column2 = new DataGridTextColumn();
            Column2.Header = "Ilość wejść";
            Column2.Binding = new Binding("Column2");
            DownloadedAuctionData.Columns.Add(Column2);           
            DataGridTextColumn Column10 = new DataGridTextColumn();
            Column10.Header = "Ilość Obserwujących";
            Column10.Binding = new Binding("Column10");
            DownloadedAuctionData.Columns.Add(Column10);
            DataGridTextColumn Column3 = new DataGridTextColumn();
            Column3.Header = "Cena";
            Column3.Binding = new Binding("Column3");
            DownloadedAuctionData.Columns.Add(Column3);
            DataGridTextColumn Column4 = new DataGridTextColumn();
            Column4.Header = "Liczba sprzedanych sztuk";
            Column4.Binding = new Binding("Column4");
            DownloadedAuctionData.Columns.Add(Column4);
            DataGridTextColumn Column5 = new DataGridTextColumn();
            Column5.Header = "Liczba pozostałych sztuk";
            Column5.Binding = new Binding("Column5");
            DownloadedAuctionData.Columns.Add(Column5);
            DataGridTextColumn Column6 = new DataGridTextColumn();
            Column6.Header = "Numer aukcji";
            Column6.Binding = new Binding("Column6");
            DownloadedAuctionData.Columns.Add(Column6);
            DataGridTextColumn Column7 = new DataGridTextColumn();
            Column7.Header = "Czas rozpoczęcia";
            Column7.Binding = new Binding("Column7");
            DownloadedAuctionData.Columns.Add(Column7);
            DataGridTextColumn Column8 = new DataGridTextColumn();
            Column8.Header = "Czas do zakończenia";
            Column8.Binding = new Binding("Column8");
            DownloadedAuctionData.Columns.Add(Column8);
            DataGridTextColumn Column9 = new DataGridTextColumn();
            /*Column9.Header = "Numer kategorii";
            Column9.Binding = new Binding("Column9");
            DownloadedAuctionData.Columns.Add(Column9);*/            
        }

        public long GetLocalVersionKey() 
        {
            try
            {
                var info = service.doQuerySysStatus(1, 1, apiKey, out version);
                Shower.Items.Add("Kod wersji: " + version);
                
            }
            catch
            {
                MessageBoxResult wrongResult = MessageBox.Show("Zgubiłem klucz wersji. Zrestartuj mnie!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            return version;
        }

        public async void LoadingShower()       //Trochę spowalnia działanie
        {
            Loader.Dispatcher.Invoke(delegate { Loader.Visibility = Visibility.Visible; });
            
        }

        public async void LoadingKiller()
        {
            Loader.Dispatcher.Invoke(delegate { Loader.Visibility = Visibility.Hidden; });
        }

        
        private async void LogBut_Click(object sender, RoutedEventArgs e)
        {
            LoadingShower();
            await Task.Run(() =>
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

                 sessionHandlerTB.Dispatcher.Invoke(delegate { sessionHandlerTB.Text = sessionHandler; });
                 sessionHandlerTB.Dispatcher.Invoke(delegate { sessionHandlerTB.Visibility = Visibility.Visible; });
                 sessionIDLabel.Dispatcher.Invoke(delegate { sessionIDLabel.Visibility = Visibility.Visible; });
                 Shower.Dispatcher.Invoke(delegate { Shower.Items.Add(sessionHandler); });
                 Shower.Dispatcher.Invoke(delegate { Shower.Items.Add(offset.ToString()); });
                 Shower.Dispatcher.Invoke(delegate { Shower.Items.Add(serverTime.ToString()); });

                 LogBut.Dispatcher.Invoke(delegate { LogBut.Visibility = Visibility.Hidden; });

             });
            CategoriesFiller();
            LoadingKiller();
        }

        public void CategoriesFiller()
        {
            CategoriesChoiceLB.Items.Add("Zgrzewarki");
            CategoriesChoiceLB.Items.Add("Tunele obkurczające");
            CategoriesChoiceLB.Items.Add("Zaklejarki kartonów");
            CategoriesChoiceLB.Items.Add("Zaszywarki");
            CategoriesChoiceLB.Items.Add("Nalewarki");
            CategoriesChoiceLB.Items.Add("Automaty");
            CategoriesChoiceLB.Items.Add("Datowniki");
            CategoriesChoiceLB.Items.Add("Solarne ogrzewacze");
            CategoriesChoiceLB.Items.Add("Maszyny do szycia/Napownice");
            CategoriesChoiceLB.Items.Add("Blister/SkinPack");
            CategoriesChoiceLB.Items.Add("Próżniowe");
            CategoriesChoiceLB.Items.Add("Silniki elektryczne i akcesoria");
            CategoriesChoiceLB.Items.Add("Inne");

            OtherCategories();
        }

        public void OtherCategories()   //Funkcja do wypełniania kategorii
        {
            
            //CatInfoType[] categories = service.doGetCatsData(1, 0x0, apiKey, false, out cat1, out cat2);


        }


        private async void StartBut_Click(object sender, RoutedEventArgs e)
        {
            LoadingShower();
            await Task.Run(() =>            //Trzeba wszystko clearować przed ponownym pobraniem
            {
                long offset = 0;
                DownloadedAuctionData.Dispatcher.Invoke(delegate
                {
                    DownloadedAuctionData.Items.Clear();
                });
                try
                {
                    accountStructTable = service.doMyAccount2(sessionHandler, "sell", (int)offset, auctionsID, 100);
                }
                catch
                {
                    Shower.Dispatcher.Invoke(delegate { Shower.Items.Add("Nie można sprawdzić obecnych aukcji"); });
                    MessageBoxResult wrongResult = MessageBox.Show("Błąd podczas pobierania aukcji", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (wrongResult == MessageBoxResult.OK)
                    {
                        return;
                    }
                }

                foreach (MyAccountStruct2 accountStruct in accountStructTable)
                {
                    DownloadedAuctionData.Dispatcher.Invoke(delegate
                    {
                        DownloadedAuctionData.Items.Add(new DataGridItem() { Column1 = accountStruct.myaccountarray[9],
                            Column2 = accountStruct.myaccountarray[33],
                            Column10 = accountStruct.myaccountarray[27],
                            Column3 = accountStruct.myaccountarray[4] + " zł",
                            Column4 = accountStruct.myaccountarray[17],
                            Column5 = accountStruct.myaccountarray[16],
                            Column6 = accountStruct.myaccountarray[0],
                            Column7 = accountStruct.myaccountarray[6],
                            Column8 = accountStruct.myaccountarray[7],                                          
                        } );
                    });
                }
            });
            LoadingKiller(); 
        }

        public class DataGridItem
        {
            public string Column1 { get; set; }
            public string Column2 { get; set; }
            public string Column3 { get; set; }
            public string Column4 { get; set; }
            public string Column5 { get; set; }
            public string Column6 { get; set; }
            public string Column7 { get; set; }
            public string Column8 { get; set; }
            public string Column9 { get; set; }
            public string Column10 { get; set; }

        }

        private async void StartAuction_Click(object sender, RoutedEventArgs e)
        {
            LoadingShower();

            await Task.Run(() =>
            { 

            URLTB.Dispatcher.Invoke(delegate {currentURL = URLTB.Text;});
            if (currentURL=="")
            {
                goto escape;
            }
                try
                {
                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument document = web.Load(currentURL);
                    CodeArtist changer = new CodeArtist();
                    ModelsDownloader currentModels = new ModelsDownloader();                    

                    //Opis
                    HtmlNode des = document.DocumentNode.SelectNodes("//div[@id='tab-description']").First();
                    currentDescription = changer.CodeArtists(des.InnerHtml, "node");
                    DescriptionTB.Dispatcher.Invoke(delegate { DescriptionTB.Text = currentDescription; });

                    //Nazwa
                    HtmlNode firstName = document.DocumentNode.SelectNodes(".//*[contains(@class,'product_title entry-title')]").First();
                    currentOrginalName = firstName.InnerHtml;
                    NameOfProdTB.Dispatcher.Invoke(delegate { NameOfProdTB.Text = currentOrginalName + " "; });

                    //Krótki opis
                    HtmlNode secondName = document.DocumentNode.SelectNodes(".//*[contains(@class,'post-content woocommerce-product-details__short-description')]").First();
                    currentChangedName = changer.CodeArtists(secondName.InnerHtml, "node");
                    NameOfProdTB.Dispatcher.Invoke(delegate { NameOfProdTB.Text += currentChangedName; });

                    NameOfProdTB.Dispatcher.Invoke(delegate
                    {
                        if (NameOfProdTB.Text.Length > 50)      //Poprawić, żeby były również modele
                        {
                            spaceSearcher = 49;
                            NameOfProdTB.Text = NameOfProdTB.Text.Remove(50, NameOfProdTB.Text.Length - 50);
                            while (NameOfProdTB.Text[spaceSearcher].ToString() != " ")
                            {
                                spaceSearcher--;
                            }
                            NameOfProdTB.Text = NameOfProdTB.Text.Remove(spaceSearcher, NameOfProdTB.Text.Length - spaceSearcher);
                        }
                    });

                    //Cena - bez wariantów 
                    HtmlNode piripiri = document.DocumentNode.SelectNodes(".//*[contains(@class,'price')]").First();
                    currentPrice = changer.CodeArtists(piripiri.InnerHtml, "prince");
                    PriceTB.Dispatcher.Invoke(delegate { PriceTB.Text = currentPrice; });

                    //Ilość - bez wariantów
                    HtmlNode countto10 = document.DocumentNode.SelectNodes(".//*[contains(@class,'avada-availability')]").First();
                    currentNumberOfProducts = changer.CodeArtists(countto10.InnerText, "tite");
                    AmountTB.Dispatcher.Invoke(delegate { AmountTB.Text = currentNumberOfProducts; });

                    //Zdjęcia
                    imageCounter = 0;
                    HtmlNode[] photo = document.DocumentNode.SelectNodes(".//*[contains(@class,'woocommerce-product-gallery__image')]").ToArray();
                    PhotoDown down = new PhotoDown(currentOrginalName, photo);
                    imageCounter = (UInt32)photo.Length;

                    //SKU - bez wariantów
                    HtmlNode SKU = document.DocumentNode.SelectNodes(".//*[contains(@class,'sku_wrapper')]").First();
                    currentSKU = changer.CodeArtists(SKU.InnerText, "SKU");
                    SKUTB.Dispatcher.Invoke(delegate { SKUTB.Text = currentSKU; });

                    //Model - odczyt zawsze zaczynać od 1 nie 0
                    try
                    {
                        HtmlNode[] Model = document.DocumentNode.SelectNodes("//option").ToArray();
                        currrentModel = currentModels.ModelDownloader(Model, Shower);
                        SKUTB.Dispatcher.Invoke(delegate { Shower.Items.Add(currrentModel); });
                    }
                    catch
                    { }
                }
                catch
                { }

            escape:
            URLTB.Dispatcher.Invoke(delegate {URLTB.Text = "";});

            });
            LoadingKiller();
        }

        private async void Continue_Click(object sender, RoutedEventArgs e)         
        {                                                                           
            LoadingShower();
            await Task.Run(() =>
            {

                Action deliveryChecker = delegate ()
                {
                    if (Kurier.IsChecked == true) { }
                    else if (Paleta.IsChecked == true) { }
                    else if (List.IsChecked == true) { }
                    else if (TylkoOsobisty.IsChecked == true) { }   
                else if (InnaDostawa.IsChecked == true) { }
                    else
                    {
                        MessageBoxResult wrongResult = MessageBox.Show("Musisz zaznaczyć opcję wysyłki!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }


                    if (OfferFrequencyCB.SelectedIndex > -1)
                    { }
                    else
                    {
                        MessageBoxResult wrongResult = MessageBox.Show("Musisz ustalić częstotliwość wystawiania!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                };
                Dispatcher.Invoke(DispatcherPriority.Normal, deliveryChecker);
                Action auctionChecker = delegate ()
                {
                    try
                    {
                        SellFormFieldsForCategoryStruct auctionForm = service.doGetSellFormFieldsForCategory(apiKey, 1, 252416);
                        timeWorker = DateTime.Now.AddMinutes(20);      
                        int i = 0;
                        int x = 0;
                        if (categoriesList.Count != 0)
                        {
                            caty:
                            if (toMuchWeight == false)
                            {
                                if (x < categoriesList.Count)
                                {
                                    int category = categoriesList[x];

                                    if (timeWorker == null)
                                    {
                                        timeWorker = DateTime.Now.AddMinutes(20);      
                                    }
                                    next:
                                    switch (i)
                                    {
                                        case 0:
                                            if (OfferFrequencyCB.SelectedIndex == i)        //Jednorazowo
                                            {
                                                timeWorker = DateTime.Now.AddMinutes(12);
                                                if (categoriesList.Count > 1)
                                                {
                                                    MakeAuction2(timeWorker, category);
                                                }
                                                else
                                                {
                                                    MakeAuction(category);
                                                }
                                                x++;
                                                goto caty;

                                            }
                                            else
                                            {
                                                i++;
                                                goto next;
                                            }

                                            break;
                                        case 1:
                                            if (OfferFrequencyCB.SelectedIndex == i)        //Co dwa tygodnie
                                            {
                                                timeWorker = DateTime.Now.AddMinutes(12);
                                                if (categoriesList.Count > 1)
                                                {
                                                    MakeAuction2(timeWorker, category);
                                                }
                                                else
                                                {
                                                    MakeAuction(category);
                                                }

                                                timeWorker = DateTime.Now.AddMinutes(20);
                                                timeWorker = timeWorker.AddDays(14);
                                                MakeAuction2(timeWorker, category);
                                                x++;
                                                goto caty;
                                            }
                                            else
                                            {
                                                i++;
                                                goto next;
                                            }

                                            break;
                                        case 2:
                                            if (OfferFrequencyCB.SelectedIndex == i)        //Co tydzień
                                            {
                                                timeWorker = DateTime.Now.AddMinutes(12);
                                                if (categoriesList.Count > 1)
                                                {
                                                    MakeAuction2(timeWorker, category);
                                                }
                                                else
                                                {
                                                    MakeAuction(category);
                                                }

                                                timeWorker = DateTime.Now.AddMinutes(20);
                                                timeWorker = timeWorker.AddDays(7);
                                                MakeAuction2(timeWorker, category);     
                                                timeWorker = timeWorker.AddDays(7);
                                                MakeAuction2(timeWorker, category);
                                                timeWorker = timeWorker.AddDays(7);
                                                MakeAuction2(timeWorker, category);
                                                x++;
                                                goto caty;
                                            }
                                            else
                                            {
                                                i++;
                                                goto next;
                                            }

                                            break;
                                        case 3:
                                            if (OfferFrequencyCB.SelectedIndex == i)        //Co 3 dni
                                            {
                                                timeWorker = DateTime.Now.AddMinutes(12);
                                                if (categoriesList.Count > 1)
                                                {
                                                    MakeAuction2(timeWorker, category);
                                                }
                                                else
                                                {
                                                    MakeAuction(category);
                                                }

                                                timeWorker = DateTime.Now.AddMinutes(20);
                                                for (UInt32 z = 0; z < 10; z++)
                                                {
                                                    timeWorker = timeWorker.AddDays(3);
                                                    MakeAuction2(timeWorker, category);
                                                }
                                                x++;
                                                goto caty;
                                            }
                                            else
                                            {
                                                i++;
                                                goto next;
                                            }

                                            break;
                                        case 4:
                                            if (OfferFrequencyCB.SelectedIndex == i)        //Codziennie
                                            {
                                                timeWorker = DateTime.Now.AddMinutes(12);
                                                if (categoriesList.Count > 1)
                                                {
                                                    MakeAuction2(timeWorker, category);
                                                }
                                                else
                                                {
                                                    MakeAuction(category);
                                                }
                                                timeWorker = DateTime.Now.AddMinutes(20);
                                                for (UInt32 z = 0; z < 31; z++)
                                                {
                                                    timeWorker = timeWorker.AddDays(1);
                                                    MakeAuction2(timeWorker, category);
                                                }
                                                x++;
                                                goto caty;
                                            }
                                            else
                                            {
                                                i++;
                                                goto next;
                                            }

                                            break;
                                        case 5:                     
                                            if (OfferFrequencyCB.SelectedIndex == i)        //Custom
                                            {
                                                CustomDates();
                                                if (OfferCallendar.SelectedDate != null)
                                                {
                                                    if (numberOfDataWorkers > 0)
                                                    {
                                                        for (UInt32 z = 0; z < numberOfDataWorkers; z++)
                                                        {

                                                            MakeAuction2(dateWorker[z], category);
                                                        }
                                                    }
                                                }
                                                x++;
                                                goto caty;
                                            }

                                            else
                                            {
                                                i++;
                                            }         
                                            Array.Clear(dateWorker, 0, dateWorker.Length);
                                            numberOfDataWorkers = 0;
                                            break;
                                    }



                                }
                            }
                        }
                        else
                        {
                            MessageBoxResult wrongResult = MessageBox.Show("Musisz wybrać kategorie!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        toMuchWeight = false;
                        toMuchWeightNoticed = false;

                    }



                    catch
                    {
                        Shower.Items.Add("Wystawianie oferty nie powiodło");
                        MessageBoxResult wrongResult = MessageBox.Show("Błąd przy wystawianiu oferty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                };
                Dispatcher.Invoke(DispatcherPriority.Normal, auctionChecker);

                categoriesList.Clear();
                Shower.Dispatcher.Invoke(delegate { Shower.Items.Add("Koszt: " + itemCost); });
                Shower.Dispatcher.Invoke(delegate { Shower.Items.Add("NR:" + itemPromStatus); });


            });

            LoadingKiller();
        }

        public void MakeAuction(int categorySwitch)
        {
            formFiller[0] = new FieldsValue();
            formFiller[0].fid = 0;

            formFiller[401] = new FieldsValue();       //Stan
            formFiller[401].fid = 20365;
            formFiller[401].fvalueint = 1;

            formFiller[402] = new FieldsValue();       //Stan
            formFiller[402].fid = 32611;
            formFiller[402].fvalueint = 1;
            
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
            formFiller[2].fvalueint = categorySwitch;               

            formFiller[4] = new FieldsValue();
            formFiller[4].fid = 4;
            formFiller[4].fvalueint = 99;
            formFiller[5] = new FieldsValue();
            formFiller[5].fid = 5;
            if (AmountTB.Text != "")
            {
                if (AmountTB.Text != "0")
                {
                    formFiller[5].fvalueint = Int32.Parse(AmountTB.Text);       //Zabezpieczenie gdyby było 0
                }
                else
                {
                    formFiller[5].fvalueint = 1;
                }
            }
            else
            {
                formFiller[5].fvalueint = 1;
            }          
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
            path = path + "/Auctions/" + currentOrginalName + "/Photos/" + currentOrginalName;

            BitmapImage photoBitmapBeggining = new BitmapImage(new Uri(normalPath + "head.png"));     //W .Net robi się to inaczej           
            byte[] photoBeggining = getJPGFromImageControl(photoBitmapBeggining);
            formFiller[342] = new FieldsValue();
            formFiller[342].fid = 342;
            formFiller[342].fvalueimage = photoBeggining;

            if (imageCounter > 7)
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
                    formFiller[400] = new FieldsValue();      
                    formFiller[400].fid = 22164;
                    formFiller[400].fvaluefloat = 20;

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
                formFiller[400] = new FieldsValue();      
                formFiller[400].fid = 22164;
                formFiller[400].fvaluefloat = 2;

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
                    formFiller[400] = new FieldsValue();      
                    formFiller[400].fid = 22164;
                    formFiller[400].fvaluefloat = 100;

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
                formFiller[400] = new FieldsValue();      
                formFiller[400].fid = 22164;
                formFiller[400].fvaluefloat = 50;

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
            switch (imageCounter)       
            {
                case 0:     
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;
                        desc.Close();
                    }
                    break;
            }
            isThereATable = false;
            if (Encoding.Default.GetByteCount(descriptionWorker) > 1569)
            {
                if (toMuchWeightNoticed == false)
                {
                    MessageBoxResult wrongResult = MessageBox.Show("Twój tekst jest za długi! Musisz go skrócić.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    toMuchWeight = true;
                    toMuchWeightNoticed = true;
                }
            }
            if (toMuchWeight == false)
            {
                service.doNewAuctionExt(sessionHandler, formFiller, 1, 1, itemStruct, variants, auctionTags, afterSale,
                addicionalServicesGroup, out itemCost, out itemPromStatus);
            }
            Thread.Sleep(200);
        }

        public double ConvertToTimestamp(DateTime value)
        {            
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());            
            return (float)span.TotalSeconds;
        }

        public void MakeAuction2(DateTime currentDateWorker, int categorySwitch)
        {
            formFiller[0] = new FieldsValue();
            formFiller[0].fid = 0;

            formFiller[401] = new FieldsValue();       //Stan
            formFiller[401].fid = 20365;
            
                formFiller[401].fvalueint = 1;
            

            formFiller[402] = new FieldsValue();       //Stan
            formFiller[402].fid = 32611;
            
                formFiller[402].fvalueint = 1;
          

            

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
            formFiller[2].fvalueint = categorySwitch;               

            formFiller[3] = new FieldsValue();
            formFiller[3].fid = 3;
            formFiller[3].fvaluedatetime = (long)ConvertToTimestamp(currentDateWorker);
            formFiller[4] = new FieldsValue();
            formFiller[4].fid = 4;
            formFiller[4].fvalueint = 99;
            formFiller[5] = new FieldsValue();
            formFiller[5].fid = 5;
            if (AmountTB.Text != "")
            {
                if(AmountTB.Text != "0")
                {
                    formFiller[5].fvalueint = Int32.Parse(AmountTB.Text);       
                }
                else
                {
                        formFiller[5].fvalueint = 1;
                }
            }            
            else
            {
                formFiller[5].fvalueint = 1;
            }
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
            path = path + "/Auctions/" + currentOrginalName + "/Photos/" + currentOrginalName;

            BitmapImage photoBitmapBeggining = new BitmapImage(new Uri(normalPath + "head.png"));     //W .Net robi się to inaczej           
            byte[] photoBeggining = getJPGFromImageControl(photoBitmapBeggining);
            formFiller[342] = new FieldsValue();
            formFiller[342].fid = 342;
            formFiller[342].fvalueimage = photoBeggining;

            if (imageCounter > 7)
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
                BitmapImage photoBitmapTable = new BitmapImage(new Uri(normalPath + "Table/" + SKUTB.Text + ".png"));    
                byte[] photoTable = getJPGFromImageControl(photoBitmapTable);
                formFiller[343] = new FieldsValue();
                formFiller[343].fid = 343;
                formFiller[343].fvalueimage = photoTable;
                isThereATable = true;
            }

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
                    formFiller[400] = new FieldsValue();      
                    formFiller[400].fid = 22164;
                    formFiller[400].fvaluefloat = 20;

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
                formFiller[400] = new FieldsValue();      
                formFiller[400].fid = 22164;
                formFiller[400].fvaluefloat = 2;

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
                    formFiller[400] = new FieldsValue();      
                    formFiller[400].fid = 22164;
                    formFiller[400].fvaluefloat = 100;

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
                formFiller[400] = new FieldsValue();      
                formFiller[400].fid = 22164;
                formFiller[400].fvaluefloat = 50;

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
            switch (imageCounter)       
            {
                case 0:     
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);                        
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
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
                        descriptionWorker = descriptionWorker.Replace("AUTYT", NameOfProdTB.Text);
                        descriptionWorker = descriptionWorker.Replace("OFFDESC", DescriptionTB.Text);
                        formFiller[341].fvaluestring = descriptionWorker;
                        desc.Close();
                    }
                    break;
            }
            isThereATable = false;
            if(Encoding.Default.GetByteCount(descriptionWorker)>1569)
            {
                if (toMuchWeightNoticed == false)
                {
                    MessageBoxResult wrongResult = MessageBox.Show("Twój tekst jest za długi! Musisz go skrócić.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    toMuchWeight = true;
                    toMuchWeightNoticed = true;
                    /*
                    MessageBoxResult wrongResult = MessageBox.Show("Twój tekst jest za długi! Skrócuć go automatycznie?", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);

                    int dotSearcher = DescriptionTB.Text.Length - 1;
                    DescriptionTB.Text = DescriptionTB.Text.Remove(dotSearcher - 3, DescriptionTB.Text.Length - dotSearcher-3);
                    while (DescriptionTB.Text[dotSearcher].ToString() != ".")
                    {
                        dotSearcher--;
                    }
                    DescriptionTB.Text = DescriptionTB.Text.Remove(dotSearcher, DescriptionTB.Text.Length - dotSearcher);
                    goto autoDescriptionChange;
                    */
                }
            }
            if (toMuchWeight == false)
            {
                service.doNewAuctionExt(sessionHandler, formFiller, 1, 1, itemStruct, variants, auctionTags, afterSale,
                    addicionalServicesGroup, out itemCost, out itemPromStatus);
            }
            Thread.Sleep(200);

        }

        public void CustomDates()
        {
            DatePick.Visibility = Visibility.Visible;
            OfferCallendar.Visibility = Visibility.Visible;
            DateConfirm.Visibility = Visibility.Visible;
        }

        private void DateConfirm_Click(object sender, RoutedEventArgs e) //Ustaw godziny by zawsze były poranne, lub dopołudniowe
        {
            Array.Clear(dateWorker, 0, dateWorker.Length);
            numberOfDataWorkers = 0;
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
                                numberOfDataWorkers++;
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
            if (OfferFrequencyCB.SelectedIndex == 5)
            {

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

        private void NameOfProdTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            signsNumberLabel.Content = 50 - NameOfProdTB.Text.Length;
        }

        private void PriceTB_TextChanged(object sender, TextChangedEventArgs e)
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

        private void ClearAllBut_Click(object sender, RoutedEventArgs e)            //funkcja czyszcząca wszystkie tablice, texboxy itp. z wystawiania tak w razie w
        {
            
        }

        private void CategoriesPopup_Click(object sender, RoutedEventArgs e)
        {
            CatPOP.IsOpen = true;
        }

        private void CatSubBUT_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesChoiceLB.SelectedIndex == 0)
            {
                if (!categoriesList.Contains(121620))
                {
                    categoriesList.Add(121620);
                }
                if (!categoriesList.Contains(121399))
                {
                    categoriesList.Add(121399);
                }
                if (!categoriesList.Contains(121420))
                {
                    categoriesList.Add(121420);
                }
                if (!categoriesList.Contains(121368))
                {
                    categoriesList.Add(121368);
                }
                if (!categoriesList.Contains(121351))
                {
                    categoriesList.Add(121351);
                }
                if (!categoriesList.Contains(147664))
                {
                    categoriesList.Add(147664);
                }

            }
            if (CategoriesChoiceLB.SelectedIndex == 1)
            {
                if (!categoriesList.Contains(121620))
                {
                    categoriesList.Add(121620);
                }
                if (!categoriesList.Contains(121399))
                {
                    categoriesList.Add(121399);
                }
                if (!categoriesList.Contains(121368))
                {
                    categoriesList.Add(121368);
                }
                if (!categoriesList.Contains(121351))
                {
                    categoriesList.Add(121351);
                }
            }
            if (CategoriesChoiceLB.SelectedIndex == 2)
            {
                if (!categoriesList.Contains(121620))
                {
                    categoriesList.Add(121620);
                }
                if (!categoriesList.Contains(121399))
                {
                    categoriesList.Add(121399);
                }
                if (!categoriesList.Contains(121368))
                {
                    categoriesList.Add(121368);
                }
            }
            if (CategoriesChoiceLB.SelectedIndex == 3)
            {
                if (!categoriesList.Contains(121620))
                {
                    categoriesList.Add(121620);
                }
                if (!categoriesList.Contains(121399))
                {
                    categoriesList.Add(121399);
                }
                if (!categoriesList.Contains(121438))
                {
                    categoriesList.Add(121438);
                }
            }
            if (CategoriesChoiceLB.SelectedIndex == 4)
            {
                if (!categoriesList.Contains(121620))
                {
                    categoriesList.Add(121620);
                }
                if (!categoriesList.Contains(121399))
                {
                    categoriesList.Add(121399);
                }
                if (!categoriesList.Contains(121419))
                {
                    categoriesList.Add(121419);
                }
                if (!categoriesList.Contains(121368))
                {
                    categoriesList.Add(121368);
                }
                if (!categoriesList.Contains(121351))
                {
                    categoriesList.Add(121351);
                }
            }
            if (CategoriesChoiceLB.SelectedIndex == 5)
            {
                if (!categoriesList.Contains(121620))
                {
                    categoriesList.Add(121620);
                }
                if (!categoriesList.Contains(121399))
                {
                    categoriesList.Add(121399);
                }
                if (!categoriesList.Contains(121420))
                {
                    categoriesList.Add(121420);
                }
                if (!categoriesList.Contains(121368))
                {
                    categoriesList.Add(121368);
                }
                if (!categoriesList.Contains(121351))
                {
                    categoriesList.Add(121351);
                }
                if (!categoriesList.Contains(147664))
                {
                    categoriesList.Add(147664);
                }
            }
            if (CategoriesChoiceLB.SelectedIndex == 6)
            {
                if (!categoriesList.Contains(121620))
                {
                    categoriesList.Add(121620);
                }
                if (!categoriesList.Contains(121399))
                {
                    categoriesList.Add(121399);
                }
                if (!categoriesList.Contains(121368))
                {
                    categoriesList.Add(121368);
                }
                if (!categoriesList.Contains(121351))
                {
                    categoriesList.Add(121351);
                }
            }
            if (CategoriesChoiceLB.SelectedIndex == 7)
            {
                if (!categoriesList.Contains(257640))
                {
                    categoriesList.Add(257640);
                }
                if (!categoriesList.Contains(256110))
                {
                    categoriesList.Add(256110);
                }
                if (!categoriesList.Contains(111867))
                {
                    categoriesList.Add(111867);
                }
                if (!categoriesList.Contains(49845))
                {
                    categoriesList.Add(49845);
                }
                if (!categoriesList.Contains(110023))
                {
                    categoriesList.Add(110023);
                }
                if (!categoriesList.Contains(82263))
                {
                    categoriesList.Add(82263);
                }
                if (!categoriesList.Contains(82285))
                {
                    categoriesList.Add(82285);
                }
                if (!categoriesList.Contains(128))
                {
                    categoriesList.Add(128);
                }
                if (!categoriesList.Contains(121620))
                {
                    categoriesList.Add(121620);
                }
                if (!categoriesList.Contains(82290))
                {
                    categoriesList.Add(82290);
                }
                if (!categoriesList.Contains(121620))
                {
                    categoriesList.Add(121620);
                }
            }
            if (CategoriesChoiceLB.SelectedIndex == 8)
            {
                if (!categoriesList.Contains(121415))
                {
                    categoriesList.Add(121415);
                }
                if (!categoriesList.Contains(121418))
                {
                    categoriesList.Add(121418);
                }
                if (!categoriesList.Contains(121412))
                {
                    categoriesList.Add(121412);
                }
                if (!categoriesList.Contains(121417))
                {
                    categoriesList.Add(121417);
                }
                if (!categoriesList.Contains(121416))
                {
                    categoriesList.Add(121416);
                }
                if (!categoriesList.Contains(121411))
                {
                    categoriesList.Add(121411);
                }
                if (!categoriesList.Contains(121435))
                {
                    categoriesList.Add(121435);
                }
            }
            if (CategoriesChoiceLB.SelectedIndex == 9)
            {
                if (!categoriesList.Contains(121399))
                {
                    categoriesList.Add(121399);
                }
                if (!categoriesList.Contains(121620))
                {
                    categoriesList.Add(121620);
                }
                if (!categoriesList.Contains(121420))
                {
                    categoriesList.Add(121420);
                }
                if (!categoriesList.Contains(121368))
                {
                    categoriesList.Add(121368);
                }
                if (!categoriesList.Contains(121351))
                {
                    categoriesList.Add(121351);
                }
                if (!categoriesList.Contains(147664))
                {
                    categoriesList.Add(147664);
                }
            }
            if (CategoriesChoiceLB.SelectedIndex == 10)
            {
                if (!categoriesList.Contains(121399))
                {
                    categoriesList.Add(121399);
                }
                if (!categoriesList.Contains(121620))
                {
                    categoriesList.Add(121620);
                }
                if (!categoriesList.Contains(121420))
                {
                    categoriesList.Add(121420);
                }
                if (!categoriesList.Contains(121368))
                {
                    categoriesList.Add(121368);
                }
                if (!categoriesList.Contains(257192))
                {
                    categoriesList.Add(257192); //????????????????????
                }
                if (!categoriesList.Contains(121351))
                {
                    categoriesList.Add(121351);
                }
                if (!categoriesList.Contains(147664))
                {
                    categoriesList.Add(147664);
                }
            }
            if (CategoriesChoiceLB.SelectedIndex == 11)
            {
                if (!categoriesList.Contains(121411))
                {
                    categoriesList.Add(121411);
                }
                if (!categoriesList.Contains(121435))
                {
                    categoriesList.Add(121435);
                }
            }
            foreach (int c in categoriesList)
            {
                Shower.Items.Add(c);
            }



            CatPOP.IsOpen = false;
            if (CategoriesChoiceLB.SelectedIndex == 12)
            {
                CatOtherPOP.IsOpen = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CatPOP.IsOpen = false;

        }

        private void CatSubOtherBUT_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                List<TextBox> catWorker = new List<TextBox>();
                int catStringNumber = 0;
                for (int i = 0; i <= CategoriesChoiceOtherLB.SelectedItems.Count - 1; i++)
                {
                    catWorker.Add(new TextBox());
                }

                foreach (ListBox x in CategoriesChoiceOtherLB.SelectedItems)
                {
                    foreach (TextBox el in catWorker)        //Sprawdź czy dobrze jak bedziesz miał kategorie
                    {
                        el.Visibility = Visibility.Hidden;
                        el.Text = x.SelectedItems[catStringNumber].ToString();
                        catStringNumber++;
                    }
                }


                for (int item = 0; item <= CategoriesChoiceOtherLB.SelectedItems.Count - 1; item++)
                {
                    categoriesList.Add((int)CategoriesChoiceOtherLB.SelectedItems[item]);
                }
            /*}
            catch
            { }*/
            CatOtherPOP.IsOpen = false;
        }

        private void WystawTabBut_Click(object sender, RoutedEventArgs e)
        {
            NazwaAukcjiLB.Visibility = Visibility.Visible;
            CustomKurierZwykłyTB.Visibility = Visibility.Visible;
            DescriptionTB.Visibility = Visibility.Visible;
            PriceTB.Visibility = Visibility.Visible;
            NameOfProdTB.Visibility = Visibility.Visible;
            OpisLB.Visibility = Visibility.Visible;
            AucDownBT.Visibility = Visibility.Hidden;
            CenaLB.Visibility = Visibility.Visible;
            Shower.Visibility = Visibility.Visible;
            StartAuction.Visibility = Visibility.Visible;
            Continue.Visibility = Visibility.Visible;
            AmountTB.Visibility = Visibility.Visible;
            IloscLB.Visibility = Visibility.Visible;
            URLTB.Visibility = Visibility.Visible;
            URLLB.Visibility = Visibility.Visible;
            SKUTB.Visibility = Visibility.Visible;
            SKULB.Visibility = Visibility.Visible;
            Kurier.Visibility = Visibility.Visible;
            Paleta.Visibility = Visibility.Visible;
            List.Visibility = Visibility.Visible;
            TylkoOsobisty.Visibility = Visibility.Visible;
            InnaDostawa.Visibility = Visibility.Visible;
            OpcjeDostawyLB.Visibility = Visibility.Visible;
            CustomKurierPobranieTB.Visibility = Visibility.Visible;            
            FrequencyLB.Visibility = Visibility.Visible;
            OfferFrequencyCB.Visibility = Visibility.Visible;
            IloscZnakowLB.Visibility = Visibility.Visible;
            signsNumberLabel.Visibility = Visibility.Visible;
            ZLLB.Visibility = Visibility.Visible;
            SZTLB.Visibility = Visibility.Visible;
            ZLKurier1LB.Visibility = Visibility.Visible;
            ZLKurier2LB.Visibility = Visibility.Visible;
            OplaconyLB.Visibility = Visibility.Visible;
            PobranieLB.Visibility = Visibility.Visible;
            ClearAllBut.Visibility = Visibility.Visible;
            InMagazineBut.Visibility = Visibility.Hidden;
            CategoriesPopup.Visibility = Visibility.Visible;
            DownloadedAuctionData.Visibility = Visibility.Hidden;
            ActTabBut.Visibility = Visibility.Hidden;
        }

        private void ZobaczTabBut_Click(object sender, RoutedEventArgs e)
        {
            NazwaAukcjiLB.Visibility = Visibility.Hidden;
            CustomKurierZwykłyTB.Visibility = Visibility.Hidden;
            DescriptionTB.Visibility = Visibility.Hidden;
            PriceTB.Visibility = Visibility.Hidden;
            NameOfProdTB.Visibility = Visibility.Hidden;
            OpisLB.Visibility = Visibility.Hidden;
            AucDownBT.Visibility = Visibility.Visible;
            CenaLB.Visibility = Visibility.Hidden;
            Shower.Visibility = Visibility.Hidden;
            StartAuction.Visibility = Visibility.Hidden;
            Continue.Visibility = Visibility.Hidden;
            AmountTB.Visibility = Visibility.Hidden;
            IloscLB.Visibility = Visibility.Hidden;
            URLTB.Visibility = Visibility.Hidden;
            URLLB.Visibility = Visibility.Hidden;
            SKUTB.Visibility = Visibility.Hidden;
            SKULB.Visibility = Visibility.Hidden;
            Kurier.Visibility = Visibility.Hidden;
            Paleta.Visibility = Visibility.Hidden;
            List.Visibility = Visibility.Hidden;
            TylkoOsobisty.Visibility = Visibility.Hidden;
            InnaDostawa.Visibility = Visibility.Hidden;
            OpcjeDostawyLB.Visibility = Visibility.Hidden;
            CustomKurierPobranieTB.Visibility = Visibility.Hidden;
            FrequencyLB.Visibility = Visibility.Hidden;
            OfferFrequencyCB.Visibility = Visibility.Hidden;
            IloscZnakowLB.Visibility = Visibility.Hidden;
            signsNumberLabel.Visibility = Visibility.Hidden;
            ZLLB.Visibility = Visibility.Hidden;
            SZTLB.Visibility = Visibility.Hidden;
            ZLKurier1LB.Visibility = Visibility.Hidden;
            ZLKurier2LB.Visibility = Visibility.Hidden;
            OplaconyLB.Visibility = Visibility.Hidden;
            PobranieLB.Visibility = Visibility.Hidden;
            ClearAllBut.Visibility = Visibility.Hidden;
            InMagazineBut.Visibility = Visibility.Visible;
            CategoriesPopup.Visibility = Visibility.Hidden;
            DownloadedAuctionData.Visibility = Visibility.Visible;
            ActTabBut.Visibility = Visibility.Visible; 

        }

        private void ActTabBut_Click(object sender, RoutedEventArgs e)
        {
            //Ceny można zmieniać tylko przy aukcjach, na których nikt jeszcze nic nie kupił - takie są zasady allegro dla WebApi
            try           //Na REST Api można zmieniać zawsze
            {             //Generalnie wywala błędy, ale działa O.o
                int itemsSold = 0;
                int itemsNow = 0;
                long itemId = 7302711402;
                string changePriceInfo = service.doChangePriceItem(sessionHandler, ref itemId, 0, 0, 1000, 0);
                Shower.Items.Add(changePriceInfo);
                service.doChangeQuantityItem(sessionHandler, ref itemId, 10, out itemsNow, out itemsSold);
                Shower.Items.Add("Pozostało: " + itemsNow);
                Shower.Items.Add("Sprzedano: " + itemsSold);
            }
            catch
            {
                MessageBoxResult wrongResult = MessageBox.Show("Ta aukcja jest właśnie modyfikowana. Spróbuj później.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
