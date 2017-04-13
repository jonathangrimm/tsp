using System;
using System.Linq;
using System.Text;

namespace TSP
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            BoltzmannMachine boltzmann = new BoltzmannMachine();
            boltzmann.FilePath = "Cities.txt";
            boltzmann.RunBoltzmannMachine();


            string path = "";

            for (int i = 0; i < boltzmann.CitiesOrderedList.Count - 1; i++)
            {
                path += boltzmann.CitiesOrderedList[i] + " -> ";
            }
            path += boltzmann.CitiesOrderedList[boltzmann.CitiesOrderedList.Count - 1];

            Console.WriteLine("Shortest Route: " + path);

            Console.WriteLine("The shortest distance is: " + boltzmann.LeastDistance);

            Console.ReadLine();
        }
    }
}
