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
    
    public partial class UserRolesInJournal
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int RoleId { get; set; }
        public Nullable<long> SectionId { get; set; }
        public bool isDeleted { get; set; }
        public Nullable<System.DateTime> SysDate { get; set; }
    
        public virtual Section Section { get; set; }
        public virtual UserRole UserRole { get; set; }
        public virtual User User { get; set; }
    }
}
