using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cloud.Core;
using Cloud.Core.Amazon;
using Cloud.Core.Factory;

namespace Cloud.Web.Controllers
{
    public class AmazonController : Controller
    {
        // GET: Amazon
        public async Task<ActionResult> Index()
        {
            ICloud amazonCloud = new AmazonCreator().CreateCloud();

            if (amazonCloud != null)
            {
                string token = await amazonCloud.GetServiceToken();
                ViewBag.Folders = amazonCloud.GetFolderList("");
                ViewBag.Files = amazonCloud.GetFileList("");
            }

            return View();
        }

        // GET: Amazon/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Amazon/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Amazon/Create
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

        // GET: Amazon/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Amazon/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Amazon/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Amazon/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
