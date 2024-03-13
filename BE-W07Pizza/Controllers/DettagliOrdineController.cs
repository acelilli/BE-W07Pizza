using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
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

        /// <summary>
        /// ////////////////////////////////
        ///
        private int GetCurrentUserId()
        {
            // Ottieni l'istanza del controller OrdiniController dal contesto
            var ordiniController = DependencyResolver.Current.GetService<OrdiniController>();

            // Chiamata al metodo nel controller OrdiniController per ottenere l'ID utente corrente
            return ordiniController.GetCurrentUserId();
        }
        // POST: DettagliOrdine/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDDettaglio,IDOrdine,IDArticolo,Quantita")] DettagliOrdine dettagliOrdine)
        {
            try
            {
                int idUtente = GetCurrentUserId(); // Otteniamo l'ID dell'utente corrente
                                                   // Controlla se l'utente ha già un ordine attivo che NON evaso
                Ordini ordineAttivo = db.Ordini.FirstOrDefault(o => o.IDUtente == idUtente && o.Evaso == false);
                // SE ORDINEATTIVO == null crea un nuovo ordine
                if (ordineAttivo == null)
                {   // NB: ordine -> OrdineController, proprio quello
                    ordineAttivo = new Ordini
                    {
                        IDUtente = idUtente,
                        Evaso = false
                    };
                    db.Ordini.Add(ordineAttivo);
                }
                else
                {
                    // Aggiornamento dell'ordine esistente
                    ordineAttivo.Evaso = false; // Aggiorna altre proprietà se necessario
                    db.Entry(ordineAttivo).State = EntityState.Modified;
                }

                // Avendo un ordine attivo, possiamo aggiungere l'articolo al carrello (all'ordine)
                dettagliOrdine.IDOrdine = ordineAttivo.IDOrdine; // Imposta l'ID dell'ordine sul dettaglio dell'ordine
                db.DettagliOrdine.Add(dettagliOrdine);
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Property: {ve.PropertyName}");
                        System.Diagnostics.Debug.WriteLine($"Error: {ve.ErrorMessage}");
                        System.Diagnostics.Debug.WriteLine($"Error: {ve.ErrorMessage}");
                    }
                }
            }

            return RedirectToAction("Index", "Home"); 
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
    }
}
