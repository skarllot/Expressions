namespace Raiqub.Expressions.Sessions;

public interface ISessionFactory
{
    ISession Create(ChangeTracking? tracking = null);
}
