using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class CityAccessor
    {
        public List<City> GetList(long id)
        {
            List<City> response = new List<City>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Cities.Where(e => e.CountryId == id && e.isDeleted == false).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public City Add(City toAdd)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.Cities.Add(toAdd);
                    db.SaveChanges();
                    return toAdd;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }


    }
}
