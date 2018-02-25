using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetUser.Support
{
    class PolishSigns
    {
        public PolishSigns() { }
        public string PolishSignsRemover(string done)
        {
            done = done.Replace("ą", "a");
            done = done.Replace("ę", "ę");
            done = done.Replace("ś", "ś");
            done = done.Replace("ć", "ć");
            done = done.Replace("ż", "z");
            done = done.Replace("ź", "z");
            done = done.Replace("ó", "o");
            done = done.Replace("ń", "n");
            done = done.Replace("!", "");
            done = done.Replace("-", "");
            done = done.Replace("/", " ");
            done = done.Replace("\\", " ");


            return done;
        }
    }
}
