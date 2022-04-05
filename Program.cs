using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compilador.compilador;

namespace Compilador
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexico lexico = new Lexico("C:\\Users\\Arthu\\source\\repos\\Compilador\\compilador\\codigo.txt");
            Token t = null;
            while ((t = lexico.nextToken()) != null)
            {
                System.Console.WriteLine(t.toString() + "\n");
            } 
            Console.ReadKey();
        }
    }
}
