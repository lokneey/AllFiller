using AllFiller.pl.allegro.webapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AllFiller.Allegro
{
    public class AuctionsInfoDownloader
    {
        long offset = 0;
        long[] auctionsID;
        MyAccountStruct2[] accountStructTable;

        public AuctionsInfoDownloader() { }

        public void Download(string sessionHandler, DataGrid DownloadedAuctionData, AllegroWebApiService service)
        {
            try
            {
                accountStructTable = service.doMyAccount2(sessionHandler, "sell", (int)offset, auctionsID, 100);
            }
            catch
            {
                //Shower.Dispatcher.Invoke(delegate { Shower.Items.Add("Nie można sprawdzić obecnych aukcji"); });
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
                    DownloadedAuctionData.Items.Add(new DataGridItem()
                    {
                        Column1 = accountStruct.myaccountarray[9],
                        Column2 = accountStruct.myaccountarray[33],
                        Column10 = accountStruct.myaccountarray[27],
                        Column3 = accountStruct.myaccountarray[4] + " zł",
                        Column4 = accountStruct.myaccountarray[17],
                        Column5 = accountStruct.myaccountarray[16],
                        Column6 = accountStruct.myaccountarray[0],
                        Column7 = accountStruct.myaccountarray[6],
                        Column8 = accountStruct.myaccountarray[7],
                    });
                });
            }
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

    }
}
