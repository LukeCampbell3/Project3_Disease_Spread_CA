namespace Project3_Disease_Spread_CA
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // I used Chatgpt, but since I uploaded images I cannot upload link :(
            // "Sharing conversations with user uploaded images is not yet supported"


            Random randy = new Random();

            // I HATE RELATIVE PATHING IN VS
            // This is the convoluded way to get a SIMPLE path to a file that is in the same dir as the program
            // thanks for reading my rant
            string configPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "config.txt");

            // Load the configuration
            Config config = new Config(configPath);

            string filePath = GetMetricsFilePath();

            // Clear or recreate metrics.csv file
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Create locations
            List<Location> locations = new List<Location>();
            for (int i = 0; i < config.NumOfLocations; i++)
            {
                locations.Add(new Location($"City_{i + 1}", config));
            }

            //Connect locations sequentially
            for (int i = 0; i < locations.Count; i++)
            {
                if (i > 0) locations[i].Neighbors.Add(locations[i - 1]); // Connect to the previous city
                if (i < locations.Count - 1) locations[i].Neighbors.Add(locations[i + 1]); // Connect to the next city
            }

            // Randomly generate connections between locations (neighbors)
            //GenerateConnectedGraph(locations, randy);

            // Populate each location with people
            foreach (var location in locations)
            {
                int population = (int)Math.Max(1, Math.Round(randy.NextDouble() * config.StdDevPopulationSize + config.MeanPopulationSize));
                for (int i = 0; i < population; i++)
                {
                    var person = new Person($"Person_{location.Id}_{i}", randy.Next(0, 24), randy.Next(0, 24), randy.NextDouble(), config.QuarantineDuration, config);
                    location.People.Add(person);
                }

                // Infect random people in each location (5 infected initially)
                for (int i = 0; i < 3 && location.People.Count > 0; i++)
                {
                    int randomIndex = randy.Next(location.People.Count);
                    location.People.ElementAt(randomIndex).IsInfected = true;
                }
            }

            PrintCityConnections(locations);

            // Open StreamWriter for the entire simulation
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                // Write the header
                sw.WriteLine("Hour,MostInfectedPerson,MostSpreaderPerson,AlivePeople,TotalDead,TotalInfected,TotalQuarantined,CityPopulations");

                

                // Simulation loop
                int simulationHours = config.SimulationHours;
                for (int hour = 0; hour < simulationHours; hour++)
                {
                    // Count alive people during this hour
                    int totalNotDead = 0; 

                    foreach (var location in locations)
                    {
                        // Attempt cure for city
                        location.AttemptCure();

                        // Update people in the city
                        location.UpdatePeople();

                        // Count total alive people in this location
                        totalNotDead += location.People.Count(p => !p.IsDead);
                    }

                    // End simulation if no one is alive
                    if (totalNotDead == 0)
                    {
                        Console.WriteLine($"Simulation ended early at hour {hour}: No alive people remaining.");
                        break;
                    }

                    // Write metrics for the current hour
                    WriteMetricsToCsv(sw, hour, locations);

                    // Console printout for progress tracking
                    Console.WriteLine($"Hour: {hour} has passed");
                }
            }
        }

        public static void WriteMetricsToCsv(StreamWriter sw, int hour, List<Location> locations)
        {
            // Calculate metrics
            Person mostInfectedPerson = null;
            Person mostSpreaderPerson = null;
            int totalNotDead = 0;
            int totalDead = 0;
            int totalInfected = 0;
            int totalQuarantined = 0;

            foreach (var location in locations)
            {
                foreach (var person in location.People)
                {
                    // Update most infected person
                    if (mostInfectedPerson == null || person.InfectionCount > mostInfectedPerson.InfectionCount)
                    {
                        mostInfectedPerson = person;
                    }

                    // Update most spreader person
                    if (mostSpreaderPerson == null || person.InfectionSpreadCount > mostSpreaderPerson.InfectionSpreadCount)
                    {
                        mostSpreaderPerson = person;
                    }

                    // Count living, dead, infected, and quarantined people
                    if (!person.IsDead)
                    {
                        totalNotDead++;
                    }
                    else
                    {
                        totalDead++;
                    }

                    if (person.IsInfected)
                    {
                        totalInfected++;
                    }

                    if (person.IsQuarantined)
                    {
                        totalQuarantined++;
                    }
                }
            }

            // Initialize metrics for the hour
            string mostInfectedInfo = mostInfectedPerson != null ? $"{mostInfectedPerson.Id}:{mostInfectedPerson.InfectionCount}" : "None";
            string mostSpreaderInfo = mostSpreaderPerson != null ? $"{mostSpreaderPerson.Id}:{mostSpreaderPerson.InfectionSpreadCount}" : "None";
            string cityPopulations = string.Join(";", locations.Select(loc => $"{loc.Id}:{loc.People.Count}"));

            // Write metrics to CSV
            sw.WriteLine($"{hour},{mostInfectedInfo},{mostSpreaderInfo},{totalNotDead},{totalDead},{totalInfected},{totalQuarantined},{cityPopulations}");
            sw.Flush();
        }

        private static string GetMetricsFilePath()
        {
            string projectRootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            return Path.Combine(projectRootPath, "metrics.csv");
        }

        private static void PrintCityConnections(List<Location> locations)
        {
            Console.WriteLine("City Connections:");
            foreach (var location in locations)
            {
                var neighborIds = string.Join(", ", location.Neighbors.Select(n => n.Id));
                Console.WriteLine($"{location.Id} -> [{neighborIds}]");
            }
            Console.WriteLine(); // Add some spacing after the output
        }

        // Randomly allocates locations

        //private static void GenerateConnectedGraph(List<Location> locations, Random rand)
        //{
        //    // Ensure a connected graph using a minimum spanning tree approach
        //    var unconnected = new HashSet<Location>(locations);
        //    var connected = new HashSet<Location>();

        //    // Start with the first city
        //    Location current = unconnected.First();
        //    unconnected.Remove(current);
        //    connected.Add(current);

        //    while (unconnected.Count > 0)
        //    {
        //        // Pick a random connected city
        //        Location fromCity = connected.ElementAt(rand.Next(connected.Count));

        //        // Pick a random unconnected city
        //        Location toCity = unconnected.ElementAt(rand.Next(unconnected.Count));

        //        // Connect them
        //        fromCity.Neighbors.Add(toCity);
        //        toCity.Neighbors.Add(fromCity);

        //        // Move the city from unconnected to connected
        //        unconnected.Remove(toCity);
        //        connected.Add(toCity);
        //    }

        //    // Optionally, add additional random edges
        //    int additionalEdges = rand.Next(locations.Count / 2); // Add some randomness
        //    for (int i = 0; i < additionalEdges; i++)
        //    {
        //        Location cityA = locations[rand.Next(locations.Count)];
        //        Location cityB = locations[rand.Next(locations.Count)];

        //        // Ensure cityA and cityB are not already connected and are not the same
        //        if (cityA != cityB && !cityA.Neighbors.Contains(cityB))
        //        {
        //            cityA.Neighbors.Add(cityB);
        //            cityB.Neighbors.Add(cityA);
        //        }
        //    }
        //}

    }

}
