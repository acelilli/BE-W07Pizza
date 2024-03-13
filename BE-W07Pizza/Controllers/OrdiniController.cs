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
    public class OrdiniController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: Ordini
        public ActionResult Index()
        {
            var ordini = db.Ordini.Include(o => o.Utenti);
            return View(ordini.ToList());
        }

        // GET: Ordini/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            return View(ordini);
        }

        // GET: Ordini/Create
        public ActionResult Create()
        {
            ViewBag.IDUtente = new SelectList(db.Utenti, "IDUtente", "NomeUtente");
            return View();
        }

        // POST: Ordini/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDOrdine,IDUtente,Nome,Cognome,Indirizzo,Note,Evaso")] Ordini ordini)
        {
            if (ModelState.IsValid)
            {
                // L'utente ha un ordine che non è stato evaso?
                int idUtente = GetCurrentUserId(); // Otteniamo l'ID dell'utente corrente
                Ordini ordineEsistente = db.Ordini.FirstOrDefault(o => o.IDUtente == idUtente && o.Evaso == false);

                if (ordineEsistente == null)
                {
                    // Se non esiste un ordine attivo per l'utente, crea un nuovo ordine
                    ordini.IDUtente = idUtente;
                    db.Ordini.Add(ordini);
                    db.SaveChanges();
                }
                else
                {
                    ordini.IDOrdine = ordineEsistente.IDOrdine;
                }

                return RedirectToAction("Index");
            }

            ViewBag.IDUtente = new SelectList(db.Utenti, "IDUtente", "NomeUtente", ordini.IDUtente);
            return View(ordini);

        }
        /// <summary>
        /// ////////////////////////////////////////////////////////////////
        /// PRENDIAMO L'USER ID
        /// // il metodo deve essere PUBLIC così me lo posso portare negli altri controller senza riscriverlo
        public int GetCurrentUserId()  
        {

            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                // Cerca l'utente nel database utilizzando l'username
                Utenti utenteCorrente = db.Utenti.FirstOrDefault(u => u.NomeUtente == username);

                if (utenteCorrente != null)
                {
                    // Se l'utente esiste nel database, restituisci il suo ID
                    return utenteCorrente.IDUtente;
                }
            }

            // Se l'utente non è autenticato o non esiste nel database, restituisce ->
            return -1; // cioè assenza di un ID utente valido
        }


        // GET: Ordini/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDUtente = new SelectList(db.Utenti, "IDUtente", "NomeUtente", ordini.IDUtente);
            return View(ordini);
        }

        // POST: Ordini/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDOrdine,IDUtente,Nome,Cognome,Indirizzo,Note,Evaso")] Ordini ordini)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ordini).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDUtente = new SelectList(db.Utenti, "IDUtente", "NomeUtente", ordini.IDUtente);
            return View(ordini);
        }

        // GET: Ordini/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            return View(ordini);
        }

        // POST: Ordini/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ordini ordini = db.Ordini.Find(id);
            db.Ordini.Remove(ordini);
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
