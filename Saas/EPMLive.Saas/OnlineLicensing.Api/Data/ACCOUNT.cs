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
    
    public partial class Account
    {
        public System.Guid account_id { get; set; }
        public int account_ref { get; set; }
        public Nullable<System.Guid> owner_id { get; set; }
        public Nullable<int> maxusers { get; set; }
        public Nullable<System.DateTime> dtCreated { get; set; }
        public Nullable<System.Guid> creator_id { get; set; }
        public Nullable<bool> inTrial { get; set; }
        public Nullable<int> version { get; set; }
        public Nullable<int> monthsfree { get; set; }
        public Nullable<int> diskquota { get; set; }
        public Nullable<int> billingType { get; set; }
        public Nullable<System.Guid> crmaccountuid { get; set; }
        public string accountDescription { get; set; }
        public Nullable<int> partnerid { get; set; }
        public bool dedicated { get; set; }
        public Nullable<bool> lockusers { get; set; }
        public string internalemail { get; set; }
        public string SalesForceId { get; set; }
    }
}