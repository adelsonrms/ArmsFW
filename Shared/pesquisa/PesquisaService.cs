using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArmsFW.Services.Shared.Pesquisa
{
    public class PaginacaoRequest
    {
        public int Skip { get; set; }
        public int ItemsPerPage { get; set; }
        public string SortField { get; set; }
        public bool SortAsc { get; set; }
        public bool AllItems { get; set; } = true;
        public int Pagina { get; set; }
    }

    public class PagedRequest<T>: PaginacaoRequest
    {
        public T Filters { get; set; }
    }

    public class FilterPagedRequest
    {
        public string MatchMode { get; set; }
        public object Value { get; set; }

        public bool HasValue => !string.IsNullOrWhiteSpace(Value?.ToString());
    }

    public class PaginacaoResponse
    {
    }

    public class Pesquisa<T> : PaginacaoResponse
    {
        public long TotalDeRegistros { get; set; }
        public List<T> Registros { get; set; } = new List<T>();
        public int Pagina { get;  set; }
        public int QtdItemsDaPesquisa { get; set; }
        [JsonIgnore]
        public Result<object> Retorno { get; set; }
    }
}