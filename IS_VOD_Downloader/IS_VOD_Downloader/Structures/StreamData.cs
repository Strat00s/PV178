﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_VOD_Downloader.Structures
{
    public record StreamData(string ChapterName, string VideoName, string EncodeKey, string StreamPath);
}
