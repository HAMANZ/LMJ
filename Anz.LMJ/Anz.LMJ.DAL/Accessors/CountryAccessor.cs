using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class CountryAccessor
    {
        public List<Country> GetList()
        {
            List<Country> response = new List<Country>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Countries.Where(e => e.isDeleted == false).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Country Get(string code)
        {
            try
            {
                Country data = new Country();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Countries.Where(e => e.Code == code && e.isDeleted == false).FirstOrDefault();

                    return data;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public Country Add(Country toAdd)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.Countries.Add(toAdd);
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
