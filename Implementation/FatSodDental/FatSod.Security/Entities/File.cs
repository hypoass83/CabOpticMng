using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Security.Entities
{
    [Serializable]
    public class File
    {
        public int FileID { get; set; } 
        public string FileName { get; set; }
        public string ContentType { get; set; } 
        public byte[] Content { get; set; }
        public FileType FileType { get; set; }
        public int GlobalPersonID { get; set; }
        [ForeignKey("GlobalPersonID")]
        public virtual GlobalPerson GlobalPerson { get; set; }
    }
}
