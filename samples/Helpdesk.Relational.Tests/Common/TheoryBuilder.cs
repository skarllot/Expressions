namespace Helpdesk.Relational.Tests.Common;

public static class TheoryBuilder
{
    public static TheoryData<T> Build<T>(params T[] values)
    {
        var theoryData = new TheoryData<T>();
        foreach (T value in values)
        {
            theoryData.Add(value);
        }

        return theoryData;
    }
}
