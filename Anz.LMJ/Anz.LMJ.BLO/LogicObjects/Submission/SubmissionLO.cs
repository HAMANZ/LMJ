using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LogicObjects.Submission.Discussion;
using Anz.LMJ.BLO.LogicObjects.User;
using Anz.LMJ.BLO.LogicObjects.Review;
using Anz.LMJ.DAL.Accessors;
using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using star = Anz.LMJ.BLO.LogicObjects.Review.star;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Web.ModelBinding;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Anz.LMJ.BLO.LogicObjects.Submission
{

    //[AttributeUsage(AttributeTargets.All)]
    public class SubmissionLO
        //: Attribute
    {
       
        public long? Id { get; set; }
        public long UserId { get; set; }
        public string Prefix { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public DateTime SubmissionDate { get; set; }

        
        public long SubmissionRequirmentId { get; set; }
        public string Stage { get; set; }

        public bool isBlindedReview { get; set; }
        public bool isDraft { get; set; }

        public bool isApproved { get; set; }
        public string AuthorInfo { get; set; }
        public bool isTopReader { get; set; }
        public bool isEditorsPick { get; set; }
        public ArticleTypeLO ArticleType { get; set; }
        public StudyTypeLO StudyType { get; set; }
        public List<star> MaxStars { get; set; }
        public List<ReviewLO> Reviews{ get; set; }
        public UserLO Author { get; set; }
        public List<UserLO> Contributors { get; set; }
        public List<UserLO> Tags { get; set; }
        public List<string> SubmissionKeywords { get; set; }
        public List<UserLO> Editors { get; set; }
        public List<UserLO> Reviewers { get; set; }
        public List<UserLO> CopyEditors { get; set; }
        public List<UserLO> ProofReaders { get; set; }
        public List<long> TagsIds { get; set; }
        public List<HttpPostedFileBase> SubmissionFilesToUpload { get; set; }
        public List<SubmissionFilesLO> Galleys { get; set; }
        public List<SubmissionFilesLO> SubmissionFiles { get; set; }
        public List<DiscussionLO> PreReviewDiscussion { get; set; }

        public List<SubmissionFilesLO> RevisionFiles { get; set; }
        public List<DiscussionLO> ReviewDiscussion { get; set; }

        public List<SubmissionFilesLO> CopyEditiedFiles { get; set; }
        public List<DiscussionLO> CopyEditedDiscussion { get; set; }

        public List<DiscussionLO> ProofReadingDiscussion { get; set; }

        public List<ProcessLO> NextProcesses { get; set; }
        public string Specialit { get; set; }
        public string Type { get; set; }
        public string Issn { get; set; }
        public DateTime Year { get; set; }

        public string yearstring { get; set; }

        public string Volume { get; set; }

        [DisplayName("IssueNO")]
        public string IssueNO { get; set; }

        [DisplayName("PublishDate")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime PublishDate { get; set; }
        public string Date { get; set; }
        public long ArticleTypeId { get; set; }
        public long OrderFile { get; set; }
        public long SectionId { get; set; }
        public long SpecialitiesId { get; set; }
        public long SubjectId { get; set; }
        public long ResearchId { get; set; }
        public long QuestionId { get; set; }
        public string FileName { get; set; }
        public string[] MetaData { get; set; }
        [AllowHtml]
        public string AbstractText { get; set; }
        public string EditorText { get; set; }
        public string CommentsForEditor { get; set; }
        public string MiniDescription { get; set; }
        public string SourcesOfFunding { get; set; }
        public string ConflictsOfInterests { get; set; }
        public string Significance { get; set; }
        public string Photo { get; set; }
        public string Banner { get; set; }
        public HttpPostedFileBase CoverPhoto { get; set; }

        public HttpPostedFileBase BannerImage { get; set; }
        
        public DateTime SentDate { get; set; }
       

        public string[] Keywords { get; set; }

        //files
        public SubmissionFile[] Files { get; set; }
        public string[] ArticleComponentId { get; set; }
        public string[] Caption { get; set; }
        public long[] Order { get; set; }
        public HttpPostedFileBase[] FilesToUpload { get; set; }


        //contributors
        public Contributor[] Contibutors { get; set; }
        public string[] ContributorFname { get; set; }
        public string[] ContributoMname { get; set; }
        public string[] ContributorLname { get; set; }
        public string[] Degrees { get; set; }
        public string[] ContributorEmail { get; set; }
        public string[] ContributorAffilation { get; set; }
        public string[] Institution { get; set; }
        public string[] ORCID { get; set; }
        public long[] City { get; set; }
        public long[] Department { get; set; }
        public long[] Country { get; set; }
        public bool[] isCorresponding { get; set; }


    }
}
