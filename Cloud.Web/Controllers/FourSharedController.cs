using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cloud.Core;
using Cloud.Core.Factory;
using Cloud.Web.Models;

namespace Cloud.Web.Controllers
{
    public class FourSharedController : Controller
    {
        // GET: FourShared
        public async Task<ActionResult> Index()
        {
            CloudViewModel model = null;

            Creator creator = new FourSharedCreator();
            ICloud fourSharedCloud = creator.CreateCloud();

            if (fourSharedCloud != null)
            {
                string token = await fourSharedCloud.GetServiceToken();
                model = new CloudViewModel
                {
                    Folders = fourSharedCloud.GetFolderList(""),
                    Files = fourSharedCloud.GetFileList("")
                };
                ViewBag.Token = token;
            }

            return View(model);
        }

        // GET: FourShared/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FourShared/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FourShared/Create
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

        // GET: FourShared/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FourShared/Edit/5
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

        // GET: FourShared/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FourShared/Delete/5
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
