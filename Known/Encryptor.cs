﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace Known
{
    public sealed class Encryptor
    {
        public static string ToMd5(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            byte[] bytes;
            using (var md5 = MD5.Create())
            {
                var buffer = Encoding.UTF8.GetBytes(value);
                bytes = md5.ComputeHash(buffer);
            }

            var sb = new StringBuilder();
            foreach (var item in bytes)
            {
                sb.Append(item.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string DESEncrypt(string value, string password = "")
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var buffer = Encoding.UTF8.GetBytes(password);
            var key = new MD5CryptoServiceProvider().ComputeHash(buffer);
            var des = new TripleDESCryptoServiceProvider
            {
                Key = key,
                Mode = CipherMode.ECB
            };
            var datas = Encoding.UTF8.GetBytes(value);
            var bytes = des.CreateEncryptor()
                           .TransformFinalBlock(datas, 0, datas.Length);
            return Convert.ToBase64String(bytes);
        }

        public static string DESDecrypt(string value, string password = "")
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var buffer = Encoding.UTF8.GetBytes(password);
            var key = new MD5CryptoServiceProvider().ComputeHash(buffer);
            var des = new TripleDESCryptoServiceProvider
            {
                Key = key,
                Mode = CipherMode.ECB
            };
            var datas = Convert.FromBase64String(value);
            var bytes = des.CreateDecryptor()
                           .TransformFinalBlock(datas, 0, datas.Length);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
