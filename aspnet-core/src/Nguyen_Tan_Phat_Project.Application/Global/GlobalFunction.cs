using Abp.IO;
using Abp.IO.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Global
{
    public static class GlobalFunction
    {
        public static void AssertNull(this object obj, bool condition, string message)
        {
            if (condition)
            {
                throw new ArgumentNullException(message);
            }
        }

        public static string RegexFormat(string input)
        {
            if (input != null)
            {
                input = Regex.Replace(input, @"\s+", " ").Trim();
                return input;
            }
            else
                return input;
        }


        public static string SaveFile(string folderPath, IFormFile importFile)
        {
            byte[] fileBytes;
            using (var stream = importFile.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }

            string uploadFileName = importFile.FileName;

            DirectoryHelper.CreateIfNotExists(folderPath);
            string uploadFilePath = Path.Combine(folderPath + @"/" + uploadFileName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            File.WriteAllBytes(uploadFilePath, fileBytes);

            return uploadFilePath;
        }
    }
}
