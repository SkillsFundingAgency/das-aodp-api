using AutoMapper;
using ViewModels = SFA.DAS.AODP.Models.Forms.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.AutoMapper.Profiles;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        #region Model -> ViewModel
        //Model -> ViewModel
        CreateMap<Entities.FormStatus, ViewModels.FormStatus>().ReverseMap();
        CreateMap<Entities.Form, ViewModels.Form>().ReverseMap();
        CreateMap<Entities.FormVersion, ViewModels.FormVersion>().ReverseMap();
        CreateMap<Entities.FormVersion?, ViewModels.FormVersion?>().ReverseMap();
        CreateMap<Entities.FormVersion?, ViewModels.FormVersion?>().ReverseMap();
        CreateMap<Entities.Section, ViewModels.Section>().ReverseMap();
        CreateMap<Entities.Page, ViewModels.Page>().ReverseMap();

        #endregion
    }
}
