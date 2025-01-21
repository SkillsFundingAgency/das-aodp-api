using AutoMapper;
using ViewModels = SFA.DAS.AODP.Models.Forms.FormBuilder;
using Entities = SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

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

        #region Command Request Mapping
        CreateMap<Entities.FormVersion, CreateFormVersionCommand.FormVersion>().ReverseMap();
        CreateMap<Entities.FormVersion, UpdateFormVersionCommand.FormVersion>().ReverseMap();
        CreateMap<Entities.Section, CreateSectionCommand.Section>().ReverseMap();
        CreateMap<Entities.Section, UpdateSectionCommand.Section>().ReverseMap();
        CreateMap<Entities.Page, CreatePageCommand.Page>().ReverseMap();
        CreateMap<Entities.Page, UpdatePageCommand.Page>().ReverseMap();
        #endregion

        #region Query Response Mapping
        CreateMap<Entities.FormVersion, GetAllFormVersionsQueryResponse.FormVersion>().ReverseMap();
        CreateMap<Entities.FormVersion, GetFormVersionByIdQueryResponse.FormVersion>().ReverseMap();
        CreateMap<Entities.Section, GetAllSectionsQueryResponse.Section>().ReverseMap();
        CreateMap<Entities.Section, GetSectionByIdQueryResponse.Section>().ReverseMap();
        CreateMap<Entities.Page, GetAllPagesQueryResponse.Page>().ReverseMap();
        CreateMap<Entities.Page, GetPageByIdQueryResponse.Page>().ReverseMap();
        #endregion
    }
}
