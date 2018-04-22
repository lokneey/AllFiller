using AllFiller.pl.allegro.webapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AllFiller.Allegro
{
    public class UpToDateAuctions
    {
        int itemsSold = 0;
        int itemsNow = 0;
        long itemId;

        public UpToDateAuctions() { }

        public void GoOn(string sessionHandler, AllegroWebApiService service)
        {
            //Ceny można zmieniać tylko przy aukcjach, na których nikt jeszcze nic nie kupił - takie są zasady allegro dla WebApi
            try           //Na REST Api można zmieniać zawsze
            {             //Generalnie wywala błędy, ale działa O.o                
                itemId = 7302711402;
                string changePriceInfo = service.doChangePriceItem(sessionHandler, ref itemId, 0, 0, 1000, 0);                
                service.doChangeQuantityItem(sessionHandler, ref itemId, 10, out itemsNow, out itemsSold);
            }
            catch
            {
                MessageBoxResult wrongResult = MessageBox.Show("Ta aukcja jest właśnie modyfikowana. Spróbuj później.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
