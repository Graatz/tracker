using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;
using Tracker.DTO;
using Tracker.Models;

namespace Tracker.Helpers
{
    public static class GeoMath
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
            double trackTime = 0;
            if (trackPoints[trackPoints.Count - 1].Date != null && trackPoints[0].Date != null)
                trackTime = trackPoints[trackPoints.Count - 1].Date.Value.Subtract(trackPoints[0].Date.Value).TotalMinutes;

            return trackTime;
        }

        public static double CalculateAvarageTrackSpeed(List<TrackPoint> trackPoints)
        {
            var trackTime = CalculateTrackTime(trackPoints) / 60;
            var trackDistance = CalculateTrackDistance(trackPoints);

            if (trackTime <= 0 || trackDistance <= 0)
                return 0;

            double avgTrackSpeed = trackDistance / trackTime;

            return avgTrackSpeed;
        }

        public static List<Track> GetTracksCloseToTrack(List<TrackPoint> points, List<TrackPoint> route, double distance, TimeSpan timeSpan)
        {
            List<Track> closeTracks = new List<Track>();

            for (int i = 0; i < route.Count - 1; i++)
            {
                double minLatitude = 0.0, maxLatitude = 0.0, minLongitude = 0.0, maxLongitude = 0.0;

                if (route[i].Latitude > route[i + 1].Latitude)
                {
                    maxLatitude = route[i].Latitude + distance;
                    minLatitude = route[i + 1].Latitude - distance;
                }
                else
                {
                    maxLatitude = route[i + 1].Latitude + distance;
                    minLatitude = route[i].Latitude - distance;
                }

                if (route[i].Longitude > route[i + 1].Longitude)
                {
                    maxLongitude = route[i].Longitude + distance;
                    minLongitude = route[i + 1].Longitude - distance;
                }
                else
                {
                    maxLongitude = route[i + 1].Longitude + distance;
                    minLongitude = route[i].Longitude - distance;
                }

                var similarTrackPoints = points.Where(
                    tp =>
                    tp.Latitude >= minLatitude &&
                    tp.Latitude <= maxLatitude &&
                    tp.Longitude >= minLongitude &&
                    tp.Longitude <= maxLongitude &&
                    tp.TrackId != route[i].TrackId).ToList();

                for (int j = 0; j < similarTrackPoints.Count - 1; j+=2)
                {
                    if (similarTrackPoints[j].Date == null || route[i].Date == null)
                        continue;

                    var timeDifference = similarTrackPoints[j].Date.Value.Subtract(route[i].Date.Value);
                    if (Math.Abs(timeDifference.TotalMinutes) > timeSpan.TotalMinutes)
                        continue;

                    if (DistanceToSegment(similarTrackPoints[j], route[i], route[i + 1]) <= distance)
                    {
                        closeTracks.Add(similarTrackPoints[j].Track);
                        points.RemoveAll(tp => tp.TrackId == similarTrackPoints[j].TrackId);
                        similarTrackPoints.RemoveAll(tp => tp.TrackId == closeTracks[closeTracks.Count-1].Id);
                    }
                }

            }

            return closeTracks;
        }

        public static List<TrackPoint> GetPointsCloseToTrack(List<TrackPoint> points, List<TrackPoint> route, double distance, TimeSpan timeSpan)
        {
            List<TrackPoint> closePoints = new List<TrackPoint>();

            double distance2 = Math.Pow(distance, 2);
            //double.TryParse(Math.Pow(distance, 2).ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out distance2);
            
            double marigndistance = distance;

            for (int i = 0; i < route.Count - 1; i++)
            {
                double minLatitude = 0.0, maxLatitude = 0.0, minLongitude = 0.0, maxLongitude = 0.0;

                if (route[i].Latitude >= route[i + 1].Latitude)
                {
                    maxLatitude = route[i].Latitude + marigndistance;
                    minLatitude = route[i + 1].Latitude - marigndistance;
                }
                else
                {
                    maxLatitude = route[i + 1].Latitude + marigndistance;
                    minLatitude = route[i].Latitude - marigndistance;
                }

                if (route[i].Longitude >= route[i + 1].Longitude)
                {
                    maxLongitude = route[i].Longitude + marigndistance;
                    minLongitude = route[i + 1].Longitude - marigndistance;
                }
                else
                {
                    maxLongitude = route[i + 1].Longitude + marigndistance;
                    minLongitude = route[i].Longitude - marigndistance;
                }

                var similarTrackPoints = points.Where(tp => tp.Latitude >= minLatitude &&
                    tp.Latitude <= maxLatitude &&
                    tp.Longitude >= minLongitude &&
                    tp.Longitude <= maxLongitude).ToList();

                for (int j = 0 ; j < similarTrackPoints.Count; j++)
                {
                    if (similarTrackPoints[j].Date == null || route[i].Date == null)
                        continue;

                    var timeDifference = similarTrackPoints[j].Date.Value.Subtract(route[i].Date.Value);
                    if (Math.Abs(timeDifference.TotalMinutes) > timeSpan.TotalMinutes)
                        continue;

                    if (DistanceToSegment(similarTrackPoints[j], route[i], route[i + 1]) <= distance2)
                    { 
                        closePoints.Add(similarTrackPoints[j]);
                        points.RemoveAll(p => p.Id == similarTrackPoints[j].Id);
                    }
                }
            
            }

            return closePoints;
        }

        /*public static List<TrackPoint> GetPointsCloseToTrack(List<TrackPoint> points, List<TrackPoint> route, double distance, TimeSpan timeSpan)
        {
            List<TrackPoint> closePoints = new List<TrackPoint>();

            double distance2 = Math.Pow(distance, 2);
            //double.TryParse(Math.Pow(distance, 2).ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out distance2);

            for (int i = 0; i < route.Count - 1; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (points[j].Date == null || route[i].Date == null)
                        continue;

                    var timeDifference = points[j].Date.Value.Subtract(route[i].Date.Value);
                    if (Math.Abs(timeDifference.TotalMinutes) > timeSpan.TotalMinutes)
                        continue;

                    if (DistanceToSegment(points[j], route[i], route[i + 1]) <= distance2)
                    {
                        closePoints.Add(points[j]);
                        points.RemoveAt(j--);
                    }
                }

            }

            return closePoints;
        }*/

        public static double DistanceToSegment(TrackPoint point, TrackPoint segmentA, TrackPoint segmentB)
        {
            double len2 = Dist2(segmentA, segmentB);

            if (len2 == 0)
                return Dist2(point, segmentA);

            double q = ((point.Longitude - segmentA.Longitude) * 
                (segmentB.Longitude - segmentA.Longitude) + 
                (point.Latitude - segmentA.Latitude) * 
                (segmentB.Latitude - segmentA.Latitude)) 
                / len2;

            if (q < 0)
                return Dist2(point, segmentA);

            if (q > 1)
                return Dist2(point, segmentB);

            TrackPoint i = new TrackPoint(
                segmentA.Latitude + q * (segmentB.Latitude - segmentA.Latitude),
                segmentA.Longitude + q * (segmentB.Longitude - segmentA.Longitude)       
                );

            return Dist2(point, i);
        }

        static double Dist2(TrackPoint p, TrackPoint q)
        {
            return (double)Math.Pow(p.Longitude - q.Longitude, 2) + (double)Math.Pow(p.Latitude - q.Latitude, 2);
        }

        public static List<TrackSegment> SplitToSegments(List<TrackPoint> trackPoints)
        {
            List<TrackSegment> segments = new List<TrackSegment>();
            trackPoints = trackPoints.OrderBy(tp => tp.Index).ToList();
            int flag = 0;
            
            for (int i = 0; i < trackPoints.Count; i++)
            {
                if (i == trackPoints.Count - 1)
                {
                    var trkpts = trackPoints.GetRange(flag, i + 1 - flag);

                    if (trkpts.Count > 1)
                    {
                        TrackSegment segment = new TrackSegment()
                        {
                            TrackPoints = AutoMapper.Mapper.Map<List<TrackPoint>, List<TrackPointDTO>>(trkpts),
                            AvarageSpeed = CalculateAvarageTrackSpeed(trkpts),
                            Distance = CalculateTrackDistance(trkpts)
                        };

                        segments.Add(segment);
                    }
                }
                else if (trackPoints[i].Index + 1 != trackPoints[i + 1].Index)
                {
                    var trkpts = trackPoints.GetRange(flag, i + 1 - flag);
                    if (trkpts.Count > 1)
                    {
                        TrackSegment segment = new TrackSegment()
                        {
                            TrackPoints = AutoMapper.Mapper.Map<List<TrackPoint>, List<TrackPointDTO>>(trkpts),
                            AvarageSpeed = CalculateAvarageTrackSpeed(trkpts),
                            Distance = CalculateTrackDistance(trkpts)
                        };

                        segments.Add(segment);
                    }

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
                polyline.Add(p);
            }

            return polyline;
        }
    }
}