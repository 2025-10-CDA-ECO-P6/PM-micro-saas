using System.Reflection;

namespace CaracterisationOutils;

/// <summary>
/// Exécute un test comme xUnit : une instance neuve de la classe par test,
/// puis <c>Dispose</c> si elle l'implémente, que le test ait passé ou non.
/// Un échec du constructeur, de la méthode ou du <c>Dispose</c> est un échec du test.
/// </summary>
internal static class ExecutionDeTests
{
    /// <summary>Renvoie <c>null</c> si le test passe, l'exception qui le fait échouer sinon.</summary>
    public static Exception? Executer(CasDeTest cas)
    {
        object? instance = null;
        Exception? echec = null;
        try
        {
            instance = Activator.CreateInstance(cas.Classe);
            cas.Methode.Invoke(instance, cas.Arguments);
        }
        catch (TargetInvocationException e) when (e.InnerException is not null)
        {
            echec = e.InnerException;
        }
        catch (Exception e)
        {
            echec = e;
        }

        try
        {
            (instance as IDisposable)?.Dispose();
        }
        catch (Exception e)
        {
            echec ??= e;
        }
        return echec;
    }
}
