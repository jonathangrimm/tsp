using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace TSP
{
    public static class Program
    {
        private const double Temp = 10000.0;
        private const double CoolingRate = 0.7;
        private const double AbsoluteTemp = .1; //0.00001;

        private static void Main()
        {
            //this creates an insant of the boltzmann machine that requires the given parameters and the data source 
            //Cities.txt specified in file path. 
            var boltzmann = new BoltzmannMachine(Temp, CoolingRate, AbsoluteTemp) { FilePath = "Cities.txt" };
            boltzmann.RunBoltzmannMachine();
           
            var resultsText = GetResultsText(boltzmann);
            var file = SaveOutput(resultsText);
         
            Console.Write(Environment.NewLine + resultsText);
            Console.Read();
            Process.Start(file.FileName);
        }

        private static string GetResultsText(BoltzmannMachine boltzmann)
        {
            var milesSingularOrPlural = boltzmann.LeastDistance > 1 ? "miles" : "mile";
            var sb = new StringBuilder();
            sb.AppendLine(
                string.Format("The shortest route is {0}. The shortest distance is {1} {2}.",
                    boltzmann.CitiesInLettersList,
                    boltzmann.LeastDistance, milesSingularOrPlural));
            sb.AppendLine(string.Format("Iterations: {0}.", boltzmann.Iteration));
            sb.AppendLine(
                string.Format(
                    "Temperature: {0}, a Cooling Rate: {1}, Absolute Temperature: {2}.",
                    Temp, CoolingRate, AbsoluteTemp));

            sb.AppendLine("To increase the accuracy (and thus the iterations or attempts), raise the value for the cooling rate.");

            return sb.ToString();
        }

        private static SaveFileDialog SaveOutput(string text)
        {
            var file = new SaveFileDialog { FileName = "TSP_Result" };

            using (var sw = new StreamWriter(file.OpenFile()))
                sw.Write(text);

            return file;

        }
    }
}