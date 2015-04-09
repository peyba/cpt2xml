using System;
using System.IO;

namespace cpt2xml
{
    class CmdMessage : IOutputMessageWrap
    {
        public void Show(string message)
        {
            Console.WriteLine(message);
        }
    }
}