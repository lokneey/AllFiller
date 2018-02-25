using HtmlAgilityPack;
using System;

using System.IO;



namespace AllFiller.Support
{
    class PhotoDown 
    {
        public string photosDoc;
        int indexered;
        public PhotoDown(string title, HtmlNode[] photo)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory;    //Dopisz kod zamieniający istniejący plik na nowy i zrób funkcję zapisującą w oddzielnym pliku
            path = path.Replace(@"\", "/");
            path = path + "/Auctions/" + title + "/Photos/";
            DirectoryInfo directoryForAuctions = new DirectoryInfo(path);
            if (!directoryForAuctions.Exists)
            {
                directoryForAuctions.Create();
            }
            path = path + title;
            for (int i = 0; i < photo.Length; i++)
            {

                photosDoc = photo[i].InnerHtml;
                indexered = photosDoc.IndexOf("data-src=");
                photosDoc = photosDoc.Remove(0, indexered);
                indexered = photosDoc.IndexOf("data-large_image=");
                photosDoc = photosDoc.Remove(indexered, photosDoc.Length - indexered);
                photosDoc = photosDoc.Replace("data-src=", "");
                photosDoc = photosDoc.Replace("\"", "");
                //List.Items.Add(photosDoc);

                //Zakomentowane na nowo
                /*
                indexered = photosDoc.IndexOf("alt", 2);
                photosDoc = photosDoc.Remove(indexered, photosDoc.Length - indexered);
                photosDoc = photosDoc.Replace("medium", "thickbox");
                indexered = photosDoc.IndexOf("src", 2);
                photosDoc = photosDoc.Substring(indexered);
                photosDoc = photosDoc.Replace("\"", "");
                photosDoc = photosDoc.Replace("src=", "");
                //TBinfozestronyLinki.Items.Add(photosDoc);
                */
                
                path = path + i;
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadFile(photosDoc, path + ".jpg");
                }
                
            }
        }

    }
}
