using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Core
{
    /// <summary>
    /// класс шифрует и дешифрует заданное значение
    /// </summary>
    static class Cryptography
    {
        /// <summary>
        /// Шифрует сообщение заданным ключём
        /// </summary>
        /// <param name="key">Ключ шифрования</param>
        /// <param name="source">Шифруемое сообщение</param>
        /// <returns>Возвращает зашифрованное сообщение</returns>
        /// <example>
        /// <code>
        /// string source ="testmessage"
        /// srring key = "key"
        /// string encrypted = Cryptography.Encrypt(key, source);
        /// </code>
        /// </example>
        public static string Encrypt(string key, string source)
        {
            byte[] pwdhash, buff;
            string encrypted = "";
            MD5CryptoServiceProvider provider;
            provider = new MD5CryptoServiceProvider();
            pwdhash = provider.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            provider = null;
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = pwdhash;
            des.Mode = CipherMode.ECB;
            buff = UTF8Encoding.UTF8.GetBytes(source);
            encrypted = Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length));
            return encrypted;
        }
        /// <summary>
        /// Расшифровывает сообщение заданным ключём
        /// </summary>
        /// <param name="key">Ключ шифрования</param>
        /// <param name="source">Сообщение для расшифровки</param>
        /// <returns>Возвращает расшифрованное сообщение</returns>
        /// <example>
        /// <code>
        /// string source ="testmessage"
        /// srring key = "key"
        /// string encrypted = Cryptography.Encrypt(key, source);
        /// string decrypted = Cryptography.Encrypt(key, encrypted);
        /// </code>
        /// </example>
        public static string Decrypt(string key, string source)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] pwdhash = provider.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            provider = null;
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = pwdhash;
            des.Mode = CipherMode.ECB;
            try
            {
                byte[] buff = Convert.FromBase64String(source);
                string decrypted = UTF8Encoding.UTF8.GetString(des.CreateDecryptor().TransformFinalBlock(buff, 0, buff.Length));
                return decrypted;
            }
            catch
            {
            }
            return string.Empty;
        }
    }
}
