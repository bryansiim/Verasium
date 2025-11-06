using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasium.Core
{
    //Guarda as informações de metadados extraídas do arquivo
    public class MetadataInfo
    {
        public string InputContent { get; set; }
        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();

        public string MetadataError { get; set; }

        public bool MetadataAvailable => Tags.Count > 0;
  
    }
}
