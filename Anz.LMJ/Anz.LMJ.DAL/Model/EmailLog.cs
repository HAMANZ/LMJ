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
    
    public partial class EmailLog
    {
        public long Id { get; set; }
        public Nullable<long> SenderId { get; set; }
        public Nullable<long> RecieverId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Nullable<System.DateTime> SysDate { get; set; }
        public Nullable<long> SubmissionId { get; set; }
    
        public virtual Submission Submission { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
