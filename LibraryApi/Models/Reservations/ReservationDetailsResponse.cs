﻿using LibraryApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Models
{
    public class ReservationDetailsResponse
    {
        public int Id { get; set; }
        public string For { get; set; }
        public string Items { get; set; }
        public DateTime? AvailableOn { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
