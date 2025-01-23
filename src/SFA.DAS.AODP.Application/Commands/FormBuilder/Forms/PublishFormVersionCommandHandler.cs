﻿using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;
using Entities = SFA.DAS.AODP.Data.Entities;
using ViewModels = SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class PublishFormVersionCommandHandler : IRequestHandler<PublishFormVersionCommand, PublishFormVersionCommandResponse>
{
    private readonly IFormVersionRepository _formRepository;

    public PublishFormVersionCommandHandler(IFormVersionRepository formRepository)
    {
        _formRepository = formRepository;
    }

    public async Task<PublishFormVersionCommandResponse> Handle(PublishFormVersionCommand request, CancellationToken cancellationToken)
    {
        var response = new PublishFormVersionCommandResponse();
        response.Success = false;

        try
        {
            var found = await _formRepository.Publish(request.FormVersionId);

            if (!found)
            {
                response.ErrorMessage = $"Not found form version with ID {request.FormVersionId}. ";
                response.NotFound = true;
                response.Success = false;
            }
            else
            {
                response.NotFound = false;
                response.Success = true;
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
