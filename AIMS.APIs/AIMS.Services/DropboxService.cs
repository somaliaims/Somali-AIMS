using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface IDropboxService
    {

    }

    public class DropboxService
    {
        string token = "";
        string backupFolder = "DBBackups";

        public DropboxService(string tkn)
        {
            token = tkn;
        }

        public async Task<string> GetAccountName()
        {
            using (var dbx = new DropboxClient(token))
            {
                var full = await dbx.Users.GetCurrentAccountAsync();
                return ("Name: " + full.Name.DisplayName + " Email: " + full.Email);
            }
        }

        public async Task<string> CreateFolder()
        {
            using (var dbx = new DropboxClient(token))
            {
                var folderArg = new CreateFolderArg("/" + backupFolder);
                var result = await dbx.Files.CreateFolderV2Async(folderArg);
                return result.Metadata.Name;
            }
        }

        public async Task<string> UploadFile(string fileToUpload, string fileName)
        {
            using (var dbx = new DropboxClient(token))
            {
                var reader = new StreamReader(fileToUpload);
                var content = reader.ReadToEnd();
                using (var stream = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(content)))
                {
                    var response = await dbx.Files.UploadAsync("/" + backupFolder + "/" + fileName, WriteMode.Overwrite.Instance, body: stream);
                    return fileName;
                }
            }
        }

        public async Task<string> DownloadFile(string backupPath, string fileName)
        {
            using (var dbx = new DropboxClient(token))
            {
                using (var response = await dbx.Files.DownloadAsync("/" + backupFolder + "/" + fileName))
                {
                    using (var fileStream = File.Create(backupPath + fileName))
                    {
                        (await response.GetContentAsStreamAsync()).CopyTo(fileStream);
                    }
                }
            }
            return fileName;
        }

        public async Task<List<string>> GetFilesList()
        {
            List<string> fileNames = new List<string>();
            using (var dbx = new DropboxClient(token))
            {
                var list = await dbx.Files.ListFolderAsync("/" + backupFolder);
                foreach (var item in list.Entries.Where(i => i.IsFile))
                {
                    var file = item.AsFile;
                    fileNames.Add(item.Name);
                }
                return fileNames;
            }
        }
    }
}
