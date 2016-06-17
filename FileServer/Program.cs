using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    class Program
    {
        static void Main(string[] args)
        {

            if (inputDirectory != "error")
            {

                sw.WriteLine("<recieve/>");

                sw.Flush();

                using (FileStream fs = new FileStream(inputDirectory, FileMode.Open, FileAccess.Read))
                {
                    sw.WriteLine(fs.Length);

                    sw.WriteLine(Path.GetFileName(inputDirectory));
                    long fileSize = fs.Length;
                    long sum = 0;
                    int count = 0;
                    int bufferSize = 1024;
                    byte[] data = new byte[bufferSize];
                    if (fs.Length < bufferSize)
                    {
                        data = null;
                        bufferSize = (int)fs.Length;
                        data = new byte[bufferSize];
                    }

                    while (sum < fileSize)
                    {
                        if ((fileSize - sum) < bufferSize)
                        {
                            bufferSize = (int)(fileSize - sum);
                            data = new byte[bufferSize];
                        }
                        count = fs.Read(data, 0, data.Length);
                        s.Write(data, 0, count);
                        sum += count;
                        Console.WriteLine(sum + " " + count + " " + fileSize);

                    }
                }
            }


            public static void Service()
        {

            string DirectoryPath = @"D:\";
            while (true)
            {
                Console.Title = "Centrall";
                Socket soc = listener.AcceptSocket();
                soc.NoDelay = true;
                Console.WriteLine("Connected: {0}", soc.RemoteEndPoint);
                try
                {
                    NetworkStream s = new NetworkStream(soc);
                    StreamReader sr = new StreamReader(s, Encoding.UTF8);
                    StreamWriter sw = new StreamWriter(s, Encoding.UTF8);
                    sw.AutoFlush = true; // enable automatic flushing


                    // start
                    string input;
                    sw.WriteLine("=====================");
                    sw.WriteLine("Hello in centrall!!!");
                    sw.WriteLine("Writed by Henri2h in 2016");
                    sw.WriteLine("=====================");
                    sw.WriteLine();
                    bool boocle = true;
                    while (boocle)
                    {
                        sw.WriteLine("<input/>");
                        input = sr.ReadLine();
                    }
                }}
}
