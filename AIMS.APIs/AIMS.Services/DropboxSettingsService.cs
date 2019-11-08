using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AIMS.Services
{
    public interface IDropboxSettingsService
    {
        /// <summary>
        /// Updates token for dropbox
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse UpdateToken(DropboxSettingsModel model);

        /// <summary>
        /// Gets token
        /// </summary>
        /// <returns></returns>
        string GetToken();
    }

    public class DropboxSettingsService
    {
        AIMSDbContext context;
        string keyValue = "E546C8DF278CD5931069B522E695D4F2";

        public DropboxSettingsService(AIMSDbContext cntxt)
        {
            context = cntxt;
        }

        public ActionResponse UpdateToken(DropboxSettingsModel model)
        {
            ActionResponse response = new ActionResponse();
            using (var unitWork = new UnitOfWork(context))
            {
                string encryptedToken = Encrypt(model.Token);
                var dropBoxSettings = unitWork.DropboxSettingsRepository.GetOne(t => t.Token.Length >= 0);
                if (dropBoxSettings == null)
                {
                    unitWork.DropboxSettingsRepository.Insert(new EFDropboxSettings()
                    {
                        Token = encryptedToken
                    });
                }
                else
                {
                    dropBoxSettings.Token = encryptedToken;
                    unitWork.DropboxSettingsRepository.Update(dropBoxSettings);
                }
                unitWork.Save();
            }
            return response;
        }

        public string GetToken()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                string token = string.Empty;
                var dropBoxSettings = unitWork.DropboxSettingsRepository.GetOne(t => t.Token.Length >= 0);
                if (dropBoxSettings != null)
                {
                    token = Decrypt(dropBoxSettings.Token);
                }
                return token;
            }
        }

        private string Encrypt(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            try
            {
                var key = Encoding.UTF8.GetBytes(keyValue);

                using (var aesAlg = Aes.Create())
                {
                    using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                    {
                        using (var msEncrypt = new MemoryStream())
                        {
                            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(value);
                            }

                            var iv = aesAlg.IV;

                            var decryptedContent = msEncrypt.ToArray();

                            var result = new byte[iv.Length + decryptedContent.Length];

                            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                            var str = Convert.ToBase64String(result);
                            var fullCipher = Convert.FromBase64String(str);
                            return str;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        private string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            try
            {
                value = value.Replace(" ", "+");
                var fullCipher = Convert.FromBase64String(value);

                var iv = new byte[16];
                var cipher = new byte[fullCipher.Length - iv.Length];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);
                var key = Encoding.UTF8.GetBytes(keyValue);

                using (var aesAlg = Aes.Create())
                {
                    using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                    {
                        string result;
                        using (var msDecrypt = new MemoryStream(cipher))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    result = srDecrypt.ReadToEnd();
                                }
                            }
                        }
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

    }
}
