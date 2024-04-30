using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FatSod.Security.Entities;

namespace FatSodDental.UI.Models
{
    public class AuthenticateUser
    {
        public User User { get; set; }
        public bool Authenticate { get; set; }
    }
}