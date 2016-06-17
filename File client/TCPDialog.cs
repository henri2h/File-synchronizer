using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class TCPDialog
    {//sd
        static NetworkStream s;
        static StreamReader sr;
        static StreamWriter sw;
        static TcpClient client;
        static Socket soc;
        public static bool start(string ip)
        {
            client = new TcpClient(ip, 2055);
            client.ReceiveTimeout = 500;
            soc = client.Client;
            s = client.GetStream();
            sr = new StreamReader(s, Encoding.UTF8);
            sw = new StreamWriter(s, Encoding.UTF8);
            sw.AutoFlush = true;
            client.NoDelay = true;
            s.ReadTimeout = 1000;
            sw.WriteLine("#connection");
            string ServerName = sr.ReadLine();

            return true;
        }
        public static string[] listFiles()
        {
            sw.WriteLine("#file");
            sw.WriteLine("#root");
            string input = input = sr.ReadLine(); ;
            List<string> files = new List<string>();

            while (input != "#end")
            {
                files.Add(input);
                input = sr.ReadLine();
            }
            return files.ToArray();
        }
        public static MemoryStream recieveFile(string File)
        {
            sw.WriteLine("#fileDownload");
            sw.WriteLine(File);



            long fileSize = long.Parse(sr.ReadLine());// your file size that you are going to receive it.
            MemoryStream inMemoryCopy = new MemoryStream();
            int count = 0;
            long sum = 0;   //sum here is the total of received bytes.
            int dataSize = int.Parse(sr.ReadLine());
            byte[] data = new byte[dataSize];  //8Kb buffer .. you might use a smaller size also.
            while (sum < fileSize)
            {
                string dataLenghtString = sr.ReadLine();
                int dataLenght = int.Parse(dataLenghtString);

                count = s.Read(data, 0, dataLenght);
                inMemoryCopy.Write(data, 0, count);
                sum = count + sum;

                bool egal = false;
                if (count == dataLenght) { egal = true; }

                System.Diagnostics.Debug.WriteLine("dataLenght: " + dataLenght + " sum : " + sum + " count : " + count + " file size : " + fileSize + " egal : " + egal);

                string fileTransfertended = sr.ReadLine();
                if (fileTransfertended == "true") { break; }

            }

            string end = sr.ReadLine();
            if (end == "#endFileDownload") { return inMemoryCopy; }
            else { return null; }

        }
        public static bool sendFile(string filePath, MemoryStream fileStream)
        {
            fileStream.Position = 0;
            System.Diagnostics.Debug.WriteLine("Can read : " + fileStream.CanRead.ToString());
            sw.WriteLine("#fileUpload");
            sw.WriteLine(filePath);
            //server is ready ?
            sr.ReadLine();

            // file sizer 
            long fileSize = fileStream.Length;
            sw.WriteLine(fileSize);

            long sum = 0;
            int count = 0;

            // buffer
            int bufferSize = (int)fileStream.Length;
            byte[] data = new byte[bufferSize];
            sw.WriteLine(bufferSize);

            while (sum < fileSize)
            {

                count = fileStream.Read(data, 0, data.Length);
                sw.WriteLine(count);

                s.Write(data, 0, count);
                sum = sum + count;

                if (sum < fileSize)
                {
                    sw.WriteLine("false");
                }
                else
                {
                    sw.WriteLine("true");
                    System.Diagnostics.Debug.WriteLine("transfert ended");
                    break;
                }
                System.Diagnostics.Debug.WriteLine("sum : " + sum + " count : " + count + " dataLengt : " + data.Length + " fileSize : " + fileSize);
                System.Diagnostics.Debug.WriteLine("spin");

            }
            //end of file transfert
            if (sr.ReadLine() == "#endFileDownload") { System.Diagnostics.Debug.WriteLine("ok, server response disconnection"); }
            sw.WriteLine("#endFileDownload");
            return true;
        }
        public static bool disconnect()
        {
            sw.WriteLine("#disconnect");
            string disconnection = sr.ReadLine();
            if (disconnection == "#disconnection_ok")
            {
                client.Close();
                return true;
            }
            else
            { return false; }

        }
    }

}
