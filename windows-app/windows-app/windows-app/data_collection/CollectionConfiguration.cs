using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windows_app.data_collection
{
    class CollectionConfiguration
    {
        public string Username { get; private set; }

        public string WebService { get; private set; }

        public static CollectionConfiguration Default => new CollectionConfiguration()
        {
            Username = "marek",
            WebService = "http://192.168.100.107:8000"
        };
    }
}
