using System;
using Microsoft.AspNetCore;



namespace CheckDefib.Core.Models
{
   public class StaffLocation
   {

       public double StaffLatitude { get; set; }

       public double StaffLongitude { get; set; }

       public double closestDistance {get; set;}

       public string closestDefibName {get; set;}

       public string closestDefibAddress {get; set;}

   }

}
