using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AIMS.Services.Helpers;

namespace AIMS.Services
{
    public interface IDocumentLinkService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<DocumentLinkView> GetAll();

        /// <summary>
        /// Deletes a document for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Delete(int id);
    }

    public class DocumentLinkService : IDocumentLinkService
    {
        AIMSDbContext context;
        IMapper mapper;

        public DocumentLinkService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<DocumentLinkView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var links = unitWork.DocumentLinksRepository.GetAll();
                links = (from link in links
                         orderby link.DatePosted descending
                         select link);
                return mapper.Map<List<DocumentLinkView>>(links);
            }
        }

        public ActionResponse Delete(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var link = unitWork.DocumentLinksRepository.GetByID(id);
                if (link == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Link");
                    response.Success = false;
                    return response;
                }

                unitWork.DocumentLinksRepository.Delete(link);
                unitWork.Save();
                return response;
            }
        }
    }
}
