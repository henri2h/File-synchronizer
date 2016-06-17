using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.Write("Server IP : ");
            string ip = Console.ReadLine();

            Console.WriteLine("Connection success : " + TCPDialog.start(ip));

            string fileedit = "";
           foreach(string filelist in TCPDialog.listFiles())
            {
                Console.WriteLine(filelist);
                fileedit = filelist;
            }

            MemoryStream file = TCPDialog.recieveFile(fileedit);
            if (file != null) { Console.WriteLine("transfert ended"); }
            else { Console.WriteLine("transfert failed"); }

            Console.ReadKey();
            string fileContent = Encoding.UTF8.GetString(file.ToArray());
            Console.WriteLine(fileContent);
            bool disconnection = TCPDialog.disconnect();
            Console.WriteLine("Disconnected : " + disconnection);
            Console.ReadKey();
        }
       
    }
}
