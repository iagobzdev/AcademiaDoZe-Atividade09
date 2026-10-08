// Autor: Iago Barboza Ceccatto
namespace AcademiaDoZe.Domain.Exceptions;

public sealed class DomainException(string message) : Exception(message)
{
}
