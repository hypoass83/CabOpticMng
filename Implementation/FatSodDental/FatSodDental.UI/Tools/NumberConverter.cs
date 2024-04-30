using System;
using System.Text;
using Developpez.Dotnet.Language.Grammar;

namespace FatSodDental.UI.Tools
{
    /// <summary>
    /// Classe statique de conversion d'un nombre en toutes lettres
    /// (en français uniquement).
    /// </summary>
    /// <remarks>Sources pour les règles d'écriture : 
    /// - http://fr.wikipedia.org/wiki/Nombres_en_fran%C3%A7ais
    /// - http://www.miakinen.net/vrac/nombres .</remarks>
    public static class NumberConverter
    {

        #region Private readonly fields

        private static readonly string[] _UNITSANDTENS;
        private static readonly string[] _HUNDREDS;
        private static readonly string[] _THOUSANDPOWERS;
        private static readonly string _MINUS;

        #endregion



        #region Constructor

        static NumberConverter()
        {

            NumberConverter._MINUS = "moins";

            // Les nombres jusqu'à cent ont beaucoup "d'exceptions" de nomenclature (23/99).
            // Il est plus simple de les stocker dans un tableau que de les générer dynamiquement :
            NumberConverter._UNITSANDTENS = new string[100]
            {
                string.Empty, "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf",
                "dix", "onze", "douze", "treize", "quatorze", "quinze", "seize", "dix-sept", "dix-huit", "dix-neuf",
                "vingt", "vingt et un", "vingt-deux", "vingt-trois", "vingt-quatre", "vingt-cinq", "vingt-six", "vingt-sept", "vingt-huit", "vingt-neuf",
                "trente", "trente et un", "trente-deux", "trente-trois", "trente-quatre", "trente-cinq", "trente-six", "trente-sept", "trente-huit", "trente-neuf",
                "quarante", "quarante et un", "quarante-deux", "quarante-trois", "quarante-quatre", "quarante-cinq", "quarante-six", "quarante-sept", "quarante-huit", "quarante-neuf",
                "cinquante", "cinquante et un", "cinquante-deux", "cinquante-trois", "cinquante-quatre", "cinquante-cinq", "cinquante-six", "cinquante-sept", "cinquante-huit", "cinquante-neuf",
                "soixante", "soixante et un", "soixante-deux", "soixante-trois", "soixante-quatre", "soixante-cinq", "soixante-six", "soixante-sept", "soixante-huit", "soixante-neuf",
                "soixante-dix", "soixante et onze", "soixante-douze", "soixante-treize", "soixante-quatorze", "soixante-quinze", "soixante-seize", "soixante-dix-sept", "soixante-dix-huit", "soixante-dix-neuf",
                "quatre-vingt", "quatre-vingt-un", "quatre-vingt-deux", "quatre-vingt-trois", "quatre-vingt-quatre", "quatre-vingt-cinq", "quatre-vingt-six", "quatre-vingt-sept", "quatre-vingt-huit", "quatre-vingt-neuf",
                "quatre-vingt-dix", "quatre-vingt-onze", "quatre-vingt-douze", "quatre-vingt-treize", "quatre-vingt-quatorze", "quatre-vingt-quinze", "quatre-vingt-seize", "quatre-vingt-dix-sept", "quatre-vingt-dix-huit", "quatre-vingt-dix-neuf"
            };
            NumberConverter._HUNDREDS = new string[10]
            {
                string.Empty, "cent", "deux cent", "trois cent", "quatre cent", "cinq cent", "six cent", "sept cent", "huit cent", "neuf cent"
            };
            NumberConverter._THOUSANDPOWERS = new string[7]
            {
                "trillion", "mille", "billion", "milliard", "million", "mille", string.Empty
            };

        }

        #endregion



        #region Private methods

        /// <summary>
        /// Accorder le mot "vingt".
        /// </summary>
        /// <returns>Un "s" si le nombre se termine par vingt et n'est pas suivi de "mille".
        /// Chaîne vide sinon.</returns>
        private static string MakeTwentyAgree(int value, int thousandPower, NumeralAdjective numeralAdjective)
        {
            // Vingt prend un "s" à la fin, si et seulement si :
            // - il fait partie d'un nombre cardinal.
            // - il n'est pas situé avant "mille".
            // - il se termine par quatre-vingts.
            if (numeralAdjective == NumeralAdjective.Cardinal &&
                thousandPower != 1 &&
                thousandPower != 5 &&
                value == 80)
                return "s";
            else
                return string.Empty;
        }


