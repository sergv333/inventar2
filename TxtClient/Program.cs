using System;
using System.Net.Sockets;
using System.IO;

namespace TxtClient
{
    class Program
    {
        const int PORT = 5006;
        const string ADDRESS = "10.68.27.160";
        static void Main(string[] args)
        { //Console.WriteLine("Получен ответ: " + ADDRESS);
            TcpClient client = null;
            try
            {
               
                Console.Write("Введите сообщение: ");
                string message = Console.ReadLine();
                client = new TcpClient(ADDRESS, PORT);
                NetworkStream stream = client.GetStream();

                // отправляем сообщение
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(message);
                writer.Flush();

                // BinaryReader reader = new BinaryReader(new BufferedStream(stream));
                StreamReader reader = new StreamReader(stream);
                message = reader.ReadLine();
                Console.WriteLine("Получен ответ: " + message);

                reader.Close();
                writer.Close();
                stream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //finally
            //{
            //    if (client != null)
            //        client.Close();
            //}
        }
    }
}
