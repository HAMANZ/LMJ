﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class LMJEntities : DbContext
    {
        public LMJEntities()
            : base("name=LMJEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ArticleType> ArticleTypes { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<ContactU> ContactUs { get; set; }
        public virtual DbSet<Contributor> Contributors { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<DiscussionParticipant> DiscussionParticipants { get; set; }
        public virtual DbSet<Discussion> Discussions { get; set; }
        public virtual DbSet<DiscussionsFile> DiscussionsFiles { get; set; }
        public virtual DbSet<EmailLog> EmailLogs { get; set; }
        public virtual DbSet<Galley> Galleys { get; set; }
        public virtual DbSet<Issue> Issues { get; set; }
        public virtual DbSet<Journal> Journals { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Library> Libraries { get; set; }
        public virtual DbSet<Logger_Admin> Logger_Admin { get; set; }
        public virtual DbSet<Logger_CyberSource> Logger_CyberSource { get; set; }
        public virtual DbSet<Logger_Error> Logger_Error { get; set; }
        public virtual DbSet<Logger_User> Logger_User { get; set; }
        public virtual DbSet<LookUp> LookUps { get; set; }
        public virtual DbSet<LookUpMedia> LookUpMedias { get; set; }
        public virtual DbSet<LookUpMultiLanguage> LookUpMultiLanguages { get; set; }
        public virtual DbSet<LookUpTable> LookUpTables { get; set; }
        public virtual DbSet<Medium> Media { get; set; }
        public virtual DbSet<Newsletter> Newsletters { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Process> Processes { get; set; }
        public virtual DbSet<ProcessStage> ProcessStages { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Requirment> Requirments { get; set; }
        public virtual DbSet<Research> Researches { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Speciality> Specialities { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<SubmissionFile> SubmissionFiles { get; set; }
        public virtual DbSet<SubmissionInProcess> SubmissionInProcesses { get; set; }
        public virtual DbSet<SubmissionKeyword> SubmissionKeywords { get; set; }
        public virtual DbSet<Submission> Submissions { get; set; }
        public virtual DbSet<SubmissionStatu> SubmissionStatus { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<UserResponsibleInProcess> UserResponsibleInProcesses { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserRolesInJournal> UserRolesInJournals { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
