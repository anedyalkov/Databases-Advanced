﻿namespace Stations.Models
{
    using Stations.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Trip
    {
        public int Id { get; set; }

        public int OriginStationId { get; set; }

        [Required]
        public Station OriginStation { get; set; }

        public int DestinationStationId { get; set; }

        [Required]
        public Station DestinationStation { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        public int TrainId { get; set; }

        [Required]
        public Train Train { get; set; }

        public TripStatus Status { get; set; }

        public TimeSpan? TimeDifference { get; set; }
    }
}