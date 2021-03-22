using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.DAL.Accessors
{
    public class IssueAccessor
    {
        public List<Issue> GetList()
        {
            List<Issue> response = new List<Issue>();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Issues.Where(e => e.IsDeleted == false).ToList();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        

        public Issue Get(long issueid)
        {
            Issue response = new Issue();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Issues.Where(e => e.Id == issueid && e.IsDeleted == false).FirstOrDefault(); ;
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public Issue GetLatestIssue()
        {
            Issue response = new Issue();
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    response = db.Issues.Where(e => e.IsDeleted == false ).OrderByDescending(s => s.SysDate).FirstOrDefault();
                }
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<Issue> GetList(long issueno)
        {
            try
            {
                List<Issue> data = new List<Issue>();
                using (LMJEntities db = new LMJEntities())
                {
                    data = db.Issues.Where(e => e.IssueNo == issueno && e.IsDeleted == false).ToList();
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        

        #region Add
        public  Newsletter Add(Newsletter toAdd)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.Newsletters.Add(toAdd);
                    db.SaveChanges();
                    return toAdd;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        #endregion

        public Issue Edit(Issue s)
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    Issue Issue = db.Issues.Where(e => e.Id == s.Id).FirstOrDefault();
                    Issue.CoverImage = s.CoverImage;
                    Issue.Title = s.Title;
                    Issue.SubTitle = s.SubTitle;
                    Issue.IssueNo = s.IssueNo;
                    db.SaveChanges();
                    return Issue;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }


    }
}
