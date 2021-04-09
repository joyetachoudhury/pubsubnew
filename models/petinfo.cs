using System;
namespace petmanagement.models
{
    public class petinfo
    {
        public petinfo()
        {
        }

        public int petid { get; set; }
        public int userid { get; set; }

        public string name { get; set; }
        public string breed { get; set; }

        public string gender { get; set; }
        public string imagepath { get; set; }

    }
}
