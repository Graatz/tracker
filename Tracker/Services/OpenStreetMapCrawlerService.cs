using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using System.Net;
using System.Web.Hosting;
using System.Net.Mime;
using Tracker.Models;
using System.IO;
using System.Diagnostics;
using Tracker.DAL;
using Tracker.Services;
using Microsoft.AspNet.Identity;
using Tracker.Helpers;

namespace Tracker.Services
{
    public class OpenStreetMapCrawlerService : ICrawlerService
    {
        private HtmlWeb web;
        private UnitOfWork unitOfWork;

        public OpenStreetMapCrawlerService(UnitOfWork unitOfWork)
        {
            this.web = new HtmlWeb();
            this.unitOfWork = unitOfWork;
        }

        public void Run(int startingPage, int endingPage)
        {
            for (int i = startingPage; i <= endingPage; i++)
                DownloadTracksFromPage(i);
        }

        public void DownloadTracksFromPage(int page)
        {
            var url = "https://www.openstreetmap.org/traces/page/" + page;
            var doc = web.Load(url);
            var nodes = doc.DocumentNode.Descendants("a").Where(d => d.GetAttributeValue("href", "").Contains("/user/"));

            nodes = nodes.Where(d => d.GetAttributeValue("href", "").Contains("/traces/"));

            HashSet<string> trackLinks = new HashSet<string>();
            foreach (var node in nodes)
            {
                var trackLink = "https://www.openstreetmap.org" + node.Attributes["href"].Value;
                var externalId = long.Parse(trackLink.Split('/')[6]);

                var tracks = unitOfWork.TrackRepository.Get(filter: t => t.ExternalId == externalId && t.ExternalSignature.Equals("OpenStreetMap")).SingleOrDefault();
                if (tracks != null)
                    continue;

                trackLinks.Add(trackLink);
            }

            DownloadTracks(trackLinks);
        }

        public void DownloadTracks(IEnumerable<string> urls)
        {
            foreach (var url in urls)
                DownloadTrack(url);
        }

        public bool DownloadTrack(string url)
        {
            var doc = web.Load(url);
            var node = doc.DocumentNode.Descendants("a").Where(d => d.GetAttributeValue("href", "").Contains("/data")).First();
            var downloadLink = "https://www.openstreetmap.org" + node.Attributes["href"].Value;

            using (var client = new WebClient())
            {
                client.OpenRead(downloadLink);

                string header_contentDisposition = client.ResponseHeaders["content-disposition"];
                string fileName = new ContentDisposition(header_contentDisposition).FileName;

                if (!fileName.EndsWith(".gz"))
                {
                    var filepath = HostingEnvironment.ApplicationPhysicalPath + "UploadedFiles/" + fileName;
                    client.DownloadFile(downloadLink, filepath);
                    if (!UploadTrack(filepath, fileName))
                        return false;
                }
            }

            return true;
        }

        public bool UploadTrack(string filePath, string fileName)
        {
            TrackHandler trackHandler = new TrackHandler("OpenStreetMap");
            GpxParser parser = new GpxParser();
            List<TrackPoint> trackPoints = parser.Parse(File.OpenRead(filePath));
            var externalId = long.Parse(fileName.Split('.')[0]);
            var userId = HttpContext.Current.User.Identity.GetUserId().ToString();

            Track track = new Track()
            {
                Name = fileName,
                Description = "Trasa pobrana przez web crawlera OpenStreetMap",
                ExternalId = externalId,
                User = unitOfWork.UserRepository.GetByID(userId)
        };

            Track trackDetails = trackHandler.SetTrackData(track, trackPoints);

            if (trackDetails == null)
                return false;

            unitOfWork.TrackRepository.Insert(trackDetails);
            unitOfWork.TrackPointRepository.InsertRange(trackDetails.TrackPoints.ToList());
            unitOfWork.Save();

            return true;
        }
    }
}