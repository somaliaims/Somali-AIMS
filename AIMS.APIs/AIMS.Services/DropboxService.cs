using AIMS.Models;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface IDropboxService
    {
        /*
        /// <summary>
        /// Get identity of the account
        /// </summary>
        /// <returns></returns>
        Task<ActionResponse> GetAccountName();

        /// <summary>
        /// Creates a new folder under dropbox
        /// </summary>
        /// <returns></returns>
        Task<ActionResponse> CreateFolder();

        /// <summary>
        /// Upload a file to dropbox backup directory
        /// </summary>
        /// <param name="fileToUpload"></param>
        /// <returns></returns>
        Task<ActionResponse> UploadFile(string fileToUpload);

        /// <summary>
        /// Downloads the file with the provided name and directory from dropbox
        /// </summary>
        /// <param name="backupDirectory"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<ActionResponse> DownloadFile(string fileName);

        /// <summary>
        /// Sets token for DB
        /// </summary>
        /// <param name="tkn"></param>
        /// <returns></returns>
        void SetToken(string tkn);*/
    }

    public class DropboxService : IDropboxService
    {
        /*IWebHostEnvironment hostingEnvironment;
        string token = "40mPFdltTIAAAAAAAAAADx0r3r5TlXdfNF6icOQciANhFYFJiDMRPwsneZXDK_sG";
        string dropboxFolder = "DataBackups";
        string backupDirectory = "";

        public DropboxService(IWebHostEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
            backupDirectory = hostingEnvironment.WebRootPath + "/" + dropboxFolder + "/";
        }

        public void SetToken(string tkn)
        {
            token = tkn;
        }

        public async Task<ActionResponse> GetAccountName()
        {
            ActionResponse response = new ActionResponse();
            try 
            {
                using (var dbx = new DropboxClient(token))
                {
                    var full = await dbx.Users.GetCurrentAccountAsync();
                    response.Message = "Name: " + full.Name.DisplayName + " Email: " + full.Email;
                }
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ActionResponse> CreateFolder()
        {
            ActionResponse response = new ActionResponse();
            try
            {
                using (var dbx = new DropboxClient(token))
                {
                    var folderArg = new CreateFolderArg("/" + dropboxFolder);
                    var result = await dbx.Files.CreateFolderV2Async(folderArg);
                    response.Message = result.Metadata.Name;
                }
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ActionResponse> UploadFile(string fileToUpload)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                using (var dbx = new DropboxClient(token))
                {
                    var reader = new StreamReader(fileToUpload);
                    var content = reader.ReadToEnd();
                    using (var stream = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(content)))
                    {
                        var result = await dbx.Files.UploadAsync("/" + dropboxFolder + "/" + fileToUpload, WriteMode.Overwrite.Instance, body: stream);
                    }
                }
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ActionResponse> DownloadFile(string fileName)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                using (var dbx = new DropboxClient(token))
                {
                    using (var result = await dbx.Files.DownloadAsync("/" + dropboxFolder + "/" + fileName))
                    {
                        using (var fileStream = File.Create(backupDirectory + fileName))
                        {
                            (await result.GetContentAsStreamAsync()).CopyTo(fileStream);
                            string backupFileName = Path.GetFileName(fileName);
                            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                            ZipFile.ExtractToDirectory(backupDirectory + "\\" + fileName, backupDirectory);
                            response.Message = backupDirectory + "\\" + fileNameWithoutExt + ".bak"; 
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ActionResponse> GetFilesList()
        {
            ActionResponse response = new ActionResponse();
            List<string> fileNames = new List<string>();
            try
            {
                using (var dbx = new DropboxClient(token))
                {
                    var list = await dbx.Files.ListFolderAsync("/" + dropboxFolder);
                    foreach (var item in list.Entries.Where(i => i.IsFile))
                    {
                        var file = item.AsFile;
                        fileNames.Add(item.Name);
                    }
                }
                response.Message = JsonConvert.SerializeObject(fileNames);
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }*/
    }
}
