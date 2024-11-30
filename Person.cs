using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project3_Disease_Spread_CA
{
    public class Person
    {
        // Variables

        /// <summary>
        /// each person's id so determining max spreader/infected is easier
        /// string (just like city.Id)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Time in which travel has began
        /// int value
        /// </summary>
        public int TravelStartTime { get; set; }

        /// <summary>
        /// Time in which a person has made it to the next location
        /// int value
        /// </summary>
        public int TravelEndTime { get; set; }

        /// <summary>
        /// Is person infected?
        /// boolean value
        /// </summary>
        public bool IsInfected { get; set; }

        /// <summary>
        /// amount a person is infected
        /// int value
        /// </summary>
        public int InfectionCount { get; set; }

        /// <summary>
        /// amount of times a person spreads disease
        /// int value
        /// </summary>
        public int InfectionSpreadCount { get; set; }

        /// <summary>
        /// Is the person dead from the disease?
        /// boolean value
        /// </summary>
        public bool IsDead { get; set; }

        /// <summary>
        /// Is the person quarantining?
        /// boolean value
        /// </summary>
        public bool IsQuarantined { get; set; }

        /// <summary>
        /// Is the person travelling?
        /// boolean value
        /// </summary>
        public bool IsTravelling { get; set; }

        /// <summary>
        /// chance in which a person can quarantine
        /// double to convey percentage
        /// </summary>
        public double QuarantineChance { get; set; }

        /// <summary>
        /// How long a quarantine lasts
        /// int value
        /// </summary>
        public int QuarantineDuration { get; set; }

        /// <summary>
        /// How long a travel takes
        /// int value
        /// </summary>
        public int TravelDuration { get; set; }

        /// <summary>
        /// the location the person is currently at
        /// uses Location object
        /// </summary>
        public Location CurrentLocation { get; set; }

        /// <summary>
        /// If a travel is initiated then this is where the person is going
        /// uses Location object
        /// </summary>
        public Location NextLocation { get; set; }

        /// <summary>
        /// instance of the config class
        /// (same as <see cref="Location"/>)
        /// </summary>
        private readonly Config _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Person"/> class.
        /// </summary>
        public Person(string id, int travelStart, int travelEnd, double quarantineChance, int quarantineDuration, Config config)
        {
            Id = id;
            TravelStartTime = travelStart;
            TravelEndTime = travelEnd;
            IsInfected = false;
            InfectionCount = 0;
            InfectionSpreadCount = 0;
            IsDead = false;
            IsQuarantined = false;
            IsTravelling = false;
            QuarantineChance = quarantineChance;
            QuarantineDuration = quarantineDuration;
            TravelDuration = 0; // Initialize to zero or an appropriate default value
            _config = config ?? throw new ArgumentNullException(nameof(config)); // Ensure config is not null
        }
    }
}
