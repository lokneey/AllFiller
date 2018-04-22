using AllFiller.pl.allegro.webapi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AllFiller.Allegro
{
    
    public class AuctionMaker
    {

        bool toMuchWeight = false;
        bool toMuchWeightNoticed = false;

    
        public AuctionMaker() { }
        
        public void MakeAuction(DateTime timeWorker, List<int> categoriesList, DateTime[] dateWorker, UInt32 numberOfDataWorkers, Calendar OfferCallendar, ComboBox OfferFrequencyCB, AllegroWebApiService service, string apiKey, AllegroFormFiller fillIt,
            string currentOrginalName, UInt32 imageCounter, FieldsValue[] formFiller, string sessionHandler, TextBox NameOfProdTB, TextBox AmountTB, TextBox PriceTB, TextBox SKUTB, TextBox CustomKurierZwykłyTB, TextBox CustomKurierPobranieTB, TextBox DescriptionTB, CheckBox Kurier, CheckBox Paleta, CheckBox ListP, CheckBox TylkoOsobisty, CheckBox InnaDostawa)
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
                                            fillIt.MakeAuction2(timeWorker, category, toMuchWeightNoticed, toMuchWeight, currentOrginalName,  imageCounter,  service,  formFiller,  sessionHandler,  NameOfProdTB,  AmountTB,  PriceTB,  SKUTB,  CustomKurierZwykłyTB,  CustomKurierPobranieTB,  DescriptionTB,  Kurier,  Paleta, ListP,  TylkoOsobisty,  InnaDostawa);
                                        }
                                        else
                                        {
                                            fillIt.MakeAuction(category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
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
                                            fillIt.MakeAuction2(timeWorker, category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
                                        }
                                        else
                                        {
                                            fillIt.MakeAuction(category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
                                        }

                                        timeWorker = DateTime.Now.AddMinutes(20);
                                        timeWorker = timeWorker.AddDays(14);
                                        fillIt.MakeAuction2(timeWorker, category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
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
                                            fillIt.MakeAuction2(timeWorker, category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
                                        }
                                        else
                                        {
                                            fillIt.MakeAuction(category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
                                        }

                                        timeWorker = DateTime.Now.AddMinutes(20);
                                        timeWorker = timeWorker.AddDays(7);
                                        fillIt.MakeAuction2(timeWorker, category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
                                        timeWorker = timeWorker.AddDays(7);
                                        fillIt.MakeAuction2(timeWorker, category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
                                        timeWorker = timeWorker.AddDays(7);
                                        fillIt.MakeAuction2(timeWorker, category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
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
                                            fillIt.MakeAuction2(timeWorker, category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
                                        }
                                        else
                                        {
                                            fillIt.MakeAuction(category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
                                        }

                                        timeWorker = DateTime.Now.AddMinutes(20);
                                        for (UInt32 z = 0; z < 10; z++)
                                        {
                                            timeWorker = timeWorker.AddDays(3);
                                            fillIt.MakeAuction2(timeWorker, category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
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
                                            fillIt.MakeAuction2(timeWorker, category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
                                        }
                                        else
                                        {
                                            fillIt.MakeAuction(category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
                                        }
                                        timeWorker = DateTime.Now.AddMinutes(20);
                                        for (UInt32 z = 0; z < 31; z++)
                                        {
                                            timeWorker = timeWorker.AddDays(1);
                                            fillIt.MakeAuction2(timeWorker, category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
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
                                        if (OfferCallendar.SelectedDate != null)
                                        {
                                            if (numberOfDataWorkers > 0)
                                            {
                                                for (UInt32 z = 0; z < numberOfDataWorkers; z++)
                                                {

                                                    fillIt.MakeAuction2(dateWorker[z], category, toMuchWeightNoticed, toMuchWeight, currentOrginalName, imageCounter, service, formFiller, sessionHandler, NameOfProdTB, AmountTB, PriceTB, SKUTB, CustomKurierZwykłyTB, CustomKurierPobranieTB, DescriptionTB, Kurier, Paleta, ListP, TylkoOsobisty, InnaDostawa);
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
                MessageBoxResult wrongResult = MessageBox.Show("Błąd przy wystawianiu oferty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            categoriesList.Clear();
        }
    }
}