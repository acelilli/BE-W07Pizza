using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BE_W07Pizza.Models;

namespace BE_W07Pizza.Controllers
{
    public class ArticoliController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: Articoli
        public ActionResult Index()
        {
            return View(db.Articoli.ToList());
        }

        // GET: Articoli/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articoli articoli = db.Articoli.Find(id);
            if (articoli == null)
            {
                return HttpNotFound();
            }
            return View(articoli);
        }

        // GET: Articoli/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Articoli/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDArticolo,NomeArticolo,FotoArticolo,Prezzo,TempoPrepMin,Ingredienti,Disponibile")] Articoli articoli, HttpPostedFileBase FotoArticolo)
        {
            /*if (ModelState.IsValid)
            {
                 if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Server.MapPath("/Content/Img"), fileName);
                    file.SaveAs(path);
                    articoli.FotoArticolo = "/Content/Img/" + fileName;
                }
                else
                {
                    // Se il file non è stato inviato correttamente, mostra un messaggio di debug
                    Debug.WriteLine("Nessun file inviato.");
                }
            }*/
                if (FotoArticolo != null && FotoArticolo.ContentLength > 0)
                {
                    string nomeFile = Path.GetFileName(FotoArticolo.FileName);
                    string pathToSave = Path.Combine(Server.MapPath("~/Content/Img/"), nomeFile);
                    FotoArticolo.SaveAs(pathToSave);
                    articoli.FotoArticolo = "/Content/Img/" + nomeFile;
                }
                if (ModelState.IsValid)
                {


                    db.Articoli.Add(articoli);
                 db.SaveChanges();
                 return RedirectToAction("Index");
                }
                 return View(articoli);
        }

        // GET: Articoli/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articoli articoli = db.Articoli.Find(id);
            if (articoli == null)
            {
                return HttpNotFound();
            }
            return View(articoli);
        }

        // POST: Articoli/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDArticolo,NomeArticolo,FotoArticolo,Prezzo,TempoPrepMin,Ingredienti,Disponibile")] Articoli articoli)
        {
            if (ModelState.IsValid)
            {
                db.Entry(articoli).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(articoli);
        }

        // GET: Articoli/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articoli articoli = db.Articoli.Find(id);
            if (articoli == null)
            {
                return HttpNotFound();
            }
            return View(articoli);
        }

        // POST: Articoli/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Articoli articoli = db.Articoli.Find(id);
            db.Articoli.Remove(articoli);
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
