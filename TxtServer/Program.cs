using System;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace TxtServer
{
    class Program
    {
        const int PORT = 5006; // порт для прослушивания подключений
        static TcpListener listener;
        static void Main(string[] args)
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("10.68.24.141"), PORT);
                listener.Start();
                Console.WriteLine("Ожидание подключений...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();

                    StreamReader reader = new StreamReader(stream);
                    // считываем строку из потока
                    string message = reader.ReadLine();
                    Console.WriteLine("Получено: " + message);

                    // отправляем ответ
                    StreamWriter writer = new StreamWriter(stream);
                    message = message.ToUpper();
                    Console.WriteLine("Отправлено: " + message);
                    writer.WriteLine(message);

                    writer.Close();
                    reader.Close();
                    stream.Close();
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
    }
}
