using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Anz.LMJ.BLO.LogicObjects.User
{
    public class UserLO
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Affiliation { get; set; }
        public string TPhone1 { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TPhone2 { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string Email2 { get; set; }
        public string Pob { get; set; }
        public string Orcid { get; set; }
        public List<int> RoleIds { get; set; }
        public long PositionId { get; set; }
        public string Desc { get; set; }
        public string DegreeIds { get; set; }
        public List<int> degIds { get; set; }
        public int Pos { get; set; }
        public bool IsAdmin { get; set; }
        public string MainDesc { get; set; }
        public string Image { get; set; }
        public HttpPostedFileBase PostedFileImage { get; set; }
        public bool isEditor { get; set; }
        public bool isReviewer { get; set; }
        public bool isCopyEditor { get; set; }
        public bool isProofReader { get; set; }
        public bool isCorresponding { get; set; }
        public bool isSubscriber { get; set; }
    }
}
