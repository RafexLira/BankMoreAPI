using System;
using MediatR;
using Transferencias.Application.Dtos;

namespace Transferencias.Application.Queries
{
    public class GetTransferenciaByIdQuery : IRequest<TransferenciaDto?>
    {
        public Guid Id { get; set; }
    }
}
