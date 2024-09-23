using InterAPI.Model;
using InterAPI.Model.Pix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterAPI.ModelResponse
{
    public class ListPixImediatoResponse
    {
        public PixGetParametro parametro { get; set; }
        public List<Pix> cobs { get; set; }

    }
}
