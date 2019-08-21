using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

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
    }

    public class HomePageService : IHomePageService
    {
        AIMSDbContext context;
        IMapper mapper;

        public HomePageService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
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
                            IntroductionHeading = model.IntroductionHeading,
                            IntroductionText = model.IntroductionText
                        });
                    }
                    else
                    {
                        settings.AIMSTitle = model.AIMSTitle;
                        settings.IntroductionHeading = model.IntroductionHeading;
                        settings.IntroductionText = model.IntroductionText;
                        unitWork.HomePageRepository.Update(settings);
                    }
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                unitWork.Save();
                return response;
            }
        }
    }
}
