using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class QuestionAccessor
    {
        public List<Question> GetList()
        {
            List<Question> response = new List<Question>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response=db.Questions.Where(e => e.isDeleted == false).ToList();
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
