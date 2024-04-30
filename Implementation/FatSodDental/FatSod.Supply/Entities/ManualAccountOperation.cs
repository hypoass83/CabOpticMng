using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    [Serializable]
    public class ManualAccountOperation : AccountOperation
    {
        public long PieceID { get; set; }
        [ForeignKey("PieceID")]
        public virtual Piece Piece { get; set; }
    }
}
