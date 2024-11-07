using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSchearcher.Application.Interfaces
{
    public interface IGitServicesHandler
    {
        string CloneRepository(string repoUrl, string localPath);
        void DeleteRepository(string localPath);

    }
}
