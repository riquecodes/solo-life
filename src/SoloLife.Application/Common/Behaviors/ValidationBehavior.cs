namespace SoloLife.Application.Common.Behaviors;

using FluentValidation;
using MediatR;
using SoloLife.Application.Common.Results;

/// <summary>
/// Pipeline MediatR que roda os validators FluentValidation antes do handler.
/// Em falha, devolve um <see cref="Result"/>/<see cref="Result{T}"/> de erro — não lança exceção,
/// para manter o contrato de retorno da camada Application (diretriz 10).
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = results.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

        if (failures.Count == 0)
            return await next();

        var error = string.Join(" ", failures.Select(f => f.ErrorMessage));
        return CreateFailure(error);
    }

    private static TResponse CreateFailure(string error)
    {
        var responseType = typeof(TResponse);

        // Result<T>: chama Result.Failure<T>(error) via reflexão para construir o tipo certo.
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var method = typeof(Result)
                .GetMethods()
                .First(m => m is { Name: nameof(Result.Failure), IsGenericMethod: true })
                .MakeGenericMethod(valueType);

            return (TResponse)method.Invoke(null, [error])!;
        }

        return (TResponse)Result.Failure(error);
    }
}
