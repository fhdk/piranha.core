﻿/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using Microsoft.AspNetCore.Mvc;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class PageController : Controller
    {
        #region Members
        /// <summary>
        /// The current api.
        /// </summary>
        private readonly IApi api;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public PageController(IApi api) {
            this.api = api;
        }

        /// <summary>
        /// Gets the list view for the pages.
        /// </summary>
        [Route("manager/pages")]
        public IActionResult List() {
            return View(Models.PageListModel.Get(api));
        }

        /// <summary>
        /// Gets the edit view for a page.
        /// </summary>
        /// <param name="id">The page id</param>
        [Route("manager/page/{id:Guid}")]
        public IActionResult Edit(Guid id) {
            return View(Models.PageEditModel.GetById(api, id));
        }

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("manager/page/save")]
        public IActionResult Save(Models.PageEditModel model) {
            if (model.Save(api))
                return RedirectToAction("List");
            return View(model);
        }

        /// <summary>
        /// Disposes the controller and its resources.
        /// </summary>
        protected override void Dispose(bool disposing) {
            api.Dispose();

            base.Dispose(disposing);
        }
    }
}
