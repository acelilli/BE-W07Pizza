﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
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
            // Recupera l'ID ordine dal cookie
            int idOrdine = 0;
            HttpCookie ordineCookie = Request.Cookies["IDOrdineCookie"];
            if (ordineCookie != null && !string.IsNullOrEmpty(ordineCookie.Value))
            {
                idOrdine = Convert.ToInt32(ordineCookie.Value);
            }
            else
            {
                TempData["Messaggio"] = "Questo utente non ha ordini attivi!";
                return RedirectToAction("Index", "Home");
            }

            // Se l'ID non è specificato, assegnamo l'ID recuperato dal cookie
            if (!id.HasValue)
            {
                id = idOrdine;
            }

            // Se l'ID dell'ordine è ancora null, restituisci un errore BadRequest
            if (id == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Trova l'ordine corrispondente all'ID
            Ordini ordine = db.Ordini.Find(id);
            if (ordine == null)
            {
                // Se non viene trovato alcun ordine corrispondente all'ID, restituisci un errore HttpNotFound
                return HttpNotFound();
            }

            // Ora che abbiamo l'ID dell'ordine, possiamo eseguire ulteriori operazioni, come trovare i dettagli dell'ordine associati
            var dettagliOrdine = db.DettagliOrdine.Where(d => d.IDOrdine == id).ToList();
            ViewBag.OrdineCorrente = ordine;
            ViewBag.DettagliOrdine = dettagliOrdine;

            return View(ordine);
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
                db.Ordini.Add(ordini);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDUtente = new SelectList(db.Utenti, "IDUtente", "NomeUtente", ordini.IDUtente);
            return View(ordini);
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
        public ActionResult Edit([Bind(Include = "IDOrdine,Nome,Cognome,Indirizzo,Note,Evaso")] Ordini ordini)
        {
            // Recupera l'ID utente dal cookie
            int idUtente = 0; // Valore predefinito nel caso in cui non sia possibile recuperare l'ID utente dai cookie
            HttpCookie cookie = Request.Cookies["IDUserCookie"];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                idUtente = Convert.ToInt32(cookie.Value);
            }

            // Imposta l'ID utente nell'entità Ordini
            ordini.IDUtente = idUtente;

            if (ModelState.IsValid)
            {
                db.Entry(ordini).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Utenti");
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
            int idUtente = 0; // Valore predefinito 
            HttpCookie cookie = Request.Cookies["IDUserCookie"];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                idUtente = Convert.ToInt32(cookie.Value);
            }

            Ordini ordini;
            ordini = db.Ordini.Find(id);

            // Se l'ordine esiste
            if (ordini != null)
            {
                // Imposta l'ID utente nell'entità Ordini
                ordini.IDUtente = idUtente;
                db.Ordini.Remove(ordini);
                // Rimuovi tutti i dettagli correlati
                var dettagliCorrelati = db.DettagliOrdine.Where(d => d.IDOrdine == id);
                db.DettagliOrdine.RemoveRange(dettagliCorrelati);
                db.SaveChanges();
            }
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
