using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services
{
    public interface IEnvelopeService
    {
        /// <summary>
        /// Gets all envelopes
        /// </summary>
        /// <returns></returns>
        IEnumerable<EnvelopeView> GetAll();

        /// <summary>
        /// Get matching envelopes for the criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<EnvelopeView> GetMatching(string criteria);

        /// <summary>
        /// Gets envelope data for funder
        /// </summary>
        /// <param name="funderId"></param>
        /// <returns></returns>
        EnvelopeView GetFunderEnvelope(int funderId);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(EnvelopeModel envelope);

        /// <summary>
        /// Updates a envelope
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        ActionResponse Update(int id, EnvelopeModel envelope);

        /// <summary>
        /// Deletes a relevant funder row from envelope data
        /// </summary>
        /// <param name="funderId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        ActionResponse Delete(int funderId, int year);
    }

    public class EnvelopeService
    {
        AIMSDbContext context;
        IMapper mapper;

        public EnvelopeService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<EnvelopeView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var envelopes = unitWork.EnvelopeRepository.GetAll();
                return mapper.Map<List<EnvelopeView>>(envelopes);
            }
        }

        public EnvelopeView GetFunderEnvelope(int funderId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                EnvelopeView envelope = new EnvelopeView();
                List<EnvelopeBreakup> envelopeList = new List<EnvelopeBreakup>();
                List<ProjectMiniView> projects = new List<ProjectMiniView>();
                List<int> projectIds = new List<int>();
                var envelopes = unitWork.EnvelopeRepository.GetManyQueryable(e => e.FunderId == funderId);
                if (envelopes != null)
                {
                    foreach(var e in envelopes)
                    {
                        envelopeList.Add(new EnvelopeBreakup()
                        {
                            TotalAmount = e.TotalAmount,
                            Year = e.Year
                        });
                    }
                }
                var funderProjects = unitWork.ProjectFundersRepository.GetWithInclude(f => f.FunderId == funderId, new string[] { "Project" });
                foreach(var project in funderProjects)
                {
                    projects.Add(new ProjectMiniView()
                    {
                        Title = project.Project.Title,
                        Description = project.Project.Description
                    });
                }

                var envelopeSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => projectIds.Contains(p.ProjectId), new string[] { "Sector" });
                return envelope;
            }
        }


        public ActionResponse Add(EnvelopeModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var isEnvelopeCreated = unitWork.EnvelopeRepository.GetOne(e => e.FunderId == model.FunderId);
                    if (isEnvelopeCreated != null)
                    {
                        response.ReturnedId = isEnvelopeCreated.FunderId;
                    }
                    else
                    {
                        var newEnvelope = unitWork.EnvelopeRepository.Insert(new EFEnvelope()
                        {
                            FunderId = model.FunderId,
                            Year = model.Year,
                            TotalAmount = model.TotalAmount
                        });
                        unitWork.Save();
                        response.ReturnedId = newEnvelope.FunderId;
                    }
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse Update(EnvelopeModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var envelopeObj = unitWork.EnvelopeRepository.GetOne(e => e.FunderId == model.FunderId && e.Year == model.Year);
                if (envelopeObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Envelope");
                    return response;
                }

                envelopeObj.TotalAmount = model.TotalAmount;
                unitWork.EnvelopeRepository.Update(envelopeObj);
                unitWork.Save();
                response.Message = true.ToString();
                return response;
            }
        }

        public ActionResponse Delete(int funderId, int year)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper = new MessageHelper();
                ActionResponse response = new ActionResponse();
                return response;
            }
        }
    }
}
