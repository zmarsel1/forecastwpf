using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ForecastConnection
{
    /// <summary>
    ///  Класс <c>FCommand</c> предназначен для передачи комманды на Сервер Прогнозирования
    /// </summary>
    public class FCommand
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="cmd">Комманда для выполнения на сервере</param>
        public FCommand(string cmd)
        {
            CommandText = cmd;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="cmd">Комманда для выполнения на сервере</param>
        /// <param name="connection">Строка соединения с сревером</param>
        public FCommand(string cmd, FConnection connection) : this(cmd)
        {
            Connection = connection;
        }
        /// <summary>
        /// Поле, для хранения соединения с сервером отчётов
        /// </summary>
        public FConnection Connection { get; set; }
        /// <summary>
        /// Поле для хранения комманды
        /// </summary>
        public string CommandText { get; set; }
        /// <summary>
        /// Метод выполняет комманду
        /// </summary>
        /// <returns>Возвращает результат комманды в виде <c>String</c></returns>
        public string Execute()
        {
            try
            {
                NetworkStream clientStream = Connection.Client.GetStream();

                
                UTF8Encoding encoder = new UTF8Encoding();
                string message = CommandText + "\n";
                byte[] buffer = encoder.GetBytes(message);
                clientStream.Write(buffer, 0, buffer.Length);

                buffer = new byte[1024];
                int bytes = clientStream.Read(buffer, 0, buffer.Length);
                string response = encoder.GetString(buffer, 0, bytes);
                return response;
            }
            catch (Exception e)
            {
                throw new FException("не удалось выполнить команду.", e);
            }
        }
    }
}
