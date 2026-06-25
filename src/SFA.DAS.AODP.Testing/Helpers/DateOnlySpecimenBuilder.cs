using AutoFixture.Kernel;

namespace SFA.DAS.AODP.Testing.Helpers;

public class DateOnlySpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(DateOnly))
        {
            return DateOnly.FromDateTime(DateTime.Now);
        }

        return new NoSpecimen();
    }
}
