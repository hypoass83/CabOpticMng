using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    class FizzBuzz
    {
        internal string generernombre(int nombredep, int nombrefin)
        {
            String result = "";
            
            while (nombredep<=nombrefin)
            {
                result += evaluatenumber(nombredep++);
            }
            return result;
        }
        public string evaluatenumber(int  nbre)
        {
            if (nbre % 15 == 0) return "FizzBuzz";
            if (nbre % 3 == 0) return "Fizz";
            if (nbre % 5 == 0) return "Buzz";
            return nbre.ToString();
        }

        public  int[] retournePrimaryDiagonal(int[,] arr)
        {
           
            int[] primarydiagonal = new int[arr.GetLength(0)];
            for (int i = 0; i < arr.GetLength(0); i++) //ligne
            {
                primarydiagonal[i] = arr[i, i]; ;
            }
            return primarydiagonal;
        }

        public  int[] retourneSecondaryDiagonal(int[,] arr)
        {
            int n = arr.GetLength(0)-1;
            int[] secondarydiagonal = new int[arr.GetLength(0)];
            for (int i = 0; i < arr.GetLength(0); i++) //ligne
            { 
                secondarydiagonal[i] = arr[i, n];
                n = n - 1;
            }
            return secondarydiagonal;
        }
        public  int diagonalDifference(int[,] arr)
        {
            int result = 0;
            int resultpri = 0;
            int resultsec = 0;
            int[] primarydiagonal = retournePrimaryDiagonal(arr);
            int[] secondarydiag = retourneSecondaryDiagonal(arr);

            for (int i = 0; i < primarydiagonal.GetLength(0); i++) //ligne
            {
                resultpri += primarydiagonal[i] ;
            }
            for (int i = 0; i < secondarydiag.GetLength(0); i++) //ligne
            {
                resultsec += secondarydiag[i];
            }
            result = Math.Abs(resultpri - resultsec);
            return result;
        }

        public decimal returnpositivenumber(int[] arr)
        {
            decimal finalvalue = 0;
            decimal totalpositivenumber = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] > 0) totalpositivenumber += 1;
            }
            decimal diviseur = arr.Length;
            finalvalue = Math.Round((totalpositivenumber / diviseur),6) ;
            return finalvalue;
        }

        public double returnnegativenumber(int[] arr)
        {
            double finalvalue = 0;
            int totalnegativenumber = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] < 0) totalnegativenumber += 1;
            }
            finalvalue = Math.Round((double)(totalnegativenumber / arr.Length), 4);
            return finalvalue;
        }
        public double returnzeronumber(int[] arr)
        {
            double finalvalue = 0;
            int totalzeronumber = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == 0) totalzeronumber += 1;
            }
            finalvalue = Math.Round((double)(totalzeronumber / arr.Length), 4);
            return finalvalue;
        }
        // Complete the plusMinus function below.
        public void plusMinus(int[] arr, int lis)
        {
            double negativevalue = returnnegativenumber(arr);

            double zerovalue = returnzeronumber(arr);

            decimal positivevalue = returnpositivenumber(arr);

            Console.WriteLine(positivevalue);
            Console.WriteLine(negativevalue);
            Console.WriteLine(zerovalue);
        }
    }
}
