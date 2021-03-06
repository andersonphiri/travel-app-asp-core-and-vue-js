using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Application.Common.Mappings;
using Travel.Domain.Entities;

namespace Travel.Application.TourLists.Queries.ExportTours
{
    public class TourPackageRecord : IMapFrom<TourPackage>
    {
        public string Name { get; set; }
        public string MapLocation { get; set; }
    }

    public class ExportToursVm
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }

}
