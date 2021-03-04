using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class ResearchAccessor
    {
        public List<Research> GetList()
        {
            List<Research> response = new List<Research>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Researches.Where(e => e.isDeleted == false && e.SubId == null).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<Research> GetListSub(long subId)
        {
            List<Research> response = new List<Research>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Researches.Where(e => e.isDeleted == false && e.SubId == subId && e.SubSubId == null).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<Research> GetListSubSub(long subsubId)
        {
            List<Research> response = new List<Research>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Researches.Where(e => e.isDeleted == false && e.SubSubId == subsubId).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
