using AllFiller.pl.allegro.webapi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AllFiller.Allegro
{
    public class AllegroFormFiller
    {
        UInt32 imageSelector;
        string descriptionWorker;
        AfterSalesServiceConditionsStruct afterSale;
        ItemTemplateCreateStruct itemStruct;
        VariantStruct[] variants;
        TagNameStruct[] auctionTags;
        string addicionalServicesGroup;
        string itemCost = null;
        int itemPromStatus;
        bool isThereATable = false;

        public AllegroFormFiller() { }
        
        public void MakeAuction(int categorySwitch, bool toMuchWeightNoticed, bool toMuchWeight, string currentOrginalName, UInt32 imageCounter, AllegroWebApiService service, FieldsValue[] formFiller, string sessionHandler, TextBox NameOfProdTB, TextBox AmountTB, TextBox PriceTB, TextBox SKUTB, TextBox CustomKurierZwykłyTB, TextBox CustomKurierPobranieTB, TextBox DescriptionTB, CheckBox Kurier, CheckBox Paleta, CheckBox ListP, CheckBox TylkoOsobisty, CheckBox InnaDostawa)
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
                ListP.IsChecked = false;
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

            if (ListP.IsChecked == true)
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

        public void MakeAuction2(DateTime currentDateWorker, int categorySwitch, bool toMuchWeightNoticed, bool toMuchWeight, string currentOrginalName, UInt32 imageCounter, AllegroWebApiService service, FieldsValue[] formFiller, string sessionHandler, TextBox NameOfProdTB, TextBox AmountTB, TextBox PriceTB, TextBox SKUTB, TextBox CustomKurierZwykłyTB, TextBox CustomKurierPobranieTB, TextBox DescriptionTB, CheckBox Kurier, CheckBox Paleta, CheckBox ListP, CheckBox TylkoOsobisty, CheckBox InnaDostawa)
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
                if (AmountTB.Text != "0")
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
                ListP.IsChecked = false;
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

            if (ListP.IsChecked == true)
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

        public byte[] getJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }
    }
}
