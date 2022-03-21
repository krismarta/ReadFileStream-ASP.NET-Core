using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadFileStream.Model
{
    public class MasterModel
    {
        public string code { get; set; }
        public string status { get; set; }
        public IEnumerable<ModelFile> data { get; set; }
    }
}
