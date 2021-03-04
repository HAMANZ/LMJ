using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Anz.LMJ.DAL.Model;


namespace Anz.LMJ.DAL.Accessor
{
    public class AllLoggerAccessor
    {
        public void Error(Logger_Error toAdd )
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.Logger_Error.Add(toAdd);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void User(Logger_User toAdd )
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.Logger_User.Add(toAdd);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void Admin(Logger_Admin toAdd )
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.Logger_Admin.Add(toAdd);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void CyberSource(Logger_CyberSource toAdd )
        {
            try
            {
                using (LMJEntities db = new LMJEntities())
                {
                    db.Logger_CyberSource.Add(toAdd);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}