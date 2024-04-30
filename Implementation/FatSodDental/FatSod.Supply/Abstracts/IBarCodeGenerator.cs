﻿using FatSod.Security.Entities;
using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IBarCodeGenerator : IRepositorySupply<BarCodeGenerator>
    {

        bool valideBarCodeGenerator(BarCodeGenerator barCodeGenerator);
        string generateBarcodeNumber();
        bool miseajourBarCodeGenerator(BarCodeGenerator barCodeGenerator);
        bool supprimeBarCodeGenerator(int BarCodeGeneratorID);
    }
}
