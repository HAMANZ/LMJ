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
    
    public partial class Issue
    {
        public long Id { get; set; }
        public long NewsletterId { get; set; }
        public string IssuePrintNo { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> SysDate { get; set; }
        public Nullable<long> IssueNo { get; set; }
        public string CoverImage { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string ISSN { get; set; }
        public string EISSN { get; set; }
    }
}
