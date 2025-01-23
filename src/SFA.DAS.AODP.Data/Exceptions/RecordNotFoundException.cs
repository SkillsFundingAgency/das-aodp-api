﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Data.Exceptions;

public class RecordNotFoundException(Guid id) : Exception
{
    public Guid Id { get; set; } = id;
}
