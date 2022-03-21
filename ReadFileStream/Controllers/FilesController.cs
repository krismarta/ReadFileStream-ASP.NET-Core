using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadFileStream.Model;
using ReadFileStream.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFileStream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class FilesController : ControllerBase
    {
        private readonly IHostingEnvironment env;

        public FilesController(IHostingEnvironment env)
        {
            this.env = env;
        }

        [HttpPost]
        public ActionResult ReadFile(ByIdVM byIdVM)
        {
            var data = new MasterModel();
            List<ModelFile> arraydata = new List<ModelFile>();

            var path = env.WebRootFileProvider.GetFileInfo("Files/bnisegmentasi.dat")?.PhysicalPath;
            var fileStream = new FileStream("wwwroot/Files/bnisegmentasi.dat", FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(fileStream, Encoding.UTF8);
            string line = String.Empty;
            string[] splittext;
            int  counter = 0;
            while ((line = reader.ReadLine()) != null)
            {
                counter++;
                splittext = line.Split("|");

                if(!splittext[2].Equals(" ") || !splittext[2].Equals(" "))
                {
                    var arrayku = new ModelFile
                    {
                        RegionCode = splittext[0],
                        BranchCode = splittext[1],
                        FileName = splittext[2],
                        PasswordFile = splittext[3],
                    };
                    arraydata.Add(arrayku);
                }
                else
                {
                    return Ok(new { code = "400", status = "File data Nomor " + counter + " Masih Kosong" });
                }
                
            }

            data.code = "200";
            data.status = "OK";
            data.data = arraydata;
            return Ok(data);
        }
        [HttpPost]
        public IActionResult WriteFile(WriteFileVM writeFileVM)
        {
        
            return Ok(new { status = "OK" });
        }

        [HttpPost]
        public async Task<ActionResult> UploadFileAsync([FromForm] UploadVM uploadVM)
        {
            var isValid = 1;
            var message = "Berhasil Upload File";
            string allowTypefile = ".dat";
            decimal MaxFileSize = 5242880; //mb to kb (5MB)
            string BaseDirectory = Path.Combine("wwwroot", "Files");

            decimal fileSize = uploadVM.filedata.Length;
            string filePath = Path.GetFileName(uploadVM.filedata.FileName);
            string fileExt = Path.GetExtension(filePath);
            //string ReplaceNameFile = DateTime.Now.ToString("ddmmyyyyhmmss") + "_" + uploadVM.filedata.FileName.Replace(" ", "_");
            string ReplaceNameFile = uploadVM.filedata.FileName;

            if (fileSize > MaxFileSize)
            {
                isValid = 0;
                message = "Ukuran File Terlalu besar";
                return BadRequest(new { code = isValid, message = message });
            }

            if (!allowTypefile.Contains(fileExt))
            {
                isValid = 0;
                message = "Extensi File Harus .dat";
                return BadRequest(new { code = isValid, message = message });
            }
            if (isValid.Equals(1))
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), BaseDirectory + "\\" + ReplaceNameFile);
                var pathOnly = $"/Files/{ReplaceNameFile}";
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    if (!Directory.Exists(fullPath))
                    {
                        try
                        {
                            await uploadVM.filedata.CopyToAsync(stream);
                        }catch(Exception e)
                        {
                            System.IO.File.Delete(fullPath);
                            await uploadVM.filedata.CopyToAsync(stream);
                        }
                    }
                }
                
                return Ok(new { code = 200, status = "Berhasil Upload File." });
            }
            else
            {
                return BadRequest(new { code = isValid, message = message });
            }
            
        }
    }
}