using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using System.Globalization;

public static class EncryptionUtility
{
    private static readonly string EncryptionKey = "!"; // ��� ���� �ؾ� ��

    public static string Encrypt(string plainText)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(EncryptionKey);
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = new byte[16];
            using (var encryptor = aes.CreateEncryptor())
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                return Convert.ToBase64String(encryptedBytes, Base64FormattingOptions.None);
            }
        }
    }

    public static string Decrypt(string encryptedText)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(EncryptionKey);
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = new byte[16];
            using (var decryptor = aes.CreateDecryptor())
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Encoding.UTF8.GetString(plainBytes);
            }
        }
    }

    public static string ToJsonInvariant<T>(T obj)
    {
        return JsonUtility.ToJson(obj);
    }

    public static T FromJsonInvariant<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }
}