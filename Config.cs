using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project3_Disease_Spread_CA
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;

    public class Config
    {
        /// <summary>
        /// Average population size of each city
        /// double 
        /// </summary>
        public double MeanPopulationSize { get; private set; }

        /// <summary>
        /// standard deviation of each cities' population size
        /// double
        /// </summary>
        public double StdDevPopulationSize { get; private set; }

        /// <summary>
        /// Chance the disease has to spread
        /// double
        /// </summary>
        public double DiseaseSpreadChance { get; private set; }

        /// <summary>
        /// Chance the disease has of killing someone
        /// double
        /// </summary>
        public double DiseaseDeathChance { get; private set; }

        /// <summary>
        /// Length a quarantine
        /// int value
        /// </summary>
        public int QuarantineDuration { get; private set; }

        /// <summary>
        /// Chance a person has of quarantining
        /// double
        /// </summary>
        public double QuarantinePercentChance { get; private set; }

        /// <summary>
        /// How long the simulation is set to last
        /// (however if the simulation ends early then it stops)
        /// int value
        /// </summary>
        public int SimulationHours { get; private set; }

        /// <summary>
        /// Chance a person has of travelling to a neighboring location
        /// double
        /// </summary>
        public double TravelPercentChance { get; private set; }

        /// <summary>
        /// number of locations
        /// int value
        /// </summary>
        public int NumOfLocations { get; private set; }

        /// <summary>
        /// Chance a city has of developing a cure to the disease
        /// double
        /// </summary>
        public double CureChance { get; private set; }

        /// <summary>
        /// init config
        /// </summary>
        /// <param name="filePath">takes in the path to the config.txt file</param>
        public Config(string filePath)
        {
            LoadConfig(filePath);
        }

        /// <summary>
        /// sets each parameter from config.txt so it is usable in this simulation
        /// </summary>
        /// <param name="filePath">string value that contains the path to the config.txt file</param>
        /// <exception cref="FileNotFoundException">if the file s not found or exists</exception>
        private void LoadConfig(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine(filePath);
                throw new FileNotFoundException("Config file not found.");
            }

            string[] lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"^(\w+):\s*(.+)$");
                if (match.Success)
                {
                    string key = match.Groups[1].Value.Trim();
                    string value = match.Groups[2].Value.Trim();
                    AssignValue(key, value);
                }
            }
        }

        /// <summary>
        /// assigns each value from the config.txt file to a varaible in C#
        /// </summary>
        /// <param name="key">string containing the variable</param>
        /// <param name="value">the value the variable is to be set to</param>
        private void AssignValue(string key, string value)
        {
            switch (key)
            {
                case "MeanPopulationSize":
                    MeanPopulationSize = double.Parse(value);
                    break;
                case "StdDevPopulationSize":
                    StdDevPopulationSize = double.Parse(value);
                    break;
                case "DiseaseSpreadChance":
                    DiseaseSpreadChance = double.Parse(value);
                    break;
                case "DiseaseDeathChance":
                    DiseaseDeathChance = double.Parse(value);
                    break;
                case "QuarantineDuration":
                    QuarantineDuration = int.Parse(value);
                    break;
                case "QuarantinePercentChance":
                    QuarantinePercentChance = double.Parse(value);
                    break;
                case "SimulationHours":
                    SimulationHours = int.Parse(value);
                    break;
                case "TravelPercentChancePerHour":
                    TravelPercentChance = double.Parse(value);
                    break;
                case "NumberOfLocations":
                    NumOfLocations = int.Parse(value);
                    break;
                case "CureChance":
                    CureChance = double.Parse(value);
                    break;
                default:
                    Console.WriteLine($"Unknown configuration key: {key}");
                    break;
            }
        }
    }
}
