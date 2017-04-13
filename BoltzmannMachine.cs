using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TSP
{
    public class BoltzmannMachine
    {
        private readonly Random _random = new Random();
        private double[,] _distances;
        private List<int> _nextOrder = new List<int>();

        public BoltzmannMachine()
        {
            CitiesOrderedList = new List<int>();
            LeastDistance = 0;
        }

        public double LeastDistance { get; private set; }

        public string FilePath { private get; set; }

        public List<int> CitiesOrderedList { get; private set; }

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
            var iteration = 1;
            var temp = 10000.0;
            double deltaDistance;
            var coolingRate = 0.9999;
            var absoluteTemp = 0.00001;

            GetDistanceData();

            var distance = GetTotalDistance(CitiesOrderedList);
            var processedCities = new List<List<int>>();

            while (temp > absoluteTemp)
            {
                _nextOrder = GetNextArrangement(CitiesOrderedList);
                deltaDistance = GetTotalDistance(_nextOrder) - distance;

                //if the new order has a smaller distance
                //or larger distance but meets Boltzmann then accept the arrangement is valid
                if ((deltaDistance < 0) || (distance > 0 && Math.Exp(-deltaDistance/temp) > _random.NextDouble()))
                {
                    CitiesOrderedList = _nextOrder;
                    processedCities.Add(CitiesOrderedList);
                    distance = deltaDistance + distance;

                    UpdateConsole(CitiesOrderedList, iteration, distance);
                    iteration++;
                }

                //turn down the thermostat
                temp *= coolingRate;
            }

            LeastDistance = distance;
        }

        private void UpdateConsole(List<int> citiesOrderedList, int iteration, double distance)
        {
            var currentCityOrder = FormatCityList(CitiesOrderedList);
            Console.WriteLine("iteration #: {0} Order of Cities: {1} Distance: {2}", iteration, currentCityOrder,
                distance);
        }

        private static string FormatCityList(List<int> citiesOrderedList)
        {
            var formattedCities =
                citiesOrderedList.Where(city => citiesOrderedList.Last() != city)
                    .Aggregate<int, string>(null, (current, city) => current + (city + " -> "));
            formattedCities += citiesOrderedList.Last();

            return formattedCities;
        }
    }
}