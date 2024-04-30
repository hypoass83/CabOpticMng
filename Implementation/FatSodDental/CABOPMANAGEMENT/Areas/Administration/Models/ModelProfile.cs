using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CABOPMANAGEMENT.Areas.Administration.Models
{
    public class ModelProfile
    {
        public int ModelProfileID { get; set; }
        public int ProfileID { get; set; }
        public string ProfileCode { get; set; }
        public string ProfileLabel { get; set; }
        public string ProfileDescription { get; set; }
        public int RadioAccess { get; set; }
        public int RadioProfileState { get; set; }

        public virtual ICollection<ModelCheckSubMenu> ModelCheckSubMenus { get; set; }
        public virtual ICollection<ModelCheckMenu> ModelCheckMenus { get; set; }
    }
}