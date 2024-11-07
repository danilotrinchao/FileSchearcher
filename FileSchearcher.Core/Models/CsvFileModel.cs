using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSchearcher.Core.Models
{
    public class CsvFileModel
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Repository { get; set; }
        public string Project { get; set; }
        public string FoundTerm { get; set; }
    }
}
