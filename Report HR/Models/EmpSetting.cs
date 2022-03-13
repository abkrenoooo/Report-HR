namespace Report_HR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EmpSetting
    {
        public int id { get; set; }

        public int EmployeeId { get; set; }

        public double HPonsP { get; set; }

        public double HPonsH { get; set; }

        public double HMinsP { get; set; }

        public double HMinsH { get; set; }

        public double DAbsentD { get; set; }

        public double DAbsentP { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
