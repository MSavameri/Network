using AutoMapper;
using NetworkInfrastructure.Web.Data.Entities;

namespace NetworkInfrastructure.Web.Models.Profile
{
    public class Profiler : AutoMapper.Profile
    {
        public Profiler()
        {
            CreateMap<NetworkAsset, NetworkAssetDto>().ReverseMap();    
        }
    }
}
