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
            { ".mp3", "mp3_icon.png" },
            { ".mp4", "mp4_icon.png" },
            { ".mov", "mov_icon.png" },
            { ".wav", "wav_icon.png" },
            { ".xlsx", "excel_icon.png" },
            { ".xls", "excel_icon.png" }
        };
        //Gamit tag hashset as dili need key value pair as reference rani para sa ImageGatewayUrl
        private HashSet<string> imageTypes = new HashSet<string>() { ".jpg",".jpeg", ".png", ".gif" };
        public string ImageGatewayUrl
        {
            get => $"https://gateway.pinata.cloud/ipfs/{this.CID}";
        }

        [AllowNull]
        public ImageSource DisplayImage// Note the updated type
        {
            get
            {
                if (imageTypes.Contains(FileType))
                {
                    // Use the ImageGatewayUrl for JPG and PNG
                    return ImageSource.FromUri(new Uri(ImageGatewayUrl));
                } else
                {
                    string imageResourceName = fileTypeImageMap.ContainsKey(FileType)
                                           ? fileTypeImageMap[FileType]
                                           : "dotnet_bot.png"; // Default image if not found
                    return ImageSource.FromFile(imageResourceName);
                }
            }
        }
        [AllowNull]
        public bool IsPreviewable
        {
            get
            {

                if (FileType == ".mp3" || FileType ==".jpg" || FileType == ".jpeg" || FileType ==".png" || FileType == ".gif" || FileType == ".svg" ||  FileType == ".wav" || FileType == ".ogg" || FileType == ".mp4" || FileType == ".mov" || FileType == ".pdf")

                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
           
           
        }
        public string Owner { get; set; }
        public string CID { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }

    }
}
