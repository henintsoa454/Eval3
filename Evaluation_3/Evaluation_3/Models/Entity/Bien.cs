using System;
using System.Collections.Generic;

namespace Evaluation_3.Models.Entity
{
    public partial class Bien
    {
        public Bien()
        {
            LocationIdbienNavigations = new HashSet<Location>();
            LocationReferencebienNavigations = new HashSet<Location>();
            Photobiens = new HashSet<Photobien>();
        }

        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public int Idtype { get; set; }
        public string Description { get; set; } = null!;
        public int Idregion { get; set; }
        public double Loyermensuel { get; set; }
        public int Idproprietaire { get; set; }
        public string Reference { get; set; } = null!;

        public virtual Proprietaire IdproprietaireNavigation { get; set; } = null!;
        public virtual Region IdregionNavigation { get; set; } = null!;
        public virtual Typebien IdtypeNavigation { get; set; } = null!;
        public virtual ICollection<Location> LocationIdbienNavigations { get; set; }
        public virtual ICollection<Location> LocationReferencebienNavigations { get; set; }
        public virtual ICollection<Photobien> Photobiens { get; set; }

        public DateTime getDateDisponibilité()
        {
            Console.WriteLine("Nombre de location: "+this.LocationIdbienNavigations.Count);
            if(this.LocationIdbienNavigations.Count > 0)
            {
                DateTime result = this.LocationIdbienNavigations.First().Datedebut.AddMonths(this.LocationIdbienNavigations.First().Duree);
                foreach(Location location in this.LocationIdbienNavigations)
                {
                    if(result <= location.Datedebut.AddMonths(location.Duree))
                    {
                        result = location.Datedebut.AddMonths(location.Duree);
                    }
                }
                result = new DateTime(result.Year, result.Month, 1);
                if(result > DateTime.Now)
                {
                    return result;
                }
                else
                {
                    return DateTime.Now;
                }
            }
            else
            {
                return DateTime.Now;
            }
        }
    }
}
