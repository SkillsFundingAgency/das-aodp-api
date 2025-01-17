using AutoMapper;

namespace SFA.DAS.AODP.Application.AutoMapper.Profiles;

using Models = SFA.DAS.AODP.Models.Forms.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;

public class CablingOrderProfile : Profile
{
    public CablingOrderProfile()
    {
        #region View/DataGrid
        //Model -> ViewModel
        CreateMap<Entities.Form, Models.Form>().ReverseMap();
        CreateMap<Entities.Section, Models.Section>().ReverseMap();
        CreateMap<Entities.Page, Models.Page>().ReverseMap();

        #endregion
    }
}
