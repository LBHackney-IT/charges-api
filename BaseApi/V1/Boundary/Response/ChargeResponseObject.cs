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
        /// </summary>
        public Guid TargetId { get; set; }
        /// <example>
        ///     Asset
        /// </example>
        public TargetType TargetType { get; set; }
        /// <example>
        /// {
        ///     "TypeCode":1,
        ///     "TypeSource":"SRC001"
        /// }
        /// </example>
        public ChargeType ChargeType { get; set; }
        /// <example>
        ///     A304
        /// </example>
        public string DebitCode { get; set; }
        /// <example>
        ///     Direct Debit Number A304
        /// </example>
        public string DebitCodeDescription { get; set; }
        /// <example>
        ///     2020-06-11
        /// </example>
        public DateTime EffectiveStartDate { get; set; }
        /// <example>
        ///     2021-06-11
        /// </example>
        public DateTime TerminationDate { get; set; }
        /// <example>
        ///     1
        /// </example>
        public string PeriodCode { get; set; }
        /// <example>
        ///     2021-09-11
        /// </example>
        public DateTime DebitNextDue { get; set; }
        /// <example>
        ///     2021-05-11
        /// </example>
        public DateTime DebitLastCharged { get; set; }
        /// <example>
        ///     True
        /// </example>
        public bool DebitActive { get; set; }
        /// <example>
        ///     13.25
        /// </example>
        public decimal DebitValue { get; set; }
        /// <example>
        ///     False
        /// </example>
        public bool PropertyDebit { get; set; }
        /// <example>
        ///     asd213as
        /// </example>
        public string DebitSource { get; set; }
        /// <example>
        ///     2021-05-05
        /// </example>
        public DateTime TimeStamp { get; set; }
        /// <example>
        ///     asdsdf13
        /// </example>
        public string ServiceChargeSchedule { get; set; }
        /// <example>
        ///     asd6s54f
        /// </example>
        public string DataImportSource { get; set; }
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
        public IEnumerable<ChargeDetails> ChargeDetails { get; set; }
        /// <example>
        ///     235.153
        /// </summary>
        public decimal? TotalAmount => ChargeDetails==null?0:ChargeDetails.Sum(p => p.Amount);
    }
}
