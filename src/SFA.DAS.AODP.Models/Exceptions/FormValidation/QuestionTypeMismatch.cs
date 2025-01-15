﻿using SFA.DAS.AODP.Models.Forms.FormSchema;

namespace SFA.DAS.AODP.Models.Exceptions.FormValidation;

public class QuestionTypeMismatch : QuestionValidationException
{
    public QuestionTypeMismatch(int id, string title, QuestionType expected, QuestionType received) 
        : base(id, title, $"Expected question of type '{nameof(expected)}', received '{nameof(received)}'.")
    {
        Expected = expected;
        Received = received;
    }
    public QuestionType Expected { get; set; }
    public QuestionType Received { get; set; }
}