using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace windows_app
{
    [DataContract]
    internal class NoteJSON
    {
        [DataMember] internal string Note;

        [DataMember] internal string Time;

        [DataMember] internal string Image;
    }
}
