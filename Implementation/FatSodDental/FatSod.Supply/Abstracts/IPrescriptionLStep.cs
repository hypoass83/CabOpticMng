using FatSod.Supply.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Abstracts
{
    public interface IPrescriptionLStep : IRepositorySupply<PrescriptionLStep>
    {
        PrescriptionLStep SaveChanges(PrescriptionLStep prescription, bool isPrescription, String HeureVente, int UserConect, int BranchID);
        PrescriptionLStep UpdateChanges(PrescriptionLStep prescription, bool isPrescription, String HeureVente,  int UserConect, int BranchID);

        OrderLens CreateOrderLens(OrderLens currentProduct);

        ConsultOldPrescr SaveConsultoldpres(ConsultOldPrescr consultOldPrescr, int UserConect, int BranchID);
        ConsultOldPrescr UpdateConsultoldpres(ConsultOldPrescr consultOldPrescr, int UserConect, int BranchID);

        ConsultPersonalMedHisto SaveChangesConsultPersonalMedHisto(ConsultPersonalMedHisto consultPersonalMedHisto, List<int> allATCDPerso, List<int> allATCDFam, int UserConect, int BranchID);
        ConsultPersonalMedHisto UpdateChangesconsultPersonalMedHisto(ConsultPersonalMedHisto consultPersonalMedHisto, List<int> allATCDPerso, List<int> allATCDFam, int UserConect, int BranchID);

        ConsultDilatation SaveConsultDilatation(ConsultDilPresc consultDilPresc, ConsultDilatation newDilation, int UserConect, int BranchID);
        ConsultDilatation UpdateConsultDilatation(ConsultDilatation existDilation, int UserConect, int BranchID);
        void DeleteConsultDilatation(int ConsultDilPrescID, int UserConect, int BranchID, DateTime DateOperation);

        ConsultLensPrescription SaveConsultLensPrescription(ConsultDilPresc consultDilPresc, ConsultLensPrescription newLensPrescrip,  int UserConect, int BranchID);
        ConsultLensPrescription UpdateConsultLensPrescription(ConsultLensPrescription newLensPrescrip, int UserConect, int BranchID);
        void DeleteConsultLensPrescription(int ConsultDilPrescID, int UserConect, int BranchID, DateTime DateOperation);
    }
}
