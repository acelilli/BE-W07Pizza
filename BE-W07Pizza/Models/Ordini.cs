namespace BE_W07Pizza.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ordini")]
    public partial class Ordini
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ordini()
        {
            DettagliOrdine = new HashSet<DettagliOrdine>();
        }

        [Key]
        public int IDOrdine { get; set; }

        public int IDUtente { get; set; }

        [Required]
        [StringLength(30)]
        public string Nome { get; set; }

        [Required]
        [StringLength(30)]
        public string Cognome { get; set; }

        [Required]
        [StringLength(60)]
        public string Indirizzo { get; set; }

        public string Note { get; set; }

        public bool? Evaso { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DettagliOrdine> DettagliOrdine { get; set; }

        public virtual Utenti Utenti { get; set; }
    }
}
