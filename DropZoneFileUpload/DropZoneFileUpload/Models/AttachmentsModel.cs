using System;

namespace DropZoneFileUpload.Models
{
    public class AttachmentsModel
    {
        public long AttachmentID { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public string UImageB64 { get; set ;}
        public long RROSE { get; set; }
        public long WROSE { get; set; }
    }
}