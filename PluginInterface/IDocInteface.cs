using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;

namespace PluginInterface
{
    /// <summary>
    /// <c>IDocPlugin</c> интерфейс для создания, редактирования и удаления документов
    /// </summary>
    public interface IDocPlugin
    {
        /// <summary>
        /// Метод создаёт документ в базе данных на основе родительского документа.
        /// </summary>
        /// <param name="connection">Строка соединения с БД</param>
        /// <param name="type">Тип документа</param>
        /// <param name="obj">Узел учёта</param>
        /// <param name="date">Дата Документа</param>
        /// <param name="basedoc">Номер Родительского документа</param>
        /// <returns>Возвращает номер документа</returns>
        int CreateDocument(string connection, string type, string obj, DateTime date, int basedoc);
        /// <summary>
        /// Метод создаёт документ в базе данных на основе шаблона.
        /// </summary>
        /// <param name="connection">Строка соединения с БД</param>
        /// <param name="type">Тип документа</param>
        /// <param name="obj">Узел учёта</param>
        /// <param name="date">Дата Документа</param>
        /// <returns>Возвращает номер документа</returns>
        int CreateDocument(string connection, string type, string obj, DateTime date);
        /// <summary>
        /// Метод вызывает окно редактирования для заданного номера документа.
        /// </summary>
        /// <param name="connection">Строка соединения с БД</param>
        /// <param name="docnum">Номер редактируемого документа</param>
        /// <returns>Возвращает окно для редактирования</returns>
        Window EditDocument(string connection, int docnum);
        /// <summary>
        /// Удаляет документ по заданному номеру.
        /// </summary>
        /// <param name="connection">Строка соединения с БД</param>
        /// <param name="docnum">Номер удаляемого документа</param>
        void DeleteDocument(string connection, int docnum);
        /// <summary>
        /// Метод отбирает документы заданного типа и узла учёта в заданном временном интервале.
        /// </summary>
        /// <param name="connection">Строка соединения с БД</param>
        /// <param name="type">Тип документа</param>
        /// <param name="obj">Узел учёта</param>
        /// <param name="start">Начальная дата поиска</param>
        /// <param name="end">Конечная дата поиска</param>
        /// <returns>Возвращает выборку в формате <c>DataTable</c></returns>
        DataTable SelectDocuments(string connection,string type, string obj, DateTime start, DateTime end);
    }
}
