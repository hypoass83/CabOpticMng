using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace FatSod.Security.Entities
{
    [Serializable]
    public abstract class Person : GlobalPerson
    {
        public bool IsConnected { get; set; }
        public int SexID { get; set; }
        [ForeignKey("SexID")]
        public virtual Sex Sex { get; set; }

        public bool IsMarketer { get; set; }
        public bool IsSeller { get; set; }
        //*******************
        /// <summary>
        /// This a NotMapped attribute is not persiste in database and readonly. 
        /// It return a Sex.SexLabel value of this Person
        /// </summary>
        [NotMapped]
        public string SexLabel
        {
            get
            {
                if (this.Sex != null)
                    return this.Sex.SexLabel;
                else
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// This a NotMapped attribute is not persiste in database and readonly. 
        /// It return a Adress.AdressPhoneNumber value of this Person
        /// </summary>
        [NotMapped]
        public string AdressPhoneNumber
        {
            get
            {
                //if (this.Adress != null)
                //    return this.Adress.AdressPhoneNumber;
                if (this.Adress != null)
                {
                    var res = this.Adress.AdressPhoneNumber;
                    res = !String.IsNullOrEmpty(res) ? res : this.Adress.AdressCellNumber;
                    return res;
                }
                else
                    return "";
            }
        }
        /// <summary>
        /// This a NotMapped attribute is not persiste in database and readonly. 
        /// It return a Adress.AdressEmail value of this Person
        /// </summary>
        [NotMapped]
        public string AdressEmail
        {
            get
            {
                if (this.Adress != null)
                    return this.Adress.AdressEmail;
                else
                    return "";
            }
        }
        /// <summary>
        /// This a NotMapped attribute is not persiste in database and readonly. 
        /// It return a Adress.AdressPOBox value of this Person
        /// </summary>
        [NotMapped]
        public string AdressPOBox
        {
            get
            {
                if (this.Adress != null)
                    return this.Adress.AdressPOBox;
                else
                    return "";
            }
        }
        [NotMapped]
        public string PhoneNumber { get; set; }
    }
}
