// ReSharper disable NotAccessedVariable
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using mFramework.Core;
using UnityEngine;

namespace mFramework.Storage
{
    public static class mStorage
    {
        public static int KeyLength = 128;
        public static string SaltKey = "Daxuzx$q7D_3#67s";
        public static string VIKey = "LpD2BMF2Y$r33e_b";
        public static string StoragePassword = "CHANGE_PASSWORD";

        public static bool Loaded { get; private set; }
        public static DataFileVersionHeader? VersionHeader { get; private set; }

        public const int Version = 1;
        public const string DbName = "mStorage.bin";

        public static readonly string DbPath = 
            Path.Combine(Path.GetFullPath(Application.persistentDataPath), "mFramework", DbName);

        private static readonly Dictionary<string, byte[]> _data;

        static mStorage()
        {
            _data = new Dictionary<string, byte[]>();

            Load(StoragePassword);
            Application.quitting += ApplicationOnQuitting;
        }

        private static void ApplicationOnQuitting()
        {
            Save(StoragePassword);
        }

        public static void Clear()
        {
            _data.Clear();
        }

        public static bool RemoveKey(string key)
        {
            return _data.Remove(key);
        }

        public static bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public static void AddData(string key, byte[] data)
        {
            if (_data.ContainsKey(key))
                _data[key] = data;
            else 
                _data.Add(key, data);
        }

        public static bool GetData(string key, out byte[] data)
        {
            return _data.TryGetValue(key, out data);
        }

        private static bool CheckEOF(Stream stream, long sizeOf)
        {
            if (stream.Position + sizeOf <= stream.Length)
                return false;

            Debug.Log($"[mStorage] End of stream (pos={stream.Position} len={stream.Length})");
            return true;
        }

        private static bool CheckEOF<T>(Stream stream)
        {
            return CheckEOF(stream, Marshal.SizeOf<T>());
        }

        public static bool Save(string storagePassword)
        {
            Debug.Log($"[mStorage] Save file ({DbPath})");
            var directory = Path.GetDirectoryName(DbPath);

            if (!Directory.Exists(directory))
            {
                try
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Directory.CreateDirectory(directory);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[mStorage] Create directory ({DbPath}) error ({e})");
                    return false;
                }
            }

            Stream stream;
            try
            {
                stream = File.Open(DbPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                stream.SetLength(0);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[mStorage] Save file ({DbPath}) error ({e})");
                return false;
            }

            stream.WriteStruct(new DataFileVersionHeader
            {
                MagicArray = new[] { 'M', 'B', 'I', 'N' },
                Version = Version
            });
            stream.Seek(Marshal.SizeOf<DataFileHeader>(), SeekOrigin.Current);

            // encryption data
            var symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.Zeros
            };
            var storageKeyBytes = new Rfc2898DeriveBytes(storagePassword, 
                Encoding.UTF8.GetBytes(SaltKey)).GetBytes(KeyLength / 8);
            var encryptor = symmetricKey.CreateEncryptor(storageKeyBytes, 
                Encoding.UTF8.GetBytes(VIKey));

            var dataStart = stream.Position;
            var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);

            foreach (var pair in _data)
            {
                var keyBytes = Encoding.UTF8.GetBytes(pair.Key);
                cryptoStream.WriteStruct(new DataFileItem()
                {
                    KeySize = (byte) keyBytes.Length,
                    DataSize = pair.Value.Length, 
                });
                cryptoStream.Write(keyBytes, 0, keyBytes.Length);
                cryptoStream.Write(pair.Value, 0, pair.Value.Length);
            }
            cryptoStream.FlushFinalBlock();

            var encryptedSize = stream.Position - dataStart;
            stream.Seek(Marshal.SizeOf<DataFileVersionHeader>(), SeekOrigin.Begin);
            stream.WriteStruct(new DataFileHeader
            {
                ItemsNum = _data.Count,
                EncryptedSize = encryptedSize,
            });

            cryptoStream.Close();
            stream.Close();
            return true;
        }

        public static void Load(string storagePassword)
        {
            Stream stream;
            if (File.Exists(DbPath))
            {
                Debug.Log($"[mStorage] Load storage, path={DbPath}");
                try
                {
                    stream = File.Open(DbPath, FileMode.Open, FileAccess.ReadWrite);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[mStorage] Load file ({DbPath}) error ({e})");
                    return;
                }
            }
            else
            {
                Debug.LogWarning($"[mStorage] File not found ({DbPath})");
                return;
            }

            if (CheckEOF<DataFileVersionHeader>(stream))
                return;

            VersionHeader = stream.ReadStruct<DataFileVersionHeader>();
            if (VersionHeader.Value.Magic != "MBIN")
            {
                Debug.Log("Wrong signature");
                return;
            }

            Debug.Log($"[mStorage] Version={VersionHeader.Value.Version} ({VersionHeader.Value.Magic})");

            if (CheckEOF<DataFileHeader>(stream))
                return;

            var header = stream.ReadStruct<DataFileHeader>();
            Debug.Log($"[mStorage] ItemsNum={header.ItemsNum} EncryptedSize={header.EncryptedSize}");

            if (CheckEOF(stream, header.EncryptedSize))
                return;

            var symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None
            };
            var storageKeyBytes = new Rfc2898DeriveBytes(storagePassword, 
                Encoding.UTF8.GetBytes(SaltKey)).GetBytes(KeyLength / 8);
            var decryptor = symmetricKey.CreateDecryptor(storageKeyBytes, 
                Encoding.UTF8.GetBytes(VIKey));

            var cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
            using (var memoryStream = new MemoryStream())
            {
                var buffer = new byte[2048];
                int bytesRead;
                while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                    memoryStream.Write(buffer, 0, bytesRead);

                cryptoStream.Close();
                stream.Close();
                memoryStream.Seek(0, SeekOrigin.Begin);

                var sizeOfFileItem = Marshal.SizeOf<DataFileItem>();
                for (var i = 0; i < header.ItemsNum; i++)
                {
                    if (CheckEOF(memoryStream, sizeOfFileItem))
                        return;
                    
                    var fileItem = memoryStream.ReadStruct<DataFileItem>();
                    if (CheckEOF(memoryStream, fileItem.KeySize + fileItem.DataSize))
                        return;

                    var keyBytes = new byte[fileItem.KeySize];
                    if (memoryStream.Read(keyBytes, 0, keyBytes.Length) == fileItem.KeySize)
                    {
                        var key = Encoding.UTF8.GetString(keyBytes);
                        var data = new byte[fileItem.DataSize];

                        if (memoryStream.Read(data, 0, data.Length) == fileItem.DataSize)
                            _data.Add(key, data);
                        else 
                            break;

                        Debug.Log($"Add data key={key} size={fileItem.DataSize}");
                    }
                    else break;
                }
            }

            Debug.Log("[mStorage] Successful loaded");
            Loaded = true;
        }
    }
}
