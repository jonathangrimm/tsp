using System;

namespace TSP
{
    public static class Program
    {
        private static void Main()
        {
            var boltzmann = new BoltzmannMachine();
            boltzmann.FilePath = "Cities.txt";
            boltzmann.RunBoltzmannMachine();
            var path = "";

            for (var i = 0; i < boltzmann.CitiesOrderedList.Count - 1; i++)
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