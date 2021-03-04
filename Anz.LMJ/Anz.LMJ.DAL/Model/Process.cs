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
    
    public partial class Process
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Process()
        {
            this.Processes1 = new HashSet<Process>();
            this.ProcessStages = new HashSet<ProcessStage>();
            this.ProcessStages1 = new HashSet<ProcessStage>();
            this.SubmissionInProcesses = new HashSet<SubmissionInProcess>();
        }
    
        public long Id { get; set; }
        public string Code { get; set; }
        public Nullable<int> RoleId { get; set; }
        public string StageName { get; set; }
        public string ButtonValue { get; set; }
        public string ButtonBackground { get; set; }
        public bool isModalRequired { get; set; }
        public string ModalName { get; set; }
        public Nullable<long> ProcessIdinModal { get; set; }
        public string ModelAction { get; set; }
        public bool isPreReview { get; set; }
        public bool isPreCopyediting { get; set; }
        public bool isPreProduction { get; set; }
        public bool isDeleted { get; set; }
        public Nullable<System.DateTime> SysDate { get; set; }
        public Nullable<long> UserId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Process> Processes1 { get; set; }
        public virtual Process Process1 { get; set; }
        public virtual UserRole UserRole { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProcessStage> ProcessStages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProcessStage> ProcessStages1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubmissionInProcess> SubmissionInProcesses { get; set; }
    }
}