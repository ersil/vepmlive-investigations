//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EPMLive.OnlineLicensing.Api.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class vwAccountOrder
    {
        public string plimusReferenceNumber { get; set; }
        public int quantity { get; set; }
        public string expiration { get; set; }
        public Nullable<int> contractlevel { get; set; }
        public System.Guid account_id { get; set; }
        public int account_ref { get; set; }
        public Nullable<int> storage { get; set; }
        public Nullable<int> ProjectLimit { get; set; }
        public int plimusAccountId { get; set; }
        public Nullable<System.DateTime> dtcreated { get; set; }
        public Nullable<int> billsystem { get; set; }
        public Nullable<bool> enabled { get; set; }
        public string BillingSys { get; set; }
        public System.Guid order_id { get; set; }
    }
}
