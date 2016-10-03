using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cloud.Core;
using Cloud.Core.Amazon;
using Cloud.Core.Factory;
using Cloud.Web.Models;

namespace Cloud.Web.Controllers
{
    public class AmazonController : Controller
    {
        // GET: Amazon
        public async Task<ActionResult> Index()
        {
            CloudViewModel model = null;

            Creator creator = new AmazonCreator();
            ICloud amazonCloud = creator.CreateCloud();

            if (amazonCloud != null)
            {
                string token = await amazonCloud.GetServiceToken();
                model = new CloudViewModel
                {
                    Folders = amazonCloud.GetFolderList(""),
                    Files = amazonCloud.GetFileList("")
                };
            }

            return View(model);
        }

        // GET: Amazon/Details/5
        public async Task<ActionResult> Details(string path)
        {
            CloudViewModel model = null;

            Creator creator = new AmazonCreator();
            ICloud amazonCloud = creator.CreateCloud();

            if (amazonCloud != null)
            {
                string token = await amazonCloud.GetServiceToken();
                model = new CloudViewModel
                {
                    Folders = amazonCloud.GetFolderList(path),
                    Files = amazonCloud.GetFileList(path)
                };
            }

            return View("Index", model);
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

        // POST: Amazon/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(string path)
        {
            Creator creator = new AmazonCreator();
            ICloud amazonCloud = creator.CreateCloud();
            
            if (amazonCloud != null)
            {
                string token = await amazonCloud.GetServiceToken();
                string result = amazonCloud.Delete(path);
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Download(string path)
        {
            Creator creator = new AmazonCreator();
            ICloud amazonCloud = creator.CreateCloud();

            Stream file = null;
            if (amazonCloud != null)
            {
                string token = await amazonCloud.GetServiceToken();
                file = amazonCloud.DownloadFile(path);
            }

            return File(file, "application/octet-stream");
        }
    }
}
