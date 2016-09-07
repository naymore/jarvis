using System.Collections.Generic;
using Rs.Exp.Jarvis.Core.Poi.Models;

namespace Rs.Exp.Jarvis.Core.Poi
{
    public interface IPoiProvider
    {
        PointOfInterest GetPointOfInterest();

        List<PointOfInterest> GetPointsOfInterest();
    }
}