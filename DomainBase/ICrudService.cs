using System.Collections.Generic;
using ArmsFW.Services.Shared;

namespace app.core.Domain.Interfaces
{
    public interface ICrudService<T>: ICrudService<T, string> { }
    public interface ICrudService<T, TKey>
    {
        List<T> Listar();
        Result<T> Obter(TKey id);
        Result<T> Criar(T tipo);
        Result<T> Atualizar(T tipo);
        Result<int> Excluir(TKey id);

        bool Existe(TKey id);
    }
}