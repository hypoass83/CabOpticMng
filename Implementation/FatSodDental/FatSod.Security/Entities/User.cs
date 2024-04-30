using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class User : Person
    {

        [Required]
        [StringLength(100)]
        [Index("Code", IsUnique = true, IsClustered = false)]
        public string Code { get; set; }
        [StringLength(100)]
        [Index("UserLogin", IsUnique = true, IsClustered = false)]
        public string UserLogin { get; set; } 
        [Required]
        public string UserPassword { get; set; }
        public bool UserAccountState { get; set; }
        public int UserAccessLevel { get; set; }
        public int ProfileID { get; set; }
        [ForeignKey("ProfileID")]
        public virtual Profile Profile { get; set; }
        public int? UserConfigurationID { get; set; }
        [ForeignKey("UserConfigurationID")]
        public virtual UserConfiguration UserConfiguration { get; set; }
        public int? JobID { get; set; }
        [ForeignKey("JobID")]
        public virtual Job Job { get; set; }
        public ICollection<UserBranch> UserBranches { get; set; }
        //*******************
        //no map fields
        [NotMapped]
        public string ProfileLabel
        {
            get
            {
                return this.Profile.ProfileLabel;
            }
        }

        [NotMapped]
        public string UserFullName
        {
            get
            {
                return this.Name + " " + this.Description;
            }
        }
        /// <summary>
        /// This a NotMapped attribute is not persiste in database and readonly. 
        /// It return a Job.JobLabel value of this Person
        /// </summary>
        /// 
        [NotMapped]
        public string JobLabel
        {
            get
            {
                return this.Job.JobLabel;
            }
        }
    }
}
