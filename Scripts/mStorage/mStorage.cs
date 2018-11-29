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

        public static readonly string DBPath = 
            Path.Combine(Path.GetFullPath(Application.persistentDataPath), "mFramework", DbName);

        private static readonly Dictionary<ulong, byte[]> _data;

        static mStorage()
        {
            _data = new Dictionary<ulong, byte[]>();

            Load(StoragePassword);
            Application.quitting += ApplicationOnQuitting;
        }

        private static void ApplicationOnQuitting()
        {
            Save(StoragePassword);
        }

        /// <summary>
        /// Clear loaded data from memory
        /// </summary>
        public static void Clear()
        {
            _data.Clear();
        }

        public static void AddData(ulong key, byte[] data)
        {
            if (_data.ContainsKey(key))
                _data[key] = data;
            else 
                _data.Add(key, data);
        }

        public static bool GetData(ulong key, out byte[] data)
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

        private static bool Save(string storagePassword)
        {
            Debug.Log($"[mStorage] Save file ({DBPath})");
            var directory = Path.GetDirectoryName(DBPath);

            if (!Directory.Exists(directory))
            {
                try
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Directory.CreateDirectory(directory);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[mStorage] Create directory ({DBPath}) error ({e})");
                    return false;
                }
            }

            Stream stream;
            try
            {
                stream = File.Open(DBPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                stream.SetLength(0);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[mStorage] Save file ({DBPath}) error ({e})");
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
            var keyBytes = new Rfc2898DeriveBytes(storagePassword, 
                Encoding.UTF8.GetBytes(SaltKey)).GetBytes(KeyLength / 8);
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, 
                Encoding.UTF8.GetBytes(VIKey));

            var dataStart = stream.Position;
            var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);

            foreach (var pair in _data)
            {
                cryptoStream.WriteStruct(new DataFileItem()
                {
                    Key = pair.Key,
                    DataSize = pair.Value.Length, 
                });
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

        private static void Load(string storagePassword)
        {
            Stream stream;
            if (File.Exists(DBPath))
            {
                Debug.Log($"[mStorage] Load storage, path={DBPath}");
                try
                {
                    stream = File.Open(DBPath, FileMode.Open, FileAccess.ReadWrite);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[mStorage] Load file ({DBPath}) error ({e})");
                    return;
                }
            }
            else
            {
                Debug.LogWarning($"[mStorage] File not found ({DBPath})");
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
            var keyBytes = new Rfc2898DeriveBytes(storagePassword, 
                Encoding.UTF8.GetBytes(SaltKey)).GetBytes(KeyLength / 8);
            var decryptor = symmetricKey.CreateDecryptor(keyBytes, 
                Encoding.UTF8.GetBytes(VIKey));

            var cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
            var sizeOfFileItem = Marshal.SizeOf<DataFileItem>();

            using (var memoryStream = new MemoryStream())
            {
                var buffer = new byte[2048];
                int bytesRead;
                while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                    memoryStream.Write(buffer, 0, bytesRead);

                cryptoStream.Close();
                stream.Close();
                memoryStream.Seek(0, SeekOrigin.Begin);

                for (var i = 0; i < header.ItemsNum; i++)
                {
                    if (CheckEOF(memoryStream, sizeOfFileItem))
                        return;

                    var fileItem = memoryStream.ReadStruct<DataFileItem>();
                    if (CheckEOF(memoryStream, fileItem.DataSize))
                        return;

                    var data = new byte[fileItem.DataSize];
                    if (memoryStream.Read(data, 0, data.Length) == fileItem.DataSize)
                        _data.Add(fileItem.Key, data);
                    else 
                        break;

                    //Debug.Log($"Add data key={fileItem.Key} size={fileItem.DataSize}");
                }
            }

            Debug.Log("[mStorage] Successful loaded");
            Loaded = true;
        }
    }
}