        /// <summary>
        /// Accorder le mot "cent".
        /// </summary>
        /// <returns>Un "s" si le nombre est un multiple de 100, strictement supérieur à 100 et n'est pas suivi de "mille".
        /// Chaîne vide sinon.</returns>
        private static string MakeHundredAgree(long hundreds, long tensAndUnits, int thousandPower, NumeralAdjective numeralAdjective)
        {
            // Cent prend un "s" à la fin, si et seulement si :
            // - il fait partie d'un nombre cardinal.
            // - il n'est pas situé avant "mille".
            // - il n'est pas suivi de dizaines ni de chiffres (remainder == 0).
            // - il strictement supérieur à 100 (hundreds > 1).
            if (numeralAdjective == NumeralAdjective.Cardinal &&
                hundreds > 1 &&
                thousandPower != 1 &&
                thousandPower != 5 &&
                tensAndUnits == 0)
                return "s";
            else
                return string.Empty;
        }


        /// <summary>
        /// Accorder la puissance de mille donnée.
        /// </summary>
        /// <returns>Un "s" si le nombre strictement supérieur à 1 et n'est pas "mille".
        /// Chaîne vide sinon.</returns>
        private static string MakeThousandPowerAgree(long value, int thousandPower)
        {
            // La puissance prend un "s" à la fin, si et seulement si :
            // - il strictement supérieur à 1.
            // - il n'est pas "mille".
            // - ce n'est pas les centaines et unités.
            if (value > 1 &&
                thousandPower != 1 &&
                thousandPower != 5 &&
                thousandPower != 6)
                return "s";
            else
                return string.Empty;
        }


        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="negative">Le nombre est négatif.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        private static string InnerSpell(ulong value, bool negative, Gender gender, NumeralAdjective numeralAdjective)
        {
            // Zéro :
            if (value == 0)
                return "zéro";

            StringBuilder result = new StringBuilder();

            // Valeurs de 1 à 99
            // (court-circuite la méthode HighNumbersSpell() pour de meilleures performances) :
            if (value < 100)
                result.AppendFormat("{0}{1}", NumberConverter._UNITSANDTENS[value], NumberConverter.MakeTwentyAgree((int)value, (NumberConverter._THOUSANDPOWERS.Length - 1), numeralAdjective));
            // Valeurs de 100 à 999
            // (court-circuite la méthode HighNumbersSpell() pour de meilleures performances) :
            else if (value < 1000)
                result.Append(NumberConverter.HundredsAndUnitsSpell((int)value, (NumberConverter._THOUSANDPOWERS.Length - 1), numeralAdjective));
            else
                result.Append(NumberConverter.HighNumbersSpell(value, numeralAdjective));

            // Négatif :
            if (negative)
                result.Insert(0, NumberConverter._MINUS + " ");

            // Genre :
            if (gender == Gender.Feminine && result[result.Length - 2] == 'u' && result[result.Length - 1] == 'n')
                result.Append("e");

            return result.ToString().TrimEnd(' ');
        }


        /// <summary>
        /// Convertit le nombre entier en toutes lettres (nombres supérieurs à 1000).
        /// </summary>
        /// <param name="value">Nombre entier (supérieur à 1000).</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        private static string HighNumbersSpell(ulong value, NumeralAdjective numeralAdjective)
        {
            StringBuilder result = new StringBuilder();
            ulong remainder = 0;
            ulong quotient = value;
            int[] groups = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
            int groupIndex = groups.Length - 1;

            // Découper le nombre en groupes de trois chiffres de la façon suivante :
            // [0]trillions,[1]milliers de billions,[2]billions,[3]milliards,
            // [4]millions,[5]milliers,[6]centaines et unités.
            while (quotient >= 1)
            {
                remainder = quotient % 1000;
                quotient = quotient / 1000; //Math.DivRem(quotient, 1000, out remainder);
                groups[groupIndex] = (int)remainder;
                groupIndex--;
            }

            // Générer le nombre en toutes lettres :
            for (groupIndex = 0; groupIndex < groups.Length; groupIndex++)
            {
                if (groups[groupIndex] == 0)
                    continue;

                if (groupIndex == 1)
                {
                    // Nombre de milliers (> 1 car "un mille" n'existe pas) :
                    if (groups[1] > 1)
                        result.AppendFormat("{0} ", NumberConverter.HundredsAndUnitsSpell(groups[groupIndex], groupIndex, numeralAdjective));
                    // Puissance des milliers (de billions) :
                    result.AppendFormat("{0} ", NumberConverter._THOUSANDPOWERS[groupIndex]);
                    // Ajouter la "billions" dans cette boucle, si le groupe des billions est nul :
                    if (groups[2] == 0)
                        result.AppendFormat("{0}s ", NumberConverter._THOUSANDPOWERS[2]);// Dans ce cas, billions prend un "s" car il y en a plus de mille.
                }
                // Exception "un mille" donne "mille" :
                else if (groupIndex == 5 && groups[5] == 1)
                    result.AppendFormat("{0} ", NumberConverter._THOUSANDPOWERS[groupIndex]);
                // Cas général :
                else
                    result.AppendFormat("{0} {1}{2} ", NumberConverter.HundredsAndUnitsSpell(groups[groupIndex], groupIndex, numeralAdjective), NumberConverter._THOUSANDPOWERS[groupIndex], NumberConverter.MakeThousandPowerAgree(groups[groupIndex], groupIndex));
            }

            return result.ToString().TrimEnd(' ');
        }


