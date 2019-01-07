using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ForecastConnection
{
    /// <summary>
    /// <c>FConnection</c> класс для установления свзяи с Сервером Прогнозов
    /// </summary>
    public class FConnection
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public FConnection()
        {
            Client = new TcpClient();
            IP = IPAddress.Parse("127.0.0.1");
            Port = 10010;
            Timeout = 30000; // 3 секунды
        }
        /// <summary>
        /// Конструктор принимает в качестве параметра <paramref name="connectionstring"/>
        /// </summary>
        /// <param name="connectionstring">строка соединения с сервером прогнозирования</param>
        public FConnection(string connectionstring) : this()
        {
            string ip = connectionstring.Split(',')[0];
            string port = connectionstring.Split(',')[1];

            try
            {
                IP = IPAddress.Parse(ip);
                Port = int.Parse(port);
            }
            catch
            {
                IP = IPAddress.Parse("127.0.0.1");
                Port = 10010;
            }
        }
        /// <summary>
        /// Поле хранит IP  сервера
        /// </summary>
        public IPAddress IP { get; set; }
        /// <summary>
        /// Поле хранит порт сервера
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Поле хранит установленное соединение  сервером прогнозов
        /// </summary>
        public TcpClient Client {get; private set;}
        /// <summary>
        /// Поле хранит время ожидания до завершения запроса
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// Метод открывает соединение с сервером
        /// </summary>
        public void Open()
        {
            try
            {
                IPEndPoint server = new IPEndPoint(IP, Port);
                Client.Connect(server);
                Client.ReceiveTimeout = Timeout;
                Client.SendTimeout = Timeout;
            }
            catch (Exception e)
            {
                throw new FException("не удалось установить соединение.", e);
            }

        }
        /// <summary>
        /// Метод закрывает соединение с сервером
        /// </summary>
        public void Close()
        {
            try
            {
                Client.GetStream().Close();
                Client.Close();
            }
            catch (Exception e)
            {
                throw new FException("не удалось установить соединение.", e);
            }
            
        }
    }
}
