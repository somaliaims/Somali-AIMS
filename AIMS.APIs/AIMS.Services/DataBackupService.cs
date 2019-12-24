using AIMS.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;
using System.Security.Permissions;

namespace AIMS.Services
{
    public interface IDataBackupService
    {
        /// <summary>
        /// Back ups the database in its current form
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        ActionResponse BackupData(string connString);

        /// <summary>
        /// Backup the database from the provided backed up data
        /// </summary>
        /// <param name="backupFile"></param>
        /// <returns></returns>
        Task<ActionResponse> RestoreDatabase(string backupFile, string connString);

        /// <summary>
        /// Gets list of backup files
        /// </summary>
        /// <returns></returns>
        IEnumerable<BackupFiles> GetBackupFiles();

        /// <summary>
        /// Gets full path of the data backup directory
        /// </summary>
        /// <returns></returns>
        string GetDataBackupDirectory();

    }

    public class DataBackupService : IDataBackupService
    {
        IHostingEnvironment hostingEnvironment;
        string backupDir = "";

        public DataBackupService(IHostingEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
            backupDir = hostingEnvironment.WebRootPath + "\\DataBackups\\";
            Directory.CreateDirectory(backupDir);
            FileIOPermission fp = new FileIOPermission(FileIOPermissionAccess.Write, backupDir);
            try
            {
                fp.Demand();
            }
            catch(Exception)
            {
            }
        }

        public string GetDataBackupDirectory()
        {
            return backupDir;
        }

        public ActionResponse BackupData(string connString)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                string[] saDatabases = new string[] { "AIMSDb" };
                using (SqlConnection sqlConnection = new SqlConnection(connString))
                {
                    sqlConnection.Open();
                    foreach (string dbName in saDatabases)
                    {
                        string backupFileNameWithoutExt = String.Format("{0}\\{1}_{2:yyyy-MM-dd-hh-mm-ss-tt}", backupDir, dbName, DateTime.Now);
                        string backupFileNameWithExt = String.Format("{0}.bak", backupFileNameWithoutExt);
                        string zipFileName = String.Format("{0}.zip", backupFileNameWithoutExt);

                        string cmdText = string.Format("BACKUP DATABASE {0}\r\nTO DISK = '{1}'", dbName, backupFileNameWithExt);

                        using (SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection))
                        {
                            sqlCommand.CommandTimeout = 0;
                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public IEnumerable<BackupFiles> GetBackupFiles()
        {
            List<BackupFiles> filesList = new List<BackupFiles>();
            try
            {
                string[] files = Directory.GetFiles(backupDir);
                int index = 1;
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    filesList.Add(new BackupFiles()
                    {
                        Id = index,
                        BackupFileName = fi.Name,
                        TakenOn = fi.CreationTime
                    });
                    ++index;
                }

                if (filesList.Count > 1)
                {
                    filesList = (from f in filesList
                                 orderby f.TakenOn descending
                                 select f).ToList();
                }
            }
            catch(Exception ex)
            {
                string message = ex.Message;
            }
            return filesList;
        }

        public async Task<ActionResponse> RestoreDatabase(string backupFile, string connString)
        {
            ActionResponse response = new ActionResponse();
            try
            {
                backupFile = backupDir + backupFile;
                using (var sqlConnection = new SqlConnection(connString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlConnection.Open();
                        sqlConnection.ChangeDatabase("master");
                        sqlCommand.Connection = sqlConnection;
                        string cmdText = "ALTER DATABASE[AIMSDb] SET Single_User WITH Rollback Immediate";
                        sqlCommand.CommandText = cmdText;
                        sqlCommand.ExecuteNonQuery();

                        cmdText = "RESTORE DATABASE AIMSDb FROM DISK = '" + backupFile + "'" +
                                    "WITH REPLACE";
                        sqlCommand.CommandText = cmdText;
                        sqlCommand.ExecuteNonQuery();

                        cmdText = "RESTORE DATABASE AIMSDb FROM DISK = '" + backupFile + "'" +
                                    "WITH REPLACE";
                        sqlCommand.CommandText = cmdText;
                        sqlCommand.ExecuteNonQuery();

                        cmdText = "ALTER DATABASE[AIMSDb] SET Multi_User";
                        sqlCommand.CommandText = cmdText;
                        sqlCommand.ExecuteNonQuery();

                        sqlConnection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return await Task<ActionResponse>.Run(() => response);
        }
    }
}
