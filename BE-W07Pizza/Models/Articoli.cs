namespace BE_W07Pizza.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Articoli")]
    public partial class Articoli
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Articoli()
        {
            DettagliOrdine = new HashSet<DettagliOrdine>();
        }

        [Key]
        public int IDArticolo { get; set; }

        [Required]
        [StringLength(30)]
        public string NomeArticolo { get; set; }

        [Required]
        public string FotoArticolo { get; set; }

        [Column(TypeName = "money")]
        public decimal Prezzo { get; set; }

        public int TempoPrepMin { get; set; }

        [Required]
        [StringLength(200)]
        public string Ingredienti { get; set; }

        public bool? Disponibile { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DettagliOrdine> DettagliOrdine { get; set; }
    }
}
