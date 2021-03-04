//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Anz.LMJ.DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Contributor
    {
        public long Id { get; set; }
        public Nullable<long> UserId { get; set; }
        public long SubmissionId { get; set; }
        public Nullable<System.DateTime> SysDate { get; set; }
        public bool isDeleted { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Mname { get; set; }
        public string Email { get; set; }
        public Nullable<bool> isCorresponding { get; set; }
        public string Affiliation { get; set; }
        public string Institution { get; set; }
        public string ORCID { get; set; }
        public Nullable<long> Order { get; set; }
        public Nullable<long> CityId { get; set; }
        public Nullable<long> CountryId { get; set; }
        public Nullable<long> DepartmentId { get; set; }
        public string Degrees { get; set; }
        public Nullable<bool> IsAuthor { get; set; }
        public Nullable<bool> IsTag { get; set; }
    
        public virtual Submission Submission { get; set; }
        public virtual User User { get; set; }
    }
}
