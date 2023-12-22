using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using DropZoneFileUpload.Models;


namespace DropZoneFileUpload.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        protected override void HandleUnknownAction(string actionName)
        {
            View(actionName).ExecuteResult(ControllerContext);
            //base.HandleUnknownAction(actionName);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Home()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        [Authorize]
        public ActionResult SaveUploadedFile()
        {
            var isSavedSuccessfully = false;
            var fName = "";
            foreach (string fileName in Request.Files)
            {
                var file = Request.Files[fileName];
                //Save file content goes here
                //fName = file.FileName;
                if (file != null && file.ContentLength > 0)
                {
                    var originalDirectory =
                        new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));

                    var pathString = Path.Combine(originalDirectory.ToString(), "imagepath");

                    var fileName1 = Path.GetFileName(file.FileName);


                    var isExists = Directory.Exists(pathString);

                    if (!isExists)
                        Directory.CreateDirectory(pathString);

                    var path = string.Format("{0}\\{1}", pathString, file.FileName);
                    file.SaveAs(path);

                    var userid = (int) Session["usersID"];

                    var getphotoByte = GetPhoto(pathString + "/" + file.FileName);
                    var uuupld = new UUPLD
                    {
                        USERID = userid,
                        FILEPATH = "/images/wallimages/imagepath",
                        FILENAME = file.FileName,
                        CREATED_DATE = DateTime.Now,
                        UImage = getphotoByte,
                        UImageB64 = ImageToBase64(pathString + "/" + file.FileName),
                        is_deleted = false
                    };

                    var localhostEntitiesDb = new localhostEntities();
                    localhostEntitiesDb.UUPLDs.Add(uuupld);
                    localhostEntitiesDb.SaveChanges();
                    isSavedSuccessfully = true;
                }
            }

            if (isSavedSuccessfully)
            {
                return Json(new {Message = "Done saving file"});

            }
            return Json(new {Message = "Error in saving file"});
        }

        public static byte[] GetPhoto(string filePath)
        {
            var stream = new FileStream(
                filePath, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(stream);

            var photo = reader.ReadBytes((int) stream.Length);

            reader.Close();
            stream.Close();

            return photo;
        }
        public string ImageToBase64(string filePath)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            string file = Convert.ToBase64String(bytes);
            return file;

        }
        [Authorize]
        public ActionResult MyUploads()
        {
            return View();
        }
        [Authorize]
        public ActionResult TopUploads()
        {
            var filelist = new localhostEntities();

            var top5File = filelist.UUPLDs
                //.Select(x => x.FILEPATH + "/" + x.FILENAME)
                .Where( x => x.is_deleted == false)
                .Select(i => new { i.FILEID, i.FILEPATH, i.FILENAME,i.UImageB64 })
                //.Take()
                .ToList();
            // TO DO .OrderByDescending();

            List<AttachmentsModel> attachmentsList = top5File.Select(a => new AttachmentsModel
            {
                FileName = a.FILENAME,
                Path = a.FILEPATH + "/" + a.FILENAME,
                AttachmentID = a.FILEID,
                UImageB64 = a.UImageB64,
                RROSE = (from c in filelist.UFILEIDs
                         where c.FILEID == a.FILEID & c.RROSE == 1
                         select c).ToList().Count,
                WROSE = (from c in filelist.UFILEIDs
                         where c.FILEID == a.FILEID & c.WROSE == 1
                         select c).ToList().Count
            }).ToList();
            
            return View(attachmentsList);
        }

        public ActionResult Get5Attachments()
        {
            var filePaths = Directory.GetFiles(Server.MapPath("~/Images/WallImages/imagepath/"));
            var attachmentsList = new List<AttachmentsModel>();
            /*
            var roles = db.UserRoles.Where(x => x.UserID == <insert id>).Select(x => x.Role)
            */

            var filelist = new localhostEntities();
            var userid = (int)Session["usersID"];

            var top5File = filelist.UUPLDs
                .Select(x => x.FILEPATH + "/" + x.FILENAME)
                .Take(5).ToList();
                            // TO DO .OrderByDescending();

            foreach (var a in top5File)
            {
                var attachment = new AttachmentsModel();
                attachment.FileName = a;
                attachment.Path = a;
                attachmentsList.Add(attachment);
            };
             

            return Json(new {Data = attachmentsList.ToList()}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Get5AttachmentsByUserId()
        {
            var filePaths = Directory.GetFiles(Server.MapPath("~/Images/WallImages/imagepath/"));
            var attachmentsList = new List<AttachmentsModel>();
            /*
            var roles = db.UserRoles.Where(x => x.UserID == <insert id>).Select(x => x.Role)
            */

            var filelist = new localhostEntities();
            var userid = (int)Session["usersID"];

            var top5File = filelist.UUPLDs
                            .Where(x => x.USERID == userid)
                            .Select(x => x.FILEPATH + "/" + x.FILENAME)
                            .Take(5).ToList();

            foreach (var a in top5File)
            {
                var attachment = new AttachmentsModel();
                attachment.FileName = a;
                attachment.Path = a;
                attachmentsList.Add(attachment);
            };


            return Json(new { Data = attachmentsList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get25Attachments()
        {
            var filePaths = Directory.GetFiles(Server.MapPath("~/Images/WallImages/imagepath/"));
            var attachmentsList = new List<AttachmentsModel>();
            /*
            var roles = db.UserRoles.Where(x => x.UserID == <insert id>).Select(x => x.Role)
            */

            var filelist = new localhostEntities();
            var userid = (int)Session["usersID"];

            var top5File = filelist.UUPLDs
                            .Select(x => x.FILEPATH + "/" + x.FILENAME)
                            .Take(25).ToList();

            foreach (var a in top5File)
            {
                var attachment = new AttachmentsModel();
                attachment.FileName = a;
                attachment.Path = a;
                attachmentsList.Add(attachment);
            };


            return Json(new { Data = attachmentsList.ToList() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAttachmentsByUserId()
        {
            var filePaths = Directory.GetFiles(Server.MapPath("~/Images/WallImages/imagepath/"));
            var attachmentsList = new List<AttachmentsModel>();
            /*
            var roles = db.UserRoles.Where(x => x.UserID == <insert id>).Select(x => x.Role)
            */

            var filelist = new localhostEntities();
            var userid = (int)Session["usersID"];

            var top5File = filelist.UUPLDs
                            .Where(x => x.USERID == userid)
                            .Select(x => x.FILEPATH + "/" + x.FILENAME)
                            .ToList();

            foreach (var a in top5File)
            {
                var attachment = new AttachmentsModel();
                attachment.FileName = a;
                attachment.Path = a;
                attachmentsList.Add(attachment);
            };


            return Json(new { Data = attachmentsList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult InsertRRose(int fileId)
        {
            var isSavedSuccessfully = false;
            var localhostEntitiesDb = new localhostEntities();
            var userid = (int)Session["usersID"];
            var ufileidexit = (from c in localhostEntitiesDb.UFILEIDs
                                where c.FILEID == fileId & c.USERID == userid
                               select c).ToList();


            if (!ufileidexit.Any())
            { 
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (!isSavedSuccessfully)
                { 
                    
                    var ufileid = new UFILEID()
                    {
                        FILEID = fileId,
                        RROSE = 1,
                        RTIME = DateTime.Now,
                        USERID = userid,
                    };

               
                    localhostEntitiesDb.UFILEIDs.Add(ufileid);
                    //rrose = (int)ufileid.RROSE;
                    localhostEntitiesDb.SaveChanges();
                    isSavedSuccessfully = true;
                }
            }
            else
            {
                var ufileid = new UFILEID()
                {
                    FILEID = fileId,
                    RROSE =  1,
                    RTIME = DateTime.Now,
                    USERID = userid,
                    //WROSE = ufileidexit.FirstOrDefault().WROSE,
                    //WTIME = ufileidexit.FirstOrDefault().WTIME

                };


                if (ufileidexit.FirstOrDefault().USERID == userid)
                {
                    localhostEntitiesDb.UFILEIDs.AddOrUpdate(ufileid);
                }
                else
                {
                    localhostEntitiesDb.UFILEIDs.Add(ufileid);
                }
                localhostEntitiesDb.SaveChanges();
                isSavedSuccessfully = true;
            }
            var ufileidCountRROSE = (from c in localhostEntitiesDb.UFILEIDs
                                     where c.FILEID == fileId & c.RROSE == 1
                                     select c).ToList().Count;

            var ufileidCountWROSE = (from c in localhostEntitiesDb.UFILEIDs
                                     where c.FILEID == fileId & c.WROSE == 1
                                     select c).ToList().Count;
            if (isSavedSuccessfully)
            {
                return Json(new { like = ufileidCountRROSE, dislike = ufileidCountWROSE }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { data = 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult InsertWRose(int fileId)
        {
            var isSavedSuccessfully = false;
            var userid = (int)Session["usersID"];

            var localhostEntitiesDb = new localhostEntities();
            var ufileidexit = (from c in localhostEntitiesDb.UFILEIDs
                               where c.FILEID == fileId & c.USERID == userid
                               select c).ToList();

            if (!ufileidexit.Any())
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (!isSavedSuccessfully)
                {

                    var ufileid = new UFILEID()
                    {
                        FILEID = fileId,
                        USERID = userid,
                        WROSE = 1,
                        WTIME = DateTime.Now
                    };


                    localhostEntitiesDb.UFILEIDs.Add(ufileid);
                    localhostEntitiesDb.SaveChanges();
                    isSavedSuccessfully = true;
                }
            }
            else
            { 
                var ufileid = new UFILEID()
                {
                    FILEID = fileId,
                    //RROSE = ufileidexit.FirstOrDefault().RROSE,
                    //RTIME = ufileidexit.FirstOrDefault().RTIME,
                    USERID = userid,
                    WROSE = 1,
                    WTIME = DateTime.Now
                };

                if (ufileidexit.FirstOrDefault().USERID == userid)
                {
                    localhostEntitiesDb.UFILEIDs.AddOrUpdate(ufileid);
                }
                else
                {
                    localhostEntitiesDb.UFILEIDs.Add(ufileid);
                }
                
                localhostEntitiesDb.SaveChanges();
                isSavedSuccessfully = true;
            }
            var ufileidCountRROSE  = (from c in localhostEntitiesDb.UFILEIDs
                                                  where c.FILEID == fileId & c.RROSE == 1
                                                  select c).ToList().Count;

            var ufileidCountWROSE = (from c in localhostEntitiesDb.UFILEIDs
                                where c.FILEID == fileId & c.WROSE == 1
                                select c).ToList().Count;
            if (isSavedSuccessfully)
            {
                return Json(new { like = ufileidCountRROSE, dislike = ufileidCountWROSE }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { data = 0 }, JsonRequestBehavior.AllowGet);
        }
    }
}