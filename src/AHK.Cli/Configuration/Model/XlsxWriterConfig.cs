using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHK.Configuration.Model
{


    public class XlsxWriterConfig : IConfigValidator
    {
        // See XlsxResultsWriterConfig.Comment_MinimalOutput for semantics.
        public bool Comment_MinimalOutput { get; set; }

        public void Validate()
        {
            // Nothing as for now
        }
    }
}
