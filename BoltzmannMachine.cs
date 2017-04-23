using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TSP
{
    public class BoltzmannMachine
    {
        private readonly double _absoluteTemp;
        private readonly double _coolingRate;
        private readonly Random _random = new Random();
        private double[,] _distances;
        private List<int> _nextOrder = new List<int>();

        public BoltzmannMachine(double temp, double coolingRate, double absoluteTemp)
        {
            Temp = temp;
            _coolingRate = coolingRate;
            _absoluteTemp = absoluteTemp;
            Iteration = 1;
            CitiesOrderedList = new List<int>();
            LeastDistance = 0;
        }

        private double Temp { get; set; }
        public double LeastDistance { get;  set; }
        public string FilePath {  get; set; }
        private List<int> CitiesOrderedList { get; set; }

        public string CitiesInLettersList
        {
            get { return FormatCityList(CitiesOrderedList); }
        }

        public int Iteration { get; set; }

        /// <summary>
        ///     Gets the  distance points for each location in the matrix in the text file.
        ///     Each row in the text file represents a new a location.
        /// </summary>
        private void GetDistanceData()
        {
            string[] locationDistances;
            using (var reader = new StreamReader(FilePath))
                locationDistances = reader.ReadToEnd().Split('\n');
            _distances = new double[locationDistances.Length, locationDistances.Length];

            for (var i = 0; i < locationDistances.Length; i++)
            {
                var distance = locationDistances[i].Split(' ');

                for (var j = 0; j < distance.Length; j++)
                {
                    if (!string.IsNullOrEmpty(distance[j]))
                    {
                        _distances[i, j] = double.Parse(distance[j]);
                    }
                }

                if (CitiesOrderedList != null) CitiesOrderedList.Add(i);
            }
        }

        /// <summary>
        ///     Calculate the total distance which is the objective function
        /// </summary>
        /// <param name="order">A list containing the order of cities</param>
        /// <returns></returns>
        private double GetTotalDistance(List<int> order)
        {
            double distance = 0;
            for (var i = 0; i < order.Count - 1; i++)
                distance += _distances[order[i], order[i + 1]];

            if (order.Count > 0)
                distance += _distances[order[order.Count - 1], 0];

            return distance;
        }

        /// <summary>
        ///     Get the next _random arrangements of cities
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private List<int> GetNextArrangement(List<int> order)
        {
            var newOrder = order.ToList();

            //we will only rearrange two cities by _random
            //starting point should be always zero - so zero should not be included

            var firstRandomCityIndex = _random.Next(1, newOrder.Count);
            var secondRandomCityIndex = _random.Next(1, newOrder.Count);

            var dummy = newOrder[firstRandomCityIndex];
            newOrder[firstRandomCityIndex] = newOrder[secondRandomCityIndex];
            newOrder[secondRandomCityIndex] = dummy;

            return newOrder;
        }

        /// <summary>
        ///     Annealing Process
        /// </summary>
        public void RunBoltzmannMachine()
        {
            Iteration = 1;

            GetDistanceData();

            var distance = GetTotalDistance(CitiesOrderedList);
            var processedCities = new List<List<int>>();

            while (Temp > _absoluteTemp)
            {
                _nextOrder = GetNextArrangement(CitiesOrderedList);
                var deltaDistance = GetTotalDistance(_nextOrder) - distance;

                //if the new order has a smaller distance
                //or larger distance but meets Boltzmann then accept the arrangement is valid
                if ((deltaDistance < 0) || (distance > 0 && Math.Exp(-deltaDistance/Temp) > _random.NextDouble()))
                {
                    CitiesOrderedList = _nextOrder;
                    processedCities.Add(CitiesOrderedList);
                    distance = deltaDistance + distance;

                    UpdateConsole(CitiesOrderedList, Iteration, distance);
                    Iteration++;
                }

                //turn down the thermostat
                Temp *= _coolingRate;
            }

            LeastDistance = distance;
        }

        private void UpdateConsole(List<int> citiesOrderedList, int iteration, double distance)
        {
            if (citiesOrderedList == null) return;

            var currentCityOrder = FormatCityList(CitiesOrderedList);
            Console.WriteLine("iteration #: {0} Order of Cities: {1} Distance: {2}", iteration, currentCityOrder,
                distance);
        }

        private static string FormatCityList(List<int> citiesOrderedList)
        {
            var citesAsLetters = ConvertCitiesToLetters(citiesOrderedList);
            var formattedCities = citesAsLetters.Where(city => citesAsLetters.Last() != city)
                .Aggregate<string, string>(null, (current, city) => current + city + " -> ");
            formattedCities += citesAsLetters.Last();

            return formattedCities;
        }

        /// <summary>
        ///     Not best practice!!!! But it works here. Conversion should not be done like this ...
        /// </summary>
        /// <param name="citiesOrderedList"></param>
        /// <returns></returns>
        private static List<string> ConvertCitiesToLetters(IEnumerable<int> citiesOrderedList)
        {
            var cityLettersList = new List<string>();
            foreach (var city in citiesOrderedList)
            {
                if (city == 0) cityLettersList.Add("A");
                if (city == 1) cityLettersList.Add("B");
                if (city == 2) cityLettersList.Add("C");
                if (city == 3) cityLettersList.Add("D");
                if (city == 4) cityLettersList.Add("E");
            }

            return cityLettersList;
        }
    }
}