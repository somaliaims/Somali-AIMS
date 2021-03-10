using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Permissions;
using System.Text;

namespace AIMS.Services
{
    public interface ISponsorLogoService
    {
        /// <summary>
        /// Sets directory path and creats the directory
        /// </summary>
        void SetLogoDirectoryPath(string path);

        /// <summary>
        /// Gets list of logos
        /// </summary>
        /// <returns></returns>
        SponsorsLogosListView GetAll();

        /// <summary>
        /// Saves logo
        /// </summary>
        /// <param name="file"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        ActionResponse SaveLogo(IFormFile file, string title, string fileName);

        /// <summary>
        /// Deletes a logo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Delete(int id);

        /// <summary>
        /// Gets logo directory path
        /// </summary>
        /// <returns></returns>
        string GetLogoDirectoryPath();
    }

    public class SponsorLogoService : ISponsorLogoService
    {
        readonly string LOGO_DIR_PATH = "Logos";
        string sWebRootFolder = "";
        AIMSDbContext context;
        IMapper mapper;

        public SponsorLogoService(AIMSDbContext dbContext, IMapper autoMapper)
        {
            context = dbContext;
            mapper = autoMapper;
        }

        public void SetLogoDirectoryPath(string webrootPath)
        {
            sWebRootFolder = Path.Combine(webrootPath, LOGO_DIR_PATH);
            Directory.CreateDirectory(sWebRootFolder);
            FileIOPermission fp = new FileIOPermission(FileIOPermissionAccess.Write, sWebRootFolder);
            try
            {
                fp.Demand();
            }
            catch (Exception)
            {
            }
        }

        public SponsorsLogosListView GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                SponsorsLogosListView logos = new SponsorsLogosListView() { HttpBaseUrl = sWebRootFolder };
                logos.SponsorLogos = mapper.Map<List<SponsorsLogosView>>(unitWork.SponsorLogosRepository.GetManyQueryable(l => l.Id != 0));
                return logos;
            }
        }

        public ActionResponse SaveLogo(IFormFile file, string title, string fileName)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

                try
                {
                    if (file.Length > 0)
                    {
                        fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fullPath = Path.Combine(sWebRootFolder, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        unitWork.SponsorLogosRepository.Insert(new EFSponsorLogos()
                        {
                            Title = title,
                            LogoPath = fileName
                        });
                        unitWork.Save();
                    }
                }
                catch(Exception ex)
                {
                    response.Message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        response.Message = ex.InnerException.Message;
                    }
                }
                return response;
            }
        }

        public ActionResponse Delete(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    IMessageHelper mHelper;
                    var sponsor = unitWork.SponsorLogosRepository.GetByID(id);
                    if (sponsor == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Sponsor");
                        response.Success = false;
                        return response;
                    }

                    string fileToDelete = Path.Combine(sWebRootFolder, sponsor.LogoPath);
                    unitWork.SponsorLogosRepository.Delete(sponsor);
                    unitWork.Save();

                    File.Delete(fileToDelete);
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        response.Message = ex.InnerException.Message;
                    }
                }
                return response;
            }
        }

        public string GetLogoDirectoryPath()
        {
            return (sWebRootFolder);
        }

    }
}
