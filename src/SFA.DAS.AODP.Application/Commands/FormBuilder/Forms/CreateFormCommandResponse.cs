﻿using SFA.DAS.AODP.Models.Forms.FormBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class CreateFormCommandResponse : BaseResponse 
{ 
    public FormVersion FormVersion { get; set; }
}
