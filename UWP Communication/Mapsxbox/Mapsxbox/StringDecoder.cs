using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapsxbox
{
    public class StringDecoder
    {
        /// <summary>
        /// Preia un dictionar cu id-urile si temperatorile camerelor, si 
        /// le transforma intr-un string care va fi trimis catre XBox.
        /// </summary>
        /// <param name="inregistrari">
        /// Un dictionar de (int, double), int fiind ID-ul camerei,
        /// iar doubleul temperatura din camera.
        /// </param>
        /// <returns>
        /// Un string care va fi trimis prin pe Xbox
        /// </returns>
        public static string dictionaryToString(Dictionary<int, double> inregistrari)
        {
            string convertedToString = "";
            foreach (KeyValuePair<int, double> pair in inregistrari)
            {
                convertedToString = convertedToString + pair.Key.ToString() + ":" + pair.Value.ToString() + ";";
            }
            return convertedToString;
        }

        /// <summary>
        /// Preia un string si il transofrma intr-un dictionar de (int, double),
        /// care reprezinta ID-ul camerelor si temperatura lor.
        /// </summary>
        /// <param name="inregistrari">
        /// Un string creat de aceasta clasa care poate fi transformata intr-un
        /// dictionar de tip (int, double)
        /// </param>
        /// Un dictionar de tip (int, double) int - ul reprezentand ID-ul, iar double-ul 
        /// reorezentand temperatura.
        /// <returns></returns>
        public static Dictionary<int, double> stringToDictionary(string inregistrari)
        {
            Dictionary<int, double> convertedToDict = new Dictionary<int, double>();

            Char delimiter = ';';
            String[] substrings = inregistrari.Split(delimiter);
            foreach (var register in substrings)
            {
                if (register != "")
                {
                    Char delimiter2 = ':';
                    String[] substrings2 = register.Split(delimiter2);

                    int id = Int32.Parse(substrings2[0]);
                    double grad = Double.Parse(substrings2[1]);

                    convertedToDict.Add(id, grad);
                }
            }

            return convertedToDict;
        }
    }
}