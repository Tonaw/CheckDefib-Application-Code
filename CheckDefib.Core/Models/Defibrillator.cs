using System;
using System.ComponentModel.DataAnnotations;

namespace CheckDefib.Core.Models
{

    public enum DefibType { Semi, Automatic};

    public class Defibrillator
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Defibrillator Name is Required")]
        public string DefibName { get; set; }

        [Required(ErrorMessage = "Postcode is Required")]
        [RegularExpression(@"^([A-PR-UWYZ]([0-9]{1,2}|([A-HK-Y][0-9]|[A-HK-Y][0-9]([0-9]|[ABEHMNPRV-Y]))|[0-9][A-HJKS-UW])\ [0-9][ABD-HJLNP-UW-Z]{2}|(GIR\ 0AA)|(SAN\ TA1)|(BFPO\ (C\/O\ )?[0-9]{1,4})|((ASCN|BBND|[BFS]IQQ|PCRN|STHL|TDCU|TKCA)\ 1ZZ))$", ErrorMessage = "Invalid Postcode")]
        public string Postcode { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        public string Address {get; set; }

        [Required(ErrorMessage = "Please input the Latitude")]
        public double Latitude {get; set; }


        [Required(ErrorMessage = "Please input the Longitude")]
        public double Longitude {get; set; }

        [Required(ErrorMessage = "Please input the Defibrillator Type")]
        public DefibType DefibType {get; set; }

        public double closestDefib {get; set; }

        public IList<Defibrillator> Defibrillators {get; set; } = new List<Defibrillator>();
        
    }
}