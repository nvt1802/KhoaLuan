//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KhoaLuan.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BillDetail
    {
        public int BillDetailsID { get; set; }
        public Nullable<int> BillID { get; set; }
        public Nullable<System.DateTime> FromDay { get; set; }
        public Nullable<System.DateTime> ToDay { get; set; }
        public Nullable<bool> BillStatus { get; set; }
        public Nullable<long> RoomRates { get; set; }
        public Nullable<long> ElectricityPrice { get; set; }
        public Nullable<long> WaterPrice { get; set; }
        public Nullable<long> InternetPrice { get; set; }
    
        public virtual Bill Bill { get; set; }
    }
}
