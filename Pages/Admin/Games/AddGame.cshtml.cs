using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using PressStart.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using PressStart.Data.Controllers;
using Amazon.S3.Model;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using PressStart.Models;
using System.Diagnostics;

namespace PressStart.Pages.Admin.Games
{




    [Authorize(Roles = "Admin")]
    public class AddGameModel : PageModel
    {
        private PressStartContext db;
        private readonly ILogger<RegisterModel> logger;


        private IHostingEnvironment _environment;

        public AddGameModel(PressStartContext db, ILogger<RegisterModel> logger, IHostingEnvironment environment)
        {
            this.db = db;
            this.logger = logger;
            _environment = environment;
        }



        [BindProperty, Required, MinLength(1), MaxLength(500), Display(Name = "GameName")]
        public string GameName { get; set; }

        [BindProperty, Required, MinLength(1), MaxLength(150), Display(Name = "GameType")]
        public string GameType { get; set; }



        [BindProperty]
        public string Description { get; set; }

        [BindProperty]

        public IFormFile UploadThumb { get; set; }


        [BindProperty]

        public IFormFile UploadRom { get; set; }








        public async Task<IActionResult> OnPostAsync(List<IFormFile> thumbnail, List<IFormFile> romfile)
        {
            if (ModelState.IsValid)
            {

                //Save Thumbnail to Uploads folder
                string uploadedThumbnails = string.Empty;
                string path = Path.Combine(this._environment.WebRootPath, "Uploads");
                string storedThumbnmailPath = string.Empty;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                foreach (IFormFile postedFile in thumbnail)
                {
                    string fileName = Path.GetFileName(postedFile.FileName);
                    uploadedThumbnails = Path.Combine(path, postedFile.FileName);
                    storedThumbnmailPath = Path.Combine("~/Uploads/", postedFile.FileName);

                    string[] allowedExtensions = { ".jpg", ".jpeg", ".gif", ".png" };
                    if (!allowedExtensions.Contains(Path.GetExtension(postedFile.FileName).ToLower()))
                    {
                        // Display error and the form again
                        ModelState.AddModelError(string.Empty, "Only image files (jpg, jpeg, gif, png) are allowed");
                        return Page();
                    }
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }
                }

                //Save Game file to S3 folder
                string uploadedGamePath = string.Empty;
                string s3FilePath = string.Empty;
                string pathImage = Path.Combine(this._environment.WebRootPath, "S3Uploads");

                if (!Directory.Exists(pathImage))
                {
                    Directory.CreateDirectory(pathImage);
                }

                foreach (IFormFile postedFile in romfile)
                {
                    string fileName = Path.GetFileName(postedFile.FileName);
                    string[] allowedExtensions = { ".nes" };
                    if (!allowedExtensions.Contains(Path.GetExtension(postedFile.FileName).ToLower()))
                    {
                        // Display error and the form again
                        ModelState.AddModelError(string.Empty, "Only ROM files  are allowed");
                        return Page();
                    }
                    using (FileStream stream = new FileStream(Path.Combine(pathImage, fileName), FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }
                    var s3Client = new AmazonS3Client(keys.AWSKey, keys.AWSSecret, RegionEndpoint.GetBySystemName("us-east-1"));

                    var fileTransferUtility = new TransferUtility(s3Client);

                    try
                    {
                        if (postedFile.Length > 0)
                        {
                            uploadedGamePath = Path.Combine(pathImage, postedFile.FileName);
                            s3FilePath = Path.Combine("https://presssroms.s3.amazonaws.com/", postedFile.FileName);
                            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                            {
                                BucketName = keys.BucketName,
                                FilePath = uploadedGamePath,
                                StorageClass = S3StorageClass.StandardInfrequentAccess,
                                PartSize = 6291456, // 6 MB.  
                                Key = postedFile.FileName,
                                CannedACL = S3CannedACL.PublicRead
                            };
                            fileTransferUtility.Upload(fileTransferUtilityRequest);
                            fileTransferUtility.Dispose();
                        }
                    }

                    catch (AmazonS3Exception amazonS3Exception)
                    {
                        if (amazonS3Exception.ErrorCode != null &&
                            (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                            ||
                            amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                        {
                        }
                        else
                        {
                        }
                    }
                }

                var newGame = new PressStart.Models.Game { GameName = GameName, GameType = GameType, GamePath = s3FilePath, ThumbnailPath = storedThumbnmailPath, Description = Description };
                db.Add(newGame);

                await db.SaveChangesAsync();

                return RedirectToPage("AddGameSuccess");
            }



            return Page();


        }
    }
}

