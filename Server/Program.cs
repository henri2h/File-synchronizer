using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        public static string DirectoryPath = @"D:\";
        public static TcpListener listener;
        const int LIMIT = 5; //5 concurrent clients
        public static int port = 2055;

        static void Main(string[] args)
        {
            // load the server
            // directory path
            DirectoryPath = Path.Combine(Environment.CurrentDirectory, "File");
            if (Directory.Exists(DirectoryPath) == false)
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            // console
            Console.Title = "File synchroniser";


            string localComputerName = Dns.GetHostName();
            Console.WriteLine("DNS : " + localComputerName);

            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress input in localIPs)
            {
                Console.WriteLine(input.ToString());
            }

            // start listener
            listener = TcpListener.Create(port);
            listener.Start();

            Console.WriteLine("Server mounted, listening to port : " + port);


            int serverNumber = 0;
            for (int i = 0; i < LIMIT; i++)
            {

                Thread t = new Thread(new ParameterizedThreadStart(Service));
                t.Start(serverNumber);

                serverNumber++;
            }

        }
        public static void Service(object server)
        {
            int serverNumb = Int32.Parse(server.ToString());
            while (true)
            {
                Console.WriteLine("started server : " + serverNumb);
                Socket soc = listener.AcceptSocket();
                soc.NoDelay = true;
                Console.WriteLine("Connected: {0} on server {1}", soc.RemoteEndPoint, serverNumb);
                try
                {
                    NetworkStream s = new NetworkStream(soc);
                    StreamReader sr = new StreamReader(s, Encoding.UTF8);
                    StreamWriter sw = new StreamWriter(s, Encoding.UTF8);
                    sw.AutoFlush = true; // enable automatic flushing


                    // start
                    string input;
                    while (true)
                    {
                        input = sr.ReadLine();
                        if (input == "#connection")
                        {
                            sw.WriteLine("Server");
                            while (soc.Connected)
                            {
                                if (s.DataAvailable)
                                {
                                    // start if a command is recieved
                                    input = sr.ReadLine();
                                    if (input == "#file")
                                    {
                                        string directory = sr.ReadLine();
                                        string[] files = Directory.GetFiles(DirectoryPath);
                                        foreach (string file in files)
                                        {
                                            sw.WriteLine(file);
                                        }
                                        sw.WriteLine("#end");
                                    }
                                    else if (input == "#fileDownload")
                                    {
                                        string inputDirectory = sr.ReadLine();

                                        using (FileStream fs = new FileStream(inputDirectory, FileMode.Open, FileAccess.Read))
                                        {
                                            long fileSize = fs.Length;
                                            sw.WriteLine(fileSize);

                                            long sum = 0;
                                            int count = 0;
                                            int bufferSize = (int)fs.Length;
                                            Console.WriteLine(fs.Length);

                                            byte[] data = new byte[bufferSize];
                                            sw.WriteLine(bufferSize);

                                            while (sum < fileSize)
                                            {

                                                count = fs.Read(data, 0, data.Length);
                                                sw.WriteLine(count);

                                                s.Write(data, 0, count);
                                                sum += count;
                                                Console.WriteLine(sum + " " + count + " " + data.Length + " " + fileSize);

                                                if (sum < fileSize)
                                                {
                                                    sw.WriteLine("false");
                                                }
                                                else
                                                {
                                                    sw.WriteLine("true");
                                                }
                                            }
                                        }
                                        //end of file transfert
                                        sw.WriteLine("#endFileDownload");
                                    }
                                    else if (input == "#fileUpload")
                                    {
                                        // file name
                                        string filePath = sr.ReadLine();

                                        // ready to recieve file
                                        sw.WriteLine("ready");

                                        long fileSize = long.Parse(sr.ReadLine());// your file size that you are going to receive it.

                                        FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                                        file.Position = 0;
                                        int count = 0;
                                        long sum = 0;   //sum here is the total of received bytes.

                                        //buffer
                                        int dataSize = int.Parse(sr.ReadLine());
                                        byte[] data = new byte[dataSize];

                                        while (sum < fileSize)
                                        {
                                            // buffer size
                                            string dataLenghtString = sr.ReadLine();
                                            int dataLenght = int.Parse(dataLenghtString);

                                            count = s.Read(data, 0, dataLenght);
                                            file.Write(data, 0, count);
                                            sum = sum + count;

                                            bool egal = false;
                                            if (count == dataLenght) { egal = true; }

                                            Console.WriteLine("dataLenght: " + dataLenght + " sum : " + sum + " count : " + count + " file size : " + fileSize + " egal : " + egal);

                                            string fileTransfertended = sr.ReadLine();
                                            if (fileTransfertended == "true")
                                            {
                                                Console.WriteLine("transfert ended");
                                                Console.WriteLine("Summary = filelength :" + file.Length);
                                                break;
                                            }
                                            if (s.DataAvailable == false) { Console.WriteLine("no data ....");
                                                break;
                                            }

                                        }
                                        sw.WriteLine("#endFileDownload");
                                        string end = sr.ReadLine();
                                        if (end == "#endFileDownload") { Console.WriteLine("File transfert ended"); }
                                    }
                                    else if (input == "#disconnect")
                                    {
                                        sw.WriteLine("#disconnection_ok");
                                        Console.WriteLine("Client disconnected on : " + serverNumb);
                                        break;
                                    }


                                }
                                // end of the while loop > restart
                            }
                        }
                    }
                }
                catch { }
                Console.WriteLine("Deconnected: {0} on server {1}", soc.RemoteEndPoint, serverNumb);
            }
        }
    }
}


