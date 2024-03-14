using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BE_W07Pizza.Models;

namespace BE_W07Pizza.Controllers
{
    public class DettagliOrdineController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: DettagliOrdine
        public ActionResult Index()
        {
            var dettagliOrdine = db.DettagliOrdine.Include(d => d.Articoli).Include(d => d.Ordini);
            return View(dettagliOrdine.ToList());
        }

        // GET: DettagliOrdine/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DettagliOrdine dettagliOrdine = db.DettagliOrdine.Find(id);
            if (dettagliOrdine == null)
            {
                return HttpNotFound();
            }
            return View(dettagliOrdine);
        }

        // GET: DettagliOrdine/Create
        public ActionResult Create()
        {
            ViewBag.IDArticolo = new SelectList(db.Articoli, "IDArticolo", "NomeArticolo");
            ViewBag.IDOrdine = new SelectList(db.Ordini, "IDOrdine", "Nome");
            return View();
        }

        // POST: DettagliOrdine/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDDettaglio,IDOrdine,IDArticolo,Quantita")] DettagliOrdine dettagliOrdine)
        {
            if (ModelState.IsValid)
            {
                db.DettagliOrdine.Add(dettagliOrdine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDArticolo = new SelectList(db.Articoli, "IDArticolo", "NomeArticolo", dettagliOrdine.IDArticolo);
            ViewBag.IDOrdine = new SelectList(db.Ordini, "IDOrdine", "Nome", dettagliOrdine.IDOrdine);
            return View(dettagliOrdine);
        }

        // GET: DettagliOrdine/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DettagliOrdine dettagliOrdine = db.DettagliOrdine.Find(id);
            if (dettagliOrdine == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDArticolo = new SelectList(db.Articoli, "IDArticolo", "NomeArticolo", dettagliOrdine.IDArticolo);
            ViewBag.IDOrdine = new SelectList(db.Ordini, "IDOrdine", "Nome", dettagliOrdine.IDOrdine);
            return View(dettagliOrdine);
        }

        // POST: DettagliOrdine/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDDettaglio,IDOrdine,IDArticolo,Quantita")] DettagliOrdine dettagliOrdine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dettagliOrdine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDArticolo = new SelectList(db.Articoli, "IDArticolo", "NomeArticolo", dettagliOrdine.IDArticolo);
            ViewBag.IDOrdine = new SelectList(db.Ordini, "IDOrdine", "Nome", dettagliOrdine.IDOrdine);
            return View(dettagliOrdine);
        }

        // GET: DettagliOrdine/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DettagliOrdine dettagliOrdine = db.DettagliOrdine.Find(id);
            if (dettagliOrdine == null)
            {
                return HttpNotFound();
            }
            return View(dettagliOrdine);
        }

        // POST: DettagliOrdine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DettagliOrdine dettagliOrdine = db.DettagliOrdine.Find(id);
            db.DettagliOrdine.Remove(dettagliOrdine);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        ////////////////////////////////////////////////////////////////
        /////////////////// AGGIUNGI AL CARRELLO ///////////////////////
        [HttpPost]
        public ActionResult AggiungiAlCarrello(int idArticolo)
        {
            try
            {
                // Recupera l'ID dell'utente dal cookie
                int idUtente = 0; // Valore predefinito 
                HttpCookie cookie = Request.Cookies["IDUserCookie"];
                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    idUtente = Convert.ToInt32(cookie.Value);
                }

                if (idUtente != 0)
                {
                    // Cerca un ordine non evaso per l'utente corrente
                    Ordini ordineEsistente = db.Ordini.FirstOrDefault(o => o.IDUtente == idUtente && o.Evaso == false);

                    if (ordineEsistente == null)
                    {
                        // Nessun ordine non evaso esistente =>  crea un nuovo ordine
                        Ordini ordine = new Ordini
                        {
                            IDUtente = idUtente,
                            Nome = User.Identity.Name,
                            Cognome = "Cognome placeholder",
                            Indirizzo = "Via/strada/piazza",
                            Note = "Note per la cucina (allergie, intolleranze), o per la consegna (piano, campanello, scala)",
                            Evaso = false
                        };
                        db.Ordini.Add(ordine);
                        db.SaveChanges();

                        // Aggiungi il dettaglio dell'ordine all'Ordine (new)
                        DettagliOrdine dettaglioOrdine = new DettagliOrdine
                        {
                            IDOrdine = ordine.IDOrdine,
                            IDArticolo = idArticolo,
                            Quantita = 1, 
                        };
                        db.DettagliOrdine.Add(dettaglioOrdine);
                    }
                    else
                    {
                        // ORDINE evaso = false, quindi aggiungo il dettaglioOrdine a Ordine esistente
                        DettagliOrdine dettaglioOrdine = new DettagliOrdine
                        {
                            IDOrdine = ordineEsistente.IDOrdine,
                            IDArticolo = idArticolo,
                            Quantita = 1,
                        };
                        db.DettagliOrdine.Add(dettaglioOrdine);
                    }

                    db.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // SE non riesce a prendere l'IDUtente dal cookie
                    ViewBag.ErrorMessage = "Si è verificato un problema durante l'aggiunta al carrello. Per favore, effettua nuovamente il login.";
                    return RedirectToAction("Login", "Utenti");
                }
            }
            catch (Exception ex)
            {
                // Varie ed eventuali
                ViewBag.ErrorMessage = "Si è verificato un errore durante l'aggiunta al carrello: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }


    }
}
