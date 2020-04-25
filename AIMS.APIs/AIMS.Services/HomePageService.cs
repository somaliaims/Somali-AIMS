using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface IHomePageService
    {
        /// <summary>
        /// Gets home page settings
        /// </summary>
        /// <returns></returns>
        HomePageModel GetSettings();

        /// <summary>
        /// Sets home page settings
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse SetSettings(HomePageModel model);

        /// <summary>
        /// Sets favicon for AIMS
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        ActionResponse SetFavicon(string filePath);

        /// <summary>
        /// Writes the image to disk
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<string> WriteFile(IFormFile file);
    }

    public class HomePageService : IHomePageService
    {
        IHostingEnvironment hostingEnvironment;
        IMapper mapper;
        AIMSDbContext context;
        string imagesDir = "";

        public HomePageService(IHostingEnvironment _hostingEnvironment, AIMSDbContext cntxt, IMapper mappr)
        {
            context = cntxt;
            mapper = mappr;
            hostingEnvironment = _hostingEnvironment;
            imagesDir = Path.Combine(hostingEnvironment.WebRootPath, "Images");
            Directory.CreateDirectory(imagesDir);
            FileIOPermission fp = new FileIOPermission(FileIOPermissionAccess.Write, imagesDir);
            try
            {
                fp.Demand();
            }
            catch (Exception)
            {
            }
        }

        public HomePageModel GetSettings()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var homePageSettings = unitWork.HomePageRepository.GetOne(h => h.Id != 0);
                if (homePageSettings != null)
                {
                    return mapper.Map<HomePageModel>(homePageSettings);
                }
                return (new HomePageModel());
            }
        }

        public ActionResponse SetSettings(HomePageModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var settings = unitWork.HomePageRepository.GetOne(h => h.Id != 0);
                    if (settings == null)
                    {
                        unitWork.HomePageRepository.Insert(new EFHomePageSettings()
                        {
                            AIMSTitle = model.AIMSTitle,
                            AIMSTitleBarText = model.AIMSTitleBarText,
                            IntroductionHeading = model.IntroductionHeading,
                            IntroductionText = model.IntroductionText
                        });
                    }
                    else
                    {
                        settings.AIMSTitle = model.AIMSTitle;
                        settings.AIMSTitleBarText = model.AIMSTitleBarText;
                        settings.IntroductionHeading = model.IntroductionHeading;
                        settings.IntroductionText = model.IntroductionText;
                        unitWork.HomePageRepository.Update(settings);
                    }
                    unitWork.Save();
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse SetFavicon(string filePath)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var settings = unitWork.HomePageRepository.GetOne(h => h.Id != 0);
                    if (settings == null)
                    {
                        unitWork.HomePageRepository.Insert(new EFHomePageSettings()
                        {
                            AIMSTitle = "AIMS",
                            AIMSTitleBarText = "AIMS",
                            IntroductionHeading = "Introduction",
                            IntroductionText = "Welcome to AIMS",
                            FaviconPath = filePath
                        });
                    }
                    else
                    {
                        settings.FaviconPath = filePath;
                        unitWork.HomePageRepository.Update(settings);
                    }
                    unitWork.Save();
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public async Task<string> WriteFile(IFormFile file)
        {
            string fileName;
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                fileName = Guid.NewGuid().ToString() + extension; //Create a new Name for the file due to security reasons.
                var path = Path.Combine(Directory.GetCurrentDirectory(), imagesDir, fileName);

                using (var bits = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return fileName;
        }
    }
}
