using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TestMvcApp.Models;

namespace TestMvcApp.Controllers
{
    public class TestController : Controller
    {
        private static TestItemsContext Db = new TestItemsContext();
        private static int idx = 3;

        //
        // GET: /Test/
        public ActionResult Index()
        {
            return View(Db.Items);
        }

        //
        // GET: /Test/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestItem item = Db.Items.Find(a=>a.Id == id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        //
        // GET: /Test/Create
        public ActionResult Create()
        {
            var item = new TestItem();
            item.Id = idx++;
            item.Caption = "Test " + item.Id;

            Db.Items.Add(item);

            return View(item);
        }

        //
        // POST: /Test/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Test/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestItem item = Db.Items.Find(a => a.Id == id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        //
        // POST: /Test/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, TestItem item)
        {
            try
            {
                // TODO: Add update logic here
                TestItem item2 = Db.Items.Find(a => a.Id == item.Id);
                if (item2 == null)
                {
                    return HttpNotFound();
                }
                var idx = Db.Items.IndexOf(item2);
                Db.Items[idx] = item;

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Test/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestItem item = Db.Items.Find(a => a.Id == id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        //
        // POST: /Test/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                TestItem item2 = Db.Items.Find(a => a.Id == id);
                if (item2 == null)
                {
                    return HttpNotFound();
                }

                Db.Items.Remove(item2);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
