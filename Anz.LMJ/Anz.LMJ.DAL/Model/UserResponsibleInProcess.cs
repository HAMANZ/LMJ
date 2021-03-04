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
    
    public partial class UserResponsibleInProcess
    {
        public long Id { get; set; }
        public Nullable<long> SubmissionProcessId { get; set; }
        public long UserId { get; set; }
        public Nullable<bool> isAssignedByManager { get; set; }
        public Nullable<long> ManagerId { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public Nullable<System.DateTime> SysDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
    
        public virtual SubmissionInProcess SubmissionInProcess { get; set; }
        public virtual User User { get; set; }
    }
}