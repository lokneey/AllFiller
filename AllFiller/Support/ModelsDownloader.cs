using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetUser.Support
{
    class ModelsDownloader
    {
        public ObservableCollection<string> modelParts = new ObservableCollection<string>();
        int indexer;
        string modelMaker;

        public ModelsDownloader() { }

        public ObservableCollection<string>  ModelDownloader(HtmlNode[] Model)
        {
            for (int modelIndexer = 0; modelIndexer < Model.Length; modelIndexer++)
            {
                try
                {
                    modelMaker = Model[modelIndexer].OuterHtml;
                    indexer = modelMaker.IndexOf("title", 2);
                    modelMaker = modelMaker.Substring(indexer + 7);
                    modelMaker = modelMaker.Replace("\"", "");
                    modelMaker = modelMaker.Replace(" > ", "");
                    modelMaker = modelMaker.Replace(" >", "");
                    modelMaker = modelMaker.Replace("> ", "");
                    modelMaker = modelMaker.Replace(">", "");
                }
                catch { }
                modelParts.Add(modelMaker);
            }

            return modelParts;
        }
    }
}
