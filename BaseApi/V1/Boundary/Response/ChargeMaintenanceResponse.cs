using ChargeApi.V1.Domain;
using System;
using System.Collections.Generic;

namespace ChargeApi.V1.Boundary.Response
{
    public class ChargeMaintenanceResponse
    {
        /// <summary>
        /// Id of the Charge Maintenance Record
        /// </summary>
        /// /// <example>
        /// 793dd4ca-d7c4-4110-a8ff-c58eac4b90a7
        /// </example>
        public Guid Id { get; set; }

        /// <summary>
        /// Id of the Charges in the Charged Record
        /// </summary>
        /// <example>
        /// 793dd4ca-d7c4-4110-a8ff-c58eac4b90a7
        /// </example>
        public Guid ChargesId { get; set; }

        /// <summary>
        /// Information about existing charges
        /// </summary>
        /// <example>
        /// [
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
        public IEnumerable<DetailedCharges> ExistingValue { get; set; }

        /// <summary>
        /// Information about new charges
        /// </summary>
        /// <example>
        /// [
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
        public IEnumerable<DetailedCharges> NewValue { get; set; }

        /// <summary>
        /// Reason for the charge update
        /// </summary>
        /// <example>
        /// "Uplifting or Adjustment as lift wasn’t working for 6 months or Tenancy changes or annual tenancy change or anything else"
        /// </example>
        public string Reason { get; set; }

        /// <summary>
        /// Charges Applicable Start Date
        /// </summary>
        /// <example>
        /// 2021-07-01
        /// </example>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Charged Aplicable Status 
        /// </summary>
        /// <example>
        /// Values: [Pending, Applied]
        /// </example>
        public ChargeMaintenanceStatus Status { get; set; }
    }
}
