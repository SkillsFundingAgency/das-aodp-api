using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Application.Exceptions;
using Entities = SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class UpdateFormVersionCommandHandler : IRequestHandler<UpdateFormVersionCommand, UpdateFormVersionCommandResponse>
{
    private readonly IFormVersionRepository _formRepository;
    

    public UpdateFormVersionCommandHandler(IFormVersionRepository formRepository)
    {
        _formRepository = formRepository;
        
    }

    public async Task<UpdateFormVersionCommandResponse> Handle(UpdateFormVersionCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateFormVersionCommandResponse();

        try
        {
            var formVersion = await _formRepository.GetFormVersionByIdAsync(request.FormVersionId);
            formVersion.Order = request.Order;
            formVersion.Title = request.Name;
            formVersion.Description = request.Description;


            await _formRepository.Update(formVersion);
            response.Success = true;
        }
        catch (RecordNotFoundException ex)
        {
            response.InnerException = new NotFoundException(ex.Id);
            response.Success = false;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
