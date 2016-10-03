using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cloud.Core.Models;

namespace Cloud.Web.Models
{
    public class CloudViewModel
    {
        public string Path { get; set; } 
        public IEnumerable<CloudFolder> Folders { get; set; } 
        public IEnumerable<CloudFile> Files { get; set; } 
    }
}