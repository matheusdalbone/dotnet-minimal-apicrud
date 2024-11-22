using ApiCrud.Data;
using Azure.Core;
using Microsoft.EntityFrameworkCore;

namespace ApiCrud.Estudantes
{
    public static class EstudantesRotas
    {
        public static void AddRotasEstudantes(this WebApplication app)
        {
            var rotasEstudantes = app.MapGroup("estudantes");

            //Post - Criar
            rotasEstudantes.MapPost("", async (AddEstudanteRequest request, AppDbContext context, CancellationToken ct) => 
            {
                var jaExiste = await context.Estudantes.AnyAsync(estudante => estudante.Nome == request.Nome, ct);

                if (jaExiste)
                    return Results.Conflict("Ja existe!");
                
                //Criando estudante novo
                var novoEstudante = new Estudante(request.Nome);
                
                //Salvar no banco
                await context.Estudantes.AddAsync(novoEstudante);
                await context.SaveChangesAsync();

                var estudanteDTO = new EstudanteDTO(novoEstudante.Id, novoEstudante.Nome);
                
                return Results.Ok(estudanteDTO);
            });

            //Get All 
            rotasEstudantes.MapGet("", async (AppDbContext context, CancellationToken ct) =>
            {
                var estudantes = await context.Estudantes
                .Where(estudante => estudante.Ativo)
                .Select(estudante => new EstudanteDTO(estudante.Id, estudante.Nome))
                .ToListAsync(ct);

                return Results.Ok(estudantes);
            });

            //Update - Nome Estudante
            rotasEstudantes.MapPut("{id:guid}", async(Guid id, UpdateEstudanteRequest request, AppDbContext context, CancellationToken ct) =>
            {
                var estudante = await context.Estudantes.SingleOrDefaultAsync(estudante => estudante.Id == id, ct);

                if (estudante == null)
                    return Results.NotFound();

                estudante.AtualizarNome(request.Nome);

                await context.SaveChangesAsync();


                return Results.Ok(new EstudanteDTO(estudante.Id, estudante.Nome));
            });

            //Delete - Soft
            rotasEstudantes.MapDelete("{id:guid}", async (Guid id, AppDbContext context, CancellationToken ct) =>
            {
                var estudante = await context.Estudantes.SingleOrDefaultAsync(estudante => estudante.Id == id, ct);

                if (estudante == null)
                    return Results.NotFound();

                estudante.Desativar();

                await context.SaveChangesAsync();
                return Results.Ok();
            });
        }
    }
}
