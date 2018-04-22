using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFiller.Support
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
                    indexer = modelMaker.IndexOf("</option>");
                    modelMaker = modelMaker.Remove(indexer, modelMaker.Length - indexer);
                    indexer = modelMaker.IndexOf(">");
                    modelMaker = modelMaker.Remove(0, indexer+1);
                    modelMaker = modelMaker.Replace("</option>", "");
                    modelMaker = modelMaker.Replace("<option value=", "");
                    modelMaker = modelMaker.Replace("\"", "");
                }
                catch { }                
                modelParts.Add(modelMaker);
            }

            return modelParts;
        }
    }
}
