using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BE_W07Pizza.Models;

namespace BE_W07Pizza.Controllers
{
    public class UtentiController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: Utenti
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View(db.Utenti.ToList());
        }

        // GET: Utenti/Details/5
        public ActionResult Details(int? id)
        {
            // Recupera l'ID dell'utente dal cookie
            int idUtente = 0; // Valore predefinito nel caso in cui non sia possibile recuperare l'IDUtente dal cookie
            HttpCookie cookie = Request.Cookies["IDUserCookie"];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                idUtente = Convert.ToInt32(cookie.Value);
            }

            // Se non lo prendiamo dall'elenco utenti -> prende l'ID dal cookie
            if (id == null)
            {
                id = idUtente;
            }

            // Se è null
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Trova l'utente corrispondente all'ID
            Utenti utenti = db.Utenti.Find(id);
            if (utenti == null)
            {
                return HttpNotFound();
            }
            // Nella tabella ordine ( + dettagli Ordine ) trova IDUtente associato -> lista
            var ordiniUtenteDettagli = db.Ordini.Include("DettagliOrdine")
                .Where(o => o.IDUtente == id).ToList();

            return View(utenti);
        }

        // GET: Utenti/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Utenti/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDUtente,NomeUtente,Password,Email,Ruolo")] Utenti utenti)
        {
            if (ModelState.IsValid)
            {
                db.Utenti.Add(utenti);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(utenti);
        }

        // GET: Utenti/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utenti utenti = db.Utenti.Find(id);
            if (utenti == null)
            {
                return HttpNotFound();
            }
            return View(utenti);
        }

        // POST: Utenti/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDUtente,NomeUtente,Password,Email,Ruolo")] Utenti utenti)
        {
            if (ModelState.IsValid)
            {
                db.Entry(utenti).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(utenti);
        }

        // GET: Utenti/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utenti utenti = db.Utenti.Find(id);
            if (utenti == null)
            {
                return HttpNotFound();
            }
            return View(utenti);
        }

        // POST: Utenti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Utenti utenti = db.Utenti.Find(id);
            db.Utenti.Remove(utenti);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin")]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //////// LOGIN //////////////////////
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Utenti user)
        {
            try
            {
                var utente = db.Utenti.FirstOrDefault(u => u.NomeUtente == user.NomeUtente && u.Password == user.Password);

                if (utente != null)
                {
                    FormsAuthentication.SetAuthCookie(user.NomeUtente, false);
                    HttpCookie EnterCookie = new HttpCookie("IDUserCookie");
                    EnterCookie.Value = utente.IDUtente.ToString();
                    EnterCookie.Expires = DateTime.Now.AddHours(1);
                    Response.Cookies.Add(EnterCookie);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.AuthError = "Login non riuscito";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.AuthError = "Errore durante l'autenticazione: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Errore: " + ex.Message);
                return View();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
