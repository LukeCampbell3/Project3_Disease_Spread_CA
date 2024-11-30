using Project3_Disease_Spread_CA;
using System;

public class Location
{
    /// <summary>
    /// each location has their own id which is "city_#"
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// each city has a population built of people this is an ICollection rather than array or list
    /// </summary>
    public ICollection<Person> People { get; set; }

    /// <summary>
    /// an ICollection of locations close to the city object
    /// </summary>
    public ICollection<Location> Neighbors { get; set; }

    /// <summary>
    /// init instance of config
    /// </summary>
    private readonly Config _config;

    /// <summary>
    /// init random
    /// one instance so one "random" value to go by
    /// </summary>
    private static readonly Random randy = new Random(); 

    /// <summary>
    /// Does the city have a cure developed?
    /// Each "hour" a city has a chance of developing a cure
    /// default is false
    /// </summary>
    public bool HasCure { get; private set; } = false;

    /// <summary>
    /// This takes account for a decrementing DiseaseSpread chance
    /// </summary>
    public double DiseaseSpreadChance { get; private set; }

    /// <summary>
    /// constructor for a location
    /// needs an id
    /// needs to read from the config so takes in a config object
    /// </summary>
    /// <param name="id">string containing "city_#" <seealso cref="Id"/></param>
    /// <param name="config">A Config object that takes in the reader and contents of config.txt file <seealso cref="_config"/></param>
    public Location(string id, Config config)
    {
        Id = id;
        People = new List<Person>();
        Neighbors = new List<Location>();
        _config = config;
        DiseaseSpreadChance = config.DiseaseSpreadChance;
    }

    /// <summary>
    /// General processes for each hour
    /// each city needs to update it's population
    /// this checks for infected, dead, quarintined, healthy, and travelling
    /// </summary>
    public void UpdatePeople()
    {
        // Logic to update people status at this location
        foreach (Person person in People.ToList()) // Using .ToList() to safely modify collection during iteration
        {
            if (person.IsTravelling)
            {
                // Decrement travel duration
                if (person.TravelDuration > 0)
                {
                    person.TravelDuration--;
                }
                else
                {
                    // Travel complete - move person to the next location
                    if (person.NextLocation != null)
                    {
                        person.IsTravelling = false;
                        person.TravelDuration = 0;

                        // Remove from current location and add to the next location
                        People.Remove(person);
                        person.CurrentLocation = person.NextLocation;
                        person.NextLocation.People.Add(person);
                        person.NextLocation = null;
                    }
                }

                continue; // Skip further updates for this person since they are traveling
            }
            if (person.IsInfected)
            {
                // Logic for spreading disease
                var healthyPeople = People
                    .Where(p => !p.IsInfected && !p.IsQuarantined && !p.IsTravelling)
                    .ToList();

                if (healthyPeople.Count > 0)
                {
                    Person randomHealthyPerson = healthyPeople[randy.Next(healthyPeople.Count)];
                    // takes in the effective disease spread based on if city is cured...
                    if (IsEventSuccessful(DiseaseSpreadChance))
                    {
                        randomHealthyPerson.IsInfected = true;
                        randomHealthyPerson.InfectionCount++;
                        person.InfectionSpreadCount++;
                    }
                }

                if (IsEventSuccessful(_config.QuarantinePercentChance) && !person.IsQuarantined)
                {
                    // Trigger quarantine for the infected person
                    person.IsQuarantined = true;
                    person.QuarantineDuration = _config.QuarantineDuration;
                }

                if (IsEventSuccessful(_config.DiseaseDeathChance))
                {
                    person.IsDead = true;
                }
            }
            else if (person.IsQuarantined)
            {
                if (person.QuarantineDuration > 0)
                {
                    person.QuarantineDuration--;
                }
                else
                {
                    person.IsQuarantined = false;
                }

                if (IsEventSuccessful(_config.DiseaseDeathChance))
                {
                    person.IsDead = true;
                }
            }
            else
            {
                // Non-infected, non-quarantined person can initiate travel
                //if (!person.IsInfected && !person.IsQuarantined && person.ShouldTravel() && Neighbors.Count > 0)
                //{
                //    person.IsTravelling = true;
                //    person.TravelDuration = 3; // Set travel duration to 3 "hours"
                //    person.NextLocation = Neighbors.ElementAt(randy.Next(Neighbors.Count));
                //}

                // Logic for non-infected, non-quarantined person
                if (!person.IsInfected && !person.IsQuarantined && IsEventSuccessful(_config.TravelPercentChance) && Neighbors.Count > 0)
                {
                    var potentialDestinations = Neighbors.ToList();

                    // Assign weights based on cure status
                    var weightedDestinations = potentialDestinations.Select(city =>
                    {
                        double weight = city.HasCure ? 15.0 : 1.0; // Higher weight for cities with a cure
                        return new { City = city, Weight = weight };
                    }).ToList();

                    // Normalize weights and select a random city based on weights
                    double totalWeight = weightedDestinations.Sum(w => w.Weight);
                    double randomValue = randy.NextDouble() * totalWeight;

                    Location chosenCity = null;
                    double cumulativeWeight = 0;

                    foreach (var destination in weightedDestinations)
                    {
                        cumulativeWeight += destination.Weight;
                        if (randomValue <= cumulativeWeight)
                        {
                            chosenCity = destination.City;
                            break;
                        }
                    }

                    // Assign the chosen city as the next location
                    person.NextLocation = chosenCity;
                    person.IsTravelling = true;
                    person.TravelDuration = 3; // Travel duration
                }
            }
        }
    }

    /// <summary>
    /// Method to attempt a cure
    /// Also decrements the disease chance over time
    /// </summary>
    public void AttemptCure()
    {
        if (!HasCure && IsEventSuccessful(_config.CureChance))
        {
            HasCure = true;
            Console.WriteLine($"Cure discovered in {Id}!");
        }

        if (HasCure)
        {
            // Decrement the disease spread chance over time
             DiseaseSpreadChance = Math.Max(0, _config.DiseaseSpreadChance - 0.01); // Ensure it doesn't go below 0
        }
    }

    /// <summary>
    /// Handles each probablility
    /// 
    /// Takes each value ex: 0.12 and turns it into 12
    /// then takes a random number from 0-100
    /// if the random nmber is less than the percentage (whole number) then it hit
    /// </summary>
    /// <param name="chance">the percentage read in from <seealso cref="_config"/></param>
    /// <returns></returns>
    public static bool IsEventSuccessful(double chance)
    {
        int percentage = (int)(chance * 100);
        int randomValue = randy.Next(0, 101); 
        return randomValue < percentage;
    }
}
