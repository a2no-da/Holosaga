/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.IO; 
using System;

public static class EncryptionUtility
{
    private static readonly byte[] key = Encoding.UTF8.GetBytes("1597534568521532"); 
    private static readonly byte[] iv = Encoding.UTF8.GetBytes("1597534568521532"); 

    public static string EncryptString(string plainText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public static string DecryptString(string cipherText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}*/