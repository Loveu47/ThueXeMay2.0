using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThueXeMay.Areas.Admin.Data
{
    public class BankData
    {
        public string code;
        public string desc;
        public List<Dataa> data;

        public List<Dataa> Getdata()
        {
            return this.data;
        }
    }

    public class Dataa
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string BIN { get; set; }
        public string ShortName { get; set; }
        public string Logo { get; set; }
        public int TransferSupported { get; set; }
        public int LookupSupported { get; set; }
    }

}
