using FileSchearcher.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSchearcher.Application.Interfaces
{
    public interface ICsvExporter
    {
        void ExportToCsv(IEnumerable<CsvFileModel> records, string outputPath = null);
    }
}
