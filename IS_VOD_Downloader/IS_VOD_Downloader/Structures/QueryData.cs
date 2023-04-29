using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IS_VOD_Downloader.Structures
{
    public class QueryData
    {
        public CookieCollection Cookies { get; set; }
        public PathName Faculty { get; set; }
        public PathName Course { get; set; }
        public PathName Term { get; set; }

        //public string BaseUrl { get; }

        public QueryData() {}
    }
}
