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
    
    public partial class UserRole
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserRole()
        {
            this.Processes = new HashSet<Process>();
            this.ProcessStages = new HashSet<ProcessStage>();
            this.UserRolesInJournals = new HashSet<UserRolesInJournal>();
        }
    
        public int Id { get; set; }
        public string Role { get; set; }
        public Nullable<bool> isSectionated { get; set; }
        public Nullable<bool> isBlinded { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public Nullable<System.DateTime> SysDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Process> Processes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProcessStage> ProcessStages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRolesInJournal> UserRolesInJournals { get; set; }
    }
}