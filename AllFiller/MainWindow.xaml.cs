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
using AllFiller.Allegro;

namespace AllFiller
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        //Zrób zapisywanie do pliku wszystkich numerów aukcji i ich tytułów, które wystawisz
        //Dorób pokazywanie pobranych obrazów i przypisywanie ich wtedy do globalnych zmiennych

        //Ogólne
        AllegroWebApiService service = new AllegroWebApiService();
        string sessionHandler;
        long offset = 0;
        long serverTime = 0;
        UInt32 imageCounter;               

        //Logowanie
        string apiKey = "";
        long version;
        AccountLogIn logIn = new AccountLogIn();

        //Aukcja
        //UInt32 numOfProd;
        //string price;
        //string nameOfProd;
        //string description;
        
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

        //Wystawianie
        string verstr;
        long verkey;
        FieldsValue[] formFiller = new FieldsValue[410];
        DateTime currentDate;
        DateTime currentTime;
        DateTime timeWorker;
        DateTime[] dateWorker = new DateTime[1000];
        UInt32 numberOfDataWorkers = 0;
        List<int> categoriesList = new List<int>();
        AllegroFormFiller fillIt = new AllegroFormFiller();

        public MainWindow()
        {
            InitializeComponent();

            this.Title = "AllFiller - Łukasz Granat";

            version = logIn.GetLocalVersionKey(service);

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
                 sessionHandler = logIn.LogIn(sessionHandler, version, service);

                 sessionHandlerTB.Dispatcher.Invoke(delegate { sessionHandlerTB.Text = sessionHandler; });
                 sessionHandlerTB.Dispatcher.Invoke(delegate { sessionHandlerTB.Visibility = Visibility.Visible; });
                 sessionIDLabel.Dispatcher.Invoke(delegate { sessionIDLabel.Visibility = Visibility.Visible; });
                 
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
                
                DownloadedAuctionData.Dispatcher.Invoke(delegate
                {
                    DownloadedAuctionData.Items.Clear();
                });
                AuctionsInfoDownloader down = new AuctionsInfoDownloader();
                down.Download(sessionHandler, DownloadedAuctionData, service);
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
                        currrentModel = currentModels.ModelDownloader(Model);                        
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
                    else if (ListP.IsChecked == true) { }
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
                    AuctionMaker all = new AuctionMaker();
                    all.MakeAuction(timeWorker, categoriesList,  dateWorker,  numberOfDataWorkers,  OfferCallendar,  OfferFrequencyCB,  service,  apiKey, fillIt, currentOrginalName, imageCounter, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
                };
                Dispatcher.Invoke(DispatcherPriority.Normal, auctionChecker);

                categoriesList.Clear();
                


            });

            LoadingKiller();
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
            ListP.Visibility = Visibility.Visible;
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
            ListP.Visibility = Visibility.Hidden;
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
            CategoriesPopup.Visibility = Visibility.Hidden;
            DownloadedAuctionData.Visibility = Visibility.Visible;
            ActTabBut.Visibility = Visibility.Visible; 

        }

        private void ActTabBut_Click(object sender, RoutedEventArgs e)
        {
            UpToDateAuctions upup = new UpToDateAuctions();
            upup.GoOn(sessionHandler, service);
        }
    }
}
