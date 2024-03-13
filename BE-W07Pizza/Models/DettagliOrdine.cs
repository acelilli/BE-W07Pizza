namespace BE_W07Pizza.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DettagliOrdine")]
    public partial class DettagliOrdine
    {
        [Key]
        public int IDDettaglio { get; set; }

        public int IDOrdine { get; set; }

        public int IDArticolo { get; set; }

        public int Quantita { get; set; }

        public virtual Articoli Articoli { get; set; }

        public virtual Ordini Ordini { get; set; }
    }
}
