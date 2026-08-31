using AutoMapper;
using Osta.Core.Feature.FavoriteTechnician.Query.Result;
using Osta.Data.Entities;

namespace Osta.Core.Mapping.FavoriteTechnicianMapping
{
    public class FavoriteTechnicianProfile : Profile
    {
        public FavoriteTechnicianProfile()
        {
            CreateMap<FavoriteTechnician, GetMyFavoriteResult>();
        }

    }
}
