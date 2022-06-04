﻿using System;
using System.Text.Json.Serialization;

namespace Invoice.API.Data.ValueObjects
{
    public class InvoiceVO
    {
        public long Id { get; set; }
        //[JsonIgnore]
        public long CustomerId { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
        public double Value { get; set; }
        public CustomerVO customer { get; set; }
    }
}
