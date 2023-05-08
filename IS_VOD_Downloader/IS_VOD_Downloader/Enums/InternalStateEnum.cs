using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_VOD_Downloader.Enums
{
    public enum InternalState
    {
        CourseSearch,
        CourseSelect,
        TermSelect,
        CookiesSelect,
        ChapterVideoSelect,
        QualitySelect,
        Download,
        Finished,
        Exit
    }
}