        /// <summary>
        /// Convertit un nombre entier en toutes lettres (nombres entre 0 et 999).
        /// </summary>
        /// <param name="value">Nombre entier (entre 0 et 999).</param>
        /// <param name="thousandPower">La puissance de mille du nombre.</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        private static string HundredsAndUnitsSpell(int value, int thousandPower, NumeralAdjective numeralAdjective)
        {
            if (value == 0)
                return string.Empty;

            int remainder = 0;
            int quotient = Math.DivRem(value, 100, out remainder);

            if (quotient > 0)
            {
                if (remainder > 0)
                    return string.Format("{0}{1} {2}{3}", NumberConverter._HUNDREDS[quotient], NumberConverter.MakeHundredAgree(quotient, remainder, thousandPower, numeralAdjective), NumberConverter._UNITSANDTENS[remainder], NumberConverter.MakeTwentyAgree(remainder, thousandPower, numeralAdjective));
                else
                    return string.Format("{0}{1}", NumberConverter._HUNDREDS[quotient], NumberConverter.MakeHundredAgree(quotient, remainder, thousandPower, numeralAdjective));
            }
            else
                return string.Format("{0}{1}", NumberConverter._UNITSANDTENS[remainder], NumberConverter.MakeTwentyAgree(remainder, thousandPower, numeralAdjective));
        }

        #endregion



        #region Public methods

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(int value)
        {
            return NumberConverter.Spell(value, Gender.Masculine);
        }


        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(int value, Gender gender)
        {
            return NumberConverter.Spell(value, gender, NumeralAdjective.Cardinal);
        }


        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(int value, Gender gender, NumeralAdjective numeralAdjective)
        {
            if (value == int.MinValue)
                return NumberConverter.InnerSpell((ulong)Math.Abs((long)value), true, gender, numeralAdjective);

            return NumberConverter.InnerSpell((ulong)Math.Abs(value), (value < 0), gender, numeralAdjective);
        }


        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(uint value)
        {
            return NumberConverter.Spell(value, Gender.Masculine);
        }


        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(uint value, Gender gender)
        {
            return NumberConverter.Spell(value, gender, NumeralAdjective.Cardinal);
        }


        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(uint value, Gender gender, NumeralAdjective numeralAdjective)
        {
            return NumberConverter.InnerSpell((ulong)value, false, gender, numeralAdjective);
        }


        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(long value)
        {
            return NumberConverter.Spell(value, Gender.Masculine);
        }


        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(long value, Gender gender)
        {
            return NumberConverter.Spell(value, gender, NumeralAdjective.Cardinal);
        }


        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(long value, Gender gender, NumeralAdjective numeralAdjective)
        {
            if (value == long.MinValue)
                return NumberConverter.InnerSpell((ulong)(-value), true, gender, numeralAdjective);

            return NumberConverter.InnerSpell((ulong)Math.Abs(value), (value < 0), gender, numeralAdjective);
        }


        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(ulong value)
        {
            return NumberConverter.Spell(value, Gender.Masculine);
        }

        

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(ulong value, Gender gender)
        {
            return NumberConverter.Spell(value, gender, NumeralAdjective.Cardinal);
        }


        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(ulong value, Gender gender, NumeralAdjective numeralAdjective)
        {
            return NumberConverter.InnerSpell(value, false, gender, numeralAdjective);
        }

        #endregion

    }
}