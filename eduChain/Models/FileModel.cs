using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduChain.Models
{
    public class FileModel
    {
        //dictionary mapping file types to image resources
        private Dictionary<string, string> fileTypeImageMap = new Dictionary<string, string>
        {
            { ".pdf", "pdf_icon.png" },
            { ".docx", "word_icon.png" },
            { ".jpg", "image_icon.png" },
            { ".png", "image_icon.png" },
            { ".xlsx", "excel_icon.png" },
            { ".xls", "excel_icon.png" }
        };
        [AllowNull]
        public ImageSource DisplayImage// Note the updated type
        {
            get
            {
                string imageResourceName = fileTypeImageMap.ContainsKey(FileType)
                                           ? fileTypeImageMap[FileType]
                                           : "dotnet_bot.png"; // Default image if not found
                return ImageSource.FromFile(imageResourceName);
            }
        }
        public string Owner { get; set; }
        public string CID { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }

    }
}
