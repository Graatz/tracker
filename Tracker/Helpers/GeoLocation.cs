using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Xml;
using Tracker.Models;

namespace Tracker.Helpers
{
    static class MyExtensions
    {
        public static List<T> Splice<T>(this List<T> list, int index, int count)
        {
            List<T> range = list.GetRange(index, count);
            list.RemoveRange(index, count);
            return range;
        }
    }

    public static class GeoLocation
    {
        public static string ReversedGeoLocation(double lat, double lng)
        {
            string requestFormat = "https://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&key={2}";
            string request = string.Format(requestFormat,
                                           lat.ToString().Replace(",", "."),
                                           lng.ToString().Replace(",", "."),
                                           "AIzaSyByHNqSflMX894xRV1DpZB81LMMrBAsDlI");

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(request);

            XmlNodeList xNodelst = xDoc.GetElementsByTagName("result");
            XmlNode xNode = xNodelst.Item(0);

            return xNode.SelectSingleNode("formatted_address").InnerText;
        }

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }

        public static double CalculateTrackDistance(List<TrackPoint> trkpts)
        {
            double distance = 0;

            for (int i = 0; i < trkpts.Count - 1; i++)
            {
                distance += CalculateDistance(trkpts[i].Latitude, trkpts[i].Longitude, trkpts[i + 1].Latitude, trkpts[i + 1].Longitude);
            }

            return distance;
        }

        public static double CalculateTrackTime(List<TrackPoint> trackPoints)
        {
            double trackTime = trackPoints[trackPoints.Count - 1].Date.Subtract(trackPoints[0].Date).TotalHours;

            return trackTime;
        }

        public static double CalculateAvarageTrackSpeed(List<TrackPoint> trackPoints)
        {
            var trackTime = CalculateTrackTime(trackPoints);

            if (trackTime <= 0)
                return 0;

            double avgTrackSpeed = CalculateTrackDistance(trackPoints) / CalculateTrackTime(trackPoints);

            return avgTrackSpeed;
        }

        public static List<Track> GetTracksCloseToTrack(List<TrackPoint> points, List<TrackPoint> route, double distance = 0.000001)
        {
            List<Track> closeTracks = new List<Track>();

            double distance2 = Math.Pow(distance, 2);
            for (int i = 0; i < route.Count - 1; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (DistanceToSegment(points[j], route[i], route[i + 1], Math.Pow(distance, 2)) <= distance)
                    {
                        closeTracks.Add(points[j].Track);
                        points.RemoveAll(tp => tp.TrackId == points[j].TrackId);
                    }
                }

            }

            return closeTracks;
        }

        public static List<TrackPoint> GetPointsCloseToTrack(List<TrackPoint> points, List<TrackPoint> route, double distance = 0.000001)
        {
            List<TrackPoint> closePoints = new List<TrackPoint>();

            double distance2 = Math.Pow(distance, 2);
            for (int i = 0; i < route.Count - 1; i++)
            {
                for (int j = points.Count - 1; j >= 0; j--)
                {
                    if (DistanceToSegment(points[j], route[i], route[i + 1], Math.Pow(distance, 2)) <= distance)
                    {
                        TrackPoint closePoint = points.Splice(j, 1)[0];
                        closePoints.Add(closePoint);
                    }
                }

            }

            return closePoints;
        }

        public static double DistanceToSegment(TrackPoint p, TrackPoint v, TrackPoint w, double distance)
        {
            double len2 = Dist2(v, w);
            if (len2 == 0) return Dist2(p, v);
            double q = ((p.Longitude - v.Longitude) * (w.Longitude - v.Longitude) + (p.Latitude - v.Latitude) * (w.Latitude - v.Latitude)) / len2;
            if (q < 0) return Dist2(p, v);
            if (q > 1) return Dist2(p, w);
            TrackPoint i = new TrackPoint(v.Longitude + q * (w.Longitude - v.Longitude), v.Latitude + q * (w.Latitude - v.Latitude));
            return Dist2(p, i);
        }

        static double Dist2(TrackPoint p, TrackPoint q)
        {
            return (double)Math.Pow(p.Longitude - q.Longitude, 2) + (double)Math.Pow(p.Latitude - q.Latitude, 2);
        }

        public static List<List<TrackPoint>> SplitToSegments(List<TrackPoint> trackPoints)
        {
            List<List<TrackPoint>> segments = new List<List<TrackPoint>>();
            trackPoints = trackPoints.OrderBy(tp => tp.Index).ToList();

            int flag = 0;
            for (int i = 0; i <= trackPoints.Count - 1; i++)
            {
                if (i == trackPoints.Count - 1)
                {
                    List<TrackPoint> segment = trackPoints.GetRange(flag, i + 1 - flag);
                    segments.Add(segment);
                }
                else if (trackPoints[i].Index + 1 != trackPoints[i + 1].Index)
                {
                    List<TrackPoint> segment = trackPoints.GetRange(flag, i + 1 - flag);
                    segments.Add(segment);
                    flag = i + 1;
                }
            }

            return segments;
        }

        public static List<TrackPoint> DecodePolylinePoints(string encodedPoints)
        {
            if (encodedPoints == null || encodedPoints == "")
                return null;

            List<TrackPoint> polyline = new List<TrackPoint>();
            char[] polylineChars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            while (index < polylineChars.Length)
            {
                // calculate next latitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && next5bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                TrackPoint p = new TrackPoint();
                p.Latitude = Convert.ToDouble(currentLat) / 100000.0;
                p.Longitude = Convert.ToDouble(currentLng) / 100000.0;
                p.Date = DateTime.Now;
                polyline.Add(p);
            }

            return polyline;
        }
    }
}