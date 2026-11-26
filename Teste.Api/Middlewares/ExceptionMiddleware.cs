using Teste.Application.Exceptions;
using Teste.Domain.Exceptions;

namespace Teste.Api.Middlewares;

/// <summary>
/// Middleware que captura as exceções de toda a aplicação em um único lugar,
/// assim os controllers não precisam de try/catch.
/// (Parecido com um Exception Filter global no NestJS.)
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await EscreverErroAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (DomainException ex)
        {
            await EscreverErroAsync(context, StatusCodes.Status422UnprocessableEntity, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado");

            // Não devolve os detalhes do erro para o cliente, por segurança
            await EscreverErroAsync(context, StatusCodes.Status500InternalServerError,
                "Ocorreu um erro inesperado. Tente novamente mais tarde.");
        }
    }

    private static async Task EscreverErroAsync(HttpContext context, int statusCode, string mensagem)
    {
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(new
        {
            status = statusCode,
            mensagem = mensagem
        });
    }
}
