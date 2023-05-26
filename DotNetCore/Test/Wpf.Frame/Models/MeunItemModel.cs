using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Frame.Models
{
  public  class MeunItemModel
    {
        public string  IconCode { get; set; }

        public string Header { get; set; }

        public List<MeunItemModel> Children { get; set; }  = new List<MeunItemModel>();
    }
}
