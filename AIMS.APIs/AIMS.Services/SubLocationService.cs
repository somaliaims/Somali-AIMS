using AIMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services
{
    public interface ISubLocationService
    {
        /// <summary>
        /// Gets list of all sub locations
        /// </summary>
        /// <returns></returns>
        IEnumerable<SubLocationView> GetAll();

        /// <summary>
        /// Get sub locations for the provided location
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SubLocationView> GetForLocation(int id);

        /// <summary>
        /// Get sub location for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SubLocationView Get(int id);

        /// <summary>
        /// Adds new sub location
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse Add(SubLocationModel model);

        /// <summary>
        /// Updates the sub location
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse Update(int id, SubLocationModel model);

        /// <summary>
        /// Deletes the provided sub location
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Delete(int id);
    }
    public class SubLocationService
    {
    }
}
