namespace Raiqub.Expressions.Repositories;

public interface ISessionFactory
{
    ISession Create(ChangeTracking? tracking = null);

    IQuerySession CreateForQuery();
}
