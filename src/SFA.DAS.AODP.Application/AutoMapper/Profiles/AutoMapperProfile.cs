using AutoMapper;

namespace SFA.DAS.AODP.Application.AutoMapper.Profiles;

using Models = SFA.DAS.AODP.Models.Forms.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        #region View/DataGrid
        //Model -> ViewModel
        CreateMap<Entities.FormStatus, Models.FormStatus>().ReverseMap();
        CreateMap<Entities.Form, Models.Form>().ReverseMap();
        CreateMap<Entities.FormVersion, Models.FormVersion>().ReverseMap();
        CreateMap<Entities.FormVersion?, Models.FormVersion?>().ReverseMap();
        CreateMap<List<Entities.FormVersion>, List<Models.FormVersion>>().ReverseMap();
        CreateMap<List<Entities.FormVersion?>, List<Models.FormVersion?>>().ReverseMap();
        CreateMap<Entities.FormVersion?, Models.FormVersion?>().ReverseMap();
        CreateMap<Entities.Section, Models.Section>().ReverseMap();
        CreateMap<Entities.Page, Models.Page>().ReverseMap();

        #endregion
    }
}
