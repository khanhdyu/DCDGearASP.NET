﻿using DCDGear.Common;
using DCDGear.DAO;
using DCDGear.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCDGear.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {

        private DCDGearDbContext db = new DCDGearDbContext();
        // GET: Admin/Product

        public ActionResult Index()
        {
            var dao = new ProductDAO().ListAll();
            return View(dao);
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.ProductCategories, "ID", "Name");
            return View();
        }
       
        public ActionResult Edit(long id)
        {
            var dao = new ProductDAO().GetByID(id);
            ViewBag.CategoryID = new SelectList(db.ProductCategories, "ID", "Name", dao.CategoryID);
            return View(dao);
        }

        [HttpDelete]
        public ActionResult Delete(long id)
        {
            new ProductDAO().Delete(id);
            return Redirect("Index");
        }
        #region Create with single img

        //[HttpPost]
        //[ValidateInput(false)]//chap nhan mã html
        //public ActionResult Create(Product products, HttpPostedFileBase fileUpload)
        //{
        //     ViewBag.CategoryID = new SelectList(db.ProductCategories, "ID", "Name", products.CategoryID);
        //    if (fileUpload == null)
        //    {
        //        SetAlert("Vui lòng chọn ảnh", "warning");
        //        return View();
        //    }
        //    else
        //    if (ModelState.IsValid) // kiem tra co valid form hay khong
        //    {
        //        var fileName = Path.GetFileName(fileUpload.FileName);
        //        var path = Path.Combine(Server.MapPath("~/Assets/Thumbnail/"), fileName);
        //        if (System.IO.File.Exists(path))
        //        {
        //            ViewBag.thongbao = "Hình ảnh đã tồn tại";

        //        }
        //        else
        //        {
        //            fileUpload.SaveAs(path);
        //        }
        //        var dao = new ProductDAO();
        //        var session = (UserLogin)Session["DUY"];
        //        products.CreateBy = session.UserName;
        //        products.Image = fileName;
        //        products.CreateDate = DateTime.Now;
        //        long id = dao.Create(products);
        //        if (id > 0)
        //        {
        //            SetAlert("Thêm sản phẩm thành công", "success");
        //            return RedirectToAction("Index", "Product");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Khong tao dc sản phẩm");
        //        }
        //    }
        //    return View("Index");
        //}
        #endregion
        #region Edit with single img

        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult Edit(Product products, HttpPostedFileBase fileUpload)
        //{
        //   ViewBag.CategoryID = new SelectList(db.ProductCategories, "ID", "Name", products.CategoryID);
        //    var dao = new ProductDAO();
        //    if (fileUpload == null)
        //    {
        //        var session = (UserLogin)Session["DUY"];
        //        products.ModifiedBy = session.UserName;
        //        products.ModifiedDate = DateTime.Now;
        //        var result = dao.Update(products);
        //        if (result == 1)
        //        {
        //            SetAlert("Sửa sản phẩm thành công", "success");
        //            return RedirectToAction("Index", "Product");
        //        }
        //        else if (result == 0)
        //        {
        //            SetAlert("Vui lòng chọn ảnh sản phẩm", "warning");
        //        }
        //        return View();
        //    }
        //    else
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var fileName = Path.GetFileName(fileUpload.FileName);
        //            var path = Path.Combine(Server.MapPath("~/Assets/Thumbnail/"), fileName);
        //            if (System.IO.File.Exists(path))
        //            {
        //                ViewBag.thongbao = "Hình ảnh đã tồn tại";

        //            }
        //            else
        //            {
        //                fileUpload.SaveAs(path);
        //            }

        //            var session = (UserLogin)Session["DUY"];
        //            products.ModifiedBy = session.UserName;
        //            products.Image = fileName;
        //            products.ModifiedDate = DateTime.Now;
        //            var result = dao.Update(products);
        //            if (result == 1)
        //            {
        //                SetAlert("Sửa tin tức thành công", "success");
        //                return RedirectToAction("Index", "Product");
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "Cap nhat khong thanh cong!!");
        //            }

        //        }
        //    }
        //    return View("Index");
        //}

        #endregion
        #region Create with multiple img
        [HttpPost]
        [ValidateInput(false)]//chap nhan mã html
        public ActionResult Create(Product products)
        {
            ViewBag.CategoryID = new SelectList(db.ProductCategories, "ID", "Name", products.CategoryID);
            if (ModelState.IsValid) // kiem tra co valid form hay khong
            {
                HttpFileCollectionBase files = Request.Files;
                var x = "";
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase fileUpload = files[i];

                    var fileName = Path.GetFileName(fileUpload.FileName);
                    x += fileName + ",";
                    var path = Path.Combine(Server.MapPath("~/Assets/Thumbnail/"), fileName);
                    fileUpload.SaveAs(path);

                }
                products.Image = x.Remove(x.Length - 1);
                var dao = new ProductDAO();
                var session = (UserLogin)Session["DUY"];
                products.CreateBy = session.UserName;
                //products.Image = fileName;
                products.CreateDate = DateTime.Now;
                long id = dao.Create(products);
                if (id > 0)
                {
                    SetAlert("Thêm sản phẩm thành công", "success");
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "Khong tao dc sản phẩm");
                }
            }
            return View("Index");
        }
        #endregion
        #region Edit with multiple img
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Product products)
        {
            ViewBag.CategoryID = new SelectList(db.ProductCategories, "ID", "Name", products.CategoryID);
            var dao = new ProductDAO();
            var session = (UserLogin)Session["DUY"];

            if (ModelState.IsValid)
            {
                HttpFileCollectionBase files = Request.Files;

                var x = "";
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase fileUpload = files[i];
                    if (i == 0 && fileUpload.ContentLength == 0)
                    {
                        products.ModifiedBy = session.UserName;
                        products.ModifiedDate = DateTime.Now;
                        var result1 = dao.Update(products);
                        if (result1 == 1)
                        {
                            SetAlert("Sửa sản phẩm thành công", "success");
                            return RedirectToAction("Index", "Product");
                        }
                        else if (result1 == 0)
                        {
                            SetAlert("Vui lòng chọn ảnh sản phẩm", "warning");
                        }
                        return View();
                    }
                    else
                    {
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        x += fileName + ",";
                        var path = Path.Combine(Server.MapPath("~/Assets/Thumbnail/"), fileName);
                        fileUpload.SaveAs(path);
                    }
                }
                products.Image = x.Remove(x.Length - 1);
                products.ModifiedBy = session.UserName;
                products.ModifiedDate = DateTime.Now;
                var result = dao.Update(products);
                if (result == 1)
                {
                    SetAlert("Sửa tin tức thành công", "success");
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "Cap nhat khong thanh cong!!");
                }

            }
            return View("Index");
        }
        #endregion
        //public void SetViewBag(long? selectedId = null)
        //{
        //    var dao = new ProductCategoryDAO();
        //    ViewBag.CategoryID = new SelectList(dao.ListAll(), "ID", "Name", selectedId);
        //}
    }
}