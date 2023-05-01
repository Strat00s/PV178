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
        public record StreamData(string ChapterName, string VideoName, string EncodeKey, string FullStreamPath);


        public CookieCollection Cookies { get; private set; }
        public PathName Faculty { get; private set; }
        public PathName Course { get; private set; }
        public PathName Term { get; private set; }

        public List<StreamData> Streams { get; private set; }

        public string BaseUrl { get; private set; }
        private string _auth;

        private string ValidPath(string path)
        {
            return path.TrimEnd('/') + "/";
        }

        public QueryData(string baseUrl)
        {
            BaseUrl = ValidPath(baseUrl);
            _auth = String.Empty;
            Faculty = new(String.Empty, String.Empty);
            Course = new(String.Empty, String.Empty);
            Term = new(String.Empty, String.Empty);
            Cookies = new();
            Streams = new();
        }

        //add cookies
        public void SetCookies(string iscreds, string issession)
        {
            Cookies.Add(new Cookie("iscreds", iscreds));
            Cookies.Add(new Cookie("issession", issession));
            _auth = "auth/";
        }

        //add course name and path from human readable course code (e.g.: IA174)
        public void AddCourse(PathName course)
        {
            Course = new(course.Name, ValidPath(course.Path));
        }

        //add term from human readable format (e.g.: Summer 2022)
        public void AddTerm(PathName term)
        {
            Term = new(term.Name, ValidPath(term.Path));
        }

        //add Faculty from human readable code (e.g.: FI)
        public void AddFaculty(PathName faculty)
        {
            Faculty = new(faculty.Name, ValidPath(faculty.Path));
        }

        //Build file url
        public string GetFileUrl()
        {
            return BaseUrl + _auth + "el/" + Faculty.Path + Term.Path + Course.Path;
        }

        //Build syllabus url
        public string GetSyllabusUrl()
        {
            return GetFileUrl() + "index.qwarp";
        }

        //build corse url
        public string GetCourseUrl()
        {
            return BaseUrl + _auth + "predmet/" + Faculty.Path + Term.Path + Course.Path;
        }

        public void AddStream(StreamData stream)
        {
            Streams.Add(stream);
        }

        public void DataReport()
        {
            Console.WriteLine("Current query data:");
            Console.WriteLine($"Faculty: {Faculty.Name} {Faculty.Path}");
            Console.WriteLine($"Course: {Course.Name} {Course.Path}");
            Console.WriteLine($"Term: {Term.Name} {Term.Path}");
            if (_auth != String.Empty)
            {
                Console.WriteLine($"Cookies:");
                foreach (var cookie in Cookies)
                {
                    Console.WriteLine($"  {cookie}");
                }
            }
            Console.WriteLine(GetFileUrl());
            Console.WriteLine(GetCourseUrl());
            Console.WriteLine(GetSyllabusUrl());

            if (Streams.Count > 0)
            {
                foreach (var stream in Streams)
                {
                    Console.WriteLine($"{stream.ChapterName}");
                    Console.WriteLine($"{stream.VideoName}");
                    Console.WriteLine($"{stream.FullStreamPath}");
                    Console.WriteLine($"{stream.EncodeKey}");
                    Console.WriteLine("");
                }
            }
        }
    }
}
