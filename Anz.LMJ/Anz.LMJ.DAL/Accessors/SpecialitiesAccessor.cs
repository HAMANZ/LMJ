using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class SectionAccessor
    {
        public List<Section> GetList(long journalId)
        {
            List<Section> response = new List<Section>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Sections.Where(e => e.JournalId == journalId).ToList();
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
