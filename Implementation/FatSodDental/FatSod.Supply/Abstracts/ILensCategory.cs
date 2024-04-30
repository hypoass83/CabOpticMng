using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;

namespace FatSod.Supply.Abstracts
{
    public interface ILensCategory : IRepositorySupply<LensCategory>
    {
        LensCategory CreateLensCategory(LensCategory lensCategory);
        LensCategory UpdateLensCategory(LensCategory lensCategory);

    }
}
