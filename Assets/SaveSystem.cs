using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor.Overlays;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.dat");
    private static readonly string Key = "1234567890abcdef"; // 16文字がAESに適している（例: "1234567890abcdef"）

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        string encrypted = Encrypt(json, Key);
        File.WriteAllText(SavePath, encrypted);
    }

    public static SaveData Load()
    {
        if (!File.Exists(SavePath)) return null;

        string encrypted = File.ReadAllText(SavePath);
        string json = Decrypt(encrypted, Key);
        return JsonUtility.FromJson<SaveData>(json);
    }

    // AES暗号化
    private static string Encrypt(string plainText, string key)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = new byte[16]; // 初期化ベクタ（固定でも問題ないが、必要なら保存＆復元を）

        using var encryptor = aes.CreateEncryptor();
        byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
        return System.Convert.ToBase64String(encryptedBytes);
    }

    // AES復号
    private static string Decrypt(string encryptedText, string key)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = new byte[16];

        using var decryptor = aes.CreateDecryptor();
        byte[] encryptedBytes = System.Convert.FromBase64String(encryptedText);
        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
