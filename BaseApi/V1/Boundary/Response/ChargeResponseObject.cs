using ChargeApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChargeApi.V1.Boundary.Response
{
    //TODO: Rename to represent to object you will be returning eg. ResidentInformation, HouseholdDetails e.t.c
    public class ChargeResponseObject
    {
        /// <example>
        ///     793dd4ca-d7c4-4110-a8ff-c58eac4b90a7
        /// </example>
        public Guid Id { get; set; }
        /// <example>
        ///     793dd4ca-d7c4-4110-a8ff-c58eac4b90f8
        /// </example>
        public Guid TargetId { get; set; }
        /// <example>
        ///     Asset
        /// </example>
        public TargetType TargetType { get; set; } 
        /// <example>
        ///     [
        ///         {
        ///             "Type":"A454",
        ///             "SubType":"a-5456",
        ///             "Frequency":"Weekly",
        ///             "Amount":1235.21,
        ///             "StartDate":2021-05-11,
        ///             "EndDate":2022-05-11
        ///         },
        ///         {
        ///             "Type":"A454",
        ///             "SubType":"a-5456",
        ///             "Frequency":"Weekly",
        ///             "Amount":1235.21,
        ///             "StartDate":2021-05-11,
        ///             "EndDate":2022-05-11
        ///         }
        ///     ]
        /// </example>
        public IEnumerable<DetailedCharges> DetailedCharges { get; set; }
        /// <example>
        ///     235.153
        /// </example>
        public decimal? TotalAmount => DetailedCharges==null?0:DetailedCharges.Sum(p => p.Amount);
    }
}
